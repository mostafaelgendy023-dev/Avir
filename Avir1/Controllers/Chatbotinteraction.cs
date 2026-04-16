using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avir1.Models
{
    // =============================================
    // ChatbotInteraction — stores each chat message
    // =============================================
    public class ChatbotInteraction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public string SessionId { get; set; }

        [Required]
        public string Role { get; set; } // "user" or "bot"

        [Required]
        public string Message { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public Users User { get; set; }
    }

    // =============================================
    // TriageDecision — stores the risk assessment result
    // =============================================
    public class TriageDecision
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserID { get; set; }

        public string SessionId { get; set; }

        public int Age { get; set; }

        public bool FamilyHistory { get; set; }

        public string Symptoms { get; set; } // comma-separated

        public string LastMenstrualDate { get; set; }

        public double? BMI { get; set; }

        [Required]
        public string RiskLevel { get; set; } // "Low", "Medium", "High"

        public string Recommendation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public Users User { get; set; }
    }
}