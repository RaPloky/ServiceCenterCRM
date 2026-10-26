using Service_Center.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("salary_records")]
    public class SalaryRecord
    {
        [Key]
        [Column("salary_id")]
        public int RecordId { get; set; }

        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Column("period")]
        public string Period { get; set; } = string.Empty;

        [Column("salary")]
        public decimal Amount { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("paid_at")]
        public DateTime? PaymentDate { get; set; }

        // Навігаційна властивість до працівника
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}