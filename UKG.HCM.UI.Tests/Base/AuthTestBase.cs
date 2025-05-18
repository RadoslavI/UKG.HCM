using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace UKG.HCM.UI.Tests.Base;

public abstract class AuthTestBase
{
    protected IWebDriver Driver;
    protected WebDriverWait Wait;
    protected const string BaseUrl = "https://localhost:7017";
    protected const string AdminEmail = "admin@ukg.com";
    protected const string AdminPassword = "Admin@123";
    protected const string ManagerEmail = "manager@ukg.com";
    protected const string ManagerPassword = "Manager@123";
    protected const string EmployeeEmail = "employee@ukg.com";
    protected const string EmployeePassword = "Employee@123";

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--disable-web-security");
        options.AddArgument("--allow-running-insecure-content");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        // options.AddArgument("--headless");

        Driver = new ChromeDriver(options);
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
    }

    [TearDown]
    public void TearDown()
    {
        Driver.Quit();
        Driver.Dispose();
    }
    
    protected void LoginAs(string email, string password)
    {
        Driver.Navigate().GoToUrl($"{BaseUrl}/Auth/Login");
        var emailInput    = Wait.Until(d => d.FindElement(By.Id("LoginRequest_Email")));
        var passwordInput = Driver.FindElement(By.Id("LoginRequest_Password"));
        var loginButton   = Driver.FindElement(By.CssSelector("button[type='submit']"));

        emailInput.Clear();
        emailInput.SendKeys(email);
        passwordInput.Clear();
        passwordInput.SendKeys(password);
        loginButton.Click();

        Wait.Until(d => d.FindElement(By.LinkText("People")));
    }
    
    protected void Logout()
    {
        Driver.Navigate().GoToUrl($"{BaseUrl}/Auth/Logout");
    }

    protected void LoginAsAdmin() => LoginAs(AdminEmail, AdminPassword);
    protected void LoginAsManager() => LoginAs(ManagerEmail, ManagerPassword);
    protected void LoginAsEmployee() => LoginAs(EmployeeEmail, EmployeePassword);
    
    protected (Guid, string) CreateTempPersonViaUI()
    {
        Driver.Navigate().GoToUrl($"{BaseUrl}/People/Create");

        var email = $"view_{Guid.NewGuid()}@example.com";
        var firstName = "View";
        var lastName = "User";
        var hireDate = DateTime.Today.ToString("dd-MM-yyyy");

        Driver.FindElement(By.Id("Person_FirstName")).SendKeys(firstName);
        Driver.FindElement(By.Id("Person_LastName")).SendKeys(lastName);
        Driver.FindElement(By.Id("Person_Email")).SendKeys(email);
        new SelectElement(Driver.FindElement(By.Id("Person_Role"))).SelectByText("Employee");
        Driver.FindElement(By.Id("Person_HireDate")).SendKeys(hireDate);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        Wait.Until(d => d.Url.Contains("/People/Details"));

        var id = Guid.Parse(Driver.Url.Split("/").Last());
        return (id, email);
    }
}
