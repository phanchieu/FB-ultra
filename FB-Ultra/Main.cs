using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace FB_Ultra
{

    public partial class Main : Form
    {
        // --- KHAI BÁO WIN32 API ĐỂ ẨN CONSOLE VÀ KÍCH HOẠT CỬA SỔ CHROME ---
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_HIDE = 0;
        private const int SW_RESTORE = 9;
        // -------------------------------------------------------------------

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // 1. Tắt các tính năng chỉnh sửa bảng không cần thiết
            dgv_accounts.AllowUserToAddRows = false;
            dgv_accounts.AllowUserToDeleteRows = false;
            dgv_accounts.AllowUserToOrderColumns = false;

            // Load dữ liệu lên Grid trước
            LoadAccountsToGrid();

            // 2. KHÓA TẤT CẢ CỘT, TRỪ CỘT CHECKBOX
            foreach (DataGridViewColumn col in dgv_accounts.Columns)
            {
                if (col.Name == "col_checkbox")
                {
                    col.ReadOnly = false; // Mở khóa duy nhất cột Checkbox
                }
                else
                {
                    col.ReadOnly = true;  // Khóa tất cả các cột dữ liệu còn lại
                }
            }
            InitChromeCheckTimer();
        }

        private void btn_addAcc_Click(object sender, EventArgs e)
        {
            using (FormAddAccount f = new FormAddAccount())
            {
                f.StartPosition = FormStartPosition.CenterParent;
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LoadAccountsToGrid();
                }
            }
        }

        private void LoadAccountsToGrid()
        {
            dgv_accounts.Rows.Clear();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "accounts.json");

            if (!File.Exists(filePath)) return;

            try
            {
                string json = File.ReadAllText(filePath);
                var listAccounts = JsonConvert.DeserializeObject<List<AccountModel>>(json);

                if (listAccounts == null) return;

                for (int i = 0; i < listAccounts.Count; i++)
                {
                    var acc = listAccounts[i];

                    dgv_accounts.Rows.Add(
                        false,              // Checkbox
                        i + 1,              // STT (#)
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
                        acc.Status,
                        acc.UserAgent
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đọc file JSON: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<AccountModel> GetSelectedAccountsFromGrid()
        {
            dgv_accounts.EndEdit();
            List<AccountModel> list = new List<AccountModel>();

            foreach (DataGridViewRow row in dgv_accounts.Rows)
            {
                if (row.IsNewRow) continue;

                object val = row.Cells["col_checkbox"].Value;
                bool isChecked = false;

                if (val != null && val != DBNull.Value)
                {
                    if (val is bool b) isChecked = b;
                    else if (bool.TryParse(val.ToString(), out bool parsedBool)) isChecked = parsedBool;
                }

                if (isChecked)
                {
                    string user = row.Cells["col_user"].Value?.ToString()?.Trim();
                    if (string.IsNullOrEmpty(user)) continue;

                    string ipV4 = row.Cells["col_ipV4"].Value?.ToString()?.Trim() ?? "";
                    string ipV6 = row.Cells["col_ipV6"].Value?.ToString()?.Trim() ?? "";
                    string ipXoay = row.Cells["col_ipXoay"].Value?.ToString()?.Trim() ?? "";

                    string selectedProxy = "";
                    if (!string.IsNullOrEmpty(ipV4)) selectedProxy = ipV4;
                    else if (!string.IsNullOrEmpty(ipV6)) selectedProxy = ipV6;
                    else if (!string.IsNullOrEmpty(ipXoay)) selectedProxy = ipXoay;

                    AccountModel acc = new AccountModel
                    {
                        User = user,
                        Proxy = selectedProxy,
                        IpV4 = ipV4,
                        IpV6 = ipV6,
                        IpXoay = ipXoay,
                        Pass = row.Cells["col_pass"].Value?.ToString()?.Trim() ?? "",
                        TwoFA = row.Cells["col_2fa"].Value?.ToString()?.Trim() ?? "",
                        Mail = row.Cells["col_mail"].Value?.ToString()?.Trim() ?? "",
                        PassMail = row.Cells["col_passMail"].Value?.ToString()?.Trim() ?? "",
                        Cookie = row.Cells["col_cookie"].Value?.ToString()?.Trim() ?? "",
                        Token = row.Cells["col_token"].Value?.ToString()?.Trim() ?? "",
                        UserAgent = row.Cells["col_userAgent"].Value?.ToString()?.Trim() ?? ""
                    };

                    list.Add(acc);
                }
            }

            return list;
        }

        private async void btn_openChrome_Click(object sender, EventArgs e)
        {
            dgv_accounts.EndEdit();
            List<AccountModel> selectedAccounts = GetSelectedAccountsFromGrid();

            if (selectedAccounts.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất 1 tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Đọc cài đặt từ JSON
            SettingsModel settings = SettingsManager.LoadSettings();
            int maxThreads = int.TryParse(settings.SoLuong, out int s) && s > 0 ? s : 2;
            int openDelaySec = int.TryParse(settings.ThoiGianMoLuong, out int d) && d >= 0 ? d : 5;

            // Lấy thời gian hẹn giờ từ setting (số phút)
            int henGioMinutes = int.TryParse(settings.HenGio.ToString(), out int hg) && hg > 0 ? hg : 0;

            cts = new CancellationTokenSource();
            btn_openChrome.Enabled = false;

            try
            {
                // === 1. ĐOẠN HẸN GIỜ CHỜ TRƯỚC KHI CHẠY (ĐẶT NGAY ĐẦU HÀM) ===
                if (henGioMinutes > 0)
                {
                    int totalSeconds = henGioMinutes * 60; // Đã sửa lỗi thiếu dấu nhân ở đây
                    for (int sec = totalSeconds; sec > 0; sec--)
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        int mins = sec / 60;
                        int se = sec % 60;
                        string timeStr = mins > 0 ? $"{mins}p {se:D2}s" : $"{se}s";

                        groupBox1.Text = $"Chức năng chính: hẹn giờ chạy {timeStr}";

                        await Task.Delay(1000, cts.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Người dùng bấm nút Stop trong lúc đang đếm ngược hẹn giờ
                groupBox1.Text = "Chức năng chính:";
                btn_openChrome.Enabled = true; // Phải bật lại nút bấm ở đây để không bị liệt
                return; // Dừng luôn, không chạy tiếp phần dưới
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hẹn giờ: {ex.Message}");
                groupBox1.Text = "Chức năng chính:";
                btn_openChrome.Enabled = true;
                return;
            }

            groupBox1.Text = "Chức năng chính:";

            // === 2. SAU KHI HẾT GIỜ HẸN, TIẾN HÀNH CHẠY LUỒNG CHÍNH ===
            if (settings.NuoiNickNgauNhien)
            {
                selectedAccounts.Shuffle();
            }

            // Sử dụng SemaphoreSlim để giới hạn số luồng chạy đồng thời đúng bằng maxThreads
            using (SemaphoreSlim semaphore = new SemaphoreSlim(maxThreads, maxThreads))
            {
                List<Task> tasks = new List<Task>();

                try
                {
                    for (int i = 0; i < selectedAccounts.Count; i++)
                    {
                        if (cts.Token.IsCancellationRequested) break;

                        var acc = selectedAccounts[i];

                        // Chờ đến khi có slot trống (đúng giới hạn số luồng)
                        await semaphore.WaitAsync(cts.Token);

                        if (cts.Token.IsCancellationRequested)
                        {
                            semaphore.Release();
                            break;
                        }

                        // Kích hoạt chạy tài khoản hiện tại ở background task
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                UpdateAccountStatus(acc.User, "Đang mở...");
                                RunSingleAccount(acc, cts.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                UpdateAccountStatus(acc.User, "Đã dừng");
                            }
                            finally
                            {
                                semaphore.Release(); // Giải phóng slot khi xong việc
                            }
                        }, cts.Token));

                        // === ĐOẠN ĐẾM NGƯỢC CHO TÀI KHOẢN TIẾP THEO ===
                        if (i < selectedAccounts.Count - 1 && openDelaySec > 0)
                        {
                            var nextAcc = selectedAccounts[i + 1];
                            try
                            {
                                for (int sec = openDelaySec; sec > 0; sec--)
                                {
                                    cts.Token.ThrowIfCancellationRequested();
                                    UpdateAccountStatus(nextAcc.User, $"Đang chờ... chờ {sec}s");
                                    await Task.Delay(1000, cts.Token);
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                UpdateAccountStatus(nextAcc.User, "Đã dừng");
                                break;
                            }
                        }
                    }

                    await Task.WhenAll(tasks); // Đợi toàn bộ các Task hoàn thành
                }
                catch (OperationCanceledException)
                {
                    // Xử lý khi tiến trình bị hủy toàn cục trong lúc đang chạy acc
                }
            }

            btn_openChrome.Enabled = true;
        }
        private void RunSingleAccount(AccountModel acc, CancellationToken token)
        {
            // Kiểm tra trước khi bắt đầu
            token.ThrowIfCancellationRequested();

            // Khai báo đường dẫn gốc ở đây để dùng chung toàn hàm
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string profileFolder = Path.Combine(baseDir, "Profiles", acc.User);

            // Đọc cài đặt hiện tại để xem người dùng đang chọn chế độ nào
            SettingsModel settings = SettingsManager.LoadSettings();

            // NẾU CHẠY CHẾ ĐỘ TỰ ĐỘNG (Mở tự động, Nuôi thường, Nuôi AI) MÀ CHƯA CÓ PROFILE -> BỎ QUA
            if (!settings.IsMoThuong && !Directory.Exists(profileFolder))
            {
                UpdateAccountStatus(acc.User, "chưa Login acc và profile");
                return; // Dừng, chuyển sang acc tiếp theo trong hàng đợi
            }
            ChromeDriver driver = null;

            // LẤY STT CỦA TÀI KHOẢN TỪ DATAGRIDVIEW TRƯỚC
            int stt = 1;
            dgv_accounts.Invoke(new Action(() =>
            {
                foreach (DataGridViewRow row in dgv_accounts.Rows)
                {
                    if (row.Cells["col_user"].Value?.ToString() == acc.User)
                    {
                        stt = Convert.ToInt32(row.Cells[1].Value);
                        break;
                    }
                }
            }));

            try
            {
                string chromiumExePath = Path.Combine(baseDir, "ungoogled-chromium", "chrome.exe");
                string driverPath = Path.Combine(baseDir, "chromedriver.exe");

                // --- KIỂM TRA XEM PROFILE CÓ ĐANG MỞ HAY KHÔNG ---
                if (IsProfileButtonExist(acc.User) || IsProfileFolderLocked(profileFolder))
                {
                    UpdateAccountStatus(acc.User, "Profile đang mở!");

                    // Active cửa sổ Chrome đang có sẵn lên màn hình (nếu tìm thấy)
                    if (IsProfileButtonExist(acc.User))
                    {
                        flp_chromeButtons.Invoke(new Action(() =>
                        {
                            string btnName = $"btn_chrome_{acc.User}";
                            var btn = flp_chromeButtons.Controls[btnName] as Button;
                            if (btn != null && btn.Tag is IntPtr handle && handle != IntPtr.Zero)
                            {
                                ShowWindow(handle, SW_RESTORE);
                                SetForegroundWindow(handle);
                            }
                        }));
                    }

                    return; // Bỏ qua, không bật đè
                }

                // Patch cờ CDC trong chromedriver
                FacebookAuthHelper.PatchChromeDriver(driverPath);

                if (!Directory.Exists(profileFolder))
                {
                    Directory.CreateDirectory(profileFolder);
                }

                // TẠO TÊN PROFILE VÀ GHI VÀO CẤU HÌNH CHROMIUM
                string profileDisplayName = $"{stt}-{acc.User}";
                SetChromeProfileName(profileFolder, profileDisplayName);

                // 1. Cấu hình ChromeDriverService ẩn CMD
                var service = ChromeDriverService.CreateDefaultService(baseDir);
                service.HideCommandPromptWindow = true;

                // 2. Cấu hình ChromeOptions cho Ungoogled Chromium
                int currentProfileSeed = 0;
                int savedSeed = 0;
                string profileSeedFile = string.Empty;
                var options = new ChromeOptions();
                options.BinaryLocation = chromiumExePath;
                options.AddArgument($"--user-data-dir={profileFolder}");

                // Gán Proxy
                if (!string.IsNullOrEmpty(acc.Proxy))
                {
                    ProxyModel proxyInfo = ProxyHelper.ParseIPv4(acc.Proxy);
                    if (proxyInfo != null)
                    {
                        if (proxyInfo.HasAuth)
                        {
                            // Chỉ gán giá trị, không khai báo lại kiểu dữ liệu (int, string) nữa
                            profileSeedFile = Path.Combine(profileFolder, "canvas_seed.txt");
                            if (File.Exists(profileSeedFile) && int.TryParse(File.ReadAllText(profileSeedFile), out savedSeed))
                            {
                                currentProfileSeed = savedSeed;
                            }
                            else
                            {
                                currentProfileSeed = new Random().Next(1000, 9999);
                                File.WriteAllText(profileSeedFile, currentProfileSeed.ToString());
                            }

                            Console.WriteLine($"[CHECK SEED] Profile Folder: {profileFolder} ==> Seed: {currentProfileSeed}");

                            string extFolder = ProxyHelper.CreateProxyExtension(proxyInfo, currentProfileSeed);
                            string fullExtPath = Path.GetFullPath(extFolder);

                            options.AddArgument($"--load-extension={fullExtPath}");
                            options.AddArgument($"--disable-extensions-except={fullExtPath}");
                        }
                        else
                        {
                            options.AddArgument($"--proxy-server={proxyInfo.Host}:{proxyInfo.Port}");
                        }
                    }
                }
                // Gán User-Agent chuẩn
                string userAgentToUse = !string.IsNullOrWhiteSpace(acc.UserAgent)
                    ? acc.UserAgent
                    : "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";

                options.AddArgument($"--user-agent={userAgentToUse}");
                options.AddArgument("about:blank");

                // Tắt các tính năng ngầm / thông báo
                options.AddArgument("--no-first-run");
                options.AddArgument("--no-default-browser-check");
                options.AddArgument("--disable-notifications");
                options.AddArgument("--disable-popup-blocking");
                options.AddArgument("--disable-prompt-on-repost");
                options.AddArgument("--disable-sync");
                options.AddArgument("--disable-component-update");
                options.AddArgument("--disable-domain-reliability");
                options.AddArgument("--disable-background-networking");
                options.AddArgument("--disable-features=Translate,OptimizationHints,MediaRouter,InProductHelp,OptimizeLoadingDataUrls");
                options.AddUserProfilePreference("profile.default_content_setting_values.media_stream_camera", 1);

                // Anti Automation
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddExcludedArgument("enable-automation");
                options.AddAdditionalOption("useAutomationExtension", false);
                options.AddArgument("--ignore-certificate-errors");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-gpu");

                // Preference sửa lỗi Crash Window
                options.AddUserProfilePreference("profile.exit_type", "Normal");
                options.AddUserProfilePreference("profile.exited_cleanly", true);
                options.AddUserProfilePreference("session.restore_on_startup", 4);
                options.AddUserProfilePreference("startup_urls", new string[] { "about:blank" });

                options.AddArgument("--use-fake-ui-for-media-stream"); // Tự động cho phép truy cập media, bỏ qua popup hỏi quyền
                options.PageLoadStrategy = PageLoadStrategy.Eager;

                token.ThrowIfCancellationRequested();
                // 3. Khởi chạy Driver
                driver = new ChromeDriver(service, options, TimeSpan.FromMinutes(3));

                // Đọc hoặc tạo seed riêng cho profile này ngay tại chỗ
                currentProfileSeed = 1234;
                string seedFile = Path.Combine(profileFolder, "canvas_seed.txt");
                if (File.Exists(seedFile) && int.TryParse(File.ReadAllText(seedFile), out savedSeed))
                {
                    currentProfileSeed = savedSeed;
                }
                // Random chọn một thông số GPU giả lập dựa theo profileSeed
                string[] fakeRenderers = {
                    "ANGLE (NVIDIA, NVIDIA GeForce RTX 3060 Direct3D11 vs_5_0 ps_5_0, D3D11)",
                    "ANGLE (NVIDIA, NVIDIA GeForce GTX 1060 6GB Direct3D11 vs_5_0 ps_5_0, D3D11)",
                    "ANGLE (AMD Radeon RX 580 Direct3D11 vs_5_0 ps_5_0, D3D11)",
                    "ANGLE (Intel, Intel(R) Iris(R) Xe Graphics Direct3D11 vs_5_0 ps_5_0, D3D11)"
                };
                string selectedRenderer = fakeRenderers[currentProfileSeed % fakeRenderers.Length];

                string webGLSpoofScript = $@"
                (function() {{
                    const fakeRenderer = '{selectedRenderer}';
    
                    // Override WebGL getParameter để che giấu GPU thật
                    const getParameter = WebGLRenderingContext.prototype.getParameter;
                    WebGLRenderingContext.prototype.getParameter = function(parameter) {{
                        // UNMASKED_VENDOR_WEBGL
                        if (parameter === 37445) return 'Google Inc. (NVIDIA)';
                        // UNMASKED_RENDERER_WEBGL
                        if (parameter === 37446) return fakeRenderer;
                        return getParameter.apply(this, arguments);
                    }};

                    // Làm tương tự cho WebGL2
                    const getParameter2 = WebGL2RenderingContext.prototype.getParameter;
                    WebGL2RenderingContext.prototype.getParameter = function(parameter) {{
                        if (parameter === 37445) return 'Google Inc. (NVIDIA)';
                        if (parameter === 37446) return fakeRenderer;
                        return getParameter2.apply(this, arguments);
                            }};
                        }})();
                    ";

                // Bơm trực tiếp qua CDP sau khi khởi tạo driver
                driver.ExecuteCdpCommand("Page.addScriptToEvaluateOnNewDocument", new Dictionary<string, object>
                {
                    { "source", webGLSpoofScript }
                });

                // 4. Quét ẩn các cửa sổ CMD dư thừa
                HideDriverConsoles();

                // Căn chỉnh kích thước cửa sổ
                ((IJavaScriptExecutor)driver).ExecuteScript("window.moveTo(100, 100); window.resizeTo(1280, 800);");

                // --- LẤY HANDLE CỬA SỔ CHROME VÀ TẠO NÚT ACTIVE ---
                IntPtr chromeHandle = GetChromeHandleByUser(driver, acc.User);

                AddChromeActiveButton(stt, acc.User, chromeHandle);

                // 5. Điều hướng & Khởi tạo Service Login Facebook
                UpdateAccountStatus(acc.User, "Đang kiểm tra đăng nhập...");

                FacebookLoginService loginService = new FacebookLoginService(driver);
                bool isLoggedIn = loginService.LoginFacebook(acc);

                if (isLoggedIn)
                {
                    UpdateAccountStatus(acc.User, "Đăng nhập thành công");

                    // NẾU LÀ MỞ THƯỜNG: Đã login xong / mở lên thành công thì dừng ở đây, nhường quyền cho user thao tác tay
                    if (settings.IsMoThuong)
                    {
                        UpdateAccountStatus(acc.User, "Mở thường (Thủ công)");
                        return;
                    }
                    // --- KIỂM TRA CÔNG TẮC TỔNG TRƯỚC KHI CHẠY ---
                    if (settings.likeCmtTheoLink_caiDatChung)
                    {
                        UpdateAccountStatus(acc.User, "Đang chạy Like/Cmt theo link...");
                        RunLikeCommentTheoLink(driver, settings, token);
                    }
                    // CÁC CHẾ ĐỘ TỰ ĐỘNG KHÁC (Mở tự động / Nuôi): Mới tiếp tục chạy pipeline kịch bản
                    RunAccountAutomationPipeline(driver, acc, settings, token);
                }
                else
                {
                    UpdateAccountStatus(acc.User, "Đăng nhập thất bại");
                }
            }
            catch (OperationCanceledException)
            {
                UpdateAccountStatus(acc.User, "Đã dừng");
            }
            catch (Exception ex)
            {
                // Kiểm tra nếu lỗi do tắt Chrome thủ công hoặc mất kết nối DevTools
                if (ex.Message.Contains("session deleted") || ex.Message.Contains("not connected to DevTools") || ex.Message.Contains("invalid session id"))
                {
                    UpdateAccountStatus(acc.User, "Đã đóng thủ công");
                }
                else
                {
                    UpdateAccountStatus(acc.User, "Lỗi khởi chạy");
                    MessageBox.Show($"Lỗi acc {acc.User}: {ex.Message}", "Lỗi Runtime", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                // CHỈ KÍCH HOẠT HÀM ĐÓNG DRIVER KHI isMoThuong == false VÀ DRIVER KHÔNG TRỐNG
                if (!settings.IsMoThuong && driver != null)
                {
                    CloseChromeDriver(driver);
                }
            }
        }
        private void RunAccountAutomationPipeline(ChromeDriver driver, AccountModel acc, SettingsModel cfg, CancellationToken token)
        {
            if (cfg.IsMoThuong) return;
            if (driver.Url.Contains("/checkpoint/"))
            {
                UpdateAccountStatus(acc.User, "Bị Checkpoint! Cần xử lý thủ công.");
                // Lưu lại trạng thái hoặc dừng luồng của account này tại đây
                return;
            }
            // ----------------------------------------------------------------------------------
            // CHẾ ĐỘ 1: MỞ TỰ ĐỘNG (Thực thi các nhiệm vụ riêng lẻ khi chọn "Mở tự động")
            // ----------------------------------------------------------------------------------
            if (cfg.IsMoTuDong)
            {
                UpdateAccountStatus(acc.User, "Bắt đầu phiên chạy các chức năng tự động...");
                driver.Navigate().GoToUrl("https://www.facebook.com/");
                Thread.Sleep(3000);

                // 1. Tự động vào nhóm theo link
                if (cfg.TuDongVaoNhom)
                {
                    UpdateAccountStatus(acc.User, "Đang thực hiện: Tự động vào nhóm theo link...");
                    PerformJoinGroupsByLink(driver, cfg, acc, token);
                }

                // 2. Tự động đăng bài vào nhóm (Đã thêm vào chế độ Mở tự động)
                if (cfg.TuDongDangBaiVaoNhom)
                {
                    UpdateAccountStatus(acc.User, "Đang thực hiện: Tự động đăng bài vào nhóm...");
                    PerformPostGroupsByLink(driver, cfg, acc, token);
                }

                // 3. Chấp nhận kết bạn
                if (cfg.KetBanCheo)
                {
                    // PerformAcceptFriends(driver, cfg, acc);
                }

                UpdateAccountStatus(acc.User, "Hoàn tất các chức năng tự động được chọn.");
                return;
            }

            // ----------------------------------------------------------------------------------
            // CHẾ ĐỘ 2: NUÔI THƯỜNG (Vòng lặp luân phiên các hành động theo xác suất)
            // ----------------------------------------------------------------------------------
            if (cfg.IsNuoiThuong)
            {
                UpdateAccountStatus(acc.User, "Bắt đầu phiên nuôi nick thường...");

                int minMin = int.TryParse(cfg.ThoiGianNuoiMin, out int mn) && mn > 0 ? mn : 2;
                int maxMin = int.TryParse(cfg.ThoiGianNuoiMax, out int mx) && mx >= minMin ? mx : minMin + 3;

                Random rnd = new Random();
                int totalSecondsToFarm = rnd.Next(minMin, maxMin + 1) * 60;
                int elapsedSeconds = 0;

                int checkLoiMoiKetBan = 1;

                UpdateAccountStatus(acc.User, "Đang điều hướng về Facebook...");
                driver.Navigate().GoToUrl("https://www.facebook.com/");
                Thread.Sleep(3000);

                while (elapsedSeconds < totalSecondsToFarm && !token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();

                    int actionChoice = rnd.Next(1, 101);

                    // 1. TỈ LỆ BÌNH LUẬN (Tạm khóa)
                    int tiLeCmt = int.TryParse(cfg.TiLeCmt, out int tlcmt) ? tlcmt : 0;
                    if (cfg.BinhLuanBaiViet && tiLeCmt > 0 && actionChoice <= tiLeCmt)
                    {
                        UpdateAccountStatus(acc.User, "Tạm dừng phát triển chức năng bình luận");
                        elapsedSeconds += 5;
                    }
                    // 2. PHÂN NHÁNH CÁC HÀNH ĐỘNG KHÁC
                    else
                    {
                        int tiLeCamXuc = int.TryParse(cfg.TiLeThaCamXuc, out int tlcx) ? tlcx : 0;
                        int reactionChoice = rnd.Next(1, 101);
                        int subActionChoice = rnd.Next(1, 101);

                        // A. THẢ CẢM XÚC
                        if (tiLeCamXuc > 0 && reactionChoice <= tiLeCamXuc && (cfg.Like || cfg.Tim || cfg.Haha || cfg.Sad || cfg.ThaCamXucNgauNhien))
                        {
                            UpdateAccountStatus(acc.User, "Đang thực hiện: Thả cảm xúc bài viết...");
                            PerformReaction(driver, cfg);
                            elapsedSeconds += 6;
                        }
                        // B. CHẤP NHẬN KẾT BẠN
                        else if (subActionChoice <= 10 && checkLoiMoiKetBan == 1)
                        {
                            UpdateAccountStatus(acc.User, "Đang thực hiện: Kiểm tra và chấp nhận kết bạn...");
                            bool hasRequests = PerformAcceptFriends(driver, cfg, acc);
                            if (!hasRequests)
                            {
                                checkLoiMoiKetBan = 0;
                            }
                            elapsedSeconds += 12;
                        }
                        // C. CHỦ ĐỘNG GỬI KẾT BẠN THEO GỢI Ý
                        else if (cfg.KetBanTheoGoiY && subActionChoice > 10 && subActionChoice <= 18)
                        {
                            UpdateAccountStatus(acc.User, "Đang thực hiện: Gửi lời mời kết bạn theo gợi ý...");
                            PerformAddFriend(driver, cfg, acc);
                            elapsedSeconds += 15;
                        }
                        // D. TỰ ĐỘNG VÀO NHÓM
                        else if (cfg.TuDongVaoNhom && subActionChoice > 18 && subActionChoice <= 25)
                        {
                            UpdateAccountStatus(acc.User, "Đang thực hiện: Xen kẽ vào 1 nhóm theo danh sách...");
                            PerformJoinOneGroup(driver, cfg, acc);
                            elapsedSeconds += 25;
                        }
                        // ĐĂNG BÀI VÀO NHÓM (Chuyển thành else if để khớp chuỗi điều kiện)
                        else if (cfg.TuDongDangBaiVaoNhom && subActionChoice > 25 && subActionChoice <= 32)
                        {
                            UpdateAccountStatus(acc.User, "Đang thực hiện: Tự động đăng bài vào nhóm...");
                            PerformPostGroupsByLink(driver, cfg, acc, token);
                            elapsedSeconds += 30;
                        }
                        // E. XEM VIDEO WATCH
                        else if (cfg.XemVideoWatch && rnd.Next(1, 100) <= 20)
                        {
                            UpdateAccountStatus(acc.User, "Đang thực hiện: Xem video Watch...");
                            driver.Navigate().GoToUrl("https://www.facebook.com/watch");
                            int watchTime = rnd.Next(15, 25);
                            Thread.Sleep(watchTime * 1000);
                            elapsedSeconds += watchTime;
                            driver.Navigate().GoToUrl("https://www.facebook.com/");
                        }
                        // F. MẶC ĐỊNH: LƯỚT NEWSFEED
                        else if (cfg.LuotNewsfeed)
                        {
                            int remainingSecs = totalSecondsToFarm - elapsedSeconds;
                            UpdateAccountStatus(acc.User, $"Đang thực hiện: Lướt Newsfeed (Còn {remainingSecs}s)...");

                            int scrollStep = rnd.Next(300, 600);
                            ((IJavaScriptExecutor)driver).ExecuteScript($"window.scrollBy(0, {scrollStep});");

                            int sleepTime = rnd.Next(4, 7);
                            Thread.Sleep(sleepTime * 1000);
                            elapsedSeconds += sleepTime;
                        }
                        else
                        {
                            Thread.Sleep(3000);
                            elapsedSeconds += 3;
                        }
                    }

                    Thread.Sleep(1000);
                }
                UpdateAccountStatus(acc.User, "Hoàn tất phiên nuôi nick");
            }
        }
        public void PerformReaction(ChromeDriver driver, SettingsModel cfg)
        {
            try
            {
                Random rnd = new Random();
                Thread.Sleep(2000);

                // Tìm nút Like/Thích trên bài viết hiển thị trên màn hình
                var likeButtons = driver.FindElements(By.XPath("//div[@aria-label='Thích' or @aria-label='Like']"));
                if (likeButtons != null && likeButtons.Count > 0)
                {
                    var btn = likeButtons[rnd.Next(Math.Min(3, likeButtons.Count))]; // Chọn ngẫu nhiên trong vài bài đầu
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", btn);
                    Thread.Sleep(1000);

                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
                    Thread.Sleep(1500);
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi ngầm để không làm chết luồng nuôi
                System.Diagnostics.Debug.WriteLine($"Lỗi thả cảm xúc: {ex.Message}");
            }
        }
        public bool PerformAcceptFriends(ChromeDriver driver, SettingsModel cfg, AccountModel acc)
        {
            try
            {
                string requestsUrl = "https://www.facebook.com/friends/requests";
                if (driver.Url != requestsUrl)
                {
                    driver.Navigate().GoToUrl(requestsUrl);
                    Thread.Sleep(2500);
                }

                // Kiểm tra hòm thư trống
                var noRequestText = driver.FindElements(By.XPath("//span[contains(text(), 'Không có yêu cầu mới') or contains(text(), 'No new requests')]"));
                if (noRequestText != null && noRequestText.Count > 0)
                {
                    UpdateAccountStatus(acc.User, "Kết bạn: Hòm thư mời trống.");
                    driver.Navigate().GoToUrl("https://www.facebook.com/");
                    return false; // Báo hiệu không còn lời mời nào để lần sau không vào nữa
                }

                // Tìm các nút Xác nhận / Confirm
                var confirmBtns = driver.FindElements(By.XPath("//div[@aria-label='Xác nhận' or @aria-label='Confirm']"));
                if (confirmBtns != null && confirmBtns.Count > 0)
                {
                    // Chỉ bấm 1 đến 2 người trong 1 lần gọi để giữ nhịp tự nhiên
                    int countToAccept = Math.Min(2, confirmBtns.Count);
                    for (int i = 0; i < countToAccept; i++)
                    {
                        var btn = confirmBtns[i];
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", btn);
                        Thread.Sleep(1000);
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
                        UpdateAccountStatus(acc.User, $"Kết bạn: Đã chấp nhận 1 lời mời.");
                        Thread.Sleep(1500);
                    }
                }
                else
                {
                    UpdateAccountStatus(acc.User, "Kết bạn: Không tìm thấy nút xác nhận nào.");
                    driver.Navigate().GoToUrl("https://www.facebook.com/");
                    return false;
                }

                driver.Navigate().GoToUrl("https://www.facebook.com/");
                return true;
            }
            catch (Exception ex)
            {
                UpdateAccountStatus(acc.User, $"Lỗi chấp nhận kết bạn: {ex.Message}");
                try { driver.Navigate().GoToUrl("https://www.facebook.com/"); } catch { }
                return false;
            }
        }
        public void PerformAddFriend(ChromeDriver driver, SettingsModel cfg, AccountModel acc)
        {
            try
            {
                string suggestUrl = "https://www.facebook.com/friends/suggestions";
                driver.Navigate().GoToUrl(suggestUrl);
                Thread.Sleep(3000);

                // Tìm các nút "Thêm bạn bè" / "Add Friend"
                var addBtns = driver.FindElements(By.XPath("//div[@aria-label='Thêm bạn bè' or @aria-label='Add Friend']"));

                if (addBtns != null && addBtns.Count > 0)
                {
                    // Chỉ bấm tối đa 2-3 người mỗi lần gọi
                    int countToAdd = Math.Min(3, addBtns.Count);
                    int successCount = 0;

                    for (int i = 0; i < countToAdd; i++)
                    {
                        var btn = addBtns[i];
                        try
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", btn);
                            Thread.Sleep(1000);
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
                            successCount++;
                            UpdateAccountStatus(acc.User, $"Gửi kết bạn: Đã gửi lời mời ({successCount}/{countToAdd})...");
                            Thread.Sleep(2000); // Nghỉ ngắn giữa các cú bấm kết bạn
                        }
                        catch { }
                    }
                }
                else
                {
                    UpdateAccountStatus(acc.User, "Gửi kết bạn: Không tìm thấy gợi ý kết bạn nào.");
                }

                Thread.Sleep(2000);
                driver.Navigate().GoToUrl("https://www.facebook.com/");
            }
            catch (Exception ex)
            {
                UpdateAccountStatus(acc.User, $"Lỗi gửi kết bạn: {ex.Message}");
                try { driver.Navigate().GoToUrl("https://www.facebook.com/"); } catch { }
            }
        }
        public void PerformJoinGroupsByLink(ChromeDriver driver, SettingsModel cfg, AccountModel acc, CancellationToken token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cfg.DanhSachLinkVaoNhom)) return;

                string[] groupLinks = cfg.DanhSachLinkVaoNhom.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (groupLinks.Length == 0) return;

                Random rnd = new Random();
                int total = groupLinks.Length;

                for (int i = 0; i < total; i++)
                {
                    token.ThrowIfCancellationRequested();

                    string link = groupLinks[i].Trim();
                    if (string.IsNullOrEmpty(link)) continue;

                    int currentNumber = i + 1;

                    // 1. NHẢY NGAY SANG LINK NHÓM TIẾP THEO (KHÔNG BỊ KẸT LẠI TRANG CŨ)
                    UpdateAccountStatus(acc.User, $"Vào nhóm ({currentNumber}/{total}): Đang truy cập {link}...");
                    driver.Navigate().GoToUrl(link);

                    try { Task.Delay(3000, token).Wait(token); } catch { break; }

                    // 2. Kiểm tra nhóm lỗi / đai
                    if (driver.Title.Contains("Content Not Found") || driver.PageSource.Contains("This content isn't available") || driver.PageSource.Contains("Trang này hiện không khả dụng"))
                    {
                        UpdateAccountStatus(acc.User, $"Nhóm ({currentNumber}/{total}) bị lỗi/đai -> Bỏ qua, trôi sang nhóm tiếp theo ngay...");
                        continue; // Nhảy ngay sang vòng lặp kế tiếp để mở link mới
                    }

                    // 3. Tìm nút Tham gia
                    var joinBtn = driver.FindElements(By.XPath("//div[@aria-label='Tham gia nhóm' or @aria-label='Join Group' or @aria-label='Tham gia'] | //span[text()='Tham gia nhóm' or text()='Join Group' or text()='Tham gia']/ancestor::div[@role='button']"));

                    if (joinBtn != null && joinBtn.Count > 0)
                    {
                        // TRƯỜNG HỢP 1: CHƯA VÀO NHÓM -> BẤM JOIN
                        var btn = joinBtn[0];
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", btn);
                        try { Task.Delay(1000, token).Wait(token); } catch { break; }

                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
                        UpdateAccountStatus(acc.User, $"Vào nhóm ({currentNumber}/{total}): Đã gửi yêu cầu tham gia thành công!");

                        // CHỈ NGHỈ DELAY KHI ĐÃ BẤM JOIN THÀNH CÔNG!
                        int baseDelay = int.TryParse(cfg.DelayVaoNhom, out int d) && d > 0 ? d : 120;
                        int variance = (int)(baseDelay * 0.25);
                        int minDelay = Math.Max(5, baseDelay - variance);
                        int maxDelay = baseDelay + variance;
                        int actualDelay = rnd.Next(minDelay, maxDelay + 1);

                        UpdateAccountStatus(acc.User, $"Đã Join nhóm thành công! Nghỉ {actualDelay}s trước khi sang nhóm kế tiếp...");
                        try { Task.Delay(actualDelay * 1000, token).Wait(token); } catch { break; }
                    }
                    else
                    {
                        // TRƯỜNG HỢP 2: ĐÃ VÀO RỒI -> KHÔNG NGHỈ DELAY, PHÁT LOG VÀ NHẢY SANG LINK TIẾP THEO NGAY LẬP TỨC
                        UpdateAccountStatus(acc.User, $"Nhóm ({currentNumber}/{total}) đã vào từ trước -> Nhảy luôn sang nhóm tiếp theo...");
                        try { Task.Delay(1000, token).Wait(token); } catch { break; } // Nghỉ nhẹ 1s để kịp đọc trang
                        continue; // Bỏ qua delay, trôi tiếp sang nhóm sau luôn!
                    }
                }

                UpdateAccountStatus(acc.User, "Đã duyệt xong toàn bộ danh sách nhóm.");
                driver.Navigate().GoToUrl("https://www.facebook.com/");
            }
            catch (OperationCanceledException)
            {
                UpdateAccountStatus(acc.User, "Đã dừng tiến trình vào nhóm.");
            }
            catch (Exception ex)
            {
                UpdateAccountStatus(acc.User, $"Lỗi vào nhóm: {ex.Message}");
            }
        }
        private int currentGroupIndex = 0; // Biến lưu vị trí nhóm hiện tại

        public void PerformJoinOneGroup(ChromeDriver driver, SettingsModel cfg, AccountModel acc)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cfg.DanhSachLinkVaoNhom)) return;

                string[] groupLinks = cfg.DanhSachLinkVaoNhom.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (groupLinks.Length == 0) return;

                int attempts = 0;
                // Vòng lặp quét: Nếu gặp nhóm lỗi hoặc đã vào, tự động trôi sang nhóm tiếp theo ngay trong cùng 1 lượt
                while (attempts < groupLinks.Length)
                {
                    if (currentGroupIndex >= groupLinks.Length)
                    {
                        currentGroupIndex = 0; // Vòng lặp lại từ đầu nếu chạy hết list
                    }

                    string link = groupLinks[currentGroupIndex].Trim();
                    int targetIndex = currentGroupIndex + 1;
                    currentGroupIndex++; // Tăng index sẵn cho lần sau
                    attempts++;

                    if (string.IsNullOrEmpty(link)) continue;

                    UpdateAccountStatus(acc.User, $"Nuôi nick - Kiểm tra nhóm ({targetIndex}/{groupLinks.Length}): {link}...");

                    driver.Navigate().GoToUrl(link);
                    Thread.Sleep(3000);

                    // 1. Kiểm tra xem nhóm có bị "bay" hoặc không tồn tại không (Page Not Found)
                    if (driver.Title.Contains("Content Not Found") || driver.PageSource.Contains("This content isn't available") || driver.PageSource.Contains("Trang này hiện không khả dụng"))
                    {
                        UpdateAccountStatus(acc.User, $"Nuôi nick: Nhóm {targetIndex} đã bay/lỗi, tự động chuyển sang nhóm tiếp theo...");
                        continue; // Lập tức nhảy sang vòng lặp check nhóm kế tiếp ngay lập tức
                    }

                    // 2. Tìm nút "Tham gia nhóm"
                    var joinBtn = driver.FindElements(By.XPath("//div[@aria-label='Tham gia nhóm' or @aria-label='Join Group' or @aria-label='Tham gia'] | //span[text()='Tham gia nhóm' or text()='Join Group' or text()='Tham gia']/ancestor::div[@role='button']"));

                    if (joinBtn != null && joinBtn.Count > 0)
                    {
                        var btn = joinBtn[0];
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", btn);
                        Thread.Sleep(1000);

                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
                        UpdateAccountStatus(acc.User, $"Nuôi nick: Đã bấm gửi yêu cầu tham gia thành công nhóm {targetIndex}!");

                        // Thành công bấm join 1 nhóm mới -> Dừng hàm ngay để trả quyền về cho vòng lặp nuôi nick lướt feed
                        Thread.Sleep(2000);
                        driver.Navigate().GoToUrl("https://www.facebook.com/");
                        return;
                    }
                    else
                    {
                        // Đã tham gia từ trước -> Tự động bỏ qua và loop tìm nhóm tiếp theo chưa vào
                        UpdateAccountStatus(acc.User, $"Nuôi nick: Nhóm {targetIndex} đã tham gia từ trước, tìm nhóm tiếp theo...");
                    }
                }

                UpdateAccountStatus(acc.User, "Nuôi nick: Đã quét hết danh sách nhưng không còn nhóm nào mới để vào.");
                driver.Navigate().GoToUrl("https://www.facebook.com/");
            }
            catch (Exception ex)
            {
                UpdateAccountStatus(acc.User, $"Lỗi vào nhóm khi nuôi: {ex.Message}");
                try { driver.Navigate().GoToUrl("https://www.facebook.com/"); } catch { }
            }
        }
        public async void PerformPostGroupsByLink(ChromeDriver driver, SettingsModel cfg, AccountModel acc, CancellationToken token)
        {
            try
            {
                // 1. Lấy danh sách link nhóm
                if (string.IsNullOrWhiteSpace(cfg.DanhSachLinkNhomSpam))
                {
                    UpdateAccountStatus(acc.User, "Đăng nhóm: Danh sách link nhóm đang trống.");
                    return;
                }

                string[] groupLinks = cfg.DanhSachLinkNhomSpam.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (groupLinks.Length == 0) return;

                // 2. Lấy nội dung bài viết (Tách theo cụm dòng trống \n\n)
                string rawContent = cfg.NoiDungDangBai;
                if (string.IsNullOrWhiteSpace(rawContent))
                {
                    UpdateAccountStatus(acc.User, "Đăng nhóm: Nội dung bài viết đang trống.");
                    return;
                }

                string[] captions = rawContent.Split(new string[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (captions.Length == 0) return;

                Random rnd = new Random();
                int totalGroups = groupLinks.Length;
                int captionIndex = 0;
                int successPostCount = 0;
                int successJoinCount = 0;

                for (int i = 0; i < totalGroups; i++)
                {
                    token.ThrowIfCancellationRequested();

                    string link = groupLinks[i].Trim();
                    if (string.IsNullOrEmpty(link)) continue;

                    int currentNumber = i + 1;

                    if (captionIndex >= captions.Length)
                    {
                        captionIndex = 0;
                    }
                    string currentCaption = captions[captionIndex].Trim();
                    captionIndex++;

                    // ==========================================
                    // 👉 TÍCH HỢP AI REWRITE (DỰA TRÊN FILE SETTING)
                    // ==========================================
                    string finalCaption = currentCaption;
                    if (cfg.DungAi_TuDongDangBai)
                    {
                        var aiService = new AiService();
                        string apiUrl = cfg.ApiAI_TuDongDangBai;
                        string sysPrompt = cfg.AiSystemPrompt_TuDongDangBai;

                        // Bắn AI chạy ngầm bằng Task.Run (không dùng await ở đây)
                        var aiTask = Task.Run(() => aiService.CallAiRewriteAsync(apiUrl, sysPrompt, currentCaption, "qwen2.5:3b"));

                        bool aiCompleted = false;

                        // Vòng lặp đếm giây từ 0 đến 30 cập nhật lên cột trạng thái
                        for (int sec = 0; sec <= 60; sec++)
                        {
                            UpdateAccountStatus(acc.User, $"Nhóm ({currentNumber}/{totalGroups}): Đang chờ AI... {sec}/60s");

                            // Kiểm tra mỗi giây xem task AI đã hoàn thành chưa
                            if (aiTask.IsCompleted)
                            {
                                aiCompleted = true;
                                break;
                            }

                            // Chờ 1 giây (có bắt exception token để thoát nếu dừng luồng ngang)
                            try { Task.Delay(1000, token).Wait(token); } catch { break; }
                        }

                        if (aiCompleted)
                        {
                            try
                            {
                                // Lấy kết quả vì task đã chạy xong
                                string aiResult = await aiTask;
                                if (!string.IsNullOrEmpty(aiResult) && !aiResult.StartsWith("[Lỗi]") && !aiResult.StartsWith("[Không]"))
                                {
                                    finalCaption = aiResult;
                                    UpdateAccountStatus(acc.User, "AI viết lại nội dung thành công.");
                                }
                                else
                                {
                                    UpdateAccountStatus(acc.User, "AI phản hồi lỗi -> Dùng nội dung gốc.");
                                }
                            }
                            catch (Exception ex)
                            {
                                UpdateAccountStatus(acc.User, $"Lỗi nhận kết quả AI: {ex.Message} -> Dùng nội dung gốc.");
                            }
                        }
                        else
                        {
                            UpdateAccountStatus(acc.User, "AI quá thời gian 30s -> Bỏ qua, dùng nội dung gốc.");
                        }
                    }

                    UpdateAccountStatus(acc.User, $"Đăng nhóm ({currentNumber}/{totalGroups}): Đang kiểm tra {link}...");
                    driver.Navigate().GoToUrl(link);

                    try { Task.Delay(3500, token).Wait(token); } catch { break; }

                    // Kiểm tra nhóm lỗi / bay / không tồn tại
                    if (driver.Title.Contains("Content Not Found") || driver.PageSource.Contains("This content isn't available") || driver.PageSource.Contains("Trang này hiện không khả dụng"))
                    {
                        UpdateAccountStatus(acc.User, $"Nhóm ({currentNumber}/{totalGroups}) bị lỗi/đai -> Bỏ qua, sang nhóm tiếp theo...");
                        continue;
                    }

                    try
                    {
                        // Kiểm tra trạng thái nhóm thông qua nút "Tham gia" hay "Đã tham gia"
                        var joinBtn = driver.FindElements(By.XPath("//div[@aria-label='Tham gia nhóm' or @aria-label='Join Group' or @aria-label='Tham gia'] | //span[text()='Tham gia nhóm' or text()='Join Group' or text()='Tham gia']/ancestor::div[@role='button']"));

                        if (joinBtn != null && joinBtn.Count > 0)
                        {
                            // TRƯỜNG HỢP 1: CHƯA VÀO NHÓM -> BẤM THAM GIA VÀ BỎ QUA KHÔNG ĐĂNG BÀI
                            var btn = joinBtn[0];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", btn);
                            try { Task.Delay(1000, token).Wait(token); } catch { break; }

                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
                            successJoinCount++;
                            UpdateAccountStatus(acc.User, $"Nhóm ({currentNumber}/{totalGroups}): Chưa vào -> Đã bấm gửi yêu cầu tham gia. Bỏ qua đăng bài!");

                            try { Task.Delay(2000, token).Wait(token); } catch { break; }
                            continue;
                        }

                        // TRƯỜNG HỢP 2: ĐÃ VÀO NHÓM RỒI -> TIẾN HÀNH ĐĂNG BÀI
                        UpdateAccountStatus(acc.User, $"Nhóm ({currentNumber}/{totalGroups}): Đã là thành viên -> Tiến hành đăng bài...");

                        // 3. XỬ LÝ MỞ KHUNG ĐĂNG BÀI ẨN DANH
                        bool openedBox = false;

                        if (cfg.DangAnDanh)
                        {
                            string jsClickAnonymOuter = @"
                        var spans = document.querySelectorAll('span, div');
                        for (var i = 0; i < spans.length; i++) {
                            var text = spans[i].innerText || spans[i].textContent;
                            if (text.trim() === 'Bài viết ẩn danh' || text.trim() === 'Anonymous post') {
                                var btn = spans[i].closest('div[role=""button""]') || spans[i];
                                btn.click();
                                return true;
                            }
                        }
                        return false;
                    ";

                            bool clickedOuter = (bool)((IJavaScriptExecutor)driver).ExecuteScript(jsClickAnonymOuter);
                            if (clickedOuter)
                            {
                                UpdateAccountStatus(acc.User, "Đã bấm nút 'Bài viết ẩn danh' bên ngoài nhóm.");
                                try { Task.Delay(2000, token).Wait(token); } catch { break; }

                                string jsClickPopup = @"
                            var buttons = document.querySelectorAll('div[role=""button""], span');
                            for (var i = 0; i < buttons.length; i++) {
                                var text = buttons[i].innerText || buttons[i].textContent;
                                if (text.includes('Tạo bài viết ẩn danh') || text.includes('Tiếp tục') || text.includes('Confirm')) {
                                    if (buttons[i].offsetParent !== null) {
                                        buttons[i].click();
                                        return true;
                                    }
                                }
                            }
                            return false;
                        ";
                                ((IJavaScriptExecutor)driver).ExecuteScript(jsClickPopup);
                                try { Task.Delay(1500, token).Wait(token); } catch { break; }
                                openedBox = true;
                            }
                        }
                        try
                        {
                            // Kiểm tra nhanh xem driver còn phản hồi không
                            string currentUrl = driver.Url;
                        }
                        catch
                        {
                            UpdateAccountStatus(acc.User, "Trình duyệt đã bị mất kết nối hoặc đóng trước đó. Bỏ qua nhóm này...");
                            continue; // Hoặc break tùy logic
                        }
                        // 3. XỬ LÝ MỞ KHUNG ĐĂNG BÀI THƯỜNG (Đã tối ưu và chống sập driver)
                        if (!openedBox)
                        {
                            var createPostBox = driver.FindElements(By.XPath("//span[contains(text(), 'Bạn viết gì') or contains(text(), 'Write something') or contains(text(), 'Tạo bài viết')]"));
                            if (createPostBox != null && createPostBox.Count > 0)
                            {
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", createPostBox[0]);
                                try { Task.Delay(1000, token).Wait(token); } catch { break; }
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", createPostBox[0]);
                                try { Task.Delay(2500, token).Wait(token); } catch { break; }
                                openedBox = true;
                            }
                        }

                        if (!openedBox)
                        {
                            UpdateAccountStatus(acc.User, $"Nhóm ({currentNumber}/{totalGroups}): Không tìm thấy khung/nút đăng bài. Bỏ qua...");
                            continue;
                        }

                        // 4. ĐIỀN NỘI DUNG CAPTION (Đã thay bằng `finalCaption` sau khi AI rewrite nếu có)
                        string jsInsertText = @"
                    var box = document.querySelector('div[role=""dialog""] [contenteditable=""true""]') || 
                              document.querySelector('div[role=""textbox""][data-lexical-editor=""true""]') || 
                              document.querySelector('[contenteditable=""true""]');
                    if (box) {
                        box.click();
                        box.focus();
                        
                        var text = arguments[0];
                        var lines = text.split('\n');
                        for (var i = 0; i < lines.length; i++) {
                            if (i > 0) {
                                document.execCommand('insertText', false, '\n');
                            }
                            document.execCommand('insertText', false, lines[i]);
                        }
                        
                        box.dispatchEvent(new Event('input', { bubbles: true }));
                        box.dispatchEvent(new Event('change', { bubbles: true }));
                        return true;
                    }
                    return false;
                ";

                        bool isFilled = false;
                        for (int retry = 0; retry < 3; retry++)
                        {
                            isFilled = (bool)((IJavaScriptExecutor)driver).ExecuteScript(jsInsertText, finalCaption);
                            if (isFilled) break;

                            try { Task.Delay(1000, token).Wait(token); } catch { break; }
                        }

                        if (isFilled)
                        {
                            try { Task.Delay(1500, token).Wait(token); } catch { break; }
                            UpdateAccountStatus(acc.User, "Đã điền nội dung bài viết thành công.");
                        }
                        else
                        {
                            UpdateAccountStatus(acc.User, $"Nhóm ({currentNumber}/{totalGroups}): Không tìm thấy ô nhập nội dung sau nhiều lần thử. Bỏ qua...");
                            continue;
                        }

                        // 5. ĐÍNH KÈM ẢNH (GIỮ NGUYÊN CODE CŨ)
                        string targetImagePath = "";
                        string inputPath = cfg.LinkFileAnh;

                        if (!string.IsNullOrWhiteSpace(inputPath))
                        {
                            try
                            {
                                string searchDir = "";
                                if (System.IO.Directory.Exists(inputPath))
                                {
                                    searchDir = inputPath;
                                }
                                else if (System.IO.File.Exists(inputPath))
                                {
                                    searchDir = System.IO.Path.GetDirectoryName(inputPath);
                                }

                                if (!string.IsNullOrEmpty(searchDir) && System.IO.Directory.Exists(searchDir))
                                {
                                    var imgFiles = System.IO.Directory.GetFiles(searchDir, "*.*")
                                        .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                    f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                    f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                                    f.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                                        .ToArray();

                                    if (imgFiles.Length > 0)
                                    {
                                        targetImagePath = imgFiles[rnd.Next(imgFiles.Length)];
                                    }
                                }

                                if (string.IsNullOrEmpty(targetImagePath) && System.IO.File.Exists(inputPath))
                                {
                                    targetImagePath = inputPath;
                                }

                                if (!string.IsNullOrEmpty(targetImagePath) && System.IO.File.Exists(targetImagePath))
                                {
                                    string tempImgPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "fb_rand_" + Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(targetImagePath));
                                    byte[] imgBytes = System.IO.File.ReadAllBytes(targetImagePath);

                                    byte[] randomBytes = new byte[rnd.Next(4, 16)];
                                    rnd.NextBytes(randomBytes);

                                    byte[] newBytes = new byte[imgBytes.Length + randomBytes.Length];
                                    Buffer.BlockCopy(imgBytes, 0, newBytes, 0, imgBytes.Length);
                                    Buffer.BlockCopy(randomBytes, 0, newBytes, imgBytes.Length, randomBytes.Length);

                                    System.IO.File.WriteAllBytes(tempImgPath, newBytes);
                                    targetImagePath = tempImgPath;
                                }
                            }
                            catch { }
                        }

                        if (!string.IsNullOrEmpty(targetImagePath) && System.IO.File.Exists(targetImagePath))
                        {
                            string jsClickPhotoBtn = @"
                        var dialog = document.querySelector('div[role=""dialog""]');
                        if (!dialog) return false;
                        var btn = dialog.querySelector('div[aria-label*=""Ảnh/video""], div[aria-label*=""Photo/video""], div[aria-label*=""Ảnh""]');
                        if (btn) {
                            btn.click();
                            return true;
                        }
                        return false;
                    ";
                            ((IJavaScriptExecutor)driver).ExecuteScript(jsClickPhotoBtn);
                            try { Task.Delay(1500, token).Wait(token); } catch { break; }

                            var fileInputs = driver.FindElements(By.XPath("//input[@type='file']"));
                            bool uploaded = false;
                            foreach (var fileInput in fileInputs)
                            {
                                try
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].style.display = 'block';
                                arguments[0].style.visibility = 'visible';
                                arguments[0].style.opacity = '1';
                                arguments[0].removeAttribute('hidden');
                            ", fileInput);

                                    fileInput.SendKeys(targetImagePath);

                                    ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].dispatchEvent(new Event('change', { bubbles: true }));
                                arguments[0].dispatchEvent(new Event('input', { bubbles: true }));
                            ", fileInput);

                                    uploaded = true;
                                    break;
                                }
                                catch { }
                            }

                            if (uploaded)
                            {
                                UpdateAccountStatus(acc.User, $"Đã đính kèm ảnh ngẫu nhiên (Làm mới MD5): {System.IO.Path.GetFileName(targetImagePath)}");
                                try { Task.Delay(3000, token).Wait(token); } catch { break; }
                            }
                            else
                            {
                                UpdateAccountStatus(acc.User, "Không tìm thấy ô chọn file ảnh để tải lên.");
                            }
                        }

                        // 6. BẤM NÚT ĐĂNG
                        var postBtn = driver.FindElements(By.XPath("//div[@aria-label='Đăng' or @aria-label='Post']//span[text()='Đăng' or text()='Post'] | //div[@role='button']//span[text()='Đăng' or text()='Post']"));
                        if (postBtn != null && postBtn.Count > 0)
                        {
                            var actualBtn = postBtn[0].FindElement(By.XPath("./ancestor::div[@role='button']"));
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", actualBtn);

                            successPostCount++;
                            UpdateAccountStatus(acc.User, $"Đăng nhóm ({currentNumber}/{totalGroups}): Đăng bài thành công!");
                            try { Task.Delay(3500, token).Wait(token); } catch { break; }
                        }
                        else
                        {
                            UpdateAccountStatus(acc.User, $"Nhóm ({currentNumber}/{totalGroups}): Không tìm thấy nút Đăng.");
                        }

                        // 7. NGHỈ DELAY NGẪU NHÂN SAU KHI ĐĂNG
                        int baseDelay = int.TryParse(cfg.DelayDangBai, out int d) && d > 0 ? d : 120;
                        int variance = (int)(baseDelay * 0.25);
                        int minDelay = Math.Max(5, baseDelay - variance);
                        int maxDelay = baseDelay + variance;
                        int actualDelay = rnd.Next(minDelay, maxDelay + 1);

                        UpdateAccountStatus(acc.User, $"Nghỉ ngẫu nhiên {actualDelay}s trước khi sang nhóm tiếp theo...");
                        try { Task.Delay(actualDelay * 1000, token).Wait(token); } catch { break; }
                    }
                    catch (Exception ex)
                    {
                        UpdateAccountStatus(acc.User, $"Lỗi thao tác nhóm {currentNumber}: {ex.Message}");
                        continue;
                    }
                }

                UpdateAccountStatus(acc.User, $"Hoàn tất! Đã gửi yêu cầu tham gia {successJoinCount} nhóm mới và đăng thành công {successPostCount} nhóm.");
                driver.Navigate().GoToUrl("https://www.facebook.com/");
            }
            catch (OperationCanceledException)
            {
                UpdateAccountStatus(acc.User, "Đã dừng tiến trình đăng bài theo yêu cầu.");
            }
            catch (Exception ex)
            {
                UpdateAccountStatus(acc.User, $"Lỗi đăng nhóm: {ex.Message}");
            }
        }
        public void RunLikeCommentTheoLink(ChromeDriver driver, SettingsModel settings, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(settings.link_likeCmtTheoLink)) return;

            // Lấy danh sách Link
            var links = settings.link_likeCmtTheoLink
                                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(l => l.Trim())
                                .Where(l => !string.IsNullOrEmpty(l))
                                .ToList();

            // Lấy danh sách Nội dung Comment
            var comments = settings.textCmt_likeCmtTheoLink
                                   .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(c => c.Trim())
                                   .Where(c => !string.IsNullOrEmpty(c))
                                   .ToList();

            Random rand = new Random();
            int delaySeconds = int.TryParse(settings.delay_likeCmtTheoLink, out int d) ? d : 3;

            foreach (var link in links)
            {
                // Kiểm tra lệnh dừng trước khi bắt đầu bài mới
                token.ThrowIfCancellationRequested();

                try
                {
                    // 1. Điều hướng đến bài viết
                    driver.Navigate().GoToUrl(link);
                    Thread.Sleep(3000);
                    token.ThrowIfCancellationRequested();

                    // 2. XỬ LÝ THẢ CẢM XÚC
                    bool hasReaction = settings.like_likeCmtTheoLink ||
                                       settings.tim_likeCmtTheoLink ||
                                       settings.haha_likeCmtTheoLink ||
                                       settings.sad_likeCmtTheoLink ||
                                       settings.camXucNgauNhien_likeCmtTheoLink;

                    if (hasReaction)
                    {
                        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                        // Danh sách các XPath dự phòng để bắt nút Like/Thích ở nhiều phiên bản giao diện Facebook khác nhau
                        string[] likeXpaths = {
                    "//div[@aria-label='Thích' or @aria-label='Like']",
                    "//div[contains(@aria-label, 'Thích') or contains(@aria-label, 'Like')]",
                    "//span[text()='Thích' or text()='Like']/ancestor::div[@role='button'][1]",
                    "//div[@role='button']//span[text()='Thích' or text()='Like']"
                };

                        IWebElement likeButton = null;
                        foreach (var xp in likeXpaths)
                        {
                            try
                            {
                                likeButton = wait.Until(drv => drv.FindElement(By.XPath(xp)));
                                if (likeButton != null) break;
                            }
                            catch { }
                        }

                        if (likeButton != null)
                        {
                            // Hover chuột vào nút Like để hiện bảng cảm xúc
                            Actions actions = new Actions(driver);
                            actions.MoveToElement(likeButton).Perform();
                            Thread.Sleep(1000);
                            token.ThrowIfCancellationRequested();

                            string reactionXpath = "";

                            if (settings.camXucNgauNhien_likeCmtTheoLink)
                            {
                                string[] listReactions = {
                            "//div[@aria-label='Thích']",
                            "//div[@aria-label='Yêu thích']",
                            "//div[@aria-label='Haha']",
                            "//div[@aria-label='Buồn']"
                        };
                                reactionXpath = listReactions[rand.Next(listReactions.Length)];
                            }
                            else
                            {
                                if (settings.like_likeCmtTheoLink) reactionXpath = "//div[@aria-label='Thích']";
                                else if (settings.tim_likeCmtTheoLink) reactionXpath = "//div[@aria-label='Yêu thích']";
                                else if (settings.haha_likeCmtTheoLink) reactionXpath = "//div[@aria-label='Haha']";
                                else if (settings.sad_likeCmtTheoLink) reactionXpath = "//div[@aria-label='Buồn']";
                            }

                            if (!string.IsNullOrEmpty(reactionXpath))
                            {
                                try
                                {
                                    IWebElement reactionElement = wait.Until(drv => drv.FindElement(By.XPath(reactionXpath)));
                                    reactionElement.Click();
                                    Thread.Sleep(1500);
                                }
                                catch { }
                            }
                        }
                    }

                    // 3. XỬ LÝ BÌNH LUẬN (CHỈ CHẠY KHI Ô CMT ĐƯỢC TICK VÀ CÓ NỘI DUNG)
                    // 3. XỬ LÝ BÌNH LUẬN (ĐÃ CẬP NHẬT XPath ĐA DẠNG VÀ AN TOÀN CHO REACTJS)
                    if (settings.cmt_likeCmtTheoLink && comments.Count > 0)
                    {
                        token.ThrowIfCancellationRequested();
                        var waitComment = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                        // Các XPath dự phòng để bắt mọi loại khung comment trên Facebook (Bài viết, Reel, Nhóm, Fanpage)
                        string[] commentXpaths = {
                        "//div[@role='textbox' and (contains(@aria-label, 'bình luận') or contains(@aria-label, 'comment'))]",
                        "//div[contains(@aria-label, 'Viết bình luận') or contains(@aria-label, 'Write a comment')]",
                        "//div[@contenteditable='true' and @role='textbox']",
                        "//form//div[@contenteditable='true']"
                    };

                        IWebElement commentBox = null;
                        foreach (var xp in commentXpaths)
                        {
                            try
                            {
                                commentBox = waitComment.Until(drv => drv.FindElement(By.XPath(xp)));
                                if (commentBox != null) break;
                            }
                            catch { }
                        }

                        if (commentBox != null)
                        {
                            try
                            {
                                // Cuộn màn hình đến khung comment cho chắc ăn hiển thị trên viewport
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", commentBox);
                                Thread.Sleep(500);

                                // Click vào khung comment
                                commentBox.Click();
                                Thread.Sleep(500);

                                // Bốc ngẫu nhiên 1 câu comment
                                string selectedComment = comments[rand.Next(comments.Count)];

                                // Dùng Actions class để gõ chữ, giúp ReactJS của Facebook nhận diện sự kiện thay đổi text mượt hơn
                                Actions actions = new Actions(driver);
                                actions.Click(commentBox).SendKeys(selectedComment).Perform();
                                Thread.Sleep(1500);
                                token.ThrowIfCancellationRequested();

                                // Gửi phím Enter để đăng bình luận
                                commentBox.SendKeys(OpenQA.Selenium.Keys.Enter);
                                Thread.Sleep(2000);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Lỗi thao tác khung comment: {ex.Message}");
                            }
                        }
                    }

                    // --- 4. DELAY THÔNG MINH SAU KHI XONG 1 BÀI ---
                    // Chia nhỏ thời gian chờ theo giây để nút "Dừng" có hiệu lực tức thì, không bị treo thread
                    for (int i = 0; i < delaySeconds; i++)
                    {
                        token.ThrowIfCancellationRequested();
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException) throw; // Ném tiếp ngoại lệ dừng để thoát luồng chuẩn xác
                    Console.WriteLine($"Lỗi chạy link {link}: {ex.Message}");
                }
            }
        }
        // --- HÀM TÌM HANDLE CỬA SỔ THEO UNIQUE TITLE ---
        private IntPtr GetChromeHandleByUser(ChromeDriver driver, string user)
        {
            try
            {
                // Đặt Title độc nhất cho cửa sổ Chrome vừa mở
                string uniqueTitle = $"FB_PROFILE_{user}";
                ((IJavaScriptExecutor)driver).ExecuteScript($"document.title = '{uniqueTitle}';");

                System.Threading.Thread.Sleep(500);

                var chromeProcesses = Process.GetProcessesByName("chrome")
                                             .Concat(Process.GetProcessesByName("chromium"));

                foreach (var proc in chromeProcesses)
                {
                    if (proc.MainWindowHandle != IntPtr.Zero && proc.MainWindowTitle.Contains(uniqueTitle))
                    {
                        return proc.MainWindowHandle;
                    }
                }
            }
            catch { }

            return IntPtr.Zero;
        }

        // --- QUẢN LÝ NÚT KÍCH HOẠT CHROME TRÊN FLOWLAYOUTPANEL ---
        private void AddChromeActiveButton(int stt, string uid, IntPtr chromeHandle)
        {
            if (flp_chromeButtons.InvokeRequired)
            {
                flp_chromeButtons.Invoke(new Action(() => AddChromeActiveButton(stt, uid, chromeHandle)));
                return;
            }

            Button btn = new Button
            {
                Text = $"#{stt} - {uid.Substring(Math.Max(0, uid.Length - 4))}",
                Width = 85,
                Height = 32,
                Margin = new Padding(3),
                Tag = chromeHandle,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White
            };

            btn.Click += (s, e) =>
            {
                IntPtr handle = (IntPtr)((Button)s).Tag;
                if (handle != IntPtr.Zero)
                {
                    ShowWindow(handle, SW_RESTORE);
                    SetForegroundWindow(handle);
                }
            };

            flp_chromeButtons.Controls.Add(btn);
        }

        private void ClearChromeActiveButtons()
        {
            if (flp_chromeButtons.InvokeRequired)
            {
                flp_chromeButtons.Invoke(new Action(ClearChromeActiveButtons));
                return;
            }
            flp_chromeButtons.Controls.Clear();
        }

        private void HideDriverConsoles()
        {
            try
            {
                foreach (var proc in Process.GetProcessesByName("chromedriver"))
                {
                    if (proc.MainWindowHandle != IntPtr.Zero)
                    {
                        ShowWindow(proc.MainWindowHandle, SW_HIDE);
                    }
                }
            }
            catch { }
        }

        private void UpdateAccountStatus(string user, string status)
        {
            if (dgv_accounts.InvokeRequired)
            {
                dgv_accounts.Invoke(new Action(() => UpdateAccountStatus(user, status)));
                return;
            }

            foreach (DataGridViewRow row in dgv_accounts.Rows)
            {
                if (!row.IsNewRow && row.Cells["col_user"].Value?.ToString() == user)
                {
                    row.Cells["col_status"].Value = status;
                    break;
                }
            }
        }

        private void menuSelect_Click(object sender, EventArgs e)
        {
            if (dgv_accounts.SelectedRows.Count == 0 && dgv_accounts.SelectedCells.Count == 0) return;

            foreach (DataGridViewRow row in dgv_accounts.SelectedRows)
            {
                if (!row.IsNewRow) row.Cells["col_checkbox"].Value = true;
            }

            foreach (DataGridViewCell cell in dgv_accounts.SelectedCells)
            {
                if (cell.RowIndex >= 0 && !dgv_accounts.Rows[cell.RowIndex].IsNewRow)
                {
                    dgv_accounts.Rows[cell.RowIndex].Cells["col_checkbox"].Value = true;
                }
            }
        }

        private void menuSelectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgv_accounts.Rows)
            {
                if (!row.IsNewRow) row.Cells["col_checkbox"].Value = true;
            }
        }

        private void menuDeselectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgv_accounts.Rows)
            {
                if (!row.IsNewRow) row.Cells["col_checkbox"].Value = false;
            }
        }

        private void menuDeselect_Click(object sender, EventArgs e)
        {
            if (dgv_accounts.SelectedRows.Count == 0 && dgv_accounts.SelectedCells.Count == 0) return;

            foreach (DataGridViewRow row in dgv_accounts.SelectedRows)
            {
                if (!row.IsNewRow) row.Cells["col_checkbox"].Value = false;
            }

            foreach (DataGridViewCell cell in dgv_accounts.SelectedCells)
            {
                if (cell.RowIndex >= 0 && !dgv_accounts.Rows[cell.RowIndex].IsNewRow)
                {
                    dgv_accounts.Rows[cell.RowIndex].Cells["col_checkbox"].Value = false;
                }
            }
        }

        private void DeleteProfileFolder(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return;

            try
            {
                string profileFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Profiles", uid);
                if (Directory.Exists(profileFolder))
                {
                    Directory.Delete(profileFolder, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xóa folder profile của {uid}: {ex.Message}");
            }
        }

        private void SaveGridToJson()
        {
            try
            {
                List<AccountModel> listAccounts = new List<AccountModel>();

                foreach (DataGridViewRow row in dgv_accounts.Rows)
                {
                    if (row.IsNewRow) continue;

                    AccountModel acc = new AccountModel
                    {
                        User = row.Cells["col_user"].Value?.ToString(),
                        Pass = row.Cells["col_pass"].Value?.ToString(),
                        TwoFA = row.Cells["col_2fa"].Value?.ToString(),
                        Mail = row.Cells["col_mail"].Value?.ToString(),
                        PassMail = row.Cells["col_passMail"].Value?.ToString(),
                        Cookie = row.Cells["col_cookie"].Value?.ToString(),
                        Token = row.Cells["col_token"].Value?.ToString(),
                        IpV4 = row.Cells["col_ipV4"].Value?.ToString(),
                        IpV6 = row.Cells["col_ipV6"].Value?.ToString(),
                        IpXoay = row.Cells["col_ipXoay"].Value?.ToString(),
                        Status = row.Cells["col_status"].Value?.ToString(),
                        UserAgent = row.Cells["col_userAgent"].Value?.ToString()
                    };

                    listAccounts.Add(acc);
                }

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "accounts.json");
                string jsonOutput = JsonConvert.SerializeObject(listAccounts, Formatting.Indented);
                File.WriteAllText(filePath, jsonOutput);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật file JSON: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RenumberGridRows()
        {
            dgv_accounts.SuspendLayout();

            int stt = 1;
            for (int i = 0; i < dgv_accounts.Rows.Count; i++)
            {
                if (!dgv_accounts.Rows[i].IsNewRow)
                {
                    dgv_accounts.Rows[i].Cells[1].Value = stt++;
                }
            }

            dgv_accounts.ResumeLayout();
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            dgv_accounts.EndEdit();
            List<int> rowsToDelete = new List<int>();

            foreach (DataGridViewRow row in dgv_accounts.Rows)
            {
                if (row.IsNewRow) continue;

                object val = row.Cells["col_checkbox"].Value;
                bool isChecked = false;

                if (val != null && val != DBNull.Value)
                {
                    if (val is bool b) isChecked = b;
                    else if (bool.TryParse(val.ToString(), out bool parsedBool)) isChecked = parsedBool;
                }

                if (isChecked)
                {
                    rowsToDelete.Add(row.Index);
                }
            }

            if (rowsToDelete.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn (check) ít nhất 1 tài khoản để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn XÓA {rowsToDelete.Count} tài khoản ĐÃ TÍCH và PROFILE tương ứng?",
                                          "Xác nhận xóa",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                rowsToDelete.Sort((a, b) => b.CompareTo(a));

                foreach (int index in rowsToDelete)
                {
                    DataGridViewRow row = dgv_accounts.Rows[index];
                    string uid = row.Cells["col_user"].Value?.ToString()?.Trim();

                    DeleteProfileFolder(uid);
                    dgv_accounts.Rows.RemoveAt(index);
                }

                RenumberGridRows();
                SaveGridToJson();

                MessageBox.Show("Đã xóa các tài khoản được chọn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void menuDeleteAll_Click(object sender, EventArgs e)
        {
            if (dgv_accounts.Rows.Count == 0 || (dgv_accounts.Rows.Count == 1 && dgv_accounts.Rows[0].IsNewRow))
            {
                MessageBox.Show("Danh sách tài khoản đang trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("CẢNH BÁO: Bạn có chắc chắn muốn XÓA SẠCH toàn bộ tài khoản và TẤT CẢ PROFILE?",
                                          "Xác nhận xóa tất cả",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dgv_accounts.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string uid = row.Cells["col_user"].Value?.ToString()?.Trim();
                        DeleteProfileFolder(uid);
                    }
                }

                dgv_accounts.Rows.Clear();
                SaveGridToJson();
                MessageBox.Show("Đã xóa sạch toàn bộ tài khoản và Profile!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgv_accounts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgv_accounts.Columns[e.ColumnIndex].Name == "col_checkbox")
            {
                dgv_accounts.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void btn_clearRam_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Hành động này sẽ đóng toàn bộ Trình duyệt và ChromeDriver đang chạy ngầm để giải phóng RAM.\n\nBạn có muốn tiếp tục?",
                "Xác nhận Clear RAM",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            int closedProcessesCount = 0;
            string[] processNames = new string[] { "chromedriver", "chrome", "chromium" };

            foreach (string procName in processNames)
            {
                try
                {
                    Process[] processes = Process.GetProcessesByName(procName);
                    foreach (Process proc in processes)
                    {
                        try
                        {
                            proc.Kill();
                            proc.WaitForExit(1000);
                            closedProcessesCount++;
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Dọn dẹp danh sách nút bấm trên FlowLayoutPanel
            ClearChromeActiveButtons();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            try
            {
                using (Process currentProcess = Process.GetCurrentProcess())
                {
                    currentProcess.MinWorkingSet = (IntPtr)(300 * 1024);
                }
            }
            catch { }

            MessageBox.Show(
                $"Đã dọn dẹp thành công!\n- Đã đóng {closedProcessesCount} tiến trình trình duyệt/driver.\n- Bộ nhớ RAM đã được dọn dẹp tối ưu.",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private System.Windows.Forms.Timer chromeCheckTimer;

        private void InitChromeCheckTimer()
        {
            chromeCheckTimer = new System.Windows.Forms.Timer();
            chromeCheckTimer.Interval = 2000; // Quét mỗi 2 giây
            chromeCheckTimer.Tick += (s, e) =>
            {
                for (int i = flp_chromeButtons.Controls.Count - 1; i >= 0; i--)
                {
                    var btn = flp_chromeButtons.Controls[i] as Button;
                    if (btn != null && btn.Tag is IntPtr handle)
                    {
                        // Kiểm tra xem Handle cửa sổ còn tồn tại trên Windows không
                        if (!IsWindow(handle))
                        {
                            flp_chromeButtons.Controls.RemoveAt(i);
                            btn.Dispose();
                        }
                    }
                }
            };
            chromeCheckTimer.Start();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindow(IntPtr hWnd);

        // 1. Kiểm tra xem trên giao diện flp_chromeButtons đã có nút của UID này chưa
        private bool IsProfileButtonExist(string uid)
        {
            bool exists = false;
            if (flp_chromeButtons.InvokeRequired)
            {
                flp_chromeButtons.Invoke(new Action(() => { exists = IsProfileButtonExist(uid); }));
                return exists;
            }

            string btnName = $"btn_chrome_{uid}";
            return flp_chromeButtons.Controls.ContainsKey(btnName);
        }

        // 2. Kiểm tra xem folder Profile có đang bị Chrome khóa (lockfile) hay không
        private bool IsProfileFolderLocked(string profileFolder)
        {
            if (!Directory.Exists(profileFolder)) return false;

            try
            {
                // Chrome tạo file 'lockfile' hoặc 'LOCK' trong thư mục Profile khi đang hoạt động
                string lockPath = Path.Combine(profileFolder, "lockfile");
                if (!File.Exists(lockPath))
                {
                    lockPath = Path.Combine(profileFolder, "LOCK");
                }

                if (File.Exists(lockPath))
                {
                    // Thử mở file lock; nếu bị IOException nghĩa là Chrome đang giữ file này
                    using (FileStream stream = new FileStream(lockPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        stream.Close();
                    }
                }
                return false;
            }
            catch (IOException)
            {
                return true; // File đang bị chiếm dụng -> Profile đang mở
            }
            catch
            {
                return false;
            }
        }
        private void SetChromeProfileName(string profileFolder, string profileName)
        {
            try
            {
                string defaultFolder = Path.Combine(profileFolder, "Default");
                if (!Directory.Exists(defaultFolder))
                {
                    Directory.CreateDirectory(defaultFolder);
                }

                string prefPath = Path.Combine(defaultFolder, "Preferences");
                Newtonsoft.Json.Linq.JObject json;

                if (File.Exists(prefPath))
                {
                    try
                    {
                        string content = File.ReadAllText(prefPath);
                        json = Newtonsoft.Json.Linq.JObject.Parse(string.IsNullOrWhiteSpace(content) ? "{}" : content);
                    }
                    catch
                    {
                        json = new Newtonsoft.Json.Linq.JObject();
                    }
                }
                else
                {
                    json = new Newtonsoft.Json.Linq.JObject();
                }

                // 1. Ghi vào nhánh profile.name
                if (json["profile"] == null) json["profile"] = new Newtonsoft.Json.Linq.JObject();
                json["profile"]["name"] = profileName;

                // 2. Ghi vào nhánh account_info (Bắt buộc đối với Chromium mới để tránh bị reset về "Your Chromium")
                if (json["account_info"] == null) json["account_info"] = new Newtonsoft.Json.Linq.JArray();

                File.WriteAllText(prefPath, json.ToString(Newtonsoft.Json.Formatting.Indented));

                // 3. Cập nhật file Local State (Quản lý ProfileCache ở thư mục gốc Profile)
                string localStatePath = Path.Combine(profileFolder, "Local State");
                Newtonsoft.Json.Linq.JObject localStateJson;

                if (File.Exists(localStatePath))
                {
                    try
                    {
                        string lsContent = File.ReadAllText(localStatePath);
                        localStateJson = Newtonsoft.Json.Linq.JObject.Parse(string.IsNullOrWhiteSpace(lsContent) ? "{}" : lsContent);
                    }
                    catch
                    {
                        localStateJson = new Newtonsoft.Json.Linq.JObject();
                    }
                }
                else
                {
                    localStateJson = new Newtonsoft.Json.Linq.JObject();
                }

                if (localStateJson["profile"] == null) localStateJson["profile"] = new Newtonsoft.Json.Linq.JObject();
                if (localStateJson["profile"]["info_cache"] == null) localStateJson["profile"]["info_cache"] = new Newtonsoft.Json.Linq.JObject();
                if (localStateJson["profile"]["info_cache"]["Default"] == null) localStateJson["profile"]["info_cache"]["Default"] = new Newtonsoft.Json.Linq.JObject();

                localStateJson["profile"]["info_cache"]["Default"]["name"] = profileName;

                File.WriteAllText(localStatePath, localStateJson.ToString(Newtonsoft.Json.Formatting.Indented));
            }
            catch { }
        }

        private void btn_editAcc_Click(object sender, EventArgs e)
        {
            // 1. Lấy danh sách các tài khoản đang tích chọn từ bảng chính
            List<AccountModel> selectedAccounts = GetSelectedAccountsFromGrid();

            if (selectedAccounts.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất 1 tài khoản để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Mở Form chứa DataGridView sửa và truyền danh sách sang
            using (FormEditAccount f = new FormEditAccount(selectedAccounts))
            {
                f.StartPosition = FormStartPosition.CenterParent;
                if (f.ShowDialog() == DialogResult.OK)
                {
                    // 3. Sau khi bấm Lưu ở form kia, tiến hành cập nhật lại file JSON gốc và load lại bảng chính
                    List<AccountModel> updatedAccounts = f.GetUpdatedAccounts();
                    MergeAndUpdateJson(updatedAccounts);

                    LoadAccountsToGrid();
                    MessageBox.Show("Cập nhật danh sách tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        // Hàm hỗ trợ cập nhật đè (Merge) những acc vừa sửa vào file accounts.json gốc
        private void MergeAndUpdateJson(List<AccountModel> editedList)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "accounts.json");
            if (!File.Exists(filePath)) return;

            try
            {
                string json = File.ReadAllText(filePath);
                var allAccounts = JsonConvert.DeserializeObject<List<AccountModel>>(json) ?? new List<AccountModel>();

                // Duyệt qua danh sách các acc đã sửa và cập nhật lại vào list gốc dựa theo User (UID)
                foreach (var editedAcc in editedList)
                {
                    var existingAcc = allAccounts.FirstOrDefault(x => x.User == editedAcc.User);
                    if (existingAcc != null)
                    {
                        existingAcc.Pass = editedAcc.Pass;
                        existingAcc.TwoFA = editedAcc.TwoFA;
                        existingAcc.Mail = editedAcc.Mail;
                        existingAcc.PassMail = editedAcc.PassMail;
                        existingAcc.Cookie = editedAcc.Cookie;
                        existingAcc.Token = editedAcc.Token;
                        existingAcc.IpV4 = editedAcc.IpV4;
                        existingAcc.IpV6 = editedAcc.IpV6;
                        existingAcc.IpXoay = editedAcc.IpXoay;
                        existingAcc.UserAgent = editedAcc.UserAgent;
                    }
                }

                // Ghi lại xuống file
                File.WriteAllText(filePath, JsonConvert.SerializeObject(allAccounts, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật file JSON: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_setting_Click(object sender, EventArgs e)
        {
            // Khởi tạo và mở FormSettings
            FormSettings f = new FormSettings();
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog();
        }

        private CancellationTokenSource cts;
        private void btn_stop_Click(object sender, EventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel(); // Hủy toàn bộ các luồng đang chạy song song
                groupBox1.Text = $"Chức năng chính:";
            }
        }
        public void CloseChromeDriver(ChromeDriver driver)
        {
            try
            {
                if (driver != null)
                {
                    // Chỉ đóng đúng cửa sổ trình duyệt và tiến trình driver của riêng luồng này
                    driver.Quit();
                    driver.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi đóng driver: {ex.Message}");
            }
        }

        private void btn_other_Click(object sender, EventArgs e)
        {
            FormOther subForm = new FormOther();
            subForm.Show();       
        }
    
    }
}