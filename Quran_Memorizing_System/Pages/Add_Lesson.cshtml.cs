using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


namespace Quran_Memorizing_System.Pages
{
    public class Add_LessonModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public Add_LessonModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public string Location { get; set; }

        [BindProperty]
        public string Availability { get; set; }

        [BindProperty]
        public string Lesson_URL { get; set; }

        public IActionResult OnGet()
        {
            if (!(User.Identity.IsAuthenticated || User.IsInRole("Sheikh")))
                return RedirectToPage("/LessonsSearch");

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!(User.Identity.IsAuthenticated || User.IsInRole("Sheikh")))
                return RedirectToPage("/LessonsSearch");

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string instructorEmail = User.Identity.Name;

            using SqlConnection con = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Lessons
          (Title, Location, Availability, lesson_url, instrutor_email)
          VALUES
          (@Title, @Location, @Availability, @Url, @Instructor)", con);

            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@Location", Location);
            cmd.Parameters.AddWithValue("@Availability", Availability);
            cmd.Parameters.AddWithValue("@Url", Lesson_URL);
            cmd.Parameters.AddWithValue("@Instructor", instructorEmail);

            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToPage("/LessonsSearch");
        }
    }
}

