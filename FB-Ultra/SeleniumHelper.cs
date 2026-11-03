using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FB_Ultra
{
    public static class SeleniumHelper
    {
        public static IWebElement FindElementSafe(IWebDriver driver, int timeoutSeconds, params string[] xpaths)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));

            foreach (var xpath in xpaths)
            {
                try
                {
                    var element = wait.Until(drv => {
                        try
                        {
                            var el = drv.FindElement(By.XPath(xpath));
                            return (el != null && el.Displayed) ? el : null;
                        }
                        catch
                        {
                            return null;
                        }
                    });

                    if (element != null)
                        return element;
                }
                catch
                {
                    // Bỏ qua XPath này và thử XPath tiếp theo trong danh sách
                }
            }

            throw new NoSuchElementException("Không tìm thấy phần tử nào với các XPath đã cho.");
        }
    }
}