using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FreeEverything.Annotations;

namespace FreeEverything
{
    public sealed class GarbageCan : INotifyPropertyChanged
    {
        

        private readonly ObservableCollection<Garbage> m_GarbageList = new ObservableCollection<Garbage>();
        private Filter m_SelectedFilter;
        private string m_FilterName;
        private string m_FilterRegular;
        private string m_FilterInclude;
        private string m_FilterExclude;
        private bool m_FilterContainFile;
        private bool m_FilterContainContainDirectory;

        public GarbageCan()
        {
            FilterList = new ObservableCollection<Filter>();
        }

        public ObservableCollection<Filter> FilterList { get; private set; }

        [System.Xml.Serialization.XmlIgnore]
        public Filter SelectedFilter
        {
            get { return m_SelectedFilter; }
            set
            {
                if (value != null && value != m_SelectedFilter)
                {
                    m_SelectedFilter = value;
                    OnPropertyChanged("SelectedFilter");

                    m_FilterName = m_SelectedFilter.Name;
                    OnPropertyChanged("FilterName");

                    m_FilterRegular = m_SelectedFilter.RegularExpression;
                    OnPropertyChanged("FilterRegular");

                    m_FilterInclude = m_SelectedFilter.Include;
                    OnPropertyChanged("FilterInclude");

                    m_FilterExclude = m_SelectedFilter.Exclude;
                    OnPropertyChanged("FilterExclude");

                    m_FilterContainFile = m_SelectedFilter.ContainFile;
                    OnPropertyChanged("FilterContainFile");

                    m_FilterContainContainDirectory = m_SelectedFilter.ContainDirectory;
                    OnPropertyChanged("FilterContainDirectory");
                }
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool FilterContainDirectory
        {
            get { return m_FilterContainContainDirectory; }
            set
            {
                m_FilterContainContainDirectory = value;
                m_SelectedFilter.ContainDirectory = m_FilterContainContainDirectory;

            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool FilterContainFile
        {
            get { return m_FilterContainFile; }
            set
            {
                m_FilterContainFile = value;
                m_SelectedFilter.ContainFile = m_FilterContainFile;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string FilterExclude
        {
            get { return m_FilterExclude; }
            set
            {
                m_FilterExclude = value;
                m_SelectedFilter.Exclude = m_FilterExclude;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string FilterInclude
        {
            get { return m_FilterInclude; }
            set
            {
                m_FilterInclude = value;
                m_SelectedFilter.Include = m_FilterInclude;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string FilterRegular
        {
            get { return m_FilterRegular; }
            set
            {
                m_FilterRegular = value;
                m_SelectedFilter.RegularExpression = m_FilterRegular;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string FilterName
        {
            get { return m_FilterName; }
            set
            {
                m_FilterName = value;
                m_SelectedFilter.Name = m_FilterName;
                OnPropertyChanged("FilterList");
                OnPropertyChanged("SelectedFilter");
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public ObservableCollection<Garbage> GarbageList
        {
            get { return m_GarbageList; }
        }

        [System.Xml.Serialization.XmlIgnore]
        public int GarbageCount
        {
            get { return m_GarbageList.Count; }
        }

        public async void Scan()
        {
            try
            {
                Progress = 0;
                OnPropertyChanged("Progress");
                startTimer();
                GarbageList.Clear();
                OnPropertyChanged("GarbageCount");
                int round = 100 / FilterList.Count;

                foreach (var filter in FilterList)
                {
                    if (filter.IsChecked)
                    {
                        filter.Waiting = Visibility.Visible;
                        List<Garbage> result = await Task<List<Garbage>>.Factory.StartNew(()=>ScanByEverything(filter));
                        foreach (var garbage in result)
                        {
                            GarbageList.Add(garbage);
                        }
                        OnPropertyChanged("GarbageList");
                        OnPropertyChanged("GarbageCount");

                        filter.Waiting = Visibility.Collapsed;

                        Progress += round;
                        OnPropertyChanged("Progress");
                    }
                }
                Progress = 100;
                OnPropertyChanged("Progress");
            }
            finally
            {
                stopTimer();
            }
        }

        private List<Garbage> ScanByEverything(Filter filter)
        {
            List<Garbage> result = new List<Garbage>();
            EverythingWraper.Everything_SetRegex(true);
            EverythingWraper.Everything_SetSearch(filter.RegularExpression);

            EverythingWraper.Everything_Query(true);

            // sort by path
            EverythingWraper.Everything_SortResultsByPath();

            int bufsize = 260;
            StringBuilder buf = new StringBuilder(bufsize);

            // loop through the results, adding each result to the list box.
            int totalNumber = EverythingWraper.Everything_GetNumResults();
            for (int i = 0; i < totalNumber; i++)
            {
                if (EverythingWraper.Everything_IsFolderResult(i) && !filter.ContainDirectory)
                {
                    continue;
                }
                if (EverythingWraper.Everything_IsFileResult(i) && !filter.ContainFile)
                {
                    continue;
                }

                EverythingWraper.Everything_GetResultFullPathName(i, buf, bufsize);
                string path = buf.ToString();

                if (filter.ShouldSkip(path))
                {
                    continue;
                }
                
                Garbage garbage = new Garbage(path);
                result.Add(garbage);
            }
            return result;
        }

        public void CalculateSize()
        {
            List<Task> tasks = new List<Task>();
            foreach (var garbage in GarbageList)
            {
                tasks.Add(Task.Factory.StartNew(() => garbage.CalculateSize()));
            }
            Task.WaitAll(tasks.ToArray());
            double size = 0;
            foreach (var garbage in GarbageList)
            {
                size += garbage.Size >= 0 ? garbage.Size : 0;
            }
            TotalSize = FileHandler.GetSizeString(size);
            OnPropertyChanged("TotalSize");
            OnPropertyChanged("GarbageList");
        }

        [System.Xml.Serialization.XmlIgnore]
        public string TotalSize { get; private set; }

        public void Free()
        {
            List<string> toDelete = new List<string>();
            for (int i = GarbageList.Count-1; i >= 0; i--)
            {
                if(GarbageList[i].IsChecked)
                {
                    toDelete.Add(GarbageList[i].Path);
                }
            }

            Parallel.ForEach(toDelete, FileHandler.DeletePath);

            GarbageList.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }


        [System.Xml.Serialization.XmlIgnore]
        public int Progress { get; private set; }

        [System.Xml.Serialization.XmlIgnore]
        public string ElapseTime { get; private set; }


        private void startTimer()
        {
            m_Timer = new DispatcherTimer();
            m_Timer.Tick += updateElapseTime;
            m_Timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            m_StopWatch = new Stopwatch();
            m_StopWatch.Start();
            m_Timer.Start();
        }

        private void stopTimer()
        {
            m_Timer.Tick -= updateElapseTime;
            m_Timer.Stop();
            m_StopWatch.Stop();
        }

        private Stopwatch m_StopWatch;
        private DispatcherTimer m_Timer;
        private void updateElapseTime(object sender, EventArgs e)
        {
            ElapseTime = m_StopWatch.Elapsed.TotalSeconds.ToString("F2"); 
            OnPropertyChanged("ElapseTime");
        }
    }
}