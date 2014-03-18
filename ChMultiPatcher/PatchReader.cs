using System.IO;
using System.IO.Compression;
using ChMultiPatcher.Data;
using System;

namespace ChMultiPatcher
{
    class PatchReader
    {
        /// <summary>
        /// Reads patch from file. Returns null if patch was invalid or could not be read.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static public Patch ReadPackedPatchFile(string filename)
        {
            Patch patch;
            try
            {
                using (var fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    patch = ReadPackedPatchFile(fileStream);
                }

                return patch;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        static public Patch ReadPackedPatchFile(Stream stream)
        {
            try
            {
                Patch patch;
                
                using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    patch = PatchDeSerializerService.GetPatchDeSerializer().Deserialize(gzipStream);
                }

                return patch;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
