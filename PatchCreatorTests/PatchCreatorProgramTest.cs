using ChPatchCreator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace PatchCreatorTests
{
    
    
    /// <summary>
    ///This is a test class for ProgramTest and is intended
    ///to contain all ProgramTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PatchCreatorProgramTest
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
        ///A test for Main
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PatchCreator.exe")]
        public void PatchCreatorStartTest()
        {
            string dir = Directory.GetCurrentDirectory();

            // strip complete prefix in test, so that only the below paths stay, without the RevX - rootdir (+1)
            int stripSlashCount = dir.Split(Path.DirectorySeparatorChar).Length + 1;

            string[] args = {
                                "TestProject", "1234", "1235", dir + @"\RevA\bin",
                                dir + @"\RevB\bin", stripSlashCount.ToString()
                            };

            Assert.IsTrue(PatchCreatorProgram.Start(args));
        }

    }
}
