using Service_Center.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("repairs")]
    public class Repair
    {
        [Key]
        [Column("repair_id")]
        public int RepairId { get; set; }

        [Column("repair_number")]
        public string RepairNumber { get; set; } = string.Empty;

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("device_id")]
        public int DeviceId { get; set; }

        [Column("created_by")]
        public int CreatedBy { get; set; }

        [Column("assigned_employee_id")]
        public int? AssignedEmployeeId { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("customer_description")]
        public string? CustomerDescription { get; set; }

        [Column("internal_notes")]
        public string? InternalNotes { get; set; }

        [Column("estimated_price")]
        public decimal EstimatedPrice { get; set; }

        [Column("final_price")]
        public decimal? FinalPrice { get; set; }

        [Column("received_at")]
        public DateTime ReceivedAt { get; set; } = DateTime.Now;

        [Column("estimated_completion_date")]
        public DateTime? EstimatedCompletionDate { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [Column("issued_at")]
        public DateTime? IssuedAt { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("StatusId")]
        public virtual RepairStatus RepairStatus { get; set; }

        [ForeignKey("AssignedEmployeeId")]
        public virtual Employee AssignedEmployee { get; set; }
    }
}