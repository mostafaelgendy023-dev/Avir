using System;
using System.ComponentModel.DataAnnotations;

namespace Avir1.Models
{
    public class SymptomLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserID { get; set; } // Foreign Key to User

        [Required]
        public string SymptomType { get; set; }

        public string Description { get; set; }

        public DateTime DateLogged { get; set; } = DateTime.Now;
    }
}