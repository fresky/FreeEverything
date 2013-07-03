using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace FreeEverything
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GarbageCan m_GarbageCan;
        private const string GarbagecanXML = "GarbageCan.xml";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(GarbageCan));
                using (Stream stream = File.Open(GarbagecanXML, FileMode.OpenOrCreate))
                {
                    m_GarbageCan = (GarbageCan)xmlSerializer.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                m_GarbageCan = new GarbageCan();
            }
            DataContext = m_GarbageCan;
            EverythingWraper.StartEverything();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null) 
                m_GarbageCan.SelectedFilter = listBox.SelectedItem as Filter;
        }


        private void New_OnClick(object sender, RoutedEventArgs e)
        {
            m_GarbageCan.FilterList.Add(new Filter());
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(GarbageCan));
            using (Stream stream = File.Open(GarbagecanXML, FileMode.Create))
            {
                xmlSerializer.Serialize(stream, m_GarbageCan);
            }
            EverythingWraper.KillEverything();
        }

        private void Scan_OnClick(object sender, RoutedEventArgs e)
        {
            m_GarbageCan.Scan();
        }

        private void CalculateSize_OnClick(object sender, RoutedEventArgs e)
        {
            m_GarbageCan.CalculateSize();
        }

        private void Free_OnClick(object sender, RoutedEventArgs e)
        {
            m_GarbageCan.Free();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            m_GarbageCan.FilterList.Remove(m_GarbageCan.SelectedFilter);
            m_GarbageCan.SelectedFilter = m_GarbageCan.FilterList[0];
        }
    }
}
