using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using UKG.HCM.UI.Tests.Base;

namespace UKG.HCM.UI.Tests.Tests;

[TestFixture]
public class EditPageTests : AuthTestBase
{
    private Guid _personId;
    private string _personEmail;
    
    [SetUp]
    public new void Setup()
    {
        LoginAsAdmin();
        (_personId, _personEmail) = CreateTempPersonViaUI();
    }
    
    [Test]
    public async Task EditPage_AllowsAdminToEditPerson()
    {
        // Arrange
        await Driver.Navigate().GoToUrlAsync($"{BaseUrl}/People/Edit/{_personId}");

        // Act
        Driver.FindElement(By.Id("Person_FirstName")).Clear();
        Driver.FindElement(By.Id("Person_FirstName")).SendKeys("Updated");

        Driver.FindElement(By.Id("Person_LastName")).Clear();
        Driver.FindElement(By.Id("Person_LastName")).SendKeys("User");

        Driver.FindElement(By.Id("Person_Email")).Clear();
        Driver.FindElement(By.Id("Person_Email")).SendKeys($"updateduser{DateTime.Now.Ticks}@example.com");

        var roleSelect = new SelectElement(Driver.FindElement(By.Id("Person_Role")));
        roleSelect.SelectByValue("Manager");

        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        Wait.Until(d => d.Url.Contains("/People/Details"));

        var fullNameText = Driver.FindElement(By.CssSelector(".row div")).Text;
        Assert.That(fullNameText.Contains("Updated User"));
    }

    [Test]
    public async Task EditPage_DisplaysValidationErrors_WhenRequiredFieldsMissing()
    {
        await Driver.Navigate().GoToUrlAsync($"{BaseUrl}/People/Edit/{_personId}");
    
        Driver.FindElement(By.Id("Person_FirstName")).Clear();
        Driver.FindElement(By.Id("Person_LastName")).Clear();
        Driver.FindElement(By.Id("Person_Email")).Clear();
    
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
    
        var validationMessages = Driver.FindElements(By.CssSelector(".text-danger"));
        Assert.That(validationMessages, Is.Not.Empty);
    }
    
    [Test]
    public async Task EditPage_ForbidsEmployeeFromAccessingEdit()
    {
        Logout();
        LoginAsEmployee();    
        await Driver.Navigate().GoToUrlAsync($"{BaseUrl}/People/Edit/{_personId}");
    
        Assert.That(Driver.Url, Does.Contain("AccessDenied").IgnoreCase);
    }
}
