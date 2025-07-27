using Race_timer.API;
using Race_timer.Data;
using Race_timer.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Path = System.IO.Path;

namespace Race_timer.Logic
{
    public class StartsController
    {
        private static StartsController? _instance;
        private MainWindow _mainWindow;
        private Dictionary<string, List<StartTime>> _startTimes;
        private string _startsLink = "";
        private StartsWindow? _startsWindow;
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private DateTime? _lastNtp;
        private List<StartTime> _current;
        private List<StartTime> _next;
        private int _startedCounter;
        public bool IsActive { get; private set; }

        private StartsController(MainWindow mw)
        {
            _mainWindow = mw;
            _startTimes = new Dictionary<string, List<StartTime>>();
            _current = new List<StartTime>();
            _next = new List<StartTime>();
        }

        public static StartsController Initialize(MainWindow mw)
        {
            if (_instance == null)
            {
                _instance = new StartsController(mw);
            }

            return _instance;
        }

        public static StartsController GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("StartsController is not initialized. Call Initialize() first.");
            }
            return _instance;
        }

        public void ClearData()
        {
            _startTimes.Clear();
            _mainWindow.IndividualStartDataGrid.Items.Clear();
        }

        public bool LoadData()
        {
            var op = new OpenFileDialog();
            op.Title = "Choose a file";
            op.Filter = "CSV file|*.csv";
            if (op.ShowDialog() != DialogResult.OK) return false;
            var filePath = op.FileName;
            var index = filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            var name = filePath[index..];
            _mainWindow.StartsFileName.Content = name;
            try
            {
                _startTimes.Clear();
                using StreamReader reader = new StreamReader(filePath);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var split = line.Split(';');
                    var d = new StartTime()
                    {
                        Bib = split[0],
                        Name = split[1],
                        Time = split[2]
                    };
                    AddData(d, split[2]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (_startTimes.Any())
            {
                _mainWindow.IndividualStartDataGrid.Items.Clear();
                foreach (var startTime in _startTimes)
                {
                    if (startTime.Value.Count > 1)
                    {
                        foreach (var st in startTime.Value)
                        {
                            _mainWindow.IndividualStartDataGrid.Items.Add(st);
                        }
                    }
                    else
                    {
                        _mainWindow.IndividualStartDataGrid.Items.Add(startTime.Value.First());
                    }
                }
            }
            return true;
        }

        public async void LoadApiData(string link, int lastSeconds)
        {
            _startTimes.Clear();
            await LinkHandler.LoadStarts(link, lastSeconds, _mainWindow);
            if (_startTimes.Any())
            {
                _mainWindow.IndividualStartDataGrid.Items.Clear();
                foreach (var startTime in _startTimes)
                {
                    if (startTime.Value.Count > 1)
                    {
                        foreach (var st in startTime.Value)
                        {
                            _mainWindow.IndividualStartDataGrid.Items.Add(st);
                        }
                    }
                    else
                    {
                        _mainWindow.IndividualStartDataGrid.Items.Add(startTime.Value.First());
                    }
                }
            }
        }

        public void AddData(StartTime data, string time)
        {
            if (!_startTimes.ContainsKey(time))
            {
                _startTimes.Add(time, new List<StartTime>());
            }
            _startTimes[time].Add(data);
        }

        public void StartStarts(string link)
        {
            _startsLink = link;
            _timer.Tick += ClockTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _startsWindow = new StartsWindow();
            GetCurrentStart();
            GetNextStart();
            _startsWindow.Show();
            _timer.Start();
            IsActive = true;
        }

        public void StopStarts()
        {
            _timer.Stop();
            _startsWindow?.Close();
            _startsWindow = null;
            IsActive = false;
        }

        private void ClockTick(object? sender, EventArgs e)
        {
            ClockTickLogic();
        }

        public void ClockTickLogic()
        {
            var now = DateTimeHandler.GetInstance().Now;
            if (_lastNtp != null)
            {
                if (_lastNtp.Value.Second == now.Second)
                {
                    return;
                }
            }
            _lastNtp = now;
            SetLabels();
            CheckStarted();
        }

        private void SetLabels()
        {
            if (_startsWindow != null)
            {
                if (_current.Any())
                {
                    var partName = $"{_current.First().Bib}. {_current.First().Name}";
                    if (_current.Count > 1)
                    {
                        var names = from s in _current select $"{s.Bib}. {s.Name}";
                        partName = string.Join(" - ", names);
                    }
                    _startsWindow.StartsNameLabel.Text = partName;

                    if (_next.Any())
                    {
                        var nextName = _next.First().Name;
                        if (_next.Count > 1)
                        {
                            var nextNames = from s in _next select s.Name;
                            nextName = string.Join(" - ", nextNames);
                        }
                        _startsWindow.NextParticipantLabel.Content = nextName;
                    }
                    else
                    {
                        _startsWindow.NextParticipantLabel.Content = "";
                    }

                    _startsWindow.ParticipantStartCounterLabel.Content = FormatStartTimeOrClock(_current.First().Time ?? "00:00:00");
                }
                else
                {
                    _startsWindow.StartsNameLabel.Text = "";
                    _startsWindow.NextParticipantLabel.Content = "";
                    _startsWindow.ParticipantStartCounterLabel.Content = FormatTime();
                    _mainWindow.IndividualStartDataGrid.Items.Clear();
                }
            }
        }

        private void CheckStarted()
        {
            if (_current.Any())
            {
                if (TimeOnly.TryParse(_current.First().Time, out var parsed))
                {
                    var now = TimeOnly.FromDateTime(DateTimeHandler.GetInstance().Now);
                    if (Math.Abs(parsed.ToTimeSpan().Subtract(now.ToTimeSpan()).TotalSeconds) > 2 && parsed < now)
                    {
                        _current.Clear();
                        _current.AddRange(_next);
                        GetNextStart();
                        if (ClockLogic.GetInstance().ApiStarts && ClockLogic.GetInstance().Starts && _next.Any())
                        {
                            _startedCounter++;
                            if (_startedCounter > 3)
                            {
                                _startedCounter = 0;
                                if (TimeOnly.TryParse(_next.First().Time, out var apiParsed))
                                {
                                    LoadApiData(_startsLink, (int)Math.Round(apiParsed.ToTimeSpan().TotalSeconds));
                                }
                            }
                        }
                    }
                }
            }
        }

        private string FormatTime()
        {
            var now = DateTimeHandler.GetInstance().Now;
            TimeSpan time = TimeSpan.FromMilliseconds(now.TimeOfDay.TotalMilliseconds);
            var timeString = time.ToString(@"hh\:mm\:ss");
            return timeString;
        }

        private string FormatStartTimeOrClock(string stime)
        {
            if (TimeOnly.TryParse(stime, out var parsed))
            {
                var now = TimeOnly.FromDateTime(DateTimeHandler.GetInstance().Now);
                TimeSpan time = TimeSpan.FromSeconds(Math.Round(now.ToTimeSpan().Subtract(parsed.ToTimeSpan()).TotalSeconds));
                var timeString = time.ToString(@"hh\:mm\:ss");
                if (Math.Abs(time.TotalSeconds) < 0.2)
                {
                    new Thread(() => Console.Beep(1100, 1000)).Start();
                }
                else if (parsed > now && Math.Abs(time.TotalSeconds) < 6)
                {
                    new Thread(() => Console.Beep()).Start();
                }
                return timeString;
            }

            return FormatTime();
        }

        private void GetCurrentStart()
        {
            _current.Clear();
            if (_startTimes.Any())
            {
                _current.AddRange(_startTimes.Values.First());
                var time = _startTimes.Keys.First();
                _startTimes.Remove(time);
            }
        }

        private void GetNextStart()
        {
            _next.Clear();
            if (_startTimes.Any())
            {
                _next.AddRange(_startTimes.Values.First());
                var time = _startTimes.Keys.First();
                _startTimes.Remove(time);
            }
        }
    }
}
