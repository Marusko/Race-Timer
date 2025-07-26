using Race_timer.API;
using Race_timer.Data;
using Race_timer.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Race_timer.Logic
{
    public class StartsController
    {
        private static StartsController? _instance;
        private MainWindow _mainWindow;
        private Dictionary<string, List<StartTime>> _startTimes;
        private string _startsLink = "";
        private StartsWindow? _startsWindow;
        private StartsController(MainWindow mw)
        {
            _mainWindow = mw;
            _startTimes = new Dictionary<string, List<StartTime>>();
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
            _startsWindow = new StartsWindow();
            _startsWindow.Show();
        }

        public void StopStarts()
        {
            _startsWindow?.Close();
            _startsWindow = null;
        }
    }
}
