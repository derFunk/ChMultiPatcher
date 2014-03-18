using System;
using System.ComponentModel;
using System.IO;
using ChMultiPatcher.BsDiff;
using ChMultiPatcher.Data;
using ChMultiPatcher.PatchRepositories;
using ChMultiPatcher.PatchSources;
using ChMultiPatcher.Tools;
using System.Collections.Generic;

namespace ChMultiPatcher
{
    class MultiPatcherProgram
    {
        static void Main(string[] args)
        {
            if (Start(args))
            {
                Environment.Exit(0);
            }
            else
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// For unit testing, we need a method with return value
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool Start(string[] args)
        {
            if (!CheckArguments(args))
                return false;

            // Read Patch from File (deserialize it) if argument is given.
            if (args.Length == 2)
            {
                Patch patch = PatchReader.ReadPackedPatchFile(args[1]);

                // Check whether patch can be applied or not
                if (!CheckPatchValid(patch, args[0]))
                    return false;

                return DoPatch(patch, args[0], null);
            }

            var patchRepository = new PatchRepository();
            patchRepository.AddPatchesFromSource(new EmbeddedPatchSource());
            patchRepository.AddPatchesFromSource(new FileSystemPatchSource());

            Console.WriteLine("Found " + patchRepository.GetAvailablePatches().Count + " possible patches");

            Console.WriteLine("");

            // if no patch file given on commandline, take embedded patch resources
            if (TryApplyPatch(args[0], patchRepository))
            {
                Console.WriteLine("Successfully patched " + args[0]);
                return true;
            }

            Console.WriteLine("Error patching " + args[0]);
            return false;
        }


        /// <summary>
        /// Tries to apply any patch available to the path <paramref name="relativeOrAbsoluteTargetPath"/>.
        /// Returns true, if patching was successful, returns fals, if no valid patch was found.
        /// </summary>
        /// <param name="relativeOrAbsoluteTargetPath"></param>
        /// <param name="patchRepository"></param>
        /// <returns></returns>
        public static bool TryApplyPatch(string relativeOrAbsoluteTargetPath, PatchRepository patchRepository)
        {
            // make absolute path, even if relative path is given
            string absoluteTargetPath = Path.GetFullPath(relativeOrAbsoluteTargetPath);
            
            Patch validPatch = null;

            foreach (Patch patch in patchRepository.GetAvailablePatches())
            {
                Console.WriteLine("Applying patch " + patch.Name + ". Patching from " + patch.FromRev + " to " + patch.ToRev);

                if (CheckPatchValid(patch, absoluteTargetPath))
                {
                    validPatch = patch;
                    break;
                }
            }

            if (validPatch == null)
            {
                Console.WriteLine("Patch is not valid for " + relativeOrAbsoluteTargetPath + "!");
                return false;
            }

            Console.WriteLine("Patch is valid for " + relativeOrAbsoluteTargetPath);

            return DoPatch(validPatch, absoluteTargetPath, null);
        }

        /// <summary>
        /// Check if the Patch <paramref name="patch"/> can be applied to the path <paramref name="absoluteTargetPath"/>.
        /// Returns true, if it can.
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="absoluteTargetPath"></param>
        /// <param name="avoidCRCCheckOfTargetFolder"></param>
        /// <returns></returns>
        public static bool CheckPatchValid(Patch patch, string absoluteTargetPath)
        {
            if (patch == null)
                return false;

            bool targetIsPatchedAlready = true; // init
            bool targetIsValid = true;
            
            Console.WriteLine("Checking " + patch.FileDiffs.Count + " diffs");

            // Check all relative paths below the target path. all files must be found to be able to patch.
            foreach (FileDiff filediff in patch.FileDiffs)
            {
                string targetFilePath = DirectoryTools.Combine(absoluteTargetPath, filediff.StrippedFilename);

                // if the 
                //if (!File.Exists(targetFilePath))
                if (filediff.ToCreate)
                    continue;

                if (File.Exists(targetFilePath))
                {
                    var targetFileCrc32 = Crc32.GetCrc32String(targetFilePath);
                    if (targetFileCrc32.Equals(filediff.Crc32FromFile))
                    {
                        // will be always false as soon as one crc32 does not match the targets crc32
                        targetIsPatchedAlready = (targetIsPatchedAlready && targetFileCrc32.Equals(filediff.Crc32ToFile));
                        continue;
                    }
                }

                // TODO ADD ERROR MESSAGE: "BECAUSE SOME OF THE FILES WHICH SHOULD BE PATCHED DON'T CORRESPOND
                // TO THE SAME SOURCE FILE WITHIN THE PATCH

                Console.WriteLine("Expected CRC32 did not match for " + filediff.StrippedFilename);
                targetIsValid = false;
            }

            // TODO: ADD SOME ENUM RETURN VALUE WHICH REPRESENTS ERROR STATE!

            if (targetIsPatchedAlready)
            {
                Console.WriteLine("==============================================");
                Console.WriteLine("Nothing to do! The target is patched already!");
                Console.WriteLine("==============================================");
            }

            return targetIsValid;
        }

        public static Patch IsFolderPatchable(string folder, List<Patch> patches)
        {
            // todo: maybe add some fuzzy logic here to find correct
            // directory one or two levels above or below, if the user
            // was not able to define the correct folder.

            if (!Directory.Exists(folder))
                return null;

            //string thisFingerprint = CreateFingerprint(folder, folder.Split(Path.DirectorySeparatorChar).Length);

            foreach (Patch patch in patches)
            {
                if (CheckPatchValid(patch, folder))
                    return patch;
            }

            return null;
        }

        /// <summary>
        /// Starts patching the <paramref name="baseDirectory"/> with the Patch <paramref name="patch"/>.
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="baseDirectory"></param>
        /// <param name="backgroundWorker">If run asynchronously with a BackgroundWorker, progress is reported. Set to null if no progess needed.</param>
        /// <returns>True, if no error (file to patch does not exists, CRC error at patching) occured.</returns>
        public static bool DoPatch(Patch patch, string baseDirectory, BackgroundWorker backgroundWorker)
        {
            var patchedFilePathToOriginalFilePath = new Dictionary<string, string>();

            double progressReportStepPercent = 0;
            int diffsApplied = 0;

            // Prepare progress report
            if (backgroundWorker != null)
                progressReportStepPercent = 100.0/patch.FileDiffs.Count;


            // Apply patch now to the target directory)
            foreach (FileDiff fileDiff in patch.FileDiffs)
            {
                // Report progress
                if (backgroundWorker != null)
                    backgroundWorker.ReportProgress((int)(++diffsApplied * progressReportStepPercent), fileDiff);

                string originalFilePath =  DirectoryTools.Combine(baseDirectory, fileDiff.StrippedFilename);

                // Check if file must be created new at the target
                if (fileDiff.ToCreate)
                {
                    Console.WriteLine("Creating " + fileDiff.StrippedFilename);

                    string dirRoot = originalFilePath.Substring(0,originalFilePath.LastIndexOf(Path.DirectorySeparatorChar));

                    if (!Directory.Exists(dirRoot))
                        Directory.CreateDirectory(dirRoot);

                    using (FileStream fs = File.Create(originalFilePath, fileDiff.Diff.Length))
                    {
                        fs.Write(fileDiff.Diff, 0, fileDiff.Diff.Length);
                    }

                    continue;
                }

                // if file should not be created, it must exist. if not, something is wrong.
                if (!File.Exists(originalFilePath))
                    throw new Exception("The file " + originalFilePath + " did not exist, but it should!");

                // Check if file must be deleted at the target
                if (fileDiff.ToRemove)
                {
                    Console.WriteLine("Removing " + fileDiff.StrippedFilename);

                    // all .patched files will be deleted later on. 
                    // we're not patching here but only adding the 
                    // .patched -suffix in order to "mark it" for deletion
                    if (File.Exists(originalFilePath))
                        patchedFilePathToOriginalFilePath.Add(originalFilePath + ".patched", originalFilePath);
                    continue;
                }

                if (fileDiff.Diff == null)
                {
                    // no change to this file necessary
                    continue;
                }

                Console.WriteLine("Applying patch to " + fileDiff.StrippedFilename);

                // target patched file name
                string patchedFilePath = originalFilePath + ".patched";

                // Apply patch
                using (var inputData = File.Open(originalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // overwrite old file.
                    using (var outputData = new FileStream(patchedFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        var diff = fileDiff;
                        BinaryPatchUtility.Apply(inputData, () => new MemoryStream(diff.Diff), outputData);
                    }
                }
                
                // check if crc32 checksum matches
                if (!File.Exists(patchedFilePath))
                {
                    Console.WriteLine("Error: " + patchedFilePath + " does not exist!");
                    continue;
                }

                string patchedFileCrc32 = Crc32.GetCrc32String(patchedFilePath);
                if (!fileDiff.Crc32ToFile.Equals(patchedFileCrc32))
                {
                    string error = "ERROR: CRC32 for " + patchedFilePath + "(" + patchedFileCrc32 +
                                   ") does not match with original file (" + fileDiff.Crc32ToFile + ")";

                    Console.WriteLine(error);

                    throw new Exception(error);
                }
                
                // Transaction - like. Just delete original file and move patched file to original file if no crc errors occured AT ALL!
                patchedFilePathToOriginalFilePath.Add(patchedFilePath, originalFilePath);
            }

            // FIRST rename the original file and add suffix .todelete
            // then rename patched file to the original filename (without .patched)
            foreach (KeyValuePair<string, string> kvp in patchedFilePathToOriginalFilePath)
            {
                if (kvp.Key.EndsWith(".patched"))
                {
                    // rename original file to mark it for delete
                    try
                    {
                        File.Move(kvp.Value, kvp.Value + ".todelete");
                    }
                    catch
                    {
                        // error, file already existed? rename not possible.
                        // ignore, these files can only be there from a prior cancelled patch try.
                    }
                    try
                    {
                        // rename patched filename to original filename
                        File.Move(kvp.Key, kvp.Value);
                    }
                    catch
                    {
                    }
                }
            }
            
            // THEN DELETE all .deleted and .patched files.
            // TODO: Implement Rollback which renames .todelete back to original if error at move happened
            foreach (KeyValuePair<string, string> kvp in patchedFilePathToOriginalFilePath)
            {
                // File.Delete doesn't throw an exception if the file does not exist.
                File.Delete(kvp.Value + ".todelete");
                File.Delete(kvp.Value + ".patched");
            }

            // Finally, delete all directories from the DirectoriesToRemove - List
            if (patch.DirectoriesToRemove != null && patch.DirectoriesToRemove.Count > 0)
            {
                foreach (string directoryToRemoveStrippedPathName in patch.DirectoriesToRemove)
                {
                    string dir =  DirectoryTools.Combine(baseDirectory, directoryToRemoveStrippedPathName);
                    if (Directory.Exists(dir))
                    {
                        Console.WriteLine("Removing orphaned directory " + directoryToRemoveStrippedPathName);
                        Directory.Delete(dir, true);
                    }
                }
            }
            return true;
        }


        static bool CheckArguments(string[] args)
        {
            if (args.Length < 1)
            {
                ShowHelp();
                return false;
            }

            if (args.Length == 2)
            {
                if (!File.Exists(args[1]))
                {
                    ShowHelp("InputPatchFile " + args[1] + " does not exist!");
                    return false;
                }
            }

            if (!Directory.Exists(args[0]))
            {
                ShowHelp("Directory ToRevisionSourceDirectory does not exist!");
                return false;
            }

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

            Console.WriteLine("Usage: " + Environment.GetCommandLineArgs()[0] + " FolderToPatch [InputPatchFile]");
        }
    }
}
