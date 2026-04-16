using System;
using System.ComponentModel.DataAnnotations;

namespace Avir1.Models
{
    public class Radiology_Library
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ImageID { get; set; } // Foreign Key to MedicalImage

        public string AnalysisType { get; set; }

        public string Findings { get; set; }

        public DateTime DateAnalyzed { get; set; } = DateTime.Now;
    }
}