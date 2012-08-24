using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FreeEverything.Model
{
    public class GarbageCan
    {
        private ObservableCollection<Garbage> m_GarbageList = new ObservableCollection<Garbage>();
        public ObservableCollection<Garbage> GetGarbageList()
        {
            return m_GarbageList;
        }
        public GarbageCan()
        {
            FilterList = new ObservableCollection<Filter>();
        }
        public ObservableCollection<Filter> FilterList { get; set; }

        public void AddEmptyFilter()
        {
            FilterList.Add(new Filter());
        }

        public void Scan()
        {
            m_GarbageList.Clear();
            foreach (var filter in FilterList)
            {
                if (filter.IsChecked)
                {
                    //EverythingWraper.Everything_Reset();
                    EverythingWraper.Everything_SetRegex(true);
                    EverythingWraper.Everything_SetSearch(filter.RegularExpression);

                    EverythingWraper.Everything_Query();

                    // sort by path
                    EverythingWraper.Everything_SortResultsByPath();

                    int bufsize = 260;
                    StringBuilder buf = new StringBuilder(bufsize);

                    // loop through the results, adding each result to the listbox.
                    int totalNumber = EverythingWraper.Everything_GetNumResults();
                    for (int i = 0; i < totalNumber; i++)
                    {
                        bool isFolder = EverythingWraper.Everything_IsFolderResult(i);
                        bool isFile = EverythingWraper.Everything_IsFileResult(i);
                        if (isFile == filter.ContainFile && isFolder == filter.ContainDirectory)
                        {
                            EverythingWraper.Everything_GetResultFullPathName(i, buf, bufsize);
                            m_GarbageList.Add(new Garbage(buf.ToString()));
                        }
                    }
                }
            }
        }

        public void CalculateSize()
        {
            foreach (var garbage in m_GarbageList)
            {
                garbage.CalculateSize();
            }
        }

        public void Free()
        {
            throw new NotImplementedException();
        }
    }
}