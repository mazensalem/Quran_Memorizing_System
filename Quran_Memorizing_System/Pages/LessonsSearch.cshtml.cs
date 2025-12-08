using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Quran_Memorizing_System.Pages
{
    public class Lesson
    {
        public string Instructor;
        public string Title;
        public List<string> Tags;
    }
    public class LessonsSearchModel : PageModel
    {
        public List<Lesson> Lessons { get; set; }
        public void OnGet()
        {
            Lessons = new List<Lesson>
            {
                new Lesson { Instructor = "Name", Title = "Lesson Title 1", Tags = new List<string> { "Tag1", "Tag2" } },
                new Lesson { Instructor = "Name", Title = "Lesson Title 2", Tags = new List<string> { "Tag1", "Tag2" } },
                new Lesson { Instructor = "Name", Title = "Lesson Title 3", Tags = new List<string> { "Tag1", "Tag2" } }
            };
        }
    }
}
