using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using FreeEverything.Model;
using GalaSoft.MvvmLight.Command;

namespace FreeEverything.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private GarbageCan m_GarbageCan;
        private Filter m_SelectedFilter;
        private string m_FilterName;
        private string m_FilterRegular;
        private string m_FilterInclude;
        private string m_FilterExclude;
        private bool m_FilterContainFile;
        private bool m_FilterContainContainDirectory;
        private ObservableCollection<Garbage> m_GarbageList;
        private const string GarbagecanXML = "GarbageCan.xml";

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                });

            LoadedCommand = new RelayCommand<EventArgs>(loadedCommand);
            ClosedCommand = new RelayCommand<EventArgs>(closedCommand);
            AddGarbageCan = new RelayCommand(addFilter);
            Scan = new RelayCommand(scan);
            CalculateSize = new RelayCommand(calculateSize);
            Free = new RelayCommand(free);
        }

        public RelayCommand<EventArgs> LoadedCommand { get; private set; }
        public RelayCommand<EventArgs> ClosedCommand { get; private set; }
        public RelayCommand AddGarbageCan { get; private set; }
        public RelayCommand Scan { get; private set; }
        public RelayCommand CalculateSize { get; private set; }
        public RelayCommand Free { get; private set; }

        private void closedCommand(EventArgs e)
        {
            saveGarbageCan(m_GarbageCan);
            EverythingWraper.KillEverything();
        }

        private void loadedCommand(EventArgs e)
        {
            m_GarbageCan = loadGarbageCan();
            FilterList = m_GarbageCan.FilterList;
            RaisePropertyChanged("FilterList");
            EverythingWraper.StartEverything();
        }

        private void addFilter()
        {
            m_GarbageCan.AddEmptyFilter();
            RaisePropertyChanged("FilterList");
        }

        private void scan()
        {
            m_GarbageCan.Scan();
            GarbageList = m_GarbageCan.GetGarbageList();
        }

        private void calculateSize()
        {
            m_GarbageCan.CalculateSize();
            GarbageList = new ObservableCollection<Garbage>();
            GarbageList = m_GarbageCan.GetGarbageList();
        }

        private void free()
        {
            m_GarbageCan.Free();
            GarbageList = m_GarbageCan.GetGarbageList();
        }

        public ObservableCollection<Filter> FilterList { get; set; }
        public Filter SelectedFilter
        {
            get { return m_SelectedFilter; }
            set { 
                m_SelectedFilter = value;
                RaisePropertyChanged("SelectedFilter");

                m_FilterName = m_SelectedFilter.Name;
                RaisePropertyChanged("FilterName");

                m_FilterRegular = m_SelectedFilter.RegularExpression;
                RaisePropertyChanged("FilterRegular");

                m_FilterInclude = m_SelectedFilter.Include;
                RaisePropertyChanged("FilterInclude");

                m_FilterExclude = m_SelectedFilter.Exclude;
                RaisePropertyChanged("FilterExclude");

                m_FilterContainFile = m_SelectedFilter.ContainFile;
                RaisePropertyChanged("FilterContainFile");

                m_FilterContainContainDirectory = m_SelectedFilter.ContainDirectory;
                RaisePropertyChanged("FilterContainDirectory");   
             
                
            }
        }

        public bool FilterContainDirectory
        {
            get { return m_FilterContainContainDirectory; }
            set
            {
                m_FilterContainContainDirectory = value;
                m_SelectedFilter.ContainDirectory = m_FilterContainContainDirectory;
                
            }
        }

        public bool FilterContainFile
        {
            get { return m_FilterContainFile; }
            set
            {
                m_FilterContainFile = value;
                m_SelectedFilter.ContainFile = m_FilterContainFile;
            }
        }

        public string FilterExclude
        {
            get { return m_FilterExclude; }
            set
            {
                m_FilterExclude = value;
                m_SelectedFilter.Exclude = m_FilterExclude;
            }
        }

        public string FilterInclude
        {
            get { return m_FilterInclude; }
            set
            {
                m_FilterInclude = value;
                m_SelectedFilter.Include = m_FilterInclude;
            }
        }

        public string FilterRegular
        {
            get { return m_FilterRegular; }
            set
            {
                m_FilterRegular = value;
                m_SelectedFilter.RegularExpression = m_FilterRegular;
            }
        }
    

        public string FilterName
        {
            get { return m_FilterName; }
            set
            {
                m_FilterName = value;
                m_SelectedFilter.Name = m_FilterName;
                FilterList = m_GarbageCan.FilterList;
                RaisePropertyChanged("FilterList");
                RaisePropertyChanged("SelectedFilter");
            }
        }

        public ObservableCollection<Garbage> GarbageList
        {
            get { return m_GarbageList; }
            set
            {
                m_GarbageList = value;
                RaisePropertyChanged("GarbageList");
                GarbageCount = m_GarbageList.Count;
                RaisePropertyChanged("GarbageCount");
                double size = 0;
                foreach (var garbage in m_GarbageList)
                {

                    size += garbage.Size>=0?garbage.Size:0;
                }
                TotalSize = GarbageCan.GetSizeString(size);
                RaisePropertyChanged("TotalSize");
            }
        }

        public int GarbageCount { get; set; }

        public string TotalSize { get; set; }

        private void saveGarbageCan(GarbageCan can)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(GarbageCan));
            using (Stream stream = File.Open(GarbagecanXML, FileMode.Create))
            {
                xmlSerializer.Serialize(stream, can);
            }
        }

        private GarbageCan loadGarbageCan()
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(GarbageCan));
                using (Stream stream = File.Open(GarbagecanXML, FileMode.OpenOrCreate))
                {
                    return (GarbageCan)xmlSerializer.Deserialize(stream);
                } 
            }
            catch (Exception)
            {              
                return new GarbageCan();    
            }
        }
        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}