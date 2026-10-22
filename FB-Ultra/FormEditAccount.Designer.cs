namespace FB_Ultra
{
    partial class FormEditAccount
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
            btn_cancel = new Button();
            groupBox1 = new GroupBox();
            dgv_edit = new DataGridView();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_edit).BeginInit();
            SuspendLayout();
            // 
            // btn_save
            // 
            btn_save.Location = new Point(1052, 407);
            btn_save.Name = "btn_save";
            btn_save.Size = new Size(97, 40);
            btn_save.TabIndex = 0;
            btn_save.Text = "Lưu";
            btn_save.UseVisualStyleBackColor = true;
            btn_save.Click += btn_save_Click;
            // 
            // btn_cancel
            // 
            btn_cancel.Location = new Point(935, 407);
            btn_cancel.Name = "btn_cancel";
            btn_cancel.Size = new Size(97, 40);
            btn_cancel.TabIndex = 2;
            btn_cancel.Text = "Huỷ";
            btn_cancel.UseVisualStyleBackColor = true;
            btn_cancel.Click += btn_cancel_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(dgv_edit);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1137, 380);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            // 
            // dgv_edit
            // 
            dgv_edit.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_edit.Location = new Point(0, 11);
            dgv_edit.Name = "dgv_edit";
            dgv_edit.Size = new Size(1156, 389);
            dgv_edit.TabIndex = 2;
            // 
            // FormEditAccount
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1161, 459);
            Controls.Add(groupBox1);
            Controls.Add(btn_cancel);
            Controls.Add(btn_save);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FormEditAccount";
            Text = "Sửa Tài Khoản";
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv_edit).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btn_save;
        private Button btn_cancel;
        private GroupBox groupBox1;
        private DataGridView dgv_edit;
    }
}