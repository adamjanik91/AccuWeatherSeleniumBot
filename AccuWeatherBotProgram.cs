using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using Xunit.Abstractions;

namespace AccuWeatherBot
{
    public class AccuWeatherBotProgram
    {

        private readonly int _pageLoadTimeoutMs = 10000;

        private readonly string _baseUrl = "https://www.accuweather.com/";

        private readonly string _cookieConsentCssSelector = "button.fc-cta-consent";
        private readonly string _privacyConsentCssSelector = ".banner-button.policy-accept";

        private readonly string _searchLocationInputCssSelector = "input.search-input";

        private readonly string _targetLocation = "Kraków";

        private readonly string _locationListFirstCssSelector = ".locations-list a";

        private readonly string _tempContainerCssSelector = ".temp-container .temp";


        private readonly ITestOutputHelper output;
        private readonly IWebDriver driver;

        public AccuWeatherBotProgram(ITestOutputHelper output)
        {
            this.output = output;  
            driver = new ChromeDriver();
        }


        [Fact]
        public void CheckTemp()
        {
            Init();

       
            driver.Navigate().GoToUrl(_baseUrl);

            Thread.Sleep(_pageLoadTimeoutMs);

            string currentUrl = driver.Url;
            Assert.True(currentUrl ==_baseUrl, $"The current URL does not match the expected URL. Expected: '{_baseUrl}'");

            var cookieConsentPrimaryBtnBy = By.CssSelector(_cookieConsentCssSelector);
            var cookieConsentPrimaryBtn = driver.FindElement(cookieConsentPrimaryBtnBy);
            ScrollToElement(driver, cookieConsentPrimaryBtn);
            WaitUntilVisible(driver, cookieConsentPrimaryBtnBy);
            ClickElement(driver, cookieConsentPrimaryBtn);

            Thread.Sleep(300);

            var privacyConsentBy = By.CssSelector(_privacyConsentCssSelector);
            var privacyConsent = driver.FindElement(privacyConsentBy);
            ScrollToElement(driver, privacyConsent);
            WaitUntilVisible(driver, privacyConsentBy);
            ClickElement(driver, privacyConsent);

            Thread.Sleep(300);

                
            var searchLocationInputBy = By.CssSelector(_searchLocationInputCssSelector);
            var searchLocationInput = driver.FindElement(searchLocationInputBy);
            InputValueWithEnterConfirm(driver, searchLocationInput, _targetLocation);

            Thread.Sleep(_pageLoadTimeoutMs);

            var locationListFirstBy = By.CssSelector(_locationListFirstCssSelector);
            var locationListFirst = driver.FindElement(locationListFirstBy);
            ClickElement(driver, locationListFirst);

            Thread.Sleep(_pageLoadTimeoutMs);

            var tempContainerBy = By.CssSelector(_tempContainerCssSelector);
            var tempContainer = driver.FindElement(tempContainerBy);

            var temp = tempContainer.Text;

            var tempMessage = $"The current temp in {_targetLocation} is: '{temp}'";

            output.WriteLine(tempMessage);

            Assert.True(temp != null && temp != String.Empty, tempMessage);
            
        }


        public void RepeatTries(IWebDriver driver, int times, int intervalMs)
        {

            for (int i = 0; i < times; i++)
            {
                
                driver.Navigate().Refresh();
                Thread.Sleep(_pageLoadTimeoutMs);

                Thread.Sleep(intervalMs);
            }
        }

   

        private void Init()
        {
            var options = new ChromeOptions();
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--allow-http-screen-capture");
            options.AddArgument("--disable-impl-side-painting");
            options.AddArgument("--disable-setuid-sandbox");
            options.AddArgument("--disable-seccomp-filter-sandbox");
            options.AddArgument("--user-agent=\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.61 Safari/537.36\"");
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
        }

        private void InputValueWithEnterConfirm(IWebDriver driver, IWebElement element, string newValue) {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].focus();", element);
            Thread.Sleep(200);
            element.SendKeys(newValue);
            Thread.Sleep(200);
            element.SendKeys(Keys.Enter);
        }


        private void InputValue(IWebDriver driver, IWebElement element, string newValue)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].focus();", element);
            Thread.Sleep(200);
            js.ExecuteScript("arguments[0].value = arguments[1];", element, newValue);
        }

        private void ScrollToElement(IWebDriver driver, IWebElement element)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        private void WaitUntilVisible(IWebDriver driver, By by)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(by));
        }

        private void ClickElement(IWebDriver driver, IWebElement element)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", element);
        }



    }
}
