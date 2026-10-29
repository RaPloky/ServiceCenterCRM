using Service_Center.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("sales")]
    public class Sale
    {
        [Key]
        [Column("sale_id")]
        public int SaleId { get; set; }

        [Column("sale_number")]
        public string SaleNumber { get; set; } = string.Empty;

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Column("sale_date")]
        public DateTime SaleDate { get; set; }

        [Column("bonus_used")]
        public decimal BonusUsed { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}