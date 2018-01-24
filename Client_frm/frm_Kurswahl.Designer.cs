namespace Client_frm
{
    partial class frm_Kurswahl
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
            this.cLst_kurse = new System.Windows.Forms.CheckedListBox();
            this.cmd_senden = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cLst_kurse
            // 
            this.cLst_kurse.FormattingEnabled = true;
            this.cLst_kurse.Location = new System.Drawing.Point(12, 12);
            this.cLst_kurse.Name = "cLst_kurse";
            this.cLst_kurse.Size = new System.Drawing.Size(141, 184);
            this.cLst_kurse.TabIndex = 0;
            // 
            // cmd_senden
            // 
            this.cmd_senden.Location = new System.Drawing.Point(197, 137);
            this.cmd_senden.Name = "cmd_senden";
            this.cmd_senden.Size = new System.Drawing.Size(75, 59);
            this.cmd_senden.TabIndex = 1;
            this.cmd_senden.Text = "Senden";
            this.cmd_senden.UseVisualStyleBackColor = true;
            this.cmd_senden.Click += new System.EventHandler(this.cmd_senden_Click);
            // 
            // frm_Kurswahl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 203);
            this.Controls.Add(this.cmd_senden);
            this.Controls.Add(this.cLst_kurse);
            this.Name = "frm_Kurswahl";
            this.Text = "frm_Kurswahl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox cLst_kurse;
        private System.Windows.Forms.Button cmd_senden;
    }
}