using System;
using System.Xml.Serialization;

namespace ChMultiPatcher.Data
{
    [Serializable]
    public class FileDiff
    {
        /// <summary>
        /// Binary diff - bsdiff patch data
        /// </summary>
        public byte[] Diff { get; set; }

        /// <summary>
        /// Relative filename including path to the to be patched file.
        /// </summary>
        public string StrippedFilename { get; set; }

        /// <summary>
        /// True, if the file in the to be patched directory
        /// should be created completely new, because it didn't
        /// exist in the old revision.
        /// </summary>
        public bool ToCreate { get; set; }

        /// <summary>
        /// True, if file in the to be patched directory
        /// should be removed, because it doesn't exist anymore
        /// in the new revision.
        /// </summary>
        public bool ToRemove { get; set; }

        /// <summary>
        /// CRC32 Checksum of the original file which should be patched to
        /// Used to counter-check the patched file.
        /// </summary>
        public string Crc32ToFile { get; set; }

        /// <summary>
        /// CRC32 Checksum of the original file which should be patched from
        /// Used to counter-check the patched file.
        /// </summary>
        public string Crc32FromFile { get; set; }
    }
}
