using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using ClickWrap;

namespace Race_timer.Logic
{
    /// <summary>
    /// Asks the ClickWrap server whether a newer version of Race Timer has been published,
    /// and hands off to the installer's update.exe when the user wants it.
    /// Wraps ClickWrap.UpdateClient so the app id and server URL live in one place.
    /// </summary>
    public static class UpdateChecker
    {
        /// <summary>
        /// App id as published on the ClickWrap server, must match apps/race-timer.yaml.
        /// Permanent, installers are built against it
        /// </summary>
        public const string AppId = "race-timer";

        /// <summary>
        /// ClickWrap server hosting the published versions
        /// </summary>
        private const string ServerUrl = "https://install.susky.net";

        /// <summary>
        /// Version this app is running, worked out by ClickWrap: what the installer recorded,
        /// falling back to the assembly version for builds it never installed
        /// </summary>
        public static string CurrentVersion => InstalledApp.GetCurrentVersion(AppId);

        /// <summary>
        /// If this app was installed by the ClickWrap installer, so an update can actually be applied
        /// </summary>
        public static bool CanUpdate => InstalledApp.GetUpdaterPath(AppId) != null;

        /// <summary>
        /// Checks the server for a newer version. Never throws, a failed check is not worth
        /// bothering the user with, the app is perfectly usable without it
        /// </summary>
        /// <returns>Newer version when there is one, null when up to date or the check failed</returns>
        public static async Task<UpdateInfo?> CheckAsync()
        {
            try
            {
                using var client = new UpdateClient(ServerUrl);
                return await client.CheckForUpdateAsync(AppId);
            }
            catch (Exception e) when (e is HttpRequestException or TaskCanceledException or ArgumentException)
            {
                //Offline, server down, or a version that cannot be compared
                return null;
            }
        }

        /// <summary>
        /// Starts the installer's update.exe and closes the app. Both are needed, setup.exe starts
        /// Race Timer again once it has updated it, so an app that stays open ends up running
        /// next to a second, newer copy of itself
        /// </summary>
        /// <returns>False when there is no updater to start, the app keeps running</returns>
        public static bool StartUpdate()
        {
            //Shutdown() rather than InstalledApp.UpdateAndExit(), that ends the process outright and
            //this app still has a clock window, the starts and the screen timer to close, see ShutDownApp
            if (!InstalledApp.StartUpdater(AppId)) return false;

            Application.Current.Shutdown();
            return true;
        }
    }
}
