using GuerrillaNtp;
using Race_timer.UI;
using System;
using System.Threading;
using System.Windows;

namespace Race_timer.Logic
{
    /// <summary>
    /// Class for handling NTP sync and fallback
    /// </summary>
    public class DateTimeHandler : IDisposable
    {
        private static DateTimeHandler? _instance;
        private readonly NtpClient _client;
        private NtpClock? _lastClock;
        private readonly Thread _syncThread;
        private bool _running = true;
        private readonly object _lock = new();
        private bool _synchronized = false;
        private MainWindow _mainWindow;

        public DateTime Now
        {
            get
            {
                lock (_lock)
                {
                    return (_lastClock ?? NtpClock.LocalFallback).Now.LocalDateTime;
                }
            }
        }

        /// <summary>
        /// Method for DateTimeHandler initialization
        /// </summary>
        /// <param name="mw">Already created MainWindow</param>
        /// <returns>DateTimeHandler instance</returns>
        public static DateTimeHandler Initialize(MainWindow mw)
        {
            if (_instance == null)
            {
                _instance = new DateTimeHandler(mw);
            }

            return _instance;
        }

        /// <summary>
        /// Method for retrieving singleton instance of DateTimeHandler
        /// </summary>
        /// <returns>Singleton instance of DateTimeHandler</returns>
        /// <exception cref="InvalidOperationException">When DateTimeHandler is not initialized first</exception>
        public static DateTimeHandler GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("DateTimeHandler is not initialized. Call Initialize() first.");
            }
            return _instance;
        }

        /// <summary>
        /// Set up the NTP client and sync thread
        /// </summary>
        /// <param name="mw">Already created MainWindow</param>
        private DateTimeHandler(MainWindow mw)
        {
            _mainWindow = mw;
            _client = NtpClient.Default;
            _syncThread = new Thread(SyncLoop)
            {
                IsBackground = true
            };
            _syncThread.Start();
        }

        /// <summary>
        /// Method called in separate sync thread for synchronizing local time with pool.ntp.org
        /// </summary>
        private void SyncLoop()
        {
            while (_running)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _mainWindow.NtpStatusLabel.Content = "Syncing NTP";
                });
                try
                {
                    var clock = QueryWithBackoff();
                    lock (_lock)
                    {
                        _lastClock = clock;
                    }
                    var localNow = DateTime.UtcNow;
                    var difference = (clock.UtcNow - localNow).TotalMilliseconds;
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _mainWindow.NtpStatusLabel.Content = "NTP success";
                    });
                }
                catch (Exception ex)
                {
                    var warning = new WarningWindow($"Cannot synchronize time with NTP server!\nError: \n[{ex.Message}]");
                    warning.Show();
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _mainWindow.NtpStatusLabel.Content = "NTP fail";
                    });
                }

                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        /// <summary>
        /// Method for querying the NTP server, if error then the delay between retries becomes longer, max 1 minute
        /// </summary>
        /// <returns>Synchronized NTP client</returns>
        /// <exception cref="InvalidOperationException">When the NTP sync stops</exception>
        private NtpClock QueryWithBackoff()
        {
            var delay = TimeSpan.FromSeconds(1);
            while (_running)
            {
                try
                {
                    var q = _client.Query();
                    return q;
                }
                catch
                {
                    Thread.Sleep(delay);
                    delay = delay * 2;
                    if (delay > TimeSpan.FromMinutes(1))
                        delay = TimeSpan.FromMinutes(1);
                }
            }

            throw new InvalidOperationException("NTP sync was stopped.");
        }

        /// <summary>
        /// Clean up method, join the sync thread
        /// </summary>
        public void Dispose()
        {
            _running = false;
            _syncThread.Join();
        }
    }
}
