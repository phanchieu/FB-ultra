using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;

namespace FB_Ultra
{
    public class FacebookLoginService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private ChromeDriver driver;

        public FacebookLoginService(ChromeDriver chromeDriver)
        {
            this.driver = chromeDriver;
        }

        public bool LoginFacebook(AccountModel acc)
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.facebook.com");
                Thread.Sleep(2000);

                if (CheckIsLoggedIn())
                {
                    return true;
                }

                if (!string.IsNullOrEmpty(acc.Cookie))
                {
                    bool isCookieSuccess = InjectAndCheckCookie(acc.Cookie);
                    if (isCookieSuccess) return true;
                }

                if (!string.IsNullOrEmpty(acc.User) && !string.IsNullOrEmpty(acc.Pass))
                {
                    return LoginWithUserPass(acc);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool InjectAndCheckCookie(string cookieString)
        {
            try
            {
                if (!driver.Url.Contains("facebook.com"))
                {
                    driver.Navigate().GoToUrl("https://www.facebook.com");
                    Thread.Sleep(1500);
                }

                driver.Manage().Cookies.DeleteAllCookies();

                string[] cookies = cookieString.Split(';');
                foreach (var cookie in cookies)
                {
                    var parts = cookie.Split('=');
                    if (parts.Length >= 2)
                    {
                        string name = parts[0].Trim();
                        string value = string.Join("=", parts, 1, parts.Length - 1).Trim();
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                        {
                            try
                            {
                                driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(
                                    name,
                                    value,
                                    ".facebook.com",
                                    "/",
                                    DateTime.Now.AddDays(30)
                                ));
                            }
                            catch { }
                        }
                    }
                }

                driver.Navigate().Refresh();
                Thread.Sleep(4000);

                return CheckIsLoggedIn();
            }
            catch
            {
                return false;
            }
        }

        private bool LoginWithUserPass(AccountModel acc)
        {
            try
            {
                if (!driver.Url.Contains("facebook.com"))
                {
                    driver.Navigate().GoToUrl("https://www.facebook.com/");
                    Thread.Sleep(2000);
                }

                IWebElement txtUser = FindElementSafe(By.Id("email"))
                                   ?? FindElementSafe(By.Name("email"))
                                   ?? FindElementSafe(By.CssSelector("input[type='text'], input[name='email']"));

                if (txtUser != null)
                {
                    txtUser.Clear();
                    txtUser.SendKeys(acc.User);
                    Thread.Sleep(500);
                }

                IWebElement txtPass = FindElementSafe(By.Id("pass"))
                                   ?? FindElementSafe(By.Name("pass"))
                                   ?? FindElementSafe(By.CssSelector("input[type='password'], input[name='pass']"));

                if (txtPass != null)
                {
                    txtPass.Clear();
                    txtPass.SendKeys(acc.Pass);
                    Thread.Sleep(500);
                }

                IWebElement btnLogin = FindElementSafe(By.Name("login"))
                                    ?? FindElementSafe(By.CssSelector("button[type='submit']"));

                if (btnLogin != null)
                {
                    btnLogin.Click();
                }
                else if (txtPass != null)
                {
                    txtPass.SendKeys(OpenQA.Selenium.Keys.Enter);
                }

                Thread.Sleep(5000);

                if (CheckIs2FAForm())
                {
                    if (!string.IsNullOrEmpty(acc.TwoFA))
                    {
                        string code2FA = Get2FACode(acc.TwoFA);
                        if (!string.IsNullOrEmpty(code2FA))
                        {
                            Submit2FACode(code2FA);
                        }
                    }
                }

                return CheckIsLoggedIn();
            }
            catch
            {
                return false;
            }
        }

        private IWebElement FindElementSafe(By by, int timeoutSeconds = 5)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            try
            {
                return wait.Until(d =>
                {
                    try
                    {
                        var elem = d.FindElement(by);
                        return (elem != null && elem.Displayed) ? elem : null;
                    }
                    catch
                    {
                        return null;
                    }
                });
            }
            catch
            {
                return null;
            }
        }

        private string Get2FACode(string secretKey)
        {
            try
            {
                secretKey = secretKey.Replace(" ", "").Trim();
                string url = $"https://2fa.live/tok/{secretKey}";
                string response = httpClient.GetStringAsync(url).Result;

                var match = Regex.Match(response, @"\d{6}");
                if (match.Success)
                {
                    return match.Value;
                }
            }
            catch { }
            return "";
        }

        private void Submit2FACode(string code)
        {
            try
            {
                IWebElement txt2FA = null;
                try { txt2FA = driver.FindElement(By.Id("approvals_code")); } catch { }
                if (txt2FA == null)
                {
                    try { txt2FA = driver.FindElement(By.CssSelector("input[type='text'], input[autocomplete='one-time-code']")); } catch { }
                }

                if (txt2FA != null)
                {
                    txt2FA.SendKeys(code);
                    Thread.Sleep(500);

                    try { driver.FindElement(By.Id("checkpointSubmitButton")).Click(); }
                    catch
                    {
                        txt2FA.SendKeys(OpenQA.Selenium.Keys.Enter);
                    }
                    Thread.Sleep(3000);

                    try
                    {
                        var btnContinue = driver.FindElement(By.Id("checkpointSubmitButton"));
                        if (btnContinue != null) btnContinue.Click();
                    }
                    catch { }
                }
            }
            catch { }
        }

        private bool CheckIs2FAForm()
        {
            string url = driver.Url.ToLower();
            string source = driver.PageSource.ToLower();
            return url.Contains("checkpoint") || source.Contains("approvals_code") || source.Contains("two_factor") || source.Contains("2fa");
        }

        private bool CheckIsLoggedIn()
        {
            try
            {
                var cUserCookie = driver.Manage().Cookies.GetCookieNamed("c_user");
                if (cUserCookie != null && !string.IsNullOrEmpty(cUserCookie.Value))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}