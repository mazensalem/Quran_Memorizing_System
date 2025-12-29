using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Data;
using System.Data.SqlClient;


namespace Quran_Memorizing_System.Pages
{
    public class Add_LessonModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public User user { get; set; }
        DB db;

        public Add_LessonModel(IConfiguration configuration, DB dB)
        {
            user = new User();
            db = dB;
            _configuration = configuration;
        }

        void getuser()
        {
            var role = HttpContext.Session.GetString("role");
            var email = HttpContext.Session.GetString("email");
            if (email == null)
            {
                return;
            }
            DataTable userdt = db.GetUser(email, role);

            user.UserName = Convert.ToString(userdt.Rows[0]["UserName"]);
            user.PhoneNumber = Convert.ToInt32(userdt.Rows[0]["Phone"]);
            user.Email = email;
            user.gender = Convert.ToString(userdt.Rows[0]["Gender"]);
            user.role = role;
            user.PhoneVisability = Convert.ToBoolean(userdt.Rows[0]["Phonevisability"]);
            user.DateOfBirth = Convert.ToDateTime(userdt.Rows[0]["DateofBirth"]).ToShortDateString();
            if (role == "Sheikh")
            {
                user.isverified = Convert.ToBoolean(userdt.Rows[0]["isverifed"]);
            }
        }

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public string Location { get; set; }

        [BindProperty]
        public int Availability { get; set; }

        [BindProperty]
        public string Lesson_URL { get; set; }

        public IActionResult OnGet()
        {
            getuser();
            if (!user.isverified)
            {
                TempData["ErrorMessage"] = "You are not allowed to add a lesson";
                return RedirectToPage("/LessonsSearch");
            }

            return Page();
        }

        // AJAX endpoint: /Add_Lesson?handler=CheckUrl&url=...
        public JsonResult OnGetCheckUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return new JsonResult(new { exists = false });
            }

            bool exists = db.LessonUrlExists(url.Trim());
            return new JsonResult(new { exists });
        }

        public IActionResult OnPost()
        {
            getuser();
            if (!user.isverified)
            {
                TempData["ErrorMessage"] = "You are not allowed to add a lesson";
                return RedirectToPage("/LessonsSearch");
            }

            // server-side duplicate check
            if (!string.IsNullOrWhiteSpace(Lesson_URL) && db.LessonUrlExists(Lesson_URL.Trim()))
            {
                ModelState.AddModelError("Lesson_URL", "This lesson URL is already in use.");
                return Page();
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string instructorEmail = user.Email;

            using SqlConnection con = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Lessons
          (Title, Location, Availability, lesson_url, instrutor_email)
          VALUES
          (@Title, @Location, @Availability, @Url, @Instructor)", con);

            cmd.Parameters.AddWithValue("@Title", Title ?? string.Empty);
            cmd.Parameters.AddWithValue("@Location", Location ?? string.Empty);
            cmd.Parameters.AddWithValue("@Availability", Availability);

            string urlValue = string.IsNullOrWhiteSpace(Lesson_URL) ? string.Empty : Lesson_URL.Trim();
            cmd.Parameters.AddWithValue("@Url", urlValue);

            cmd.Parameters.AddWithValue("@Instructor", instructorEmail ?? string.Empty);

            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToPage("/LessonsSearch");
        }
    }
}

