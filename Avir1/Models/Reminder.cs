using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avir1.Models
{
    public class Reminder
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime ReminderDate { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public Users User { get; set; }
    }
}