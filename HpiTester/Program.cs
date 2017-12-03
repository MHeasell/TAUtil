namespace HpiTester
{
    using System;
    using System.IO;
    using TAUtil.Hpi;

    public class Program
    {
        public static void Main(string[] args)
        {
            var command = args[0];

            switch (command)
            {
                case "list":
                    ListHpi(args[1]);
                    break;
                case "extract":
                    ExtractHpi(args[1], args[2], args[3]);
                    break;
            }
        }

        private static void ListHpi(string filename)
        {
            var archive = new HpiArchive(filename);
            var root = archive.GetRoot();

            foreach (var e in root.Entries)
            {
                PrintHpiEntry(e, 0);
            }
        }

        private static void PrintHpiEntry(HpiArchive.DirectoryEntry e, int level)
        {
            if (e is HpiArchive.FileInfo)
            {
                PrintHpiEntry((HpiArchive.FileInfo)e, level);
                return;
            }

            if (e is HpiArchive.DirectoryInfo)
            {
                PrintHpiEntry((HpiArchive.DirectoryInfo)e, level);
                return;
            }

            throw new Exception("Unknown entry class");
        }

        private static void PrintHpiEntry(HpiArchive.FileInfo f, int level)
        {
            Console.Write(new string(' ', level));
            Console.WriteLine("{0} {1} {2}", f.Name, f.Size, f.CompressionScheme);
        }

        private static void PrintHpiEntry(HpiArchive.DirectoryInfo d, int level)
        {
            Console.Write(new string(' ', level));
            Console.WriteLine("{0} directory", d.Name);
            foreach (var e in d.Entries)
            {
                PrintHpiEntry(e, level + 4);
            }
        }

        private static void ExtractHpi(string hpiPath, string filePath, string outputPath)
        {
            var archive = new HpiArchive(hpiPath);
            var fileInfo = archive.FindFile(filePath);
            var buffer = new byte[fileInfo.Size];
            archive.Extract(fileInfo, buffer);

            using (var output = File.OpenWrite(outputPath))
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
