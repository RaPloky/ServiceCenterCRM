using Service_Center.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("employee_shifts")]
    public class EmployeeShift
    {
        [Key]
        [Column("shift_id")]
        public int ShiftId { get; set; }

        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Column("shift_date")]
        public DateTime ShiftDate { get; set; }

        [Column("start_time")]
        public TimeSpan StartTime { get; set; }

        [Column("end_time")]
        public TimeSpan EndTime { get; set; }

        // Навігаційна властивість
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}