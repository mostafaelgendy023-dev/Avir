using System;
using System.ComponentModel.DataAnnotations;

namespace Avir1.Models
{
    public class ResultSummary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserID { get; set; }

        public string SummaryText { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}