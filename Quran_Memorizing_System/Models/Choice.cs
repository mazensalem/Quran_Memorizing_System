using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Quran_Memorizing_System.Models
{
    public class Choice
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }


        public string Text { get; set; }
    }
}
