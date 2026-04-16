using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avir1.Models
{
    public class Upload
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        public int UserID { get; set; }

        public Users User { get; set; }
    }
}