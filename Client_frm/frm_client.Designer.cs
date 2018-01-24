namespace Client_frm
{
    partial class frm_client
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.lst_chat = new System.Windows.Forms.ListBox();
            this.cmd_connect = new System.Windows.Forms.Button();
            this.cmd_loginFrm = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lst_chat
            // 
            this.lst_chat.FormattingEnabled = true;
            this.lst_chat.Location = new System.Drawing.Point(12, 218);
            this.lst_chat.Name = "lst_chat";
            this.lst_chat.Size = new System.Drawing.Size(476, 368);
            this.lst_chat.TabIndex = 0;
            // 
            // cmd_connect
            // 
            this.cmd_connect.Location = new System.Drawing.Point(12, 12);
            this.cmd_connect.Name = "cmd_connect";
            this.cmd_connect.Size = new System.Drawing.Size(128, 49);
            this.cmd_connect.TabIndex = 4;
            this.cmd_connect.Text = "Connect";
            this.cmd_connect.UseVisualStyleBackColor = true;
            this.cmd_connect.Click += new System.EventHandler(this.cmd_connect_Click);
            // 
            // cmd_loginFrm
            // 
            this.cmd_loginFrm.Location = new System.Drawing.Point(640, 12);
            this.cmd_loginFrm.Name = "cmd_loginFrm";
            this.cmd_loginFrm.Size = new System.Drawing.Size(112, 49);
            this.cmd_loginFrm.TabIndex = 18;
            this.cmd_loginFrm.Text = "Login/Register";
            this.cmd_loginFrm.UseVisualStyleBackColor = true;
            this.cmd_loginFrm.Click += new System.EventHandler(this.cmd_loginFrm_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(289, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 49);
            this.button1.TabIndex = 19;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frm_client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 598);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmd_loginFrm);
            this.Controls.Add(this.cmd_connect);
            this.Controls.Add(this.lst_chat);
            this.Name = "frm_client";
            this.Text = "Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_client_FormClosing);
            this.Load += new System.EventHandler(this.frm_client_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lst_chat;
        private System.Windows.Forms.Button cmd_connect;
        private System.Windows.Forms.Button cmd_loginFrm;
        private System.Windows.Forms.Button button1;
    }
}

