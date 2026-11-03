namespace FB_Ultra
{
    partial class FormOther
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
            btn_close = new Button();
            groupBox1 = new GroupBox();
            rtb_resultProxy = new RichTextBox();
            label2 = new Label();
            label1 = new Label();
            btn_check = new Button();
            rtb_inputProxy = new RichTextBox();
            rdo_checkXoay = new RadioButton();
            rdo_checkV6 = new RadioButton();
            rdo_checkV4 = new RadioButton();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btn_close
            // 
            btn_close.Location = new Point(626, 297);
            btn_close.Name = "btn_close";
            btn_close.Size = new Size(75, 29);
            btn_close.TabIndex = 0;
            btn_close.Text = "Đóng";
            btn_close.UseVisualStyleBackColor = true;
            btn_close.Click += btn_close_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rtb_resultProxy);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(btn_check);
            groupBox1.Controls.Add(rtb_inputProxy);
            groupBox1.Controls.Add(rdo_checkXoay);
            groupBox1.Controls.Add(rdo_checkV6);
            groupBox1.Controls.Add(rdo_checkV4);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(689, 260);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Check Proxy trùng trên danh sách tk";
            // 
            // rtb_resultProxy
            // 
            rtb_resultProxy.Location = new Point(347, 62);
            rtb_resultProxy.Name = "rtb_resultProxy";
            rtb_resultProxy.Size = new Size(335, 163);
            rtb_resultProxy.TabIndex = 8;
            rtb_resultProxy.Text = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(347, 44);
            label2.Name = "label2";
            label2.Size = new Size(241, 15);
            label2.TabIndex = 7;
            label2.Text = "Proxy không trùng trên danh sách tài khoản:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 44);
            label1.Name = "label1";
            label1.Size = new Size(71, 15);
            label1.TabIndex = 6;
            label1.Text = "Nhập proxy:";
            // 
            // btn_check
            // 
            btn_check.Location = new Point(614, 231);
            btn_check.Name = "btn_check";
            btn_check.Size = new Size(68, 23);
            btn_check.TabIndex = 5;
            btn_check.Text = "check";
            btn_check.UseVisualStyleBackColor = true;
            btn_check.Click += btn_check_Click;
            // 
            // rtb_inputProxy
            // 
            rtb_inputProxy.Location = new Point(6, 62);
            rtb_inputProxy.Name = "rtb_inputProxy";
            rtb_inputProxy.Size = new Size(335, 164);
            rtb_inputProxy.TabIndex = 3;
            rtb_inputProxy.Text = "";
            // 
            // rdo_checkXoay
            // 
            rdo_checkXoay.AutoSize = true;
            rdo_checkXoay.Location = new Point(114, 22);
            rdo_checkXoay.Name = "rdo_checkXoay";
            rdo_checkXoay.Size = new Size(61, 19);
            rdo_checkXoay.TabIndex = 2;
            rdo_checkXoay.Text = "ipXoay";
            rdo_checkXoay.UseVisualStyleBackColor = true;
            // 
            // rdo_checkV6
            // 
            rdo_checkV6.AutoSize = true;
            rdo_checkV6.Location = new Point(60, 22);
            rdo_checkV6.Name = "rdo_checkV6";
            rdo_checkV6.Size = new Size(48, 19);
            rdo_checkV6.TabIndex = 1;
            rdo_checkV6.Text = "ipV6";
            rdo_checkV6.UseVisualStyleBackColor = true;
            // 
            // rdo_checkV4
            // 
            rdo_checkV4.AutoSize = true;
            rdo_checkV4.Checked = true;
            rdo_checkV4.Location = new Point(6, 22);
            rdo_checkV4.Name = "rdo_checkV4";
            rdo_checkV4.Size = new Size(48, 19);
            rdo_checkV4.TabIndex = 0;
            rdo_checkV4.TabStop = true;
            rdo_checkV4.Text = "ipV4";
            rdo_checkV4.UseVisualStyleBackColor = true;
            // 
            // FormOther
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(712, 338);
            Controls.Add(groupBox1);
            Controls.Add(btn_close);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FormOther";
            Text = "Chức năng khác";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btn_close;
        private GroupBox groupBox1;
        private RichTextBox richTextBox2;
        private RichTextBox rtb_inputProxy;
        private RadioButton rdo_checkXoay;
        private RadioButton rdo_checkV6;
        private RadioButton rdo_checkV4;
        private Label label2;
        private Label label1;
        private Button btn_check;
        private RichTextBox rtb_resultProxy;
    }
}