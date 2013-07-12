using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace FreeEverything
{
    public class Filter
    {
        private string m_Include;
        private string m_Exclude;

        public Filter()
        {
            Name = "Filter Name";
            RegularExpression = String.Empty;
            Include = String.Empty;
            Exclude = String.Empty;
            ContainFile = false;
            ContainDirectory = false;
            Waiting = Visibility.Collapsed;
        }
        public string Name { get; set; }
        public string RegularExpression { get; set; }

        public string Include
        {
            get { return m_Include; }
            set
            {
                m_Include = formatDirectory(value);
            }
        }

        public string Exclude
        {
            get { return m_Exclude; }
            set
            {
                m_Exclude = formatDirectory(value);
            }
        }

        private string formatDirectory(string value)
        {
            StringBuilder sb = new StringBuilder();
            string[] pathArray = value.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string p in pathArray)
            {
                var path = p.Trim();
                while (path.EndsWith("\\"))
                    path = path.Remove(path.Length - 1);
                if (!String.IsNullOrEmpty(path))
                {
                    path = path.EndsWith("\\") ? path : (path + "\\");
                }
                sb.Append(path.Trim()).Append(",");
            }
            return sb.ToString();
        }

        public bool ContainFile { get; set; }
        public bool ContainDirectory { get; set; }

        public bool IsChecked { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public Visibility Waiting { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool ShouldSkip(string path)
        {
            if (!String.IsNullOrEmpty(Exclude))
            {
                if (Exclude.Split(new[] {",", ";"}, StringSplitOptions.RemoveEmptyEntries)
                           .Any(s => path.Contains(s.Trim())))
                    return true;
            }

            if (!String.IsNullOrEmpty(Include))
            {
                if (
                    Include.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries)
                           .All(s => !path.Contains(s.Trim())))
                    return true;
            }

            return false;
        }
    }
}