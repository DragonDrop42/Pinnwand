namespace Client_frm
{
    partial class frm_register
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
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_tel = new System.Windows.Forms.TextBox();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.txt_vName = new System.Windows.Forms.TextBox();
            this.label_register = new System.Windows.Forms.Label();
            this.cmd_register = new System.Windows.Forms.Button();
            this.txt_newPass = new System.Windows.Forms.TextBox();
            this.txt_newEmail = new System.Windows.Forms.TextBox();
            this.cKlassen = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 136);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "Tel.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 28;
            this.label5.Text = "Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Vorname";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Email";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 188);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Passwort";
            // 
            // txt_tel
            // 
            this.txt_tel.Location = new System.Drawing.Point(83, 133);
            this.txt_tel.Name = "txt_tel";
            this.txt_tel.Size = new System.Drawing.Size(100, 20);
            this.txt_tel.TabIndex = 24;
            // 
            // txt_name
            // 
            this.txt_name.Location = new System.Drawing.Point(83, 77);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(100, 20);
            this.txt_name.TabIndex = 23;
            // 
            // txt_vName
            // 
            this.txt_vName.Location = new System.Drawing.Point(83, 50);
            this.txt_vName.Name = "txt_vName";
            this.txt_vName.Size = new System.Drawing.Size(100, 20);
            this.txt_vName.TabIndex = 22;
            // 
            // label_register
            // 
            this.label_register.AutoSize = true;
            this.label_register.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_register.Location = new System.Drawing.Point(24, 20);
            this.label_register.Name = "label_register";
            this.label_register.Size = new System.Drawing.Size(67, 16);
            this.label_register.TabIndex = 21;
            this.label_register.Text = "Register";
            // 
            // cmd_register
            // 
            this.cmd_register.Location = new System.Drawing.Point(108, 211);
            this.cmd_register.Name = "cmd_register";
            this.cmd_register.Size = new System.Drawing.Size(75, 23);
            this.cmd_register.TabIndex = 20;
            this.cmd_register.Text = "Senden";
            this.cmd_register.UseVisualStyleBackColor = true;
            this.cmd_register.Click += new System.EventHandler(this.cmd_register_Click);
            // 
            // txt_newPass
            // 
            this.txt_newPass.Location = new System.Drawing.Point(83, 185);
            this.txt_newPass.Name = "txt_newPass";
            this.txt_newPass.Size = new System.Drawing.Size(100, 20);
            this.txt_newPass.TabIndex = 19;
            // 
            // txt_newEmail
            // 
            this.txt_newEmail.Location = new System.Drawing.Point(83, 159);
            this.txt_newEmail.Name = "txt_newEmail";
            this.txt_newEmail.Size = new System.Drawing.Size(100, 20);
            this.txt_newEmail.TabIndex = 18;
            // 
            // cKlassen
            // 
            this.cKlassen.FormattingEnabled = true;
            this.cKlassen.Location = new System.Drawing.Point(83, 106);
            this.cKlassen.Name = "cKlassen";
            this.cKlassen.Size = new System.Drawing.Size(100, 21);
            this.cKlassen.TabIndex = 30;
            // 
            // frm_register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 263);
            this.Controls.Add(this.cKlassen);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_tel);
            this.Controls.Add(this.txt_name);
            this.Controls.Add(this.txt_vName);
            this.Controls.Add(this.label_register);
            this.Controls.Add(this.cmd_register);
            this.Controls.Add(this.txt_newPass);
            this.Controls.Add(this.txt_newEmail);
            this.Name = "frm_register";
            this.Text = "frm_register";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_tel;
        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.TextBox txt_vName;
        private System.Windows.Forms.Label label_register;
        private System.Windows.Forms.Button cmd_register;
        private System.Windows.Forms.TextBox txt_newPass;
        private System.Windows.Forms.TextBox txt_newEmail;
        private System.Windows.Forms.ComboBox cKlassen;
    }
}