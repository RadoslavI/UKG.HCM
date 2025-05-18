using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using UKG.HCM.UI.Tests.Base;

namespace UKG.HCM.UI.Tests;

[TestFixture]
public class CreatePersonTests : AuthTestBase
{
    private const string CreatePageUrl = $"{BaseUrl}/People/Create";

    [SetUp]
    public void BeforeEach()
    {
        LoginAsAdmin();
        Driver.Navigate().GoToUrl(CreatePageUrl);
    }

    [Test]
    public void CreatePerson_WithValidData_ShouldRedirectToDetails()
    {
        var firstNameInput = Driver.FindElement(By.Id("Person_FirstName"));
        var lastNameInput = Driver.FindElement(By.Id("Person_LastName"));
        var emailInput = Driver.FindElement(By.Id("Person_Email"));
        var roleSelect = new SelectElement(Driver.FindElement(By.Id("Person_Role")));
        var hireDateInput = Driver.FindElement(By.Id("Person_HireDate"));
        var createButton = Driver.FindElement(By.CssSelector("button[type='submit']"));

        string uniqueEmail = $"test{Guid.NewGuid()}@example.com";

        firstNameInput.SendKeys("Test");
        lastNameInput.SendKeys("User");
        emailInput.SendKeys(uniqueEmail);
        roleSelect.SelectByText("Employee");
        hireDateInput.SendKeys(DateTime.Today.ToString("dd-MM-yyyy"));

        createButton.Click();

        // Wait for redirection to Details page
        Wait.Until(d => d.Url.Contains("/People/Details"));

        Assert.That(Driver.Url, Does.Contain("/People/Details"), "Did not redirect to Details page after creation.");
    }

    [Test]
    public void CreatePerson_WithMissingFields_ShouldShowValidationErrors()
    {
        var createButton = Driver.FindElement(By.CssSelector("button[type='submit']"));
        createButton.Click();

        var firstNameValidation = Wait.Until(d => d.FindElement(By.CssSelector("span[data-valmsg-for='Person.FirstName']")));
        var lastNameValidation = Driver.FindElement(By.CssSelector("span[data-valmsg-for='Person.LastName']"));
        var emailValidation = Driver.FindElement(By.CssSelector("span[data-valmsg-for='Person.Email']"));

        Assert.Multiple(() =>
        {
            Assert.IsTrue(firstNameValidation.Displayed, "First name validation error not shown.");
            Assert.IsTrue(lastNameValidation.Displayed, "Last name validation error not shown.");
            Assert.IsTrue(emailValidation.Displayed, "Email validation error not shown.");
        });
    }

    [Test]
    public void CreatePerson_WithInvalidEmail_ShouldShowEmailValidationError()
    {
        Driver.FindElement(By.Id("Person_FirstName")).SendKeys("Test");
        Driver.FindElement(By.Id("Person_LastName")).SendKeys("User");
        Driver.FindElement(By.Id("Person_Email")).SendKeys("invalid-email");
        new SelectElement(Driver.FindElement(By.Id("Person_Role"))).SelectByText("Employee");
        Driver.FindElement(By.Id("Person_HireDate")).SendKeys(DateTime.Today.ToString("dd-MM-yyyy"));
        
        var emailValidation = Wait.Until(d => d.FindElement(By.CssSelector("span[data-valmsg-for='Person.Email']")));
        Assert.IsTrue(emailValidation.Displayed);
    }
    
    [Test]
    public void CreatePerson_AsManager_ShouldAccessPage()
    {
        Logout();
        LoginAsManager();
        Driver.Navigate().GoToUrl(CreatePageUrl);

        var heading = Wait.Until(d => d.FindElement(By.TagName("h3")));
        Assert.That(heading.Text, Is.EqualTo("Create New Person"));
    }
    
    [Test]
    public void CreatePerson_AsEmployee_ShouldBeForbidden()
    {
        Logout();
        LoginAsEmployee();
        Driver.Navigate().GoToUrl(CreatePageUrl);

        // Expect to either be redirected or see access denied
        bool accessDenied = Wait.Until(d =>
            d.Url.Contains("/AccessDenied"));

        Assert.IsTrue(accessDenied, "Employee should not be able to access Create page.");
    }
}