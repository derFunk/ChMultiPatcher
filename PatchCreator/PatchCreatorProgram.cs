using System;
using System.IO;

namespace ChPatchCreator
{
    class PatchCreatorProgram
    {
        static string m_fromRev;
        static string m_toRev;
        static int m_stripPrefixDirSlashCount;
        static string m_targetFilePathName;
        static string m_projectName;
        static string m_fromSourceDir;
        static string m_toSourceDir;

        static void Main(string[] args)
        {
            if (Start(args))
            {
                Console.WriteLine("Successfully created patch file!");
                Environment.Exit(0);
            }
            else
            {
                Console.Error.WriteLine("Error creating patch file!");
                Environment.Exit(1);
            }
        }

        public static bool Start(string[] args)
        {
            if (!CheckArguments(args))
                Environment.Exit(2);

            // those arg vars must have been checked. revisions must have been converted to int.
            var pc = new PatchCreator(m_projectName, m_fromRev, m_toRev, m_fromSourceDir, m_toSourceDir, m_stripPrefixDirSlashCount);

            return pc.CreatePatchFile(m_targetFilePathName);
        }

        static bool CheckArguments(string[] args)
        {
            if (args.Length < 6)
            {
                ShowHelp();
                return false;
            }

            if (args.Length == 7)
                m_targetFilePathName = args[6];

            m_fromRev = args[1];

            m_toRev = args[2];

            if (!Directory.Exists(args[3]))
            {
                ShowHelp("Directory FromRevisionSourceDirectory does not exist!");
                return false;
            }

            if (!Directory.Exists(args[4]))
            {
                ShowHelp("Directory ToRevisionSourceDirectory does not exist!");
                return false;
            }

            if (!Int32.TryParse(args[5], out m_stripPrefixDirSlashCount))
            {
                ShowHelp("StripPrefixDirectorySlashNumber is not an Integer!");
                return false;
            }

            m_projectName = args[0];
            m_fromSourceDir = args[3];
            m_toSourceDir = args[4];

            return true;
        }

		static void ShowHelp()
		{
			ShowHelp(null);
		}
		
        static void ShowHelp(string error)
        {
            if (!string.IsNullOrEmpty(error))
                Console.WriteLine("Error: " + error);

            Console.WriteLine("Usage: " + new FileInfo(Environment.GetCommandLineArgs()[0]).Name
                + " ProjectName FromRevision ToRevision FromRevisionSourceDirectory ToRevisionSourceDirectory StripPrefixDirectorySlashNumber [Target File or Path]");
            /*
             *  -pnum  or  --strip=num
                    Strip  the  smallest prefix containing num leading slashes from each
                    file name found in the patch file.  A sequence of one or more  adja-
                    cent  slashes  is counted as a single slash.  This controls how file
                    names found in the patch file are treated, in  case  you  keep  your
                    files  in  a  different  directory  than the person who sent out the
                    patch.  For example, supposing the file name in the patch file was
             * */
            Console.WriteLine("Will output: <ProjectName>_<FromRevision>-<ToRevision>.patch - File.");
        }

    }
}
