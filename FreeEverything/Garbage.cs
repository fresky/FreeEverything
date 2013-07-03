using System.IO;

namespace FreeEverything
{
    public class Garbage
    {
        public Garbage(string path)
        {
            Path = path;
            Size = -1;
            IsChecked = true;
        }
        public string Path { get; private set; }
        public double Size { get; private set; }

        public bool IsChecked { get; set; }

        public string Display { get { return ToString(); }
        }
        public override string ToString()
        {
            return FileHandler.GetSizeString(Size) + Path;
        }

        public void CalculateSize()
        {
            if(Directory.Exists(Path))
            {
                double result = 0;
                foreach (FileInfo file in new DirectoryInfo(Path).GetFiles("*", SearchOption.AllDirectories))
                    result = result + file.Length;
                Size = result;
            }
            else if (File.Exists(Path))
            {
                Size = new FileInfo(Path).Length;
            }
        }
    }
}