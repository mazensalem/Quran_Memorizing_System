using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class SearchCircuitsModel : PageModel
    {
        [BindProperty]
        public string searchinput { get; set; }
        
        DB db;
        
        public DataTable Circles { get; set; }
        public SearchCircuitsModel(DB dB)
        {
            db = dB;
            Circles = new DataTable();
        }
        public void OnGet()
        {
            Circles = db.getallCircles();
        }

        public void OnPostSearch() {
            Circles = db.findCircuits(searchinput);
        }
    }
}
