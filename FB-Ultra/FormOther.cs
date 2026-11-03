using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FB_Ultra
{
    public partial class FormOther : Form
    {
        public FormOther()
        {
            InitializeComponent();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_check_Click(object sender, EventArgs e)
        {
            // 1. Lấy danh sách proxy từ ô nhập bên trái
            var inputLines = rtb_inputProxy.Text
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToList();

            if (inputLines.Count == 0)
            {
                MessageBox.Show("Vui lòng nhập danh sách proxy cần check!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Xác định đang chọn loại proxy nào để lọc và lấy đúng trường dữ liệu
            string selectedType = "ipv4"; // mặc định
            List<string> filteredInput = new List<string>();

            if (rdo_checkV4.Checked)
            {
                selectedType = "ipv4";
                foreach (var p in inputLines)
                {
                    string ipPart = p.Split(':')[0];
                    if (ipPart.Contains('.') && !ipPart.Contains(':'))
                    {
                        filteredInput.Add(p);
                    }
                }
            }
            else if (rdo_checkV6.Checked)
            {
                selectedType = "ipv6";
                foreach (var p in inputLines)
                {
                    if (p.Contains(':') && (p.Split(':').Length > 3 || p.Contains('[')))
                    {
                        filteredInput.Add(p);
                    }
                }
            }
            else if (rdo_checkXoay.Checked)
            {
                selectedType = "ipXoay";
                filteredInput = inputLines; // Proxy xoay tùy định dạng ông nhập
            }
            else
            {
                filteredInput = inputLines;
            }

            if (filteredInput.Count == 0)
            {
                MessageBox.Show("Không có proxy nào khớp với định dạng loại đã chọn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Đọc danh sách proxy đang dùng từ file accounts.json dựa theo đúng trường tương ứng
            var usedProxies = GetProxiesFromAccountsJson(selectedType);

            // 4. Lọc ra những proxy KHÔNG bị trùng
            var uniqueProxies = filteredInput
                .Where(p => !usedProxies.Contains(p, StringComparer.OrdinalIgnoreCase))
                .ToList();

            // 5. Hiển thị kết quả ra ô bên phải
            rtb_resultProxy.Text = string.Join(Environment.NewLine, uniqueProxies);

            MessageBox.Show($"Check hoàn tất! Còn lại {uniqueProxies.Count} proxy không bị trùng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // Hàm đọc file json và bốc đúng trường IpV4, IpV6 hoặc IpXoay ra
        private List<string> GetProxiesFromAccountsJson(string proxyType)
        {
            List<string> proxies = new List<string>();
            string path = "accounts.json";

            if (File.Exists(path))
            {
                try
                {
                    string jsonContent = File.ReadAllText(path);
                    var accounts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AccountModel>>(jsonContent);

                    if (accounts != null)
                    {
                        foreach (var acc in accounts)
                        {
                            string val = "";
                            if (proxyType == "ipv4") val = acc.IpV4;
                            else if (proxyType == "ipv6") val = acc.IpV6;
                            else if (proxyType == "ipXoay") val = acc.IpXoay;

                            if (!string.IsNullOrEmpty(val))
                            {
                                proxies.Add(val.Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi đọc file accounts.json: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return proxies;
        }
    }
}
