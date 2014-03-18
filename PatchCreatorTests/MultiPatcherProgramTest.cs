using System.Collections.Generic;
using System.IO;
using ChMultiPatcher;
using ChMultiPatcher.PatchRepositories;
using ChMultiPatcher.PatchSources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChPatchCreator;
using ChMultiPatcher.Tools;

namespace PatchCreatorTests
{
    /// <summary>
    ///This is a test class for MultiPatcherProgramTest and is intended
    ///to contain all MultiPatcherProgramTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MultiPatcherProgramTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        ///A test for Start
        ///</summary>
        [TestMethod()]
        public void MultiPatcherStartTest()
        {
            string cdir = Directory.GetCurrentDirectory();

            string origDir =  Path.Combine(cdir, "orig");
            string targetDir = Path.Combine(cdir, "target");
            
            var listOfFiles = new List<string>();

            // create orig random files
            for (int i = 0; i < 20; i++)
            {
                string origFilePath = Path.Combine(origDir, Path.GetTempFileName());
                listOfFiles.Add(origFilePath);
                TestHelpers.CreateRandomFile(2, origFilePath);

            }

            string targetFilePath = Path.Combine(targetDir, "file.bin");

            TestHelpers.CreateRandomFile(2, targetFilePath);

            var origFileContent = File.ReadAllBytes(targetFilePath);

            TestHelpers.CreateRandomFile(3, Path.Combine(cdir, "origfile.bin"));



            string[] args = {
                            cdir + @"\RevA",
                            cdir + @"\Patch\testpatch_reva-revb.patch",
                            };

               Assert.IsTrue(MultiPatcherProgram.Start(args));
        }

        
        [TestMethod()]
        public void TryApplyEmbeddedPatchTest()
        {
            string dir = Directory.GetCurrentDirectory();

            var patchRepository = new PatchRepository();
            patchRepository.AddPatchesFromSource(new EmbeddedPatchSource());
            patchRepository.AddPatchesFromSource(new FileSystemPatchSource());

            Assert.IsTrue(MultiPatcherProgram.TryApplyPatch(@"RevA", patchRepository));
        }
    }
}
