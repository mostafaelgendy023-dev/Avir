using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Avir1.Models
{
    public class ChatbotInteraction
    {
        [Key]
        public int Id { get; set; }
        public int UserID { get; set; }
        public string? SessionId { get; set; }
        public string? Role { get; set; }
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        [ForeignKey("UserID")]
        public Users? User { get; set; }
    }

    public class TriageDecision
    {
        [Key]
        public int Id { get; set; }
        public int UserID { get; set; }
        public string? SessionId { get; set; }
        public int Age { get; set; }
        public bool FamilyHistory { get; set; }
        public string? Symptoms { get; set; }
        public string? LastMenstrualDate { get; set; }
        public double? BMI { get; set; }
        public string? RiskLevel { get; set; }
        public string? Recommendation { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [ForeignKey("UserID")]
        public Users? User { get; set; }
    }
}