using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("payments")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("sale_id")]
        public int SaleId { get; set; }

        [Column("payment_method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("SaleId")]
        public virtual Sale Sale { get; set; }
    }
}