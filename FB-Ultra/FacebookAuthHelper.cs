using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace FB_Ultra
{
    public static class FacebookAuthHelper
    {
        /// <summary>
        /// Bơm Cookie dạng chuỗi (c_user=...; xs=...;) vào trình duyệt
        /// </summary>
        public static void PatchChromeDriver(string driverPath)
        {
            try
            {
                if (!File.Exists(driverPath)) return;

                byte[] bytes = File.ReadAllBytes(driverPath);
                string content = System.Text.Encoding.ASCII.GetString(bytes);

                if (content.Contains("$cdc_"))
                {
                    content = content.Replace("$cdc_asdjflasutdfhvcZLmcfl_", "$abc_asdjflasutdfhvcZLmcfl_");
                    File.WriteAllBytes(driverPath, System.Text.Encoding.ASCII.GetBytes(content));
                }
            }
            catch
            {
                // Bỏ qua nếu file đang được sử dụng bởi tiến trình khác
            }
        }
        public static bool InjectCookie(IWebDriver driver, string rawCookie)
        {
            if (string.IsNullOrWhiteSpace(rawCookie)) return false;

            try
            {
                // 1. Mở Facebook nếu chưa ở đúng trang để Chrome nhận Domain
                if (!driver.Url.Contains("facebook.com"))
                {
                    driver.Navigate().GoToUrl("https://www.facebook.com/");
                    Thread.Sleep(1500);
                }

                // 2. Tách từng cặp cookie bởi dấu ;
                string[] cookiePairs = rawCookie.Split(';');
                foreach (var pair in cookiePairs)
                {
                    string trimmed = pair.Trim();
                    if (string.IsNullOrEmpty(trimmed) || !trimmed.Contains("=")) continue;

                    int equalIndex = trimmed.IndexOf('=');
                    string name = trimmed.Substring(0, equalIndex).Trim();
                    string value = trimmed.Substring(equalIndex + 1).Trim();

                    // Khởi tạo Cookie gán vào domain .facebook.com
                    var cookie = new OpenQA.Selenium.Cookie(
                        name,
                        value,
                        ".facebook.com",
                        "/",
                        DateTime.Now.AddDays(60)
                    );

                    driver.Manage().Cookies.AddCookie(cookie);
                }

                // 3. F5 lại trang để nhận Cookie vừa bơm
                driver.Navigate().Refresh();
                Thread.Sleep(2500);

                return IsLoggedIn(driver);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi Inject Cookie: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Bơm Token EAAA... vào LocalStorage của trình duyệt
        /// </summary>
        public static void InjectToken(IWebDriver driver, string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                // Lưu Token vào LocalStorage
                js.ExecuteScript($"window.localStorage.setItem('accessToken', '{token.Trim()}');");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi Inject Token: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra xem nick đã đăng nhập thành công chưa
        /// </summary>
        public static bool IsLoggedIn(IWebDriver driver)
        {
            try
            {
                var cUser = driver.Manage().Cookies.GetCookieNamed("c_user");
                if (cUser != null && !string.IsNullOrEmpty(cUser.Value))
                {
                    string currentUrl = driver.Url.ToLower();
                    if (!currentUrl.Contains("checkpoint") && !currentUrl.Contains("login"))
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }
    }
}
