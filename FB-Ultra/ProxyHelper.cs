using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;

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
        // Hàm tách chuỗi IPv4: Chấp nhận cả "IP:Port" và "IP:Port:User:Pass"
        public static ProxyModel ParseIPv4(string proxyStr)
        {
            if (string.IsNullOrWhiteSpace(proxyStr)) return null;

            string[] parts = proxyStr.Trim().Split(':');
            var result = new ProxyModel();

            if (parts.Length == 2)
            {
                result.Host = parts[0];
                if (int.TryParse(parts[1], out int port))
                {
                    result.Port = port;
                }
                else
                {
                    return null;
                }
            }
            else if (parts.Length == 4)
            {
                result.Host = parts[0];
                if (int.TryParse(parts[1], out int port))
                {
                    result.Port = port;
                }
                else
                {
                    return null;
                }
                result.User = parts[2];
                result.Pass = parts[3];
            }
            else
            {
                return null; // Chuỗi không đúng định dạng IPv4
            }

            return result;
        }

        // Hàm tự động sinh Extension Chrome để Authenticate Proxy (Chuẩn Manifest V3)
        public static string CreateProxyExtension(ProxyModel proxy)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string extDir = Path.Combine(baseDir, "ProxyExtensions", $"{proxy.Host}_{proxy.Port}");

            if (!Directory.Exists(extDir))
            {
                Directory.CreateDirectory(extDir);
            }

            // 1. Cấu hình manifest.json theo chuẩn Manifest V3
            string manifestJson = @"{
                ""version"": ""1.0.0"",
                ""manifest_version"": 3,
                ""name"": ""Chrome Proxy Auth"",
                ""permissions"": [
                    ""proxy"",
                    ""tabs"",
                    ""unlimitedStorage"",
                    ""storage"",
                    ""webRequest"",
                    ""webRequestAuthProvider""
                ],
                ""host_permissions"": [
                    ""<all_urls>""
                ],
                ""background"": {
                    ""service_worker"": ""background.js""
                }
            }";

            // 2. Cấu hình background.js tương thích Service Worker V3
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

            File.WriteAllText(Path.Combine(extDir, "manifest.json"), manifestJson);
            File.WriteAllText(Path.Combine(extDir, "background.js"), backgroundJs);

            return extDir;
        }
    }
}
