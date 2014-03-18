using System;
using System.Collections.Generic;
using System.IO;
using ChMultiPatcher;
using ChMultiPatcher.BsDiff;
using System.IO.Compression;
using ChMultiPatcher.Data;
using ChMultiPatcher.Tools;

namespace ChPatchCreator
{
	internal class PatchCreator
	{
		private const string PatchFileFormat = "{0}_{1}-{2}_{3}.patch";

		private readonly string m_projectName;
        private readonly string m_fromRev;
        private readonly string m_toRev;
        private readonly string m_fromSourceDir;
        private readonly string m_toSourceDir;
        private readonly int m_stripPrefixDirSlashCount;

        public PatchCreator(string projectName, string fromRev, string toRev, string fromSourceDir, string toSourceDir, int stripPrefixDirSlashCount)
		{
			m_projectName = projectName;
			m_fromRev = fromRev;
			m_toRev = toRev;
			m_fromSourceDir = fromSourceDir;
			m_toSourceDir = toSourceDir;
			m_stripPrefixDirSlashCount = stripPrefixDirSlashCount;
		}
		
		internal bool CreatePatchFile()
		{
			return CreatePatchFile(null);
		}
		
		internal bool CreatePatchFile(string patchFileName)
		{
            if (string.IsNullOrEmpty(patchFileName))
                patchFileName = String.Format(PatchFileFormat, m_projectName.ToLower(), m_fromRev, m_toRev, DateTime.Now.ToString("MM-dd-yy"));

            Console.WriteLine("Creating patch file for " + m_projectName + " from revision " + m_fromRev + " to revision " + m_toRev);

			/**
			 * 1.) Traverse from-dir
			 * 2.) Traverse to-dir
			 * 3.) create diffs for each file
			 * 
			 * Files which are not in ToRev but in FromRev must be deleted
			 * 
			 * */

			// Maps original path to stripped path
			var fromFileList = DirectoryTools.GetTraversedFilteredStrippedFiles(m_fromSourceDir, m_stripPrefixDirSlashCount);
			var toFileList = DirectoryTools.GetTraversedFilteredStrippedFiles(m_toSourceDir, m_stripPrefixDirSlashCount);
            var removedDirectoriesFileList = DirectoryTools.CreateRemovedDirectoriesList(fromFileList, toFileList, m_stripPrefixDirSlashCount);

			Patch p = CreatePatch(fromFileList, toFileList);
		    p.DirectoriesToRemove = removedDirectoriesFileList;
		    p.Name = m_projectName;

			return WritePackedPatchToFile(p, patchFileName);
		}

		private static bool WritePackedPatchToFile(Patch patch, string patchFileName)
		{
			try
			{
				using (var stream = File.Open(patchFileName, FileMode.Create))
				{
					using (var gzipStream = new GZipStream(stream, CompressionMode.Compress))
					{
						PatchDeSerializerService.GetPatchDeSerializer().SerializeIntoStream(patch, gzipStream);
					}
				}

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

		}

		Patch CreatePatch(SortedDictionary<ComparableFileInfo, string> fromFileList, SortedDictionary<ComparableFileInfo, string> toFileList)
		{
			var patch = new Patch(m_fromRev, m_toRev);

			// patch.FromRevStrippedFilePathToCrc32 = new Dictionary<string, string>();
			
			foreach (KeyValuePair<ComparableFileInfo, string> fromFile in fromFileList)
			{
                Console.WriteLine("Calculating file diff for " + fromFile.Value);

				// Populate the dictionary which maps Stripped File Name to Crc32 Checksum
				// patch.FromRevStrippedFilePathToCrc32.Add(fromFile.Value, fromFile.Key.m_Crc32);

				// Initialize this filediff
				var fileDiff = new FileDiff {StrippedFilename = fromFile.Value, Crc32FromFile = fromFile.Key.m_Crc32};

			    // Find out if the from-File is also existent in the to-Files-List. If yes
				// the diff must be calculated. If not, it must be deleted at patch time.
				FileInfo foundToFileinfo = null;
				foreach (KeyValuePair<ComparableFileInfo, string> toFile in toFileList)
				{
					if (toFile.Value == fromFile.Value)
					{
						foundToFileinfo = toFile.Key.m_FileInfo;
						break;
					}
				}

				if (foundToFileinfo == null)
				{
					// file not found in toFileList, must be deleted on the target
					fileDiff.ToRemove = true;
				}
				else
				{
					// File is also existent at destination revision, so create patch.
					var memStream = new MemoryStream();
					BinaryPatchUtility.Create(ReadWholeArray(fromFile.Key.m_FileInfo), ReadWholeArray(foundToFileinfo), memStream);
					fileDiff.Diff = memStream.ToArray();
					// Create CRC32 checksum for the file after the patch was applied, to be able to check validity
					fileDiff.Crc32ToFile = Crc32.GetCrc32String(foundToFileinfo.FullName);
				}

                // Add the diff-information to the patch
				patch.FileDiffs.Add(fileDiff);
			}
			
			// Add all files which are new in the new revision, and were not existend in the old revision
			foreach (KeyValuePair<ComparableFileInfo, string> toFile in toFileList)
			{
				bool found = false;

				foreach (KeyValuePair<ComparableFileInfo, string> fromFile in fromFileList)
				{
					if (fromFile.Value == toFile.Value)
					{
						found = true;
						break;
					}
				}

				// Add new files to the patch
				if (!found)
				{
					var fileDiff = new FileDiff();
					fileDiff.StrippedFilename = toFile.Value;
					fileDiff.Diff = ReadWholeArray(toFile.Key.m_FileInfo);
					fileDiff.ToCreate = true;
					patch.FileDiffs.Add(fileDiff);
				}
			}

			return patch;
		}


		static byte[] ReadWholeArray(FileInfo fileinfo)
		{
			var data = new byte[fileinfo.Length];

			using (FileStream stream = File.OpenRead(fileinfo.FullName))
			{
				int offset = 0;
				int remaining = data.Length;
				while (remaining > 0)
				{
					int read = stream.Read(data, offset, remaining);
					if (read <= 0)
						throw new EndOfStreamException
							(String.Format("End of stream reached with {0} bytes left to read", remaining));
					remaining -= read;
					offset += read;
				}
			}

			return data;
		}

	}
}
