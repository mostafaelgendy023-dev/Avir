using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Avir1.Models;

namespace Avir1.Data
{
    public class AppDbContext : IdentityDbContext<Users, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // باقي الـ DbSets بتاعتك هنا


// ---- existing tables (unchanged) ----
public DbSet<Users> Users { get; set; }
        public DbSet<Reminder> Reminder { get; set; }
        public DbSet<Upload> Upload { get; set; }
        public DbSet<SymptomLog> SymptomLogs { get; set; }
        public DbSet<SelfExam> SelfExams { get; set; }
       
        public DbSet<MedicalImage> MedicalImages { get; set; }
        public DbSet<Radiology_Library> Radiology_Libraries { get; set; }
        public DbSet<ResultSummary> ResultSummaries { get; set; }

        // ---- new tables (Group 3) ----
        public DbSet<ChatbotInteraction> ChatbotInteractions { get; set; }
        public DbSet<TriageDecision> TriageDecisions { get; set; }
        public DbSet<SurvivorStory> SurvivorStories { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlans { get; set; }
        public DbSet<CommunityPost> CommunityPosts { get; set; }
    }
}