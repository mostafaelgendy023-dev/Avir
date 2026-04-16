using System;
using System.ComponentModel.DataAnnotations;

namespace Avir1.Models
{
    public class MedicalImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public DateTime DateUploaded { get; set; } = DateTime.Now;
    }
}