using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ChMultiPatcher.Data;

namespace ChMultiPatcher.PatchSources
{
    class EmbeddedPatchSource : IPatchSource
    {
        public IEnumerable<Patch> GetPatchesFromSource()
        {
            List<string> resNames = new List<string>();

            foreach (string s in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                resNames.Add(s);

            foreach(string s in Assembly.GetCallingAssembly().GetManifestResourceNames())
                resNames.Add(s);

            foreach (string resourceName in resNames)
            {
                if (!resourceName.EndsWith(".patch"))
                    continue;

                using (Stream stream = Assembly.GetExecutingAssembly()
                               .GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        var p = PatchReader.ReadPackedPatchFile(stream);
                        if (p != null)
                            yield return p;
                    }

                }
            }
        }
    }
}
