using GuerrillaNtp;
using Race_timer.UI;
using System;
using System.Threading;

namespace Race_timer.Logic
{
    public class DateTimeHandler : IDisposable
    {
        private static DateTimeHandler? _instance;
        private readonly NtpClient _client;
        private NtpClock? _lastClock;
        private readonly Thread _syncThread;
        private bool _running = true;
        private readonly object _lock = new();

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

        public static DateTimeHandler GetInstance()
        {
            return _instance ??= new DateTimeHandler();
        }

        private DateTimeHandler()
        {
            _client = NtpClient.Default;
            _syncThread = new Thread(SyncLoop)
            {
                IsBackground = true
            };
            _syncThread.Start();
        }

        private void SyncLoop()
        {
            while (_running)
            {
                try
                {
                    var clock = QueryWithBackoff();
                    lock (_lock)
                    {
                        _lastClock = clock;
                    }
                    var localNow = DateTime.UtcNow;
                    var difference = (clock.UtcNow - localNow).TotalMilliseconds;
                }
                catch (Exception ex)
                {
                    var warning = new WarningWindow($"Cannot synchronize time with NTP server!\nError: \n[{ex.Message}]");
                    warning.Show();
                }

                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        private NtpClock QueryWithBackoff()
        {
            var delay = TimeSpan.FromSeconds(1);
            while (_running)
            {
                try
                {
                    return _client.Query();
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

        public void Dispose()
        {
            _running = false;
            _syncThread.Join();
        }
    }
}
