Chimera MultiPatcher
===


What it is
---
The Chimera Multipatcher creates a binary diff between two **directories** and lets you apply this patch to a valid target directory.

Since the patch applies the patches bytewise to the files, the target files have to be the exact same version than the files the patch is created for. This is verified by comparing the CRC32 of the target file with the one from the source file.

Components
---
The Chimera Multipatcher currently consists of these components:

 - The PatchCreator.exe: Command line tool to create the patch.
 - The MultiPatcher UI: Provides a nice looking user interface to select the target directory to apply the patch on. Also, it verifies if the target directory is valid to be patched.
 - The Multipatcher CLI Tool: Command line tool to apply the patch on a directory.    

Usage
---

**Patch Creator**

    PatchCreator.exe ProjectName FromRevision ToRevision FromRevisionSourceDirectory ToRevisionSourceDirectory StripPrefixDirectorySlashNumber [Patch target file or path]
    
Arguments:

 - ProjectName: Name of the project this patch is for
 - FromRevision: Identifier (Version Number) of the source version, wherefrom should be patched
 - ToRevision: Identifier (Version Number) of the target version, whereto should be patched
 - FromRevisionSourceDirectory: The directory of the source version, wherefrom should be patched. This is the "Old Data".
 - ToRevisionSourceDirectory: The directory of the target version, whereto should be patched. This is the "New Data".
 - StripPrefixDirectorySlashNumber: This one is most tricky: Its an integer which defines the number of slashes in the *FromRevisionSourceDirectory*, which define the parts of the leading path which is not going to be part of the created patch.
	 - Example:
		 - If you want to create a patch between the files and folders under the directory C:\abc\def\, your StripPrefixDirectorySlashNumber would be "3", because you see 3 slashes (you always have to count the trailing slash in!)   


This will output a patch file named <ProjectName>_<FromRevision>-<ToRevision>.patch.

**Multipatcher UI**

 - This is easy. Make sure you have the ChMultiPatcher.exe and a patch.
	 - The patch can be included into the ChMultiPatcher.exe, making it bigger than ~400kb. The patch can also lay beside the ChMultiPatcher.exe, it's then called ***.patch.
 - Start the ChMultiPatcher.exe by double-clicking it. A UI will oben, asking you for a target directory.
 - This target directory must contain exactly the files which have been used when creating the patch. All files which have to be patched are verified by there CRC32 before applying the patch.
 - If the patch is applicable for the target folder you choose, you can happily start patching. 

**Multipatcher CLI**

`ChMultiPatcher.exe FolderToPatch [InputPatchFile]`

 - This should be self-describing.

How patches can be deployed
---
 1. You can put patch files into the same directory as the Multipatcher.exe. The Multipatcher will find and load them automatically.
 2. Patches can be integrated into the Multipatcher.exe as resources. 
 3. This section is WIP

Running on Mono
---
 - Yes, you can do that. It's as simple as
`mono ChMultiPatcher.exe ...arguments...`
`mono PatchCreator.exe ...arguments...`


Todos
---
- Continue unit test project setup and implementation.  
- Add zip compression to patch file (also, decompress it before applying the patch)
 - Add support for applying multiple patches one after another automatically 
 - Create a UI for the Patch Creator.
 - Maybe better error messages (why did the patch fail?)

Third Party References
---
 - WIP

License
---
 - TBD