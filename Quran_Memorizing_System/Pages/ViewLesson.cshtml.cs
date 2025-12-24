using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Quran_Memorizing_System.Pages
{
    public class ViewLessonModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ViewLessonModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Title = "";
        public string Location = "";
        public string Availability = "";
        public string lesson_url = "";
        public string instrutor_email = "";

        public void OnGet()
        {
            int id = int.Parse(Request.Query["id"]);
            string cs = _configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Elabd;Initial Catalog=MemorizationSystem;Integrated Security=True;";

            using SqlConnection con = new SqlConnection(cs);
            using SqlCommand cmd = new SqlCommand(
                @"SELECT Title, Location, Availability, lesson_url, instrutor_email 
                  FROM Lessons 
                  WHERE Lesson_ID = @id", con);

            cmd.Parameters.AddWithValue("@id", id);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Title = reader["Title"].ToString();
                Location = reader["Location"].ToString();
                Availability = reader["Availability"].ToString();
                lesson_url = reader["lesson_url"].ToString();
                instrutor_email = reader["instrutor_email"].ToString();
            }
        }
    }
}