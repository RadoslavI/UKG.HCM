using OpenQA.Selenium;
using UKG.HCM.UI.Tests.Base;

namespace UKG.HCM.UI.Tests
{
    [TestFixture]
    public class LoginTests : AuthTestBase
    {
        [Test]
        public void Login_WithValidCredentials_ShouldRedirectToHomeAndShowPeopleLink()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Auth/Login");

            var emailInput = Driver.FindElement(By.Id("LoginRequest_Email"));
            var passwordInput = Driver.FindElement(By.Id("LoginRequest_Password"));
            var loginButton = Driver.FindElement(By.CssSelector("button[type='submit']"));

            emailInput.SendKeys(AdminEmail);
            passwordInput.SendKeys(AdminPassword);

            loginButton.Click();

            var peopleLink = Wait.Until(d => d.FindElement(By.LinkText("People")));
            Assert.IsNotNull(peopleLink, "Login failed or People link not found.");
        }

        [Test]
        public void Login_WithInvalidCredentials_ShowsErrorMessage()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Auth/Login");

            var emailInput = Driver.FindElement(By.CssSelector("input[name='LoginRequest.Email']"));
            var passwordInput = Driver.FindElement(By.CssSelector("input[name='LoginRequest.Password']"));
            var loginButton = Driver.FindElement(By.CssSelector("button[type='submit']"));

            emailInput.SendKeys("invalid@example.com");
            passwordInput.SendKeys("wrongpassword");
            loginButton.Click();

            var errorMessage = Wait.Until(d => d.FindElement(By.CssSelector(".alert-danger")));

            Assert.IsTrue(errorMessage.Displayed);
            Assert.IsTrue(errorMessage.Text.Contains("Unauthorized"), "Error message not displayed or incorrect.");
        }

        [Test]
        public void Login_WithEmptyFields_ShowsValidationMessages()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Auth/Login");

            var loginButton = Wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();

            var emailValidation = Wait.Until(d => d.FindElement(By.CssSelector("span[data-valmsg-for='LoginRequest.Email']")));
            var passwordValidation = Driver.FindElement(By.CssSelector("span[data-valmsg-for='LoginRequest.Password']"));

            Assert.IsTrue(emailValidation.Displayed);
            Assert.IsTrue(passwordValidation.Displayed);
            Assert.IsFalse(string.IsNullOrWhiteSpace(emailValidation.Text));
            Assert.IsFalse(string.IsNullOrWhiteSpace(passwordValidation.Text));
        }

        [Test]
        public void Logout_ShouldRedirectToHomeAndShowLoginLink()
        {
            // Login first
            LoginAsAdmin();

            // Open user dropdown and click logout
            var userDropdown = Driver.FindElement(By.Id("userDropdown"));
            userDropdown.Click();

            var logoutLink = Wait.Until(d => d.FindElement(By.CssSelector("a.dropdown-item[href='/Auth/Logout']")));
            logoutLink.Click();

            // Wait for redirect to home or index page
            Wait.Until(d => d.Url == $"{BaseUrl}/" || d.Url == $"{BaseUrl}/Index");

            var loginLink = Driver.FindElement(By.LinkText("Login"));
            Assert.IsTrue(loginLink.Displayed, "Login link not found after logout.");

            var userDropdowns = Driver.FindElements(By.Id("userDropdown"));
            Assert.IsEmpty(userDropdowns, "User dropdown should not be visible after logout.");
        }

        [Test]
        public void AccessProtectedPageAfterLogout_ShouldRedirectToLogin()
        {
            // Login first
            LoginAsAdmin();

            // Logout
            var userDropdown = Driver.FindElement(By.Id("userDropdown"));
            userDropdown.Click();
            var logoutLink = Wait.Until(d => d.FindElement(By.CssSelector("a.dropdown-item[href='/Auth/Logout']")));
            logoutLink.Click();

            Wait.Until(d => d.Url == $"{BaseUrl}/" || d.Url == $"{BaseUrl}/Index");

            // Try to access protected page after logout
            Driver.Navigate().GoToUrl($"{BaseUrl}/People/Index");

            Wait.Until(d => d.Url.Contains("/Auth/Login"));

            Assert.IsTrue(Driver.Url.Contains("/Auth/Login"),
                "User was not redirected to login page after trying to access protected page post-logout.");
        }
    }
}
