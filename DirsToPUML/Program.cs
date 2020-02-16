using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DirsToPUML
{
    internal class Program
    {
        static StringBuilder _puml = new StringBuilder();
        static bool _isRoot = true;
        static List<string> _notPrintExtensionsOrDirs = new List<string>();
        static int _level = 1;

        public static void DirToPuml(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);

            if (_isRoot)
            {
                _puml.Append($"@startmindmap\n* {dirInfo.Name}\n");
                _isRoot = false;
                _level++;
            }
            
            FileSystemInfo[] subDirsAndFilesInfo = dirInfo.GetFileSystemInfos();
            string symbol = new string('*', _level);
            
            foreach (FileSystemInfo FSInfo in subDirsAndFilesInfo)
            {
                if (FSInfo is DirectoryInfo && !_notPrintExtensionsOrDirs.Contains(FSInfo.Name))
                {
                    _puml.Append( symbol + $" {FSInfo.Name}\n");
                    _level++;
                    DirToPuml(FSInfo.FullName);
                }
                else
                {
                    bool isPrinted = true;
                    foreach (string ex in _notPrintExtensionsOrDirs)
                    {
                        if (FSInfo.Name.EndsWith(ex))
                        {
                            isPrinted = false;
                            break;
                        }
                    }
                    if (isPrinted) _puml.Append(symbol + $"_ {FSInfo.Name}\n");
                }
            }

            _level--;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Please input the directory:");
            string targetDir = Console.ReadLine();
            Console.WriteLine("Please input the extensions or directories not printed with spaces as gaps(for example: .git .sln .meta):");
            _notPrintExtensionsOrDirs = new List<string>(Console.ReadLine().Split(' '));
            DirToPuml(targetDir);
            _puml.Append("@endmindmap");
            StreamWriter writer = new StreamWriter("out.puml");
            using (writer)
            {
                writer.Write(_puml.ToString());
            }
        }
    }
}