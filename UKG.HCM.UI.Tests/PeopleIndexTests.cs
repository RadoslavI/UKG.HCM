using OpenQA.Selenium;
using UKG.HCM.UI.Tests.Base;

namespace UKG.HCM.UI.Tests;

[TestFixture]
public class PeopleIndexTests : AuthTestBase
{
    private const string PeopleIndexUrl = $"{BaseUrl}/People/Index";

    [SetUp]
    public void SetupTest()
    {
        LoginAsAdmin();
        Driver.Navigate().GoToUrl(PeopleIndexUrl);
    }

    [Test]
    public void Page_ShouldDisplayTitle()
    {
        var heading = Driver.FindElement(By.TagName("h2"));
        Assert.AreEqual("People Management", heading.Text);
    }

    [Test]
    public void AddNewPersonButton_ShouldBeVisibleForManagerOrAbove()
    {
        var addButton = Driver.FindElements(By.CssSelector("a.btn.btn-primary"));
        Assert.IsTrue(addButton.Any(btn => btn.Text.Contains("Add New Person")));
    }

    [Test]
    public void AddNewPersonButton_ShouldNotBeVisibleForRegularEmployee()
    {
        Logout();
        LoginAsEmployee();
        Driver.Navigate().GoToUrl(PeopleIndexUrl);

        var addButtons = Driver.FindElements(By.CssSelector("a.btn.btn-primary"));
        Assert.IsFalse(addButtons.Any(btn => btn.Text.Contains("Add New Person")));
    }

    [Test]
    public void PeopleTable_DisplaysCorrectColumns_AndRows()
    {
        var tableHeaders = Driver.FindElements(By.CssSelector("table thead th"));
        string[] expectedHeaders = { "Full Name", "Email", "Role", "HireDate", "Actions" };
        CollectionAssert.AreEquivalent(expectedHeaders, tableHeaders.Select(h => h.Text).ToArray());

        var tableRows = Driver.FindElements(By.CssSelector("table tbody tr"));
        Assert.IsTrue(tableRows.Count > 0, "Expected people records in the table.");
    }
        
    [Test]
    public void PeopleTable_ShouldShowViewEditDeleteButtons_BasedOnRoles()
    {
        // Admin: all three buttons
        var row = Driver.FindElement(By.CssSelector("table tbody tr"));
        var btns = row.FindElements(By.CssSelector("td:last-child a.btn")).Select(b => b.Text).ToList();
        Assert.Contains("View", btns);
        Assert.Contains("Edit", btns);
        Assert.Contains("Delete", btns);

        // Manager: no Delete
        Logout();
        LoginAsManager();
        Driver.Navigate().GoToUrl(PeopleIndexUrl);
        row = Driver.FindElement(By.CssSelector("table tbody tr"));
        btns = row.FindElements(By.CssSelector("td:last-child a.btn")).Select(b => b.Text).ToList();
        Assert.Contains("View", btns);
        Assert.Contains("Edit", btns);
        Assert.IsFalse(btns.Contains("Delete"));

        // Employee: only View
        Logout();
        LoginAsEmployee();
        Driver.Navigate().GoToUrl(PeopleIndexUrl);
        row = Driver.FindElement(By.CssSelector("table tbody tr"));
        btns = row.FindElements(By.CssSelector("td:last-child a.btn")).Select(b => b.Text).ToList();
        Assert.Contains("View", btns);
        Assert.IsFalse(btns.Contains("Edit"));
        Assert.IsFalse(btns.Contains("Delete"));
    }

    [Test]
    public void EmployeeSeesOnlyTheirOwnRecord()
    {
        // Log in as a seeded employee and verify only their row appears
        Logout();
        LoginAsEmployee();
        Driver.Navigate().GoToUrl(PeopleIndexUrl);

        var rows = Driver.FindElements(By.CssSelector("table tbody tr"));
        Assert.AreEqual(1, rows.Count, "Employee should see exactly one row.");

        var emailCell = rows[0].FindElement(By.CssSelector("td:nth-child(2)")).Text;
        Assert.That(emailCell, Is.EqualTo(EmployeeEmail));
    }
}