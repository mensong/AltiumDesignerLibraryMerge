using OriginalCircuit.AltiumSharp;
using System.IO;

namespace AdLibraryMerge
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: \"主文件\" \"输出文件\" \"子文件1\" \"子文件2\"");
                return;
            }

            string mainFile = args[0];
            string outputFile = args[1];
            List<string> subFiles = new List<string>();
            for (int i = 2; i < args.Length; i++)
            {
                subFiles.Add(args[i]);
            }

            bool sucess = false;
            string errMsg;
            var libType = Path.GetExtension(mainFile);
            if (libType.Equals(".schlib", StringComparison.OrdinalIgnoreCase))
            {
                sucess = mergeSchLib(mainFile, outputFile, subFiles, out errMsg);
            }
            else if (libType.Equals(".pcblib", StringComparison.OrdinalIgnoreCase))
            {
                sucess = mergePcbLib(mainFile, outputFile, subFiles, out errMsg);
            }
            
            if (sucess)
            {
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("Failed");
            }
        }

        private static bool mergeSchLib(string mainFile, string outputFile, List<string> subFiles, 
            out string errMsg)
        {
            errMsg = "";

            using (var readerMain = new SchLibReader())
            {
                var libMain = readerMain.Read(mainFile);
                if (libMain == null)
                {
                    errMsg = "Open mainFile error";
                    return false;
                }

                foreach (var subFile in subFiles)
                {
                    using (var readerSub = new SchLibReader())
                    {
                        var libSub = readerSub.Read(subFile);
                        if (libSub == null)
                        {
                            errMsg = "Open " + subFile + " error";
                            return false;
                        }

                        foreach (var item in libSub)
                        {
                            libMain.Add(item);
                        }
                    }
                }

                var outputDir = Path.GetDirectoryName(outputFile);
                if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                    Directory.CreateDirectory(outputDir);

                using (var writer = new SchLibWriter())
                {
                    writer.Write(libMain, outputFile, true);
                }

                return true;
            }
        }

        private static bool mergePcbLib(string mainFile, string outputFile, List<string> subFiles,
            out string errMsg)
        {
            errMsg = "";

            using (var readerMain = new PcbLibReader())
            {
                var libMain = readerMain.Read(mainFile);
                if (libMain == null)
                {
                    errMsg = "Open mainFile error";
                    return false;
                }

                foreach (var subFile in subFiles)
                {
                    using (var readerSub = new PcbLibReader())
                    {
                        var libSub = readerSub.Read(subFile);
                        if (libSub == null)
                        {
                            errMsg = "Open " + subFile + " error";
                            return false;
                        }

                        foreach (var item in libSub)
                        {
                            libMain.Add(item);
                        }
                    }
                }

                var outputDir = Path.GetDirectoryName(outputFile);
                if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                    Directory.CreateDirectory(outputDir);

                using (var writer = new PcbLibWriter())
                {
                    writer.Write(libMain, outputFile, true);
                }

                return true;
            }
        }
    }
}