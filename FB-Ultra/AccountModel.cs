using System;
using System.Collections.Generic;
using System.Text;

namespace FB_Ultra
{
    public class AccountModel
    {
        public string User { get; set; } = "";
        public string Pass { get; set; } = "";
        public string TwoFA { get; set; } = "";
        public string Mail { get; set; } = "";
        public string PassMail { get; set; } = "";
        public string Cookie { get; set; } = "";
        public string Token { get; set; } = "";

        public string IpV4 { get; set; }
        public string IpV6 { get; set; }
        public string IpXoay { get; set; }
        public string UserAgent { get; set; }
        // Lưu Proxy kèm loại (v4, v6, xoay) trong cùng 1 object
        public string ProxyType { get; set; } = ""; // "v4", "v6", "xoay" hoặc ""
        public string Proxy { get; set; } = "";     // "192.168.1.1:8080"
        public string Status { get; set; } = "Sẵn sàng";
    }
}
