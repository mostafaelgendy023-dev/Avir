using System;
using System.ComponentModel.DataAnnotations;

namespace Avir1.Models
{
    public class SelfExam
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserID { get; set; }

        public string Result { get; set; }

        public DateTime DatePerformed { get; set; } = DateTime.Now;

        public string Notes { get; set; }
    }
}