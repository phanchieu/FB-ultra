using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FB_Ultra
{
    public class ProxyModel
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public bool HasAuth => !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Pass);
    }

    public static class ProxyHelper
    {
        // Hàm Parse chung hỗ trợ cả IPv4 và IPv6 (Có hoặc không có Auth)
        public static ProxyModel ParseProxy(string proxyStr)
        {
            if (string.IsNullOrWhiteSpace(proxyStr)) return null;
            proxyStr = proxyStr.Trim();

            // 1. Tự động lột bỏ tiền tố http:// hoặc https:// nếu có ở đầu chuỗi
            if (proxyStr.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                proxyStr = proxyStr.Substring(7);
            }
            else if (proxyStr.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                proxyStr = proxyStr.Substring(8);
            }

            var result = new ProxyModel();

            // 2. Kiểm tra định dạng IPv6 bọc trong ngoặc vuông [...]
            if (proxyStr.StartsWith("["))
            {
                int closeBracketIdx = proxyStr.IndexOf(']');
                if (closeBracketIdx == -1) return null;

                result.Host = proxyStr.Substring(1, closeBracketIdx - 1);

                string remainder = proxyStr.Substring(closeBracketIdx + 1);
                if (!remainder.StartsWith(":")) return null;

                string[] parts = remainder.Substring(1).Split(':');
                if (parts.Length >= 1 && int.TryParse(parts[0], out int port))
                {
                    result.Port = port;
                }
                else
                {
                    return null;
                }

                if (parts.Length == 3)
                {
                    result.User = parts[1];
                    result.Pass = parts[2];
                }
                else if (parts.Length > 3)
                {
                    result.User = parts[1];
                    result.Pass = string.Join(":", parts, 2, parts.Length - 2);
                }
            }
            else
            {
                // 3. Định dạng chuẩn ip:port hoặc ip:port:user:pass (Dù là v4 hay v6 dạng thô)
                string[] parts = proxyStr.Split(':');
                if (parts.Length == 2)
                {
                    result.Host = parts[0];
                    if (int.TryParse(parts[1], out int port)) result.Port = port;
                    else return null;
                }
                else if (parts.Length >= 4)
                {
                    result.Host = parts[0];
                    if (int.TryParse(parts[1], out int port)) result.Port = port;
                    else return null;
                    result.User = parts[2];
                    result.Pass = string.Join(":", parts, 3, parts.Length - 3);
                }
                else
                {
                    return null;
                }
            }
            return result;
        }

        public static ProxyModel ParseIPv4(string proxyStr) => ParseProxy(proxyStr);

        // Tạo Extension Manifest V2 tích hợp Proxy (Cả IPv4 & IPv6) và chống vân tay
        public static string CreateProxyExtension(ProxyModel proxy, int profileSeed = 0)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (profileSeed == 0)
            {
                profileSeed = !string.IsNullOrEmpty(proxy.User) ? Math.Abs(proxy.User.GetHashCode()) % 9000 + 1000 : new Random().Next(1000, 9999);
            }

            // Xử lý an toàn: Thay thế dấu ':' trong IPv6 thành '_' để không bị lỗi tạo thư mục trên Windows
            string safeHost = proxy.Host.Replace(":", "_");
            string extDir = Path.Combine(baseDir, "ProxyExtensions", $"{safeHost}_{proxy.Port}_{profileSeed}");

            if (!Directory.Exists(extDir))
            {
                Directory.CreateDirectory(extDir);
            }

            string manifestJson = @"{
                ""version"": ""1.0.0"",
                ""manifest_version"": 2,
                ""name"": ""FB Ultra Shield & Proxy"",
                ""permissions"": [
                    ""proxy"",
                    ""tabs"",
                    ""unlimitedStorage"",
                    ""storage"",
                    ""webRequest"",
                    ""webRequestBlocking"",
                    ""<all_urls>""
                ],
                ""content_scripts"": [
                    {
                        ""matches"": [""<all_urls>""],
                        ""js"": [""inject.js""],
                        ""run_at"": ""document_start""
                    }
                ],
                ""background"": {
                    ""scripts"": [""background.js""]
                }
            }";

            // Chrome API chấp nhận chuỗi IPv6 thô bên trong trường host
            string backgroundJs = $@"
            var config = {{
                mode: ""fixed_servers"",
                rules: {{
                  singleProxy: {{
                    scheme: ""http"",
                    host: ""{proxy.Host}"",
                    port: parseInt({proxy.Port})
                  }},
                  bypassList: [""localhost""]
                }}
              }};

            chrome.proxy.settings.set({{value: config, scope: ""regular""}}, function() {{}});

            chrome.webRequest.onAuthRequired.addListener(
                function(details) {{
                    return {{
                        authCredentials: {{
                            username: ""{proxy.User}"",
                            password: ""{proxy.Pass}""
                        }}
                    }};
                }},
                {{urls: [""<all_urls>""]}},
                [""blocking""]
            );";

            string injectJs = $@"
            (function() {{
                const seed = {profileSeed};

                const actualCode = '(' + function(s) {{
                    const getParameter = WebGLRenderingContext.prototype.getParameter;
                    WebGLRenderingContext.prototype.getParameter = function(parameter) {{
                        if (parameter === 37445) return 'Google Inc. (NVIDIA)';
                        if (parameter === 37446) return 'ANGLE (NVIDIA, NVIDIA GeForce RTX 3060 Direct3D11 vs_5_0 ps_5_0)';
                        return getParameter.apply(this, arguments);
                    }};

                    const originalGetImageData = CanvasRenderingContext2D.prototype.getImageData;
                    CanvasRenderingContext2D.prototype.getImageData = function(sx, sy, sw, sh) {{
                        const imgData = originalGetImageData.apply(this, arguments);
                        try {{
                            if (imgData && imgData.data) {{
                                for (let i = 0; i < imgData.data.length; i += 16) {{
                                    imgData.data[i] = (imgData.data[i] + (s % 7)) % 256;
                                }}
                            }}
                        }} catch(e) {{}}
                        return imgData;
                    }};

                    const originalToDataURL = HTMLCanvasElement.prototype.toDataURL;
                    HTMLCanvasElement.prototype.toDataURL = function(type, quality) {{
                        try {{
                            const ctx = this.getContext('2d');
                            if (ctx) {{
                                const imgData = ctx.getImageData(0, 0, this.width, this.height);
                                for (let i = 0; i < imgData.data.length; i += 16) {{
                                    imgData.data[i] = (imgData.data[i] + (s % 7)) % 256;
                                }}
                                ctx.putImageData(imgData, 0, 0);
                            }}
                        }} catch(e) {{}}
                        return originalToDataURL.call(this, type, quality);
                    }};

                    Object.defineProperty(navigator, 'webdriver', {{ get: () => undefined }});
                }} + ')(' + seed + ');';

                const script = document.createElement('script');
                script.textContent = actualCode;
                (document.head || document.documentElement).appendChild(script);
                script.remove();
            }})();
            ";

            File.WriteAllText(Path.Combine(extDir, "manifest.json"), manifestJson);
            File.WriteAllText(Path.Combine(extDir, "background.js"), backgroundJs);
            File.WriteAllText(Path.Combine(extDir, "inject.js"), injectJs);

            return extDir;
        }
    }
}