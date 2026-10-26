using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("customers")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Column("middle_name")]
        public string? MiddleName { get; set; }

        [Column("phone")]
        public string Phone { get; set; } = string.Empty;

        [Column("bonus_balance")]
        public decimal BonusBalance { get; set; }
    }
}