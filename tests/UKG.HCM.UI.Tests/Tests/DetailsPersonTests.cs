using OpenQA.Selenium;
using UKG.HCM.UI.Tests.Base;

namespace UKG.HCM.UI.Tests.Tests;

[TestFixture]
public class DetailsPersonTests : UITestsBase
{
    private Guid _personId;
    private string _personEmail;

    [SetUp]
    public void Setup()
    {
        LoginAsAdmin();
        (_personId, _personEmail) = CreateTempPersonViaUI();
    }

    [Test]
    public void Details_AsAdmin_ShouldDisplayInfo()
    {
        Driver.Navigate().GoToUrl($"{BaseUrl}/People/Details/{_personId}");

        var fullName = Driver.FindElement(By.XPath("//div[contains(@class,'row')][1]/div[2]")).Text;
        var email = Driver.FindElement(By.XPath("//div[contains(@class,'row')][2]/div[2]")).Text;
        var role = Driver.FindElement(By.XPath("//div[contains(@class,'row')][3]/div[2]")).Text;

        Assert.Multiple(() =>
        {
            Assert.That(fullName, Is.Not.Empty);
            Assert.That(email, Is.EqualTo(_personEmail));
            Assert.That(role, Is.EqualTo("Employee"));
        });
    }

    [Test]
    public void Details_NonexistentId_ShouldRedirectWithError()
    {
        var fakeId = Guid.NewGuid();
        Driver.Navigate().GoToUrl($"{BaseUrl}/People/Details/{fakeId}");
        
        Assert.That(Driver.PageSource, Does.Contain("404").IgnoreCase);
    }

    [Test]
    public void Details_AsOtherEmployee_ShouldBeForbidden()
    {
        // Create another user
        Logout();
        LoginAsEmployee(); // This user must not match _personEmail

        Driver.Navigate().GoToUrl($"{BaseUrl}/People/Details/{_personId}");

        // Forbidden is returned as 403 HTML page or redirect to access denied page depending on config
        Assert.That(Driver.PageSource, Does.Contain("AccessDenied").IgnoreCase
                    .Or.Contain("403").IgnoreCase
                    .Or.Contain("forbidden").IgnoreCase);
    }
}
