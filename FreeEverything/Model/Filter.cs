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
                if (!string.IsNullOrEmpty(m_Include))
                {
                    m_Include = m_Include.EndsWith("\\") ? m_Include : m_Include + "\\";
                }
            }
        }

        public string Exclude
        {
            get { return m_Exclude; }
            set
            {
                m_Exclude = value;
                if (!string.IsNullOrEmpty(m_Exclude))
                {
                    m_Exclude = m_Include.EndsWith("\\") ? m_Exclude : m_Exclude + "\\";
                }
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