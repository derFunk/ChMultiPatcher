using System;
using System.IO;

namespace ChMultiPatcher.Tools
{
    public class ComparableFileInfo : IComparable
    {
        public readonly FileInfo m_FileInfo;
        public readonly string m_Crc32;

        public ComparableFileInfo(FileInfo fileInfo)
        {
            m_FileInfo = fileInfo;
            m_Crc32 = Crc32.GetCrc32String(fileInfo.FullName);
        }

        public int CompareTo(object obj)
        {
            return String.Compare(m_FileInfo.FullName, ((ComparableFileInfo)obj).m_FileInfo.FullName);
        }
    }
}
