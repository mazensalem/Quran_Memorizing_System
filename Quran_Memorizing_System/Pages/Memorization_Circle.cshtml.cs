using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Quran_Memorizing_System.Pages
{
    public class MemorizationCircleModel : PageModel
    {
        public record Post(string AuthorName, string Content);
        public record Member(string Name, string Status);

        public List<Post> Posts { get; set; } = new();
        public List<Member> Members { get; set; } = new();
        public string Schedule { get; set; } = string.Empty;
        public bool isAdmin { get; set; }

        public void OnGet()
        {
            // sample data
            Posts = new List<Post>
            {
                new Post("Ali", "Welcome to the circle!"),
                new Post("Sara", "Please review from page 1 to 20   .")
            };

            Members = new List<Member>
            {
                new Member("Omar", "Active"),
                new Member("Huda", "Pending")
            };

            Schedule = "Saturdays 5PM";
            isAdmin = true;
        }
    }
}
