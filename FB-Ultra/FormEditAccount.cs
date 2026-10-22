using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FB_Ultra
{
    public partial class FormEditAccount : Form
    {
        private List<AccountModel> accountsToEdit;
        public FormEditAccount(List<AccountModel> accounts)
        {
            InitializeComponent();
            // Tự động tạo các cột cho dgv_edit nếu chưa có
            if (dgv_edit.Columns.Count == 0)
            {
                dgv_edit.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_user", HeaderText = "UID" });
                dgv_edit.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_pass", HeaderText = "Mật khẩu" });
                dgv_edit.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_2fa", HeaderText = "2FA" });
                dgv_edit.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_mail", HeaderText = "Email" });
                dgv_edit.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_passMail", HeaderText = "MK Email" });
                dgv_edit.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_cookie", HeaderText = "Cookie" });
                dgv_edit.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_token", HeaderText = "Token" });
                dgv_edit.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_ipV4", HeaderText = "IP V4" });
                dgv_edit.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_ipV6", HeaderText = "IP V6" });
                dgv_edit.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_ipXoay", HeaderText = "IP Xoay" });
                dgv_edit.Columns.Add(new DataGridViewTextBoxColumn { Name = "col_userAgent", HeaderText = "User-Agent" });
            }
            // --- KHÓA CỘT UID VÀ USER-AGENT KHÔNG CHO SỬA ---
            dgv_edit.Columns["col_user"].ReadOnly = true;
            dgv_edit.Columns["col_user"].DefaultCellStyle.BackColor = System.Drawing.Color.LightGray; // Tô màu xám nhận diện

            dgv_edit.Columns["col_userAgent"].ReadOnly = true;
            dgv_edit.Columns["col_userAgent"].DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;

            accountsToEdit = accounts;
            LoadDataToGrid();
        }
        private void LoadDataToGrid()
        {
            dgv_edit.Rows.Clear();
            foreach (var acc in accountsToEdit)
            {
                dgv_edit.Rows.Add(
                    acc.User,
                    acc.Pass,
                    acc.TwoFA,
                    acc.Mail,
                    acc.PassMail,
                    acc.Cookie,
                    acc.Token,
                    acc.IpV4,
                    acc.IpV6,
                    acc.IpXoay,
                    acc.UserAgent
                );
            }
        }
        // Hàm trả về danh sách sau khi người dùng sửa xong trên lưới
        public List<AccountModel> GetUpdatedAccounts()
        {
            dgv_edit.EndEdit();
            List<AccountModel> list = new List<AccountModel>();

            foreach (DataGridViewRow row in dgv_edit.Rows)
            {
                if (row.IsNewRow) continue;

                AccountModel acc = new AccountModel
                {
                    User = row.Cells["col_user"].Value?.ToString()?.Trim() ?? "",
                    Pass = row.Cells["col_pass"].Value?.ToString()?.Trim() ?? "",
                    TwoFA = row.Cells["col_2fa"].Value?.ToString()?.Trim() ?? "",
                    Mail = row.Cells["col_mail"].Value?.ToString()?.Trim() ?? "",
                    PassMail = row.Cells["col_passMail"].Value?.ToString()?.Trim() ?? "",
                    Cookie = row.Cells["col_cookie"].Value?.ToString()?.Trim() ?? "",
                    Token = row.Cells["col_token"].Value?.ToString()?.Trim() ?? "",
                    IpV4 = row.Cells["col_ipV4"].Value?.ToString()?.Trim() ?? "",
                    IpV6 = row.Cells["col_ipV6"].Value?.ToString()?.Trim() ?? "",
                    IpXoay = row.Cells["col_ipXoay"].Value?.ToString()?.Trim() ?? "",
                    UserAgent = row.Cells["col_userAgent"].Value?.ToString()?.Trim() ?? ""
                };

                list.Add(acc);
            }

            return list;
        }
        private void btn_save_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
