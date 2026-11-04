using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace FB_Ultra
{
    public partial class FormAddAccount : Form
    {
        public FormAddAccount()
        {
            InitializeComponent();
        }

        private void FormAddAccount_Load(object sender, EventArgs e)
        {
            if (cbo_AccountFormat.Items.Count > 0)
            {
                cbo_AccountFormat.SelectedIndex = 0;
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            var accountLines = rtb_accounts.Lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()).ToList();
            var rawProxyLines = rtb_proxies.Lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()).ToList();

            if (accountLines.Count == 0)
            {
                MessageBox.Show("Vui lòng nhập ít nhất 1 tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtb_accounts.Focus();
                return;
            }

            // --- ĐỌC DANH SÁCH USERAGENT TỪ FILE ---
            string uaFilePath = "UserAgent.txt";
            List<string> listUserAgents = new List<string>();

            if (File.Exists(uaFilePath))
            {
                listUserAgents = File.ReadAllLines(uaFilePath)
                                     .Where(line => !string.IsNullOrWhiteSpace(line))
                                     .Select(line => line.Trim())
                                     .ToList();
            }

            Random rand = new Random();

            // 1. Đọc danh sách tài khoản hiện có từ file accounts.json
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "accounts.json");
            List<AccountModel> listAccounts = new List<AccountModel>();

            if (File.Exists(filePath))
            {
                try
                {
                    string currentJson = File.ReadAllText(filePath);
                    listAccounts = JsonConvert.DeserializeObject<List<AccountModel>>(currentJson) ?? new List<AccountModel>();
                }
                catch
                {
                    listAccounts = new List<AccountModel>();
                }
            }

            // Tạo HashSet chứa danh sách User (UID) đã tồn tại để lọc trùng
            HashSet<string> existingUsers = new HashSet<string>(
                listAccounts.Select(a => a.User?.Trim()).Where(u => !string.IsNullOrEmpty(u)),
                StringComparer.OrdinalIgnoreCase
            );

            int addedCount = 0;
            int skippedCount = 0;

            // Lấy chuỗi định dạng đang chọn trên ComboBox
            string selectedFormatText = cbo_AccountFormat.Text.ToLower().Trim();

            // 2. Duyệt qua từng dòng dữ liệu mới nhập
            for (int i = 0; i < accountLines.Count; i++)
            {
                string[] parts = accountLines[i].Split('|').Select(p => p.Trim()).ToArray();
                string user = parts.Length > 0 ? parts[0] : "";

                if (string.IsNullOrEmpty(user)) continue;

                // --- KIỂM TRA LỌC TRÙNG USER ---
                if (existingUsers.Contains(user))
                {
                    skippedCount++;
                    continue;
                }

                string pass = "", twoFa = "", mail = "", passMail = "", cookie = "", token = "";

                // --- PARSE CHUỖI LINH HOẠT THEO COMBOBOX ---

                // DẠNG 6 (MỚI): UID | Pass | Cookie | Token | 2FA | JsonInfo (Index 5)
                if (selectedFormatText.Contains("jsoninfo") || cbo_AccountFormat.SelectedIndex == 5)
                {
                    pass = parts.Length > 1 ? parts[1] : "";
                    cookie = parts.Length > 2 ? parts[2] : "";
                    token = parts.Length > 3 ? parts[3] : "";
                    twoFa = parts.Length > 4 ? parts[4] : ""; // Lấy đúng mã 2FA ở phần thứ 5

                    string jsonInfo = parts.Length > 5 ? parts[5] : "";
                    if (!string.IsNullOrEmpty(jsonInfo) && jsonInfo.Contains("{"))
                    {
                        try
                        {
                            string cleanJson = jsonInfo.Replace("'", "\"");
                            var mailObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(cleanJson);
                            if (mailObj != null)
                            {
                                if (mailObj.ContainsKey("email")) mail = mailObj["email"];
                                if (mailObj.ContainsKey("password")) passMail = mailObj["password"];
                            }
                        }
                        catch { }
                    }
                }
                // DẠNG 5: UID | Pass | 2FA | Cookie | Token | Mail | PassMail (Index 4)
                else if (selectedFormatText.Contains("2fa|cookie|token|mail") || cbo_AccountFormat.SelectedIndex == 4)
                {
                    pass = parts.Length > 1 ? parts[1] : "";
                    twoFa = parts.Length > 2 ? parts[2] : "";
                    cookie = parts.Length > 3 ? parts[3] : "";
                    token = parts.Length > 4 ? parts[4] : "";
                    mail = parts.Length > 5 ? parts[5] : "";
                    passMail = parts.Length > 6 ? parts[6] : "";
                }
                // DẠNG 4: UID | Pass | 2FA | Cookie | Token (Index 3)
                else if (selectedFormatText.Contains("2fa|cookie|token") || cbo_AccountFormat.SelectedIndex == 3)
                {
                    pass = parts.Length > 1 ? parts[1] : "";
                    twoFa = parts.Length > 2 ? parts[2] : "";
                    cookie = parts.Length > 3 ? parts[3] : "";
                    token = parts.Length > 4 ? parts[4] : "";
                }
                // DẠNG 3: UID | Pass | Cookie | Token (Index 2)
                else if (selectedFormatText.Contains("cookie|token") || cbo_AccountFormat.SelectedIndex == 2)
                {
                    pass = parts.Length > 1 ? parts[1] : "";
                    cookie = parts.Length > 2 ? parts[2] : "";
                    token = parts.Length > 3 ? parts[3] : "";
                }
                // DẠNG 2: UID | Pass | Cookie | Mail | PassMail (Index 1)
                else if (selectedFormatText.Contains("cookie|mail") || cbo_AccountFormat.SelectedIndex == 1)
                {
                    pass = parts.Length > 1 ? parts[1] : "";
                    cookie = parts.Length > 2 ? parts[2] : "";
                    mail = parts.Length > 3 ? parts[3] : "";
                    passMail = parts.Length > 4 ? parts[4] : "";
                }
                // DẠNG 1: Mặc định (Index 0)
                else
                {
                    pass = parts.Length > 1 ? parts[1] : "";
                    twoFa = parts.Length > 2 ? parts[2] : "";
                    mail = parts.Length > 3 ? parts[3] : "";
                    passMail = parts.Length > 4 ? parts[4] : "";
                    cookie = parts.Length > 5 ? parts[5] : "";
                    token = parts.Length > 6 ? parts[6] : "";
                }

                // --- XỬ LÝ TỰ ĐỘNG BÓC TÁCH NẾU TRƯỜNG MAIL LÀ CHUỖI JSON ---
                if (!string.IsNullOrEmpty(mail) && mail.Contains("{"))
                {
                    try
                    {
                        string cleanJson = mail.Replace("'", "\"");
                        var mailObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(cleanJson);
                        if (mailObj != null)
                        {
                            if (mailObj.ContainsKey("email")) mail = mailObj["email"];
                            if (mailObj.ContainsKey("password")) passMail = mailObj["password"];
                        }
                    }
                    catch
                    {
                        // Bỏ qua nếu parse thất bại
                    }
                }

                // Lấy ngẫu nhiên 1 User-Agent nếu có danh sách
                string randomUA = listUserAgents.Count > 0 ? listUserAgents[rand.Next(listUserAgents.Count)] : "";

                // Gán Proxy tương ứng với RadioButton chọn
                string currentProxy = (i < rawProxyLines.Count) ? rawProxyLines[i] : "";
                string ipV4 = rdo_ipV4.Checked ? currentProxy : "";
                string ipV6 = rdo_ipV6.Checked ? currentProxy : "";
                string ipXoay = rdo_ipXoayV4.Checked ? currentProxy : "";

                var acc = new AccountModel
                {
                    User = user,
                    Pass = pass,
                    TwoFA = twoFa,
                    Mail = mail,
                    PassMail = passMail,
                    Cookie = cookie,
                    Token = token,
                    IpV4 = ipV4,
                    IpV6 = ipV6,
                    IpXoay = ipXoay,
                    Status = "",
                    UserAgent = randomUA
                };

                listAccounts.Add(acc);
                existingUsers.Add(user);
                addedCount++;
            }

            // 3. Nếu không thêm được con nào (do trùng tất cả)
            if (addedCount == 0)
            {
                MessageBox.Show($"Tất cả {skippedCount} tài khoản nhập vào đều ĐÃ TỒN TẠI trên hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Lưu lại vào file JSON
            try
            {
                string jsonOutput = JsonConvert.SerializeObject(listAccounts, Formatting.Indented);
                File.WriteAllText(filePath, jsonOutput);

                string msg = $"Đã thêm thành công {addedCount} tài khoản!";
                if (skippedCount > 0)
                {
                    msg += $"\n(Bỏ qua {skippedCount} tài khoản bị trùng UID)";
                }

                MessageBox.Show(msg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi ghi file JSON: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}