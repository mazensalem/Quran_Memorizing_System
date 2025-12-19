using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Quran_Memorizing_System.Models;

namespace Quran_Memorizing_System.Pages
{
    public class LessonsSearchModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Dictionary<string, object>> Lessons = new List<Dictionary<string, object>>();

        public LessonsSearchModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using SqlConnection con = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand("SELECT * FROM Lessons", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

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

