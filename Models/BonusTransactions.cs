using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("bonus_transactions")]
    public class BonusTransaction
    {
        [Key]
        [Column("bonus_transaction_id")]
        public int BonusTransactionId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("sale_id")]
        public int? SaleId { get; set; }

        [Column("transaction_type")]
        public string TransactionType { get; set; } = string.Empty;

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("SaleId")]
        public virtual Sale? Sale { get; set; }
    }
}