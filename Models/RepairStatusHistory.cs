using Service_Center.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("repair_status_history")]
    public class RepairStatusHistory
    {
        [Key]
        [Column("history_id")]
        public int HistoryId { get; set; }

        [Column("repair_id")]
        public int RepairId { get; set; }

        [Column("old_status_id")]
        public int? OldStatusId { get; set; }

        [Column("new_status_id")]
        public int NewStatusId { get; set; }

        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Навігаційні властивості
        [ForeignKey("RepairId")]
        public virtual Repair Repair { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}