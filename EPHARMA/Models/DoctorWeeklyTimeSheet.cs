using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPHARMA.Models
{
    public class DoctorWeeklyTimeSheet
    {
        [Key]
        public int DoctorWeeklyTimeSheetId { get; set; }
        [Required]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctors { get; set; }
        public string WeekDay { get; set; }
        public string TimeRange { get; set; }
        public bool Available { get; set; } = true;
        public bool Status { get; set; } = true;
        public DateTime Date { get; set; }= getDate();
        public static DateTime getDate()
        {
            DateTime date1 = DateTime.UtcNow;
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime date2 = TimeZoneInfo.ConvertTime(date1, tz);
            return date2;
        }
    }
}
