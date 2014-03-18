using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ChMultiPatcher.Tools
{
    public class DirectoryTools
    {
        private static readonly string[] m_fileIgnoreFilter = new[] {@"^.*\.svn.*$"};

        private static readonly List<Regex> m_ignoreFileFilters;

        static DirectoryTools()
        {
            m_ignoreFileFilters = new List<Regex>();

            foreach (string regex in m_fileIgnoreFilter)
                m_ignoreFileFilters.Add(new Regex(regex));
        }

        /// <summary>
        /// Makes  DirectoryTools.Combine MONO - Compatible
        /// </summary>
        /// <param name="directory1"></param>
        /// <param name="directory2"></param>
        /// <returns></returns>
        static public string Combine(string directory1, string directory2)
        {
            if (Path.DirectorySeparatorChar == '/' && directory1.Contains("\\"))
                directory1 = directory1.Replace("\\", Path.DirectorySeparatorChar.ToString());

            if (Path.DirectorySeparatorChar == '\\' && directory1.Contains("/"))
                directory1 = directory1.Replace("/", Path.DirectorySeparatorChar.ToString());

            if (Path.DirectorySeparatorChar == '/' && directory2.Contains("\\"))
                directory2 = directory2.Replace("\\", Path.DirectorySeparatorChar.ToString());

            if (Path.DirectorySeparatorChar == '\\' && directory2.Contains("/"))
                directory2 = directory2.Replace("/", Path.DirectorySeparatorChar.ToString());

            return  Path.Combine(directory1, directory2);
        }
        /// <summary>
        /// Creates a fingerprint for the given path, stripping off the directory prefix
        /// until the stripSlashCount-th directory separator
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stripSlashCount"></param>
        /// <returns></returns>
        static public string CreateFingerprint(string path, int stripSlashCount)
        {
            return CreateFingerprint(GetTraversedFilteredStrippedFiles(path, stripSlashCount));
        }

        /// <summary>
        /// Creates a fingerprint for a given filelist. A fingerprint string is unique which enables us to
        /// check if two filelists are identical just by comparing their fingerprints.
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns></returns>
        static public string CreateFingerprint(SortedDictionary<ComparableFileInfo, string> fileList)
        {
            var sb = new StringBuilder();

            foreach (KeyValuePair<ComparableFileInfo, string> kvp in fileList)
            {
                // add all important data to generate a fingerprint. must be reproducable,
                // thus a sorted dictionary should be used.
                sb.Append(kvp.Value);
                sb.Append(Crc32.GetCrc32String(kvp.Key.m_FileInfo.FullName));
            }

            string hash = HashTools.GetMD5(sb.ToString());

            return hash;
        }

        /// <summary>
        /// @see http://msdn.microsoft.com/de-de/library/bb513869.aspx
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="fileList"></param>
        static public void TraverseDirToList(string dirName, ref List<FileInfo> fileList)
        {
            var root = new DirectoryInfo(dirName);
            FileInfo[] files = null;

            try
            {
                files = root.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                Console.WriteLine(e.Message);
                return;
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            // If we want to open, delete or modify the file, then
            // a try-catch block is required here to handle the case
            // where the file has been deleted since the call to TraverseTree().
            fileList.AddRange(files);


            // Now find all the subdirectories under this directory.
            DirectoryInfo[] subDirs = root.GetDirectories();

            foreach (DirectoryInfo dirInfo in subDirs)
            {
                // Resursive call for each subdirectory.
                TraverseDirToList(dirInfo.FullName, ref fileList);
            }
        }

        private static bool IgnoreFile(string strippedFilePath)
        {
            if (m_ignoreFileFilters == null)
                return false;

            // check for all filters, if one matches,
            // return true to ignore the file
            foreach (Regex ignoreFileFilter in m_ignoreFileFilters)
            {
                if (ignoreFileFilter.IsMatch(strippedFilePath))
                    return true;
            }

            return false;
        }

        public static SortedDictionary<ComparableFileInfo, string> GetTraversedFilteredStrippedFiles(string path, int stripSlashCount)
        {
            var fileList = new List<FileInfo>();
            TraverseDirToList(path, ref fileList);

            var fileInfoToStrippedFilename = new SortedDictionary<ComparableFileInfo, string>();

            foreach (FileInfo fileInfo in fileList)
            {
                string strippedFileName = StripDirectoryPrefix(fileInfo.FullName, stripSlashCount);
                if (!IgnoreFile(strippedFileName))
                    fileInfoToStrippedFilename.Add(new ComparableFileInfo(fileInfo), strippedFileName);
            }

            return fileInfoToStrippedFilename;
        }

        static string StripDirectoryPrefix(string filenOrPath, int stripPrefixDirSlashCount)
        {
            if (stripPrefixDirSlashCount == 0)
                return filenOrPath;

            string pathname = "";
            string filename = "";

            FileAttributes attr = File.GetAttributes(filenOrPath);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                pathname = filenOrPath;
            else
            {
                var fi = new FileInfo(filenOrPath);
                pathname = fi.DirectoryName;
                filename = fi.Name;
            }

            string stripped = pathname.Substring(GetNthIndex(pathname, Path.DirectorySeparatorChar, stripPrefixDirSlashCount) + 1);

            return  DirectoryTools.Combine(stripped, filename);
        }

        static int GetNthIndex(string s, char t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                        return i;
                }
            }
            return s.Length-1; // must be last position, "length" would be one position too much resulting in "startindex cannot be greater.." with substring
        }


        internal static ArrayList CreateRemovedDirectoriesList(SortedDictionary<ComparableFileInfo, string> fromFileList, SortedDictionary<ComparableFileInfo, string> toFileList, int stripSlashCount)
        {
            var directoriesToRemoveOnTarget = new ArrayList();

            // TODO this looks insanely slow and like it could be improved a lot. Do it!
            foreach (var comparableFileInfoFrom in fromFileList.Keys)
            {
                string strippedDirFrom = StripDirectoryPrefix(comparableFileInfoFrom.m_FileInfo.DirectoryName, stripSlashCount);

                bool delete = true;

                foreach (var comparableFileInfoTo in toFileList.Keys)
                {
                    string strippedDirTo = StripDirectoryPrefix(comparableFileInfoTo.m_FileInfo.DirectoryName, stripSlashCount);

                    // 1. Check if the the "from" directory matches the "to" directory exactly
                    // 2. Make sure that the "from" directory is not just a parent folder of the "to" directory.
                    //  -> if its just a parent folder mark it not to be deleted, because we wouldn't have a strippedDirTo if there weren't any files in it!
                    if (strippedDirFrom.Equals(strippedDirTo) || strippedDirTo.StartsWith(strippedDirFrom + Path.DirectorySeparatorChar))
                    {
                        delete = false;
                        break;
                    }
                }

                if (delete && !directoriesToRemoveOnTarget.Contains(strippedDirFrom))
                {
                    directoriesToRemoveOnTarget.Add(strippedDirFrom);
                }
            }

            return directoriesToRemoveOnTarget;
        }
    }
}
