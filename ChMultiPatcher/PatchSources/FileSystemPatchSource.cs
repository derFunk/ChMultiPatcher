using System.Linq;
using ChMultiPatcher.Data;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;

namespace ChMultiPatcher.PatchSources
{
    class FileSystemPatchSource : IPatchSource
    {
        public IEnumerable<Patch> GetPatchesFromSource()
        {
            string exeFilePath = Assembly.GetExecutingAssembly().Location;

            string path = exeFilePath.Substring(0, exeFilePath.LastIndexOf(Path.DirectorySeparatorChar));

            Console.WriteLine("Looking for patches in " + path);
            var loc1 = GetPatchesFromAssemblyLocationDir(path);

            path = Directory.GetCurrentDirectory();
            Console.WriteLine("Looking for patches in " + path);
            var loc2 = GetPatchesFromAssemblyLocationDir(path);

            return loc1.Union(loc2);
        }

        IEnumerable<Patch> GetPatchesFromAssemblyLocationDir(string path)
        {
            var files = Directory.GetFiles(path);

            foreach (string filePath in files)
            {
                if (filePath.EndsWith(".patch"))
                {
                    Patch p = PatchReader.ReadPackedPatchFile(filePath);
                    if (p != null)
                        yield return p;
                }
            }
        }
    }
}
