using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Race_timer.Data;
using RaceResultClient;

namespace Race_timer.API
{
    /// <summary>
    /// The RaceResult session behind the Create API window: logs in, says which events the account
    /// can reach, and creates the Simple API entries Race Timer needs on the one that was picked.
    ///
    /// It looks before it writes: an entry that is already there, enabled and pointing at the right
    /// endpoint is left exactly as it is (key and all, so a link already pasted somewhere else
    /// stays valid), one that is missing is created, and one that points somewhere else is
    /// corrected. Entries the catalog does not mention - other apps' APIs on the same event - are
    /// read and posted back untouched, because <c>simpleapi/saveall</c> replaces the whole set.
    /// Nothing is ever deleted, an API the user unticks is simply not created.
    ///
    /// Nothing else in the app talks to RaceResult this way. The running timer reads the Simple API
    /// links (<see cref="LinkHandler"/>), which need no login at all - this session exists only
    /// while the Create API window is open, and <see cref="Dispose"/> logs it out again
    /// </summary>
    internal sealed class RaceResultSetupService : IDisposable
    {
        private ApiClient? _client;
        private bool _loggedIn;
        private string _server = RaceResultApiCatalog.DefaultServer;
        private bool _useHttps = true;

        /// <summary>Whether a RaceResult session is currently open</summary>
        public bool IsLoggedIn => _loggedIn;

        /// <summary>
        /// Logs in and returns the upcoming events the account can reach. Upcoming only: setup
        /// prepares an event that has yet to be timed, and one already run has nothing to add.
        /// A second call replaces the first session rather than piling one on top of it
        /// </summary>
        /// <param name="credentials">Server, scheme and one of the two ways of logging in</param>
        /// <returns>The upcoming events of the account</returns>
        /// <exception cref="HttpRequestException">When the login or the event list fails</exception>
        public async Task<IReadOnlyList<EventListItem>> LoginAndListEventsAsync(RaceResultCredentials credentials)
        {
            await LogoutAsync();

            _server = string.IsNullOrWhiteSpace(credentials.Server)
                ? RaceResultApiCatalog.DefaultServer
                : credentials.Server.Trim().Trim('/');
            _useHttps = credentials.UseHttps;

            var client = new ApiClient(_server, _useHttps);
            try
            {
                await RetryAsync(() => client.Public.LoginAsync(LoginOptionsFor(credentials)));
            }
            catch (Exception e)
            {
                client.Dispose();
                throw new HttpRequestException($"Login failed\n[{Describe(e)}]");
            }

            _client = client;
            _loggedIn = true;

            try
            {
                return await RetryAsync(() => client.Public.GetNextEventListAsync(100));
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"Can't load the events\n[{Describe(e)}]");
            }
        }

        /// <summary>
        /// Creates what the event is missing and corrects what points elsewhere, then returns the
        /// links to copy out - exactly the links that would have been copied out of RaceResult by
        /// hand
        /// </summary>
        /// <param name="eventId">The RaceResult event to set up</param>
        /// <param name="selected">
        /// The rows the user left ticked, the mandatory ones included. A row that is not in the
        /// list is not created, and if it already exists on the event it is left alone
        /// </param>
        /// <returns>The links and what it took to get them</returns>
        /// <exception cref="HttpRequestException">When reading or saving the event's APIs fails</exception>
        public async Task<RaceResultSetupResult> ProvisionAsync(
            string eventId, IReadOnlyCollection<RaceResultApiDefinition> selected)
        {
            var client = _client ?? throw new InvalidOperationException("Not logged in to RaceResult.");
            var ev = client.ForEvent(eventId);

            SimpleApiItem[] existing;
            try
            {
                existing = await RetryAsync(() => ev.SimpleApi.GetAsync());
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"Can't read the APIs of the event\n[{Describe(e)}]");
            }

            //Everything already on the event, so entries the catalog does not mention travel back
            //untouched in the save - saveall replaces the whole set
            var merged = new Dictionary<string, SimpleApiItem>(StringComparer.Ordinal);
            foreach (var item in existing)
            {
                merged[item.Label] = item;
            }

            var desired = selected.SelectMany(e => e.Endpoints).ToList();
            var created = 0;
            var corrected = 0;
            var unchanged = 0;

            foreach (var endpoint in desired)
            {
                var current = merged.GetValueOrDefault(endpoint.Label);

                //Already there, enabled and pointing at the same endpoint - leave it, key and all
                if (current != null && !current.Disabled
                                    && string.Equals(current.Url, endpoint.Url, StringComparison.Ordinal))
                {
                    unchanged++;
                    continue;
                }

                //Each API needs its own access key. Reuse the existing one if the entry is already
                //on the event, so its link stays valid, otherwise generate a fresh one
                var key = string.IsNullOrEmpty(current?.Key) ? NewKey() : current.Key;
                merged[endpoint.Label] = new SimpleApiItem(false, key, endpoint.Url, endpoint.Label);

                if (current == null)
                {
                    created++;
                }
                else
                {
                    corrected++;
                }
            }

            if (created + corrected > 0)
            {
                try
                {
                    await RetryAsync(() => ev.SimpleApi.SaveAllAsync(merged.Values.ToList()));

                    //Re-read rather than trust what was posted, the links are built from the keys
                    //RaceResult ended up storing, not from the ones we proposed
                    existing = await RetryAsync(() => ev.SimpleApi.GetAsync());
                }
                catch (Exception e)
                {
                    throw new HttpRequestException($"Can't save the APIs\n[{Describe(e)}]");
                }
            }

            var api = existing.FirstOrDefault(e =>
                string.Equals(e.Label, RaceResultApiCatalog.ApiLabel, StringComparison.OrdinalIgnoreCase));
            if (api == null || string.IsNullOrWhiteSpace(api.Key))
            {
                throw new HttpRequestException(
                    $"Can't read the link\n[API '{RaceResultApiCatalog.ApiLabel}' was not found on the event]");
            }

            //Only when the user asked for the starts in this run, so a starts API left on the event
            //by somebody else is not offered as if it had just been prepared
            string? startsLink = null;
            if (selected.Any(e => e.Endpoints.Any(p =>
                    string.Equals(p.Label, RaceResultApiCatalog.StartsLabel, StringComparison.OrdinalIgnoreCase))))
            {
                var starts = existing.FirstOrDefault(e =>
                    string.Equals(e.Label, RaceResultApiCatalog.StartsLabel, StringComparison.OrdinalIgnoreCase));
                if (starts != null && !string.IsNullOrWhiteSpace(starts.Key))
                {
                    startsLink = BuildLink(eventId, starts.Key);
                }
            }

            return new RaceResultSetupResult
            {
                ApiLink = BuildLink(eventId, api.Key),
                StartsLink = startsLink,
                Created = created,
                Corrected = corrected,
                Unchanged = unchanged
            };
        }

        /// <summary>
        /// Ends the session: logs out, then disposes the client - which owns an
        /// <see cref="HttpClient"/> of its own, so it has to be disposed whatever happened to the
        /// logout. Best effort on the logout itself, a session left open expires on its own
        /// </summary>
        public async Task LogoutAsync()
        {
            var client = _client;
            if (client == null)
            {
                return;
            }

            _client = null;
            try
            {
                if (_loggedIn)
                {
                    await client.Public.LogoutAsync();
                }
            }
            catch (Exception)
            {
                // best effort, there is nothing useful to do about a failed logout here
            }
            finally
            {
                _loggedIn = false;
                client.Dispose();
            }
        }

        /// <summary>
        /// Called when the Create API window closes. Fire and forget: Dispose cannot await, and
        /// closing the window must not block on the network. The client is disposed inside
        /// LogoutAsync either way
        /// </summary>
        public void Dispose()
        {
            if (_client == null)
            {
                return;
            }

            _ = LogoutAsync();
        }

        /// <summary>
        /// Builds the Simple API link of one entry, in the same shape as the link copied out of
        /// RaceResult. The scheme is the one the login used, https unless it was turned off for an
        /// on-premise server without a certificate
        /// </summary>
        /// <param name="eventId">Event the entry belongs to</param>
        /// <param name="key">Access key of the entry</param>
        /// <returns>Link of the entry</returns>
        private string BuildLink(string eventId, string key)
        {
            var host = RaceResultApiCatalog.SimpleApiHostFor(_server).Trim('/');
            var scheme = _useHttps ? "https" : "http";
            return $"{scheme}://{host}/{eventId}/{key}";
        }

        /// <summary>
        /// Turns the two ways of logging in into what the client expects. An API key wins when both
        /// are filled in, so a key left in the box is never silently ignored
        /// </summary>
        /// <param name="credentials">Entered credentials</param>
        /// <returns>Login options for the client</returns>
        private static LoginOptions LoginOptionsFor(RaceResultCredentials credentials)
        {
            if (credentials.HasApiKey)
            {
                return new LoginOptions { ApiKey = credentials.ApiKey!.Trim() };
            }

            return new LoginOptions
            {
                User = credentials.User,
                Password = credentials.Password
            };
        }

        /// <summary>
        /// Generates a Simple API access key in the same style RaceResult uses - a 32-character
        /// uppercase alphanumeric string. Keys are event-bound, so they only need to be distinct
        /// per API within one event
        /// </summary>
        /// <returns>New access key</returns>
        private static string NewKey()
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var chars = new char[32];
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
            }

            return new string(chars);
        }

        /// <summary>
        /// Retries a call a couple of times on a transient failure. The RaceResult client uses a
        /// plain HttpClient, and the server sits behind a gateway that drops idle keep-alive
        /// connections - so a POST can land on a dead connection and fail with "response ended
        /// prematurely" or a 502/503/504. A retry opens a fresh connection
        /// </summary>
        /// <param name="action">Call to run</param>
        /// <param name="maxAttempts">How many times to try before giving up</param>
        private static async Task RetryAsync(Func<Task> action, int maxAttempts = 3)
        {
            await RetryAsync<object?>(async () =>
            {
                await action();
                return null;
            }, maxAttempts);
        }

        /// <summary>
        /// Retries a call returning a value, see <see cref="RetryAsync(Func{Task},int)"/>
        /// </summary>
        /// <typeparam name="T">Type the call returns</typeparam>
        /// <param name="action">Call to run</param>
        /// <param name="maxAttempts">How many times to try before giving up</param>
        /// <returns>What the call returned</returns>
        private static async Task<T> RetryAsync<T>(Func<Task<T>> action, int maxAttempts = 3)
        {
            for (var attempt = 1; ; attempt++)
            {
                try
                {
                    return await action();
                }
                catch (Exception e) when (attempt < maxAttempts && IsTransient(e))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250 * attempt));
                }
            }
        }

        /// <summary>
        /// Whether a failure is worth retrying - a dead connection or a gateway saying it is
        /// temporarily unavailable
        /// </summary>
        /// <param name="e">Caught exception</param>
        /// <returns>True when the call can be retried</returns>
        private static bool IsTransient(Exception e)
        {
            return e switch
            {
                HttpRequestException => true,
                ApiException api => api.StatusCode is 502 or 503 or 504,
                _ => false
            };
        }

        /// <summary>
        /// A user-facing description of a failure from the client. The inner-exception chain is
        /// flattened so the real transport-level cause is shown rather than the outer wrapper
        /// </summary>
        /// <param name="e">Caught exception</param>
        /// <returns>Description to show</returns>
        private static string Describe(Exception e)
        {
            if (e is ApiException api)
            {
                return $"{api.StatusCode} - {api.Message}";
            }

            var messages = new List<string>();
            for (Exception? current = e; current != null; current = current.InnerException)
            {
                if (!string.IsNullOrWhiteSpace(current.Message))
                {
                    messages.Add(current.Message);
                }
            }

            return string.Join(" - ", messages);
        }
    }
}
