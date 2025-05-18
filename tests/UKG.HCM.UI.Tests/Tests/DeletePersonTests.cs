using OpenQA.Selenium;
using UKG.HCM.UI.Tests.Base;

namespace UKG.HCM.UI.Tests.Tests;

[TestFixture]
public class DeletePersonTests : UITestsBase
{
    private Guid _personId;

    [SetUp]
    public void Setup()
    {
        LoginAsAdmin();
        _personId = CreateTempPersonViaUI().Item1;
        Driver.Navigate().GoToUrl($"{BaseUrl}/People/Delete/{_personId}");
    }

    [Test]
    public void DeletePerson_ConfirmDelete_ShouldRedirectToIndex()
    {
        var deleteButton = Wait.Until(d => d.FindElement(By.CssSelector("form button[type='submit']")));
        deleteButton.Click();

        Wait.Until(d => d.Url.Contains("/People"));
        Assert.That(Driver.Url, Does.Contain("/People"));

        var pageSource = Driver.PageSource;
        Assert.IsFalse(pageSource.Contains(_personId.ToString()), "Deleted person still appears in list.");
    }

    [Test]
    public void DeletePerson_Cancel_ShouldReturnToIndex()
    {
        var cancelLink = Driver.FindElement(By.LinkText("Cancel"));
        cancelLink.Click();

        Wait.Until(d => d.Url.Contains("/People"));
        Assert.That(Driver.Url, Does.Contain("/People"));
    }

    [Test]
    public void DeletePerson_NonExistentId_ShouldRedirectWithError()
    {
        var fakeId = Guid.NewGuid();
        Driver.Navigate().GoToUrl($"{BaseUrl}/People/Delete/{fakeId}");

        Wait.Until(d => d.Url.Contains("/People"));
        Assert.That(Driver.Url, Does.Contain("/People"));
        Assert.That(Driver.PageSource, Does.Contain("404").IgnoreCase);
    }

    [Test]
    public void DeletePerson_AsManager_ShouldBeForbidden()
    {
        Logout();
        LoginAsManager();
        Driver.Navigate().GoToUrl($"{BaseUrl}/People/Delete/{_personId}");

        var accessDenied = Wait.Until(d =>
            d.Url.Contains("/AccessDenied") ||
            d.PageSource.Contains("not authorized", StringComparison.OrdinalIgnoreCase));

        Assert.IsTrue(accessDenied, "Manager should not be able to access Delete page.");
    }

    [Test]
    public void DeletePerson_AsEmployee_ShouldBeForbidden()
    {
        Logout();
        LoginAsEmployee();
        Driver.Navigate().GoToUrl($"{BaseUrl}/People/Delete/{_personId}");

        var accessDenied = Wait.Until(d =>
            d.Url.Contains("/AccessDenied") ||
            d.PageSource.Contains("not authorized", StringComparison.OrdinalIgnoreCase));

        Assert.IsTrue(accessDenied, "Employee should not be able to access Delete page.");
    }
}
