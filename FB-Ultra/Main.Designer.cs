namespace FB_Ultra
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            grb_Account = new GroupBox();
            dgv_accounts = new DataGridView();
            col_checkbox = new DataGridViewCheckBoxColumn();
            col_id = new DataGridViewTextBoxColumn();
            col_user = new DataGridViewTextBoxColumn();
            col_pass = new DataGridViewTextBoxColumn();
            col_2fa = new DataGridViewTextBoxColumn();
            col_mail = new DataGridViewTextBoxColumn();
            col_passMail = new DataGridViewTextBoxColumn();
            col_cookie = new DataGridViewTextBoxColumn();
            col_token = new DataGridViewTextBoxColumn();
            col_ipV4 = new DataGridViewTextBoxColumn();
            col_ipV6 = new DataGridViewTextBoxColumn();
            col_ipXoay = new DataGridViewTextBoxColumn();
            col_status = new DataGridViewTextBoxColumn();
            col_userAgent = new DataGridViewTextBoxColumn();
            contextMenuStrip1 = new ContextMenuStrip(components);
            menuSelect = new ToolStripMenuItem();
            menuSelectAll = new ToolStripMenuItem();
            menuDeselect = new ToolStripMenuItem();
            menuDeselectAll = new ToolStripMenuItem();
            menuDelete = new ToolStripMenuItem();
            menuDeleteAll = new ToolStripMenuItem();
            Log = new GroupBox();
            flp_chromeButtons = new FlowLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btn_openChrome = new Button();
            btn_stop = new Button();
            btn_setting = new Button();
            btn_addAcc = new Button();
            btn_editAcc = new Button();
            btn_openAuto = new Button();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            btn_clearRam = new Button();
            radioButton3 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            button1 = new Button();
            textBox1 = new TextBox();
            groupBox3 = new GroupBox();
            label2 = new Label();
            label1 = new Label();
            button2 = new Button();
            textBox2 = new TextBox();
            grb_Account.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_accounts).BeginInit();
            contextMenuStrip1.SuspendLayout();
            Log.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // grb_Account
            // 
            grb_Account.Controls.Add(dgv_accounts);
            grb_Account.Location = new Point(11, 12);
            grb_Account.Name = "grb_Account";
            grb_Account.Size = new Size(1257, 492);
            grb_Account.TabIndex = 1;
            grb_Account.TabStop = false;
            grb_Account.Text = "Tài Khoản";
            // 
            // dgv_accounts
            // 
            dgv_accounts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_accounts.Columns.AddRange(new DataGridViewColumn[] { col_checkbox, col_id, col_user, col_pass, col_2fa, col_mail, col_passMail, col_cookie, col_token, col_ipV4, col_ipV6, col_ipXoay, col_status, col_userAgent });
            dgv_accounts.ContextMenuStrip = contextMenuStrip1;
            dgv_accounts.Location = new Point(-41, 17);
            dgv_accounts.Name = "dgv_accounts";
            dgv_accounts.Size = new Size(1315, 495);
            dgv_accounts.TabIndex = 0;
            dgv_accounts.CellContentClick += dgv_accounts_CellContentClick;
            // 
            // col_checkbox
            // 
            col_checkbox.HeaderText = "#";
            col_checkbox.Name = "col_checkbox";
            col_checkbox.Width = 30;
            // 
            // col_id
            // 
            col_id.HeaderText = "ID";
            col_id.Name = "col_id";
            col_id.Width = 50;
            // 
            // col_user
            // 
            col_user.HeaderText = "User";
            col_user.Name = "col_user";
            col_user.Resizable = DataGridViewTriState.True;
            col_user.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // col_pass
            // 
            col_pass.HeaderText = "Pass";
            col_pass.Name = "col_pass";
            // 
            // col_2fa
            // 
            col_2fa.HeaderText = "2fa";
            col_2fa.Name = "col_2fa";
            // 
            // col_mail
            // 
            col_mail.HeaderText = "Mail";
            col_mail.Name = "col_mail";
            // 
            // col_passMail
            // 
            col_passMail.HeaderText = "PassMail";
            col_passMail.Name = "col_passMail";
            // 
            // col_cookie
            // 
            col_cookie.HeaderText = "Cookie";
            col_cookie.Name = "col_cookie";
            // 
            // col_token
            // 
            col_token.HeaderText = "Token";
            col_token.Name = "col_token";
            // 
            // col_ipV4
            // 
            col_ipV4.HeaderText = "IP V4";
            col_ipV4.Name = "col_ipV4";
            // 
            // col_ipV6
            // 
            col_ipV6.HeaderText = "IP V6";
            col_ipV6.Name = "col_ipV6";
            // 
            // col_ipXoay
            // 
            col_ipXoay.HeaderText = "IP Xoay";
            col_ipXoay.Name = "col_ipXoay";
            // 
            // col_status
            // 
            col_status.HeaderText = "Trạng Thái";
            col_status.Name = "col_status";
            col_status.Width = 175;
            // 
            // col_userAgent
            // 
            col_userAgent.HeaderText = "UserAgent";
            col_userAgent.Name = "col_userAgent";
            col_userAgent.Width = 5;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { menuSelect, menuSelectAll, menuDeselect, menuDeselectAll, menuDelete, menuDeleteAll });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(151, 136);
            // 
            // menuSelect
            // 
            menuSelect.Name = "menuSelect";
            menuSelect.Size = new Size(150, 22);
            menuSelect.Text = "Chọn";
            menuSelect.Click += menuSelect_Click;
            // 
            // menuSelectAll
            // 
            menuSelectAll.Name = "menuSelectAll";
            menuSelectAll.Size = new Size(150, 22);
            menuSelectAll.Text = "Chọn tất cả";
            menuSelectAll.Click += menuSelectAll_Click;
            // 
            // menuDeselect
            // 
            menuDeselect.Name = "menuDeselect";
            menuDeselect.Size = new Size(150, 22);
            menuDeselect.Text = "Bỏ chọn";
            menuDeselect.Click += menuDeselect_Click;
            // 
            // menuDeselectAll
            // 
            menuDeselectAll.Name = "menuDeselectAll";
            menuDeselectAll.Size = new Size(150, 22);
            menuDeselectAll.Text = "Bỏ chọn tất cả";
            menuDeselectAll.Click += menuDeselectAll_Click;
            // 
            // menuDelete
            // 
            menuDelete.Name = "menuDelete";
            menuDelete.Size = new Size(150, 22);
            menuDelete.Text = "Xoá";
            menuDelete.Click += menuDelete_Click;
            // 
            // menuDeleteAll
            // 
            menuDeleteAll.Name = "menuDeleteAll";
            menuDeleteAll.Size = new Size(150, 22);
            menuDeleteAll.Text = "Xoá tất cả";
            menuDeleteAll.Click += menuDeleteAll_Click;
            // 
            // Log
            // 
            Log.Controls.Add(flp_chromeButtons);
            Log.Location = new Point(1274, 12);
            Log.Name = "Log";
            Log.Size = new Size(188, 492);
            Log.TabIndex = 2;
            Log.TabStop = false;
            Log.Text = "Profile Đang Mở";
            // 
            // flp_chromeButtons
            // 
            flp_chromeButtons.AutoScroll = true;
            flp_chromeButtons.Location = new Point(3, 19);
            flp_chromeButtons.Name = "flp_chromeButtons";
            flp_chromeButtons.Size = new Size(182, 467);
            flp_chromeButtons.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btn_openChrome);
            flowLayoutPanel1.Controls.Add(btn_stop);
            flowLayoutPanel1.Controls.Add(btn_setting);
            flowLayoutPanel1.Controls.Add(btn_addAcc);
            flowLayoutPanel1.Controls.Add(btn_editAcc);
            flowLayoutPanel1.Controls.Add(btn_openAuto);
            flowLayoutPanel1.Location = new Point(4, 18);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(788, 50);
            flowLayoutPanel1.TabIndex = 6;
            // 
            // btn_openChrome
            // 
            btn_openChrome.Location = new Point(3, 3);
            btn_openChrome.Name = "btn_openChrome";
            btn_openChrome.Size = new Size(124, 45);
            btn_openChrome.TabIndex = 0;
            btn_openChrome.Text = "Mở Thường / Auto";
            btn_openChrome.UseVisualStyleBackColor = true;
            btn_openChrome.Click += btn_openChrome_Click;
            // 
            // btn_stop
            // 
            btn_stop.Location = new Point(133, 3);
            btn_stop.Name = "btn_stop";
            btn_stop.Size = new Size(124, 45);
            btn_stop.TabIndex = 4;
            btn_stop.Text = "Dừng";
            btn_stop.UseVisualStyleBackColor = true;
            btn_stop.Click += btn_stop_Click;
            // 
            // btn_setting
            // 
            btn_setting.Location = new Point(263, 3);
            btn_setting.Name = "btn_setting";
            btn_setting.Size = new Size(124, 45);
            btn_setting.TabIndex = 3;
            btn_setting.Text = "Cài đặt";
            btn_setting.UseVisualStyleBackColor = true;
            btn_setting.Click += btn_setting_Click;
            // 
            // btn_addAcc
            // 
            btn_addAcc.Location = new Point(393, 3);
            btn_addAcc.Name = "btn_addAcc";
            btn_addAcc.Size = new Size(124, 45);
            btn_addAcc.TabIndex = 2;
            btn_addAcc.Text = "Thêm TK";
            btn_addAcc.UseVisualStyleBackColor = true;
            btn_addAcc.Click += btn_addAcc_Click;
            // 
            // btn_editAcc
            // 
            btn_editAcc.Location = new Point(523, 3);
            btn_editAcc.Name = "btn_editAcc";
            btn_editAcc.Size = new Size(129, 45);
            btn_editAcc.TabIndex = 5;
            btn_editAcc.Text = "Sửa TK";
            btn_editAcc.UseVisualStyleBackColor = true;
            btn_editAcc.Click += btn_editAcc_Click;
            // 
            // btn_openAuto
            // 
            btn_openAuto.Location = new Point(658, 3);
            btn_openAuto.Name = "btn_openAuto";
            btn_openAuto.Size = new Size(124, 45);
            btn_openAuto.TabIndex = 1;
            btn_openAuto.Text = "Khác";
            btn_openAuto.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(flowLayoutPanel1);
            groupBox1.Location = new Point(11, 512);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(792, 74);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Chức Năng Chính";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btn_clearRam);
            groupBox2.Controls.Add(radioButton3);
            groupBox2.Controls.Add(radioButton2);
            groupBox2.Controls.Add(radioButton1);
            groupBox2.Controls.Add(button1);
            groupBox2.Controls.Add(textBox1);
            groupBox2.Location = new Point(809, 512);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(408, 74);
            groupBox2.TabIndex = 8;
            groupBox2.TabStop = false;
            groupBox2.Text = "Chức Năng Khác";
            // 
            // btn_clearRam
            // 
            btn_clearRam.Location = new Point(305, 24);
            btn_clearRam.Name = "btn_clearRam";
            btn_clearRam.Size = new Size(97, 44);
            btn_clearRam.TabIndex = 5;
            btn_clearRam.Text = "Dọn Ram";
            btn_clearRam.UseVisualStyleBackColor = true;
            btn_clearRam.Click += btn_clearRam_Click;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(145, 18);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(51, 19);
            radioButton3.TabIndex = 4;
            radioButton3.TabStop = true;
            radioButton3.Text = "IP V6";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(75, 18);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(51, 19);
            radioButton2.TabIndex = 3;
            radioButton2.TabStop = true;
            radioButton2.Text = "IP V4";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(6, 18);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(48, 19);
            radioButton1.TabIndex = 2;
            radioButton1.TabStop = true;
            radioButton1.Text = "User";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(202, 24);
            button1.Name = "button1";
            button1.Size = new Size(97, 44);
            button1.TabIndex = 1;
            button1.Text = "Tìm";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(6, 43);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(190, 23);
            textBox1.TabIndex = 0;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(button2);
            groupBox3.Controls.Add(textBox2);
            groupBox3.Location = new Point(1223, 512);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(236, 74);
            groupBox3.TabIndex = 9;
            groupBox3.TabStop = false;
            groupBox3.Text = "Mã Kích Hoạt";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(75, 49);
            label2.Name = "label2";
            label2.Size = new Size(13, 15);
            label2.TabIndex = 3;
            label2.Text = "0";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 49);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 2;
            label1.Text = "Số Ngày:";
            // 
            // button2
            // 
            button2.Location = new Point(155, 46);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 1;
            button2.Text = "copy";
            button2.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(6, 17);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(224, 23);
            textBox2.TabIndex = 0;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1471, 598);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(Log);
            Controls.Add(grb_Account);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Main";
            Text = "FB-Ultra";
            Load += Main_Load;
            grb_Account.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv_accounts).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            Log.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private GroupBox grb_Account;
        private GroupBox Log;
        private DataGridView dgv_accounts;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btn_openChrome;
        private Button btn_openAuto;
        private Button btn_addAcc;
        private Button btn_setting;
        private Button btn_stop;
        private Button btn_editAcc;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private RadioButton radioButton3;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private Button button1;
        private TextBox textBox1;
        private GroupBox groupBox3;
        private Button button2;
        private TextBox textBox2;
        private Label label2;
        private Label label1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem menuSelectAll;
        private ToolStripMenuItem menuDeselectAll;
        private ToolStripMenuItem menuDelete;
        private ToolStripMenuItem menuDeleteAll;
        private ToolStripMenuItem menuSelect;
        private ToolStripMenuItem menuDeselect;
        private Button btn_clearRam;
        private FlowLayoutPanel flp_chromeButtons;
        private DataGridViewCheckBoxColumn col_checkbox;
        private DataGridViewTextBoxColumn col_id;
        private DataGridViewTextBoxColumn col_user;
        private DataGridViewTextBoxColumn col_pass;
        private DataGridViewTextBoxColumn col_2fa;
        private DataGridViewTextBoxColumn col_mail;
        private DataGridViewTextBoxColumn col_passMail;
        private DataGridViewTextBoxColumn col_cookie;
        private DataGridViewTextBoxColumn col_token;
        private DataGridViewTextBoxColumn col_ipV4;
        private DataGridViewTextBoxColumn col_ipV6;
        private DataGridViewTextBoxColumn col_ipXoay;
        private DataGridViewTextBoxColumn col_status;
        private DataGridViewTextBoxColumn col_userAgent;
    }
}
