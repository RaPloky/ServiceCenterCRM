using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("customer_devices")]
    public class CustomerDevice
    {
        [Key]
        [Column("device_id")]
        public int DeviceId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("device_type")]
        public string DeviceType { get; set; } = string.Empty;

        [Column("brand")]
        public string Brand { get; set; } = string.Empty;

        [Column("model")]
        public string Model { get; set; } = string.Empty;

        [Column("serial_number")]
        public string? SerialNumber { get; set; }

        // Навігаційна властивість для зв'язку з клієнтом
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}