using System.IO;
using System.Linq;

namespace FreeEverything.Model
{
    public class Garbage
    {
        public Garbage(string path)
        {
            Path = path;
            Size = -1;
        }
        public string Path { get; private set; }
        public double Size { get; private set; }
        public override string ToString()
        {
            return (Size<0? string.Empty: (Size+"\t")) + Path;
        }
        public void CalculateSize()
        {
            if(Directory.Exists(Path))
                Size = new DirectoryInfo(Path).GetFiles(Path, SearchOption.AllDirectories).Aggregate<FileInfo, double>(0, (current, file) => current + file.Length);
            else if (File.Exists(Path))
            {
                Size = new FileInfo(Path).Length;
            }
        }
    }
}