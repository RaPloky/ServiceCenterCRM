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

        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("sale_date")]
        public DateTime SaleDate { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }
    }
}