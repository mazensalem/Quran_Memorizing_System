using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;


namespace Quran_Memorizing_System.Pages
{
    public class EditLessonModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public EditLessonModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty(SupportsGet = true)]
        public int Lesson_ID { get; set; }

        [BindProperty]
        public string Title { get; set; } = "";

        [BindProperty]
        public string Location { get; set; } = "";

        [BindProperty]
        public int Availability { get; set; }

        [BindProperty]
        public string LessonUrl { get; set; } = "";

        public IActionResult OnGet()
        {
            string cs = _configuration.GetConnectionString("DefaultConnection");

            using SqlConnection con = new SqlConnection(cs);
            using SqlCommand cmd = new SqlCommand(
                @"SELECT Title, Location, Availability, lesson_url
              FROM Lessons
              WHERE Lesson_ID = @id", con);

            cmd.Parameters.AddWithValue("@id", Lesson_ID);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Title = reader["Title"].ToString();
                Location = reader["Location"].ToString();
                Availability = Convert.ToInt32(reader["Availability"]);
                LessonUrl = reader["lesson_url"].ToString();
            }
            else
            {
                return RedirectToPage("/LessonsSearch");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            string cs = _configuration.GetConnectionString("DefaultConnection");

            using SqlConnection con = new SqlConnection(cs);
            using SqlCommand cmd = new SqlCommand(
                @"UPDATE Lessons
              SET Title = @t,
                  Location = @l,
                  Availability = @a,
                  lesson_url = @u
              WHERE Lesson_ID = @id", con);

            cmd.Parameters.AddWithValue("@t", Title);
            cmd.Parameters.AddWithValue("@l", Location);
            cmd.Parameters.AddWithValue("@a", Availability);
            cmd.Parameters.AddWithValue("@u", LessonUrl);
            cmd.Parameters.AddWithValue("@id", Lesson_ID);

            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToPage("/LessonsSearch");
        }
    }

}