using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("warehouse_stock")]
    public class WarehouseStock
    {
        [Key]
        [Column("stock_id")]
        public int StockId { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("reserved_quantity")]
        public int ReservedQuantity { get; set; }

        [Column("minimum_quantity")]
        public int MinimumQuantity { get; set; }

        // Навігаційна властивість для зв'язку з товаром
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}