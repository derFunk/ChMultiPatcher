using System;
using System.IO;

namespace ChPatchCreator
{
    internal class TestHelpers
    {
        static Random rng = new Random();

        internal static void CreateRandomFile(int sizeInMb, string fileName)
        {
            var data = CreateRandomData(sizeInMb);
            rng.NextBytes(data);
            File.WriteAllBytes(fileName, data);
        }

        internal static byte[] CreateRandomData(int sizeInMb)
        {
            return new byte[sizeInMb * 1024 * 1024];
        }

        internal static int GetRandomInt(int max)
        {
            return (int) (rng.NextDouble()*max);
        }
    }
}
