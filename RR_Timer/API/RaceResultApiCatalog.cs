using System;
using System.Collections.Generic;
using System.Linq;
using Race_timer.Data;

namespace Race_timer.API
{
    /// <summary>
    /// Every Simple API entry Race Timer can use, in one place. It is the same list the API
    /// Creator writes for Race Timer, kept here so the app can create the entries itself instead
    /// of the user defining them in RaceResult by hand.
    ///
    /// Nothing at runtime depends on an entry's URL, only on its label - <see cref="LinkHandler"/>
    /// finds <c>main</c>, <c>count</c> and <c>contest</c> by label in the <c>simpleapi/get</c>
    /// listing. The URLs matter to setup, which compares them against what the event already has
    /// and corrects an entry pointing somewhere else
    /// </summary>
    internal static class RaceResultApiCatalog
    {
        /// <summary>Label of the entry that lists all the others. Its link is the one pasted into the app</summary>
        public const string ApiLabel = "api";

        /// <summary>Label of the entry the individual starts are read from, over its own link</summary>
        public const string StartsLabel = "starts";

        /// <summary>The host RaceResult's own cloud administers events on, which is where all but an on-premise install logs in</summary>
        public const string DefaultServer = "events.raceresult.com";

        /// <summary>The host RaceResult's own cloud serves the Simple API from</summary>
        public const string CloudSimpleApiHost = "api.raceresult.com";

        /// <summary>Heading of the rows the API timer needs</summary>
        private const string ApiTimerGroup = "Finish API Timer";

        /// <summary>Heading of the rows individual starts need</summary>
        private const string StartsGroup = "Individual starts";

        /// <summary>
        /// The rows, in the order the selection dialog lists them. Mandatory first within each
        /// group, so what cannot be turned off is read before what can
        /// </summary>
        public static IReadOnlyList<RaceResultApiDefinition> Entries { get; } = new[]
        {
            new RaceResultApiDefinition(ApiTimerGroup, "All API list", true,
                new[] { new RaceResultApiEndpoint(ApiLabel, "simpleapi/get") },
                "The link loaded in the Finish API Timer tab, it lists all the other APIs."),
            new RaceResultApiDefinition(ApiTimerGroup, "Event name and type", true,
                new[] { new RaceResultApiEndpoint("main", "settings/getsettings?names=EventName,EventType") },
                "Without it the API timer cannot run."),
            new RaceResultApiDefinition(ApiTimerGroup, "Finished count", false,
                new[] { new RaceResultApiEndpoint("count", "data/count?filter=[Finished]=1") },
                "Without it switching to the small timer is manual."),
            new RaceResultApiDefinition(ApiTimerGroup, "Contests", false,
                new[] { new RaceResultApiEndpoint("contest", "contests/get") },
                "Without it contests and their start times have to be added by hand."),
            new RaceResultApiDefinition(StartsGroup, "Start times", false,
                new[]
                {
                    new RaceResultApiEndpoint(StartsLabel,
                        "data/list?&fields=Bib,DisplayName,Start.ToD&sort=Start.ToD&listformat=JSON"),
                },
                "Individual start times, loaded from their own link. A CSV file can be used instead. "
                + "If the starts are not kept in a split named Start, change the Start.ToD field in RaceResult."),
        };

        /// <summary>The definition of one row, found by the label of its first endpoint</summary>
        public static RaceResultApiDefinition? ByLabel(string label)
        {
            return Entries.FirstOrDefault(e =>
                e.Endpoints.Any(p => string.Equals(p.Label, label, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Where the Simple API of an event administered on <paramref name="loginServer"/> lives.
        /// On RaceResult's cloud the two are different hosts - events are administered on
        /// <see cref="DefaultServer"/> and the Simple API is served from
        /// <see cref="CloudSimpleApiHost"/> - so the login server cannot simply be reused.
        /// Anywhere else it is the same host, which is what an on-premise install wants
        /// </summary>
        public static string SimpleApiHostFor(string loginServer)
        {
            return string.Equals(loginServer, DefaultServer, StringComparison.OrdinalIgnoreCase)
                ? CloudSimpleApiHost
                : loginServer;
        }
    }
}
