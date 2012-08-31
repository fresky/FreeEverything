namespace FreeEverything.Model
{
    public class Filter
    {
        private string m_Include;
        private string m_Exclude;

        public Filter()
        {
            Name = "Filter Name";
            RegularExpression = string.Empty;
            Include = string.Empty;
            Exclude = string.Empty;
            ContainFile = false;
            ContainDirectory = false;
        }
        public string Name { get; set; }
        public string RegularExpression { get; set; }

        public string Include
        {
            get { return m_Include; }
            set
            {
                m_Include = value;
                formatDirectory(m_Include);
            }
        }

        public string Exclude
        {
            get { return m_Exclude; }
            set
            {
                m_Exclude = value;
                formatDirectory(m_Exclude);
            }
        }

        private void formatDirectory(string path)
        {
            while (path.EndsWith("\\"))
                path = path.Remove(path.Length - 1);
            if (!string.IsNullOrEmpty(path))
            {
                m_Exclude = path.EndsWith("\\") ? path : (path + "\\");
            }
        }

        public bool ContainFile { get; set; }
        public bool ContainDirectory { get; set; }

        public bool IsChecked { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}