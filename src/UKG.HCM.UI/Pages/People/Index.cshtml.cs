using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UKG.HCM.UI.Pages.People
{
    public class IndexModel : PageModel
    {
        // A sample list of people
        public List<Person> People { get; set; }

        public void OnGet()
        {
            // Here you can replace this with logic to fetch people from a database
            People = new List<Person>
            {
                new Person { Name = "John Doe", Role = "Manager" },
                new Person { Name = "Jane Smith", Role = "Employee" },
                new Person { Name = "Paul Brown", Role = "Developer" }
            };
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Role { get; set; }
    }
}