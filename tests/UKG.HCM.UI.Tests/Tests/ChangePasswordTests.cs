using OpenQA.Selenium;
using UKG.HCM.UI.Tests.Base;

namespace UKG.HCM.UI.Tests.Tests;

[TestFixture]
public class ChangePasswordTests : UITestsBase
{
    private const string ChangePasswordUrl = $"{BaseUrl}/Auth/ChangePassword";
    private const string NewPassword = "NewStrongPassword@123";

    [SetUp]
    public void BeforeEach()
    {
        LoginAsAdmin();
        Driver.Navigate().GoToUrl(ChangePasswordUrl);
    }

    [Test]
    public void ChangePassword_WithValidData_ShouldShowSuccessMessage_AndResetPassword()
    {
        var currentPasswordInput = Driver.FindElement(By.Id("PasswordRequest_CurrentPassword"));
        var newPasswordInput = Driver.FindElement(By.Id("PasswordRequest_NewPassword"));
        var confirmNewPasswordInput = Driver.FindElement(By.Id("PasswordRequest_ConfirmPassword"));
        var changeButton = Driver.FindElement(By.CssSelector("button[type='submit']"));

        currentPasswordInput.Clear();
        currentPasswordInput.SendKeys(AdminPassword);
        newPasswordInput.Clear();
        newPasswordInput.SendKeys(NewPassword);
        confirmNewPasswordInput.Clear();
        confirmNewPasswordInput.SendKeys(NewPassword);

        changeButton.Click();

        var successMessage = Wait.Until(d => d.FindElement(By.CssSelector(".alert-success")));
        Assert.IsTrue(successMessage.Displayed, "Success message was not displayed after password change.");
        Assert.IsTrue(successMessage.Text.Contains("Password changed"), "Success message text not as expected.");

        // Reset password back to AdminPassword so subsequent tests are stable
        ResetPasswordToAdminPassword();
    }

    private void ResetPasswordToAdminPassword()
    {
        Wait.Until(d => d.FindElement(By.Id("PasswordRequest_CurrentPassword")));

        var currentPasswordInput = Driver.FindElement(By.Id("PasswordRequest_CurrentPassword"));
        var newPasswordInput = Driver.FindElement(By.Id("PasswordRequest_NewPassword"));
        var confirmNewPasswordInput = Driver.FindElement(By.Id("PasswordRequest_ConfirmPassword"));
        var changeButton = Driver.FindElement(By.CssSelector("button[type='submit']"));

        currentPasswordInput.Clear();
        currentPasswordInput.SendKeys(NewPassword);
        newPasswordInput.Clear();
        newPasswordInput.SendKeys(AdminPassword);
        confirmNewPasswordInput.Clear();
        confirmNewPasswordInput.SendKeys(AdminPassword);

        changeButton.Click();

        var resetSuccessMessage = Wait.Until(d => d.FindElement(By.CssSelector(".alert-success")));
        Assert.IsTrue(resetSuccessMessage.Displayed, "Success message not shown after resetting password.");
        Assert.IsTrue(resetSuccessMessage.Text.Contains("Password changed"), "Reset password success message text incorrect.");
    }

    [Test]
    public void ChangePassword_WithIncorrectCurrentPassword_ShowsErrorMessage()
    {
        var currentPasswordInput = Driver.FindElement(By.Id("PasswordRequest_CurrentPassword"));
        var newPasswordInput = Driver.FindElement(By.Id("PasswordRequest_NewPassword"));
        var confirmNewPasswordInput = Driver.FindElement(By.Id("PasswordRequest_ConfirmPassword"));
        var changeButton = Driver.FindElement(By.CssSelector("button[type='submit']"));

        currentPasswordInput.Clear();
        currentPasswordInput.SendKeys("WrongPassword@123");
        newPasswordInput.Clear();
        newPasswordInput.SendKeys(NewPassword);
        confirmNewPasswordInput.Clear();
        confirmNewPasswordInput.SendKeys(NewPassword);

        changeButton.Click();

        var errorMessage = Wait.Until(d => d.FindElement(By.CssSelector(".alert-danger")));

        Assert.IsTrue(errorMessage.Displayed, "Error message was not displayed for incorrect current password.");
        Assert.IsTrue(errorMessage.Text.Contains("Change password failed"),
            "Expected error message for incorrect current password not shown.");
    }

    [Test]
    public void ChangePassword_WithMismatchedNewPasswords_ShowsValidationError()
    {
        var currentPasswordInput = Driver.FindElement(By.Id("PasswordRequest_CurrentPassword"));
        var newPasswordInput = Driver.FindElement(By.Id("PasswordRequest_NewPassword"));
        var confirmNewPasswordInput = Driver.FindElement(By.Id("PasswordRequest_ConfirmPassword"));
        var changeButton = Driver.FindElement(By.CssSelector("button[type='submit']"));

        currentPasswordInput.Clear();
        currentPasswordInput.SendKeys(AdminPassword);
        newPasswordInput.Clear();
        newPasswordInput.SendKeys(NewPassword);
        confirmNewPasswordInput.Clear();
        confirmNewPasswordInput.SendKeys("DifferentPassword@123");

        changeButton.Click();

        var validationMessage = Wait.Until(d => d.FindElement(By.CssSelector("span[data-valmsg-for='PasswordRequest.ConfirmPassword']")));

        Assert.IsTrue(validationMessage.Displayed, "Validation message for confirm password was not displayed.");
        Assert.IsFalse(string.IsNullOrWhiteSpace(validationMessage.Text), "Validation message for confirm password is empty.");
    }

    [Test]
    public void ChangePassword_WithEmptyFields_ShowsValidationMessages()
    {
        var changeButton = Driver.FindElement(By.CssSelector("button[type='submit']"));
        changeButton.Click();

        var currentPasswordValidation = Wait.Until(d => d.FindElement(By.CssSelector("span[data-valmsg-for='PasswordRequest.CurrentPassword']")));
        var newPasswordValidation = Driver.FindElement(By.CssSelector("span[data-valmsg-for='PasswordRequest.NewPassword']"));
        var confirmPasswordValidation = Driver.FindElement(By.CssSelector("span[data-valmsg-for='PasswordRequest.ConfirmPassword']"));

        Assert.IsTrue(currentPasswordValidation.Displayed, "Validation message for current password not displayed.");
        Assert.IsTrue(newPasswordValidation.Displayed, "Validation message for new password not displayed.");
        Assert.IsTrue(confirmPasswordValidation.Displayed, "Validation message for confirm password not displayed.");

        Assert.IsFalse(string.IsNullOrWhiteSpace(currentPasswordValidation.Text), "Current password validation message is empty.");
        Assert.IsFalse(string.IsNullOrWhiteSpace(newPasswordValidation.Text), "New password validation message is empty.");
        Assert.IsFalse(string.IsNullOrWhiteSpace(confirmPasswordValidation.Text), "Confirm password validation message is empty.");
    }
}