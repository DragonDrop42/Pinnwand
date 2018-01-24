namespace Client_frm
{
    partial class frm_login
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
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmd_login = new System.Windows.Forms.Button();
            this.txt_pass = new System.Windows.Forms.TextBox();
            this.txt_email = new System.Windows.Forms.TextBox();
            this.cmd_register = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 50);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Email";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 76);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Passwort";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 16);
            this.label1.TabIndex = 23;
            this.label1.Text = "Login";
            // 
            // cmd_login
            // 
            this.cmd_login.Location = new System.Drawing.Point(77, 100);
            this.cmd_login.Name = "cmd_login";
            this.cmd_login.Size = new System.Drawing.Size(133, 33);
            this.cmd_login.TabIndex = 22;
            this.cmd_login.Text = "Senden";
            this.cmd_login.UseVisualStyleBackColor = true;
            this.cmd_login.Click += new System.EventHandler(this.cmd_login_Click);
            // 
            // txt_pass
            // 
            this.txt_pass.Location = new System.Drawing.Point(77, 74);
            this.txt_pass.Name = "txt_pass";
            this.txt_pass.Size = new System.Drawing.Size(133, 20);
            this.txt_pass.TabIndex = 21;
            // 
            // txt_email
            // 
            this.txt_email.Location = new System.Drawing.Point(77, 47);
            this.txt_email.Name = "txt_email";
            this.txt_email.Size = new System.Drawing.Size(133, 20);
            this.txt_email.TabIndex = 20;
            // 
            // cmd_register
            // 
            this.cmd_register.Location = new System.Drawing.Point(231, 133);
            this.cmd_register.Name = "cmd_register";
            this.cmd_register.Size = new System.Drawing.Size(81, 31);
            this.cmd_register.TabIndex = 26;
            this.cmd_register.Text = "Registrieren";
            this.cmd_register.UseVisualStyleBackColor = true;
            this.cmd_register.Click += new System.EventHandler(this.cmd_register_Click);
            // 
            // frm_login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 174);
            this.Controls.Add(this.cmd_register);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmd_login);
            this.Controls.Add(this.txt_pass);
            this.Controls.Add(this.txt_email);
            this.Name = "frm_login";
            this.Text = "frm_login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmd_login;
        private System.Windows.Forms.TextBox txt_pass;
        private System.Windows.Forms.TextBox txt_email;
        private System.Windows.Forms.Button cmd_register;
    }
}