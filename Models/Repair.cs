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
        public int CustomerId { get; set; } // Додано зв'язок з клієнтом

        [Column("device_id")]
        public int DeviceId { get; set; }   // Додано зв'язок з пристроєм

        [Column("created_by")]
        public int CreatedBy { get; set; }  // Додано зв'язок з працівником

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("customer_description")]
        public string CustomerDescription { get; set; } = string.Empty;

        [Column("received_at")]
        public DateTime ReceivedAt { get; set; }

        [Column("estimated_price")]
        public decimal EstimatedPrice { get; set; }
    }
}