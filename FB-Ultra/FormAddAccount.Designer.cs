namespace FB_Ultra
{
    partial class FormAddAccount
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
            btn_save = new Button();
            groupBox1 = new GroupBox();
            rtb_accounts = new RichTextBox();
            groupBox2 = new GroupBox();
            rtb_proxies = new RichTextBox();
            groupBox3 = new GroupBox();
            cbo_AccountFormat = new ComboBox();
            groupBox4 = new GroupBox();
            rdo_ipXoayV4 = new RadioButton();
            rdo_ipV6 = new RadioButton();
            rdo_ipV4 = new RadioButton();
            btn_cancel = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // btn_save
            // 
            btn_save.Location = new Point(533, 361);
            btn_save.Name = "btn_save";
            btn_save.Size = new Size(95, 39);
            btn_save.TabIndex = 2;
            btn_save.Text = "Thêm";
            btn_save.UseVisualStyleBackColor = true;
            btn_save.Click += btn_save_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rtb_accounts);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(450, 244);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Nhập Tài Khoản";
            // 
            // rtb_accounts
            // 
            rtb_accounts.Location = new Point(0, 22);
            rtb_accounts.Name = "rtb_accounts";
            rtb_accounts.Size = new Size(450, 222);
            rtb_accounts.TabIndex = 0;
            rtb_accounts.Text = "";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(rtb_proxies);
            groupBox2.Location = new Point(468, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(283, 244);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "Nhập Proxy";
            // 
            // rtb_proxies
            // 
            rtb_proxies.Location = new Point(0, 22);
            rtb_proxies.Name = "rtb_proxies";
            rtb_proxies.Size = new Size(283, 222);
            rtb_proxies.TabIndex = 1;
            rtb_proxies.Text = "";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(cbo_AccountFormat);
            groupBox3.Location = new Point(12, 271);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(450, 65);
            groupBox3.TabIndex = 7;
            groupBox3.TabStop = false;
            groupBox3.Text = "Chọn Định Dạng Tài Khoản";
            // 
            // cbo_AccountFormat
            // 
            cbo_AccountFormat.FormattingEnabled = true;
            cbo_AccountFormat.Items.AddRange(new object[] { "UID | Pass | 2fa | Mail | PassMail | Cookie | Token", "UID | Password | Cookie | Email | Pass Email", "UID | Pass | Cookie | Token", "UID | Pass | 2FA | Cookie | Token", "UID | Pass | 2FA | Cookie | Token | Mail | PassMail | RefreshToken | ClientID | Name" });
            cbo_AccountFormat.Location = new Point(6, 25);
            cbo_AccountFormat.Name = "cbo_AccountFormat";
            cbo_AccountFormat.Size = new Size(438, 23);
            cbo_AccountFormat.TabIndex = 2;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(rdo_ipXoayV4);
            groupBox4.Controls.Add(rdo_ipV6);
            groupBox4.Controls.Add(rdo_ipV4);
            groupBox4.Location = new Point(468, 271);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(283, 65);
            groupBox4.TabIndex = 8;
            groupBox4.TabStop = false;
            groupBox4.Text = "Chọn Định Dạng Proxy";
            // 
            // rdo_ipXoayV4
            // 
            rdo_ipXoayV4.AutoSize = true;
            rdo_ipXoayV4.Location = new Point(197, 25);
            rdo_ipXoayV4.Name = "rdo_ipXoayV4";
            rdo_ipXoayV4.Size = new Size(80, 19);
            rdo_ipXoayV4.TabIndex = 2;
            rdo_ipXoayV4.Text = "IP Xoay V4";
            rdo_ipXoayV4.UseVisualStyleBackColor = true;
            // 
            // rdo_ipV6
            // 
            rdo_ipV6.AutoSize = true;
            rdo_ipV6.Location = new Point(109, 25);
            rdo_ipV6.Name = "rdo_ipV6";
            rdo_ipV6.Size = new Size(51, 19);
            rdo_ipV6.TabIndex = 1;
            rdo_ipV6.Text = "IP V6";
            rdo_ipV6.UseVisualStyleBackColor = true;
            // 
            // rdo_ipV4
            // 
            rdo_ipV4.AutoSize = true;
            rdo_ipV4.Checked = true;
            rdo_ipV4.Location = new Point(6, 25);
            rdo_ipV4.Name = "rdo_ipV4";
            rdo_ipV4.Size = new Size(51, 19);
            rdo_ipV4.TabIndex = 0;
            rdo_ipV4.TabStop = true;
            rdo_ipV4.Text = "IP V4";
            rdo_ipV4.UseVisualStyleBackColor = true;
            // 
            // btn_cancel
            // 
            btn_cancel.Location = new Point(650, 361);
            btn_cancel.Name = "btn_cancel";
            btn_cancel.Size = new Size(95, 39);
            btn_cancel.TabIndex = 9;
            btn_cancel.Text = "Hủy";
            btn_cancel.UseVisualStyleBackColor = true;
            btn_cancel.Click += btn_cancel_Click;
            // 
            // FormAddAccount
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(763, 412);
            Controls.Add(btn_cancel);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btn_save);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FormAddAccount";
            Text = "Thêm Tài Khoản";
            Load += FormAddAccount_Load;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button btn_save;
        private GroupBox groupBox1;
        private RichTextBox rtb_accounts;
        private GroupBox groupBox2;
        private RichTextBox rtb_proxies;
        private GroupBox groupBox3;
        private ComboBox cbo_AccountFormat;
        private GroupBox groupBox4;
        private RadioButton rdo_ipV6;
        private RadioButton rdo_ipV4;
        private RadioButton rdo_ipXoayV4;
        private Button btn_cancel;
    }
}