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

        public IActionResult OnPost()
        {
            getuser();
            if (!user.isverified)
            {
                TempData["ErrorMessage"] = "You are not allowed to add a lesson";
                return RedirectToPage("/LessonsSearch");
            }

            string connectionString = "Data Source=MAZEN\\SQLEXPRESS;Initial Catalog=MemorizationSystem;Integrated Security=True;";
            string instructorEmail = user.Email;

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

