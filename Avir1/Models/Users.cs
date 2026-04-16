using System;
using Microsoft.AspNetCore.Identity;

namespace Avir1.Models
{
    public class Users : IdentityUser<int>
    {
        // IdentityUser بيجيب معاه تلقائياً:
        // Id, UserName, Email, PasswordHash, PhoneNumber, etc.
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime? DOB { get; set; }
        public bool? FamilyHistory { get; set; }
        public double? BMI { get; set; }
        public string ActivityLevel { get; set; }
    }
}