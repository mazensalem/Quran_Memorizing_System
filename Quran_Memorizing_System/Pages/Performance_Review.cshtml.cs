using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Quran_Memorizing_System.Pages
{
    public class PerformanceRecord
    {
        public string Date;
        public string Issuer;
        public int Percentage;
        public PerformanceRecord(string d, string i, int p)
        {
            Date = d;Issuer = i;Percentage = p;
        }
    }
    public class Performace_ReviewModel : PageModel
    {
        public List<PerformanceRecord> P_Records { get; set; }
        public void OnGet()
        {
            P_Records = new List<PerformanceRecord> {
                new PerformanceRecord("2024-02-01", "Ahmed", 90),
                new PerformanceRecord("2024-02-05", "Mohamed", 100),
                new PerformanceRecord("2024-02-20", "Abdalla", 90),

            };
        }
    }
}
