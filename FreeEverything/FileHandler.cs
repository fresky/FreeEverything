using System;
using System.IO;

static internal class FileHandler
{
    public static void DeletePath(string path)
    {
        FileSystemInfo fsi;
        if (File.Exists(path))
        {
            fsi = new FileInfo(path);
        }
        else if (Directory.Exists(path))
        {
            fsi = new DirectoryInfo(path);

        }
        else
        {
            return;
        }
        DeleteFileSystemInfo(fsi);
    }

    public static void DeleteFileSystemInfo(FileSystemInfo fsi)
    {
        try
        {
            fsi.Attributes = FileAttributes.Normal;
            var di = fsi as DirectoryInfo;

            if (di != null)
            {
                foreach (var dirInfo in di.GetFileSystemInfos())
                {
                    DeleteFileSystemInfo(dirInfo);
                }
            }
            fsi.Delete();
        }
        catch (IOException)
        {
        }

    }

    public static string GetSizeString(double size)
    {
        if (size*10>=1024*1024*1024)
        {
            return String.Format("{0} {1}\t\t", (size / (1024 * 1024*1024)).ToString("F1"), "G");
        }
        if (size*10>=1024*1024)
        {
            return String.Format("{0} {1}\t\t", (size / (1024 * 1024)).ToString("F1"), "M");
        }
        if (size*10 >= 1024)
        {
            return String.Format("{0} {1}\t\t", (size / 1024).ToString("F1"), "K");
        }
        return (size < 0 ? "\t\t" : (size.ToString("F1") + " B\t"));
    }
}