using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;

namespace FB_Ultra
{
    public static class SystemHelper
    {
        private static string _cachedUserAgent = null;

        /// <summary>
        /// Lấy User-Agent thực tế từ nhân Chrome/Orbita trên máy
        /// </summary>
        public static string GetSystemUserAgent(string orbitaPath)
        {
            // Nếu đã lấy 1 lần thì dùng lại cache để đỡ phải khởi động lại driver
            if (!string.IsNullOrEmpty(_cachedUserAgent))
                return _cachedUserAgent;

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var service = ChromeDriverService.CreateDefaultService(baseDir);
                service.HideCommandPromptWindow = true;

                var options = new ChromeOptions();
                if (File.Exists(orbitaPath))
                {
                    options.BinaryLocation = orbitaPath;
                }
                options.AddArgument("--headless=new"); // Chạy ẩn
                options.AddArgument("--disable-gpu");

                using (IWebDriver tempDriver = new ChromeDriver(service, options))
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)tempDriver;
                    _cachedUserAgent = js.ExecuteScript("return navigator.userAgent;").ToString();
                }
            }
            catch
            {
                // Fallback User-Agent Windows 10/11 Chrome 112 chuẩn nếu gặp lỗi
                _cachedUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.5615.138 Safari/537.36";
            }

            return _cachedUserAgent;
        }
    }
}
