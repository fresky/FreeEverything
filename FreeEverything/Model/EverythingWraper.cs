using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace FreeEverything.Model
{
    
    public class EverythingWraper
    {
        const int EVERYTHING_OK = 0;
        const int EVERYTHING_ERROR_MEMORY = 1;
        const int EVERYTHING_ERROR_IPC = 2;
        const int EVERYTHING_ERROR_REGISTERCLASSEX = 3;
        const int EVERYTHING_ERROR_CREATEWINDOW = 4;
        const int EVERYTHING_ERROR_CREATETHREAD = 5;
        const int EVERYTHING_ERROR_INVALIDINDEX = 6;
        const int EVERYTHING_ERROR_INVALIDCALL = 7;

        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern int Everything_SetSearch(string lpSearchString);
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern void Everything_SetMatchPath(bool bEnable);
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern void Everything_SetMatchCase(bool bEnable);
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern void Everything_SetMatchWholeWord(bool bEnable);
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern void Everything_SetRegex(bool bEnable);
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern void Everything_SetMax(int dwMax);
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern void Everything_SetOffset(int dwOffset);

        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern bool Everything_GetMatchPath();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern bool Everything_GetMatchCase();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern bool Everything_GetMatchWholeWord();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern bool Everything_GetRegex();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern UInt32 Everything_GetMax();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern UInt32 Everything_GetOffset();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern string Everything_GetSearch();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern int Everything_GetLastError();

        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern bool Everything_Query();

        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern void Everything_SortResultsByPath();

        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern int Everything_GetNumFileResults();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern int Everything_GetNumFolderResults();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern int Everything_GetNumResults();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern int Everything_GetTotFileResults();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern int Everything_GetTotFolderResults();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern int Everything_GetTotResults();
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern bool Everything_IsVolumeResult(int nIndex);
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern bool Everything_IsFolderResult(int nIndex);
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern bool Everything_IsFileResult(int nIndex);
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern void Everything_GetResultFullPathName(int nIndex, StringBuilder lpString, int nMaxCount);
        [DllImport(@"ThirdParty\Everything.dll")]
        public static extern void Everything_Reset();

        public static string Search(string keyword)
        {
            Everything_SetSearch(keyword);

            // use our own custom scrollbar... 			
            // Everything_SetMax(listBox1.ClientRectangle.Height / listBox1.ItemHeight);
            // Everything_SetOffset(VerticalScrollBarPosition...);

            // execute the query
            Everything_Query();

            // sort by path
            // Everything_SortResultsByPath();

            int bufsize = 260;
            StringBuilder buf = new StringBuilder(bufsize);

            // loop through the results, adding each result to the listbox.
            if(Everything_GetNumResults()>0)
            {
                // get the result's full path and file name.
                Everything_GetResultFullPathName(0, buf, bufsize);	
                
            }
            return buf.ToString();
        }

        private static bool m_EverythingLaunch;
        public static void StartEverything()
        {
            Regex regex = new Regex(@"Everything([-.0-9])");
            bool found = false;
            foreach (var process in Process.GetProcesses())
            {
                if (regex.Match(process.ProcessName).Success)
                {
                    found = true;
                    break;
                }

            }
            if (!found)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo(@"ThirdParty\Everything.exe");
                processStartInfo.CreateNoWindow = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(processStartInfo);
                m_EverythingLaunch = true;
            }
        }
        public static void KillEverything()
        {
            if (m_EverythingLaunch)
            {
                Regex regex = new Regex(@"Everything([-.0-9])");
                foreach (var process in Process.GetProcesses())
                {
                    if (regex.Match(process.ProcessName).Success)
                    {
                        process.Kill();
                    }

                }
            }
        }
    }
}
