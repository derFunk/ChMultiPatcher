using System.Drawing;

namespace ChMultiPatcherGui
{
    partial class PatcherGui
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.imgLogo = new System.Windows.Forms.PictureBox();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnChooseFolder = new System.Windows.Forms.Button();
            this.btnPatch = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.lblPatchInfo = new System.Windows.Forms.Label();
            this.prgbarPatch = new System.Windows.Forms.ProgressBar();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblProgressInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // imgLogo
            // 
            this.imgLogo.Location = new System.Drawing.Point(12, 12);
            this.imgLogo.Name = "imgLogo";
            this.imgLogo.Size = new System.Drawing.Size(128, 128);
            this.imgLogo.TabIndex = 0;
            this.imgLogo.TabStop = false;
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.Location = new System.Drawing.Point(146, 12);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(264, 16);
            this.lblWelcome.TabIndex = 1;
            this.lblWelcome.Text = "Welcome to the Chimera Multipatcher";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(146, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Please choose the directory to use the patch on:";
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(149, 64);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(300, 20);
            this.txtFolder.TabIndex = 3;
            this.txtFolder.TextChanged += new System.EventHandler(this.txtFolder_TextChanged);
            // 
            // btnChooseFolder
            // 
            this.btnChooseFolder.Location = new System.Drawing.Point(455, 61);
            this.btnChooseFolder.Name = "btnChooseFolder";
            this.btnChooseFolder.Size = new System.Drawing.Size(117, 23);
            this.btnChooseFolder.TabIndex = 4;
            this.btnChooseFolder.Text = "Choose Folder";
            this.btnChooseFolder.UseVisualStyleBackColor = true;
            this.btnChooseFolder.Click += new System.EventHandler(this.btnChooseFolder_Click);
            // 
            // btnPatch
            // 
            this.btnPatch.Enabled = false;
            this.btnPatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPatch.ForeColor = System.Drawing.Color.Green;
            this.btnPatch.Location = new System.Drawing.Point(393, 90);
            this.btnPatch.Name = "btnPatch";
            this.btnPatch.Size = new System.Drawing.Size(179, 40);
            this.btnPatch.TabIndex = 5;
            this.btnPatch.Text = "Update!";
            this.btnPatch.UseVisualStyleBackColor = true;
            this.btnPatch.Click += new System.EventHandler(this.btnPatch_Click);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(150, 88);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 6;
            // 
            // lblPatchInfo
            // 
            this.lblPatchInfo.AutoSize = true;
            this.lblPatchInfo.Location = new System.Drawing.Point(12, 146);
            this.lblPatchInfo.Name = "lblPatchInfo";
            this.lblPatchInfo.Size = new System.Drawing.Size(85, 13);
            this.lblPatchInfo.TabIndex = 7;
            this.lblPatchInfo.Text = "Patch count info";
            this.lblPatchInfo.Visible = false;
            // 
            // prgbarPatch
            // 
            this.prgbarPatch.Location = new System.Drawing.Point(149, 90);
            this.prgbarPatch.Name = "prgbarPatch";
            this.prgbarPatch.Size = new System.Drawing.Size(423, 30);
            this.prgbarPatch.TabIndex = 8;
            this.prgbarPatch.Visible = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(455, 136);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(117, 23);
            this.btnExit.TabIndex = 9;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblProgressInfo
            // 
            this.lblProgressInfo.AutoSize = true;
            this.lblProgressInfo.Location = new System.Drawing.Point(146, 121);
            this.lblProgressInfo.Name = "lblProgressInfo";
            this.lblProgressInfo.Size = new System.Drawing.Size(97, 13);
            this.lblProgressInfo.TabIndex = 10;
            this.lblProgressInfo.Text = "patch progress info";
            this.lblProgressInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblProgressInfo.Visible = false;
            // 
            // PatcherGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 172);
            this.Controls.Add(this.lblProgressInfo);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.prgbarPatch);
            this.Controls.Add(this.lblPatchInfo);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnPatch);
            this.Controls.Add(this.btnChooseFolder);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.imgLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "PatcherGui";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.PictureBox imgLogo;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnChooseFolder;
        private System.Windows.Forms.Button btnPatch;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblPatchInfo;
        private System.Windows.Forms.ProgressBar prgbarPatch;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblProgressInfo;
    }
}

