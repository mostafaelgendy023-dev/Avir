using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avir1.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public string Email { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime ReminderDate { get; set; }
    }
}