using System.Resources;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using ChMultiPatcher;
using ChMultiPatcher.Data;
using System.ComponentModel;
using ChMultiPatcher.PatchRepositories;
using ChMultiPatcher.PatchSources;
using System;


namespace ChMultiPatcherGui
{
    public partial class PatcherGui : Form
    {
        private string m_folderName;
        private ResourceManager m_resMan;
        private PatchRepository m_patchRepository;
        private static Patch m_validPatch;
        private string m_FolderChooserLastWorkingDir;

        public PatcherGui()
        {
            InitializeComponent();

            SetFromResources();

            int availablePatches = InitPatchRepository();

            if (availablePatches == 0)
            {
                ShowNoPatchesAvailable();
                Environment.Exit(1);
            }

            SetPatchCountInfoLabel(availablePatches);
            
            lblProgressInfo.Font = SystemFonts.StatusFont;

            m_FolderChooserLastWorkingDir = Environment.CurrentDirectory;
        }

        private void ShowNoPatchesAvailable()
        {
            MessageBox.Show("Unfortunately, there are no patches available you can use.", "Aborting...", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private int InitPatchRepository()
        {
            m_patchRepository = new PatchRepository();
            m_patchRepository.AddPatchesFromSource(new EmbeddedPatchSource());
            m_patchRepository.AddPatchesFromSource(new FileSystemPatchSource());
            return m_patchRepository.GetAvailablePatches().Count;
        }

        private void SetPatchCountInfoLabel(int availablePatches)
        {
            string patchInfoCountText = "patch available";

            if (availablePatches > 0)
                patchInfoCountText += ": " + m_patchRepository.GetAvailablePatches()[0].Name + " " + m_patchRepository.GetAvailablePatches()[0].FromRev + " -> " + m_patchRepository.GetAvailablePatches()[0].ToRev;
            
            if (availablePatches > 1)
                patchInfoCountText = "patches available";

            lblPatchInfo.Text = availablePatches + " " + patchInfoCountText;
            lblPatchInfo.Visible = true;
        }

        private void SetFromResources()
        {
            m_resMan = new ResourceManager("ChMultiPatcherGui.Resources.Resources", GetType().Assembly);

            Text = m_resMan.GetString("PatcherGuiTitle");
            Icon = (Icon) m_resMan.GetObject("AppIcon");
            imgLogo.Image = (Bitmap) m_resMan.GetObject("AppLogo");
        }

        
        private void btnChooseFolder_Click(object sender, System.EventArgs e)
        {
            var fb = new FolderBrowserDialog();

            //assign m_FolderChooserLastWorkingDir new
            if (Directory.Exists(txtFolder.Text))
                m_FolderChooserLastWorkingDir = txtFolder.Text;

            fb.SelectedPath = m_FolderChooserLastWorkingDir;

            DialogResult result = fb.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_folderName = fb.SelectedPath;
                txtFolder.Text = m_folderName;
            }
        }

        private void btnPatch_Click(object sender, System.EventArgs e)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += TryApplyPatchWorker;
            bw.RunWorkerCompleted += TryApplyPatchWorkerCompleted;
            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += PatchingProgressChanged;
            bw.RunWorkerAsync();

            prgbarPatch.Visible = true;
            prgbarPatch.MarqueeAnimationSpeed = 100;
            // prgbarPatch.ForeColor = Color.Red; // just with disabled visual styles
            btnPatch.Visible = false;

            
            btnPatch.Enabled = false;
        }

        private void PatchingProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prgbarPatch.Value = e.ProgressPercentage;

            var currentlyAppliedDiff = e.UserState as FileDiff;

            if (currentlyAppliedDiff == null)
                return;
            
            lblProgressInfo.Visible = true;

            string infoPrefix = "Patching";
            if (currentlyAppliedDiff.ToCreate)
                infoPrefix = "Creating";
            else if (currentlyAppliedDiff.ToRemove)
                infoPrefix = "Deleting";

            double percent = e.ProgressPercentage;

            lblProgressInfo.Text = percent + "% " + infoPrefix + " " + currentlyAppliedDiff.StrippedFilename;


            //using (Graphics gr = prgbarPatch.CreateGraphics())
            //{
            //    gr.DrawString(percent.ToString() + "%",
            //        SystemFonts.DefaultFont,
            //        Brushes.Black,
            //        new PointF(prgbarPatch.Width / 2 - (gr.MeasureString(percent.ToString() + "%",
            //            SystemFonts.DefaultFont).Width / 2.0F),
            //        prgbarPatch.Height / 2 - (gr.MeasureString(percent.ToString() + "%",
            //            SystemFonts.DefaultFont).Height / 2.0F)));
            //}
        }

        private void TryApplyPatchWorker(object sender, DoWorkEventArgs args)
        {
            if (m_validPatch == null)
                return;

            // don't catch exceptions - they will be caught by the BackgroundWorker and added as
            // Error to the RunWorkerCompletedEventArgs
            args.Result = MultiPatcherProgram.DoPatch(m_validPatch, txtFolder.Text, (BackgroundWorker) sender);
        }

        private void TryApplyPatchWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            prgbarPatch.Visible = false;
            lblProgressInfo.Text = "";
            lblProgressInfo.Visible = false;

            if (e.Error != null || !((bool)e.Result))
            {
                btnPatch.Visible = true; // show button again

                btnPatch.Text = "Update!"; // original button text

                MessageBox.Show("An error occured applying the patch!: " + e.Error, m_resMan.GetString("PatcherGuiTitle"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if ((bool)e.Result)
            {
                btnPatch.Text = "Update done!";

                MessageBox.Show("Patch applied sucessfully!", m_resMan.GetString("PatcherGuiTitle"), MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }


        private void txtFolder_TextChanged(object sender, System.EventArgs e)
        {
            if (Directory.Exists(txtFolder.Text))
            {
                m_validPatch = MultiPatcherProgram.IsFolderPatchable(txtFolder.Text, m_patchRepository.GetAvailablePatches());

                if (m_validPatch == null)
                {
                    lblError.Text = "You cannot patch this directory!";
                }
                else
                {
                    lblError.Text = string.Empty;
                    btnPatch.Text = "Start Update to " + m_validPatch.ToRev + "!";
                    btnPatch.Enabled = true;
                    btnPatch.Visible = true;
                }
            }
            else
            {
                lblError.Text = "Directory not found";
                btnPatch.Enabled = false;
            }
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

    }
}
