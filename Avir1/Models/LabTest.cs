using System;
using System.ComponentModel.DataAnnotations;

namespace Avir1.Models
{
    public class LabTest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public string Type { get; set; }

        public string Result { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}