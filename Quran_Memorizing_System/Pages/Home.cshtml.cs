using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Quran_Memorizing_System.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public record Circle(string Name, string AdminName, string ImageUrl);
        public record Notification(string Message, string TimeAgo);

        public List<Circle> Circles { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;

            // sample data to avoid null references in Razor during development
            Circles = new List<Circle>
            {
                new Circle("Tajweed Circle", "Ahmed", "/images/circle1.png"),
                new Circle("Memorization Circle", "Fatima", "/images/circle2.png")
            };

            Notifications = new List<Notification>
            {
                new Notification("New lesson available", "2h ago"),
                new Notification("Exam scheduled tomorrow", "1d ago")
            };
        }

        public void OnGet()
        {
        }
    }
}
