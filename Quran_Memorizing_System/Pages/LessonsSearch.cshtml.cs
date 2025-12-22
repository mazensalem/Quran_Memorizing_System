using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Quran_Memorizing_System.Models;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class LessonsSearchModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Dictionary<string, object>> Lessons = new List<Dictionary<string, object>>();

        public User user { get; set; }
        DB db;

        public LessonsSearchModel(IConfiguration configuration, DB dB)
        {
            _configuration = configuration;
            user = new User();
            db = dB;
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

        public void OnGet()
        {
            getuser();
            string connectionString = "Data Source=MAZEN\\SQLEXPRESS;Initial Catalog=MemorizationSystem;Integrated Security=True;";

            using SqlConnection con = new SqlConnection(connectionString);

            SqlCommand cmd;

            
            if (HttpContext.Session.GetString("email") == null)
            {
                cmd = new SqlCommand(
                    "SELECT * FROM Lessons WHERE Availability = 1",
                    con
                );
            }
            
            else
            {
                cmd = new SqlCommand(
                    "SELECT * FROM Lessons",
                    con
                );
            }

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();


            //using SqlCommand cmd = new SqlCommand("SELECT * FROM Lessons", con);
            //con.Open();
            //SqlDataReader reader = cmd.ExecuteReader();



            while (reader.Read())
            {
                var lesson = new Dictionary<string, object>();
                lesson["Lesson_ID"] = reader["Lesson_ID"];
                lesson["Title"] = reader["Title"];
                lesson["Location"] = reader["Location"];
                lesson["Availability"] = reader["Availability"];
                lesson["instrutor_email"] = reader["instrutor_email"];
                lesson["lesson_url"] = reader["lesson_url"];
                Lessons.Add(lesson);
            }
        }

        public IActionResult OnPostDelete(int lessonId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using SqlConnection con = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand("DELETE FROM Lessons WHERE Lesson_ID=@id", con);
            cmd.Parameters.AddWithValue("@id", lessonId);
            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToPage("/LessonsSearch");
        }
    }
}

