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
    /// <summary>
    /// Class for handling individual starts
    /// </summary>
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

        /// <summary>
        /// Private StartsController contstructor for singleton use
        /// </summary>
        /// <param name="mw">Already created MainWindow</param>
        private StartsController(MainWindow mw)
        {
            _mainWindow = mw;
            _startTimes = new Dictionary<string, List<StartTime>>();
            _current = new List<StartTime>();
            _next = new List<StartTime>();
        }

        /// <summary>
        /// Method for StartsController initialization
        /// </summary>
        /// <param name="mw">Already created MainWindow</param>
        /// <returns>Instance of StartsController</returns>
        public static StartsController Initialize(MainWindow mw)
        {
            if (_instance == null)
            {
                _instance = new StartsController(mw);
            }

            return _instance;
        }

        /// <summary>
        /// Method for retrieving singleton instance of StartsController
        /// </summary>
        /// <returns>Singleton instance of StartsController</returns>
        /// <exception cref="InvalidOperationException">When StartsController is not initialized first</exception>
        public static StartsController GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("StartsController is not initialized. Call Initialize() first.");
            }
            return _instance;
        }

        /// <summary>
        /// Clear all saved start times, alo in UI individual start time data grid
        /// </summary>
        public void ClearData()
        {
            _startTimes.Clear();
            _mainWindow.IndividualStartDataGrid.Items.Clear();
        }

        /// <summary>
        /// Load start times from CSV file, saves it to dictionary and updates main window individual starts data grid
        /// </summary>
        /// <returns>True if successful load</returns>
        /// <exception cref="Exception">When something went wrong with reading or parsing data</exception>
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

        /// <summary>
        /// Load start times from RaceResult API, saves it to dictionary and updates main window individual starts data grid
        /// </summary>
        /// <param name="link">Starts API link</param>
        /// <param name="lastSeconds">Load times after this in seconds</param>
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

        /// <summary>
        /// Add data to start times dictionary
        /// </summary>
        /// <param name="data">StartTime object with all data</param>
        /// <param name="time">Start time to group by</param>
        public void AddData(StartTime data, string time)
        {
            if (!_startTimes.ContainsKey(time))
            {
                _startTimes.Add(time, new List<StartTime>());
            }
            _startTimes[time].Add(data);
        }

        /// <summary>
        /// Setup and start the timer, setup first starts, opens the starts window
        /// </summary>
        /// <param name="link">Starts API link for recurring auto update</param>
        public void StartStarts(string link)
        {
            _startsLink = link;
            _timer.Tick += ClockTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _startsWindow = new StartsWindow();
            GetCurrentStart();
            GetNextStart();
            _startsWindow.Show();
            ClockTickLogic();
            _timer.Start();
            IsActive = true;
        }

        /// <summary>
        /// Stops the timer and close the starts window
        /// </summary>
        public void StopStarts()
        {
            _timer.Stop();
            _startsWindow?.Close();
            _startsWindow = null;
            IsActive = false;
        }

        /// <summary>
        /// Method called by timer, calls ClockTickLogic()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClockTick(object? sender, EventArgs e)
        {
            ClockTickLogic();
        }

        /// <summary>
        /// Main control loop for the starts window
        /// </summary>
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

        /// <summary>
        /// Method for setting correct current, next participants and start time counter labels
        /// </summary>
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

        /// <summary>
        /// Method for selecting next start in line, if API is used also auto update the start time every 4th start
        /// </summary>
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

        /// <summary>
        /// Format the now time in hh:mm:ss format
        /// </summary>
        /// <returns>Formated now time in hh:mm:ss format</returns>
        private string FormatTime()
        {
            var now = DateTimeHandler.GetInstance().Now;
            TimeSpan time = TimeSpan.FromMilliseconds(now.TimeOfDay.TotalMilliseconds);
            var timeString = time.ToString(@"hh\:mm\:ss");
            return timeString;
        }

        /// <summary>
        /// Format the start time in hh:mm:ss format, beep every second if start is less than 5 seconds from now, long beep on start
        /// </summary>
        /// <param name="stime">Start time to format</param>
        /// <returns>Formated start time in hh:mm:ss format</returns>
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

        /// <summary>
        /// First load of currently starting participants, removes it from start time dictionary
        /// </summary>
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

        /// <summary>
        /// Get the next in line start time, removes it from start time dictionary
        /// </summary>
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
