using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avir1.Models
{
    // =============================================
    // SurvivorStory — inspirational videos/stories
    // =============================================
    public class SurvivorStory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Content { get; set; }

        public string VideoUrl { get; set; }

        public bool IsApproved { get; set; } = false;

        public DateTime DatePosted { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public Users User { get; set; }
    }

    // =============================================
    // TreatmentPlan — personalized treatment schedule
    // =============================================
    public class TreatmentPlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string NutritionalAdvice { get; set; }

        public string PsychologicalSupport { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime? EndDate { get; set; }

        [ForeignKey("UserID")]
        public Users User { get; set; }
    }

    // =============================================
    // CommunityPost — forum posts
    // =============================================
    public class CommunityPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;

        public int Likes { get; set; } = 0;

        [ForeignKey("UserID")]
        public Users User { get; set; }
    }
}