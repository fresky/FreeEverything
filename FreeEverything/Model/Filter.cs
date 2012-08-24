namespace FreeEverything.Model
{
    public class Filter
    {
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
        public string Include { get; set; }
        public string Exclude { get; set; }
        public bool ContainFile { get; set; }
        public bool ContainDirectory { get; set; }

        public bool IsChecked { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}