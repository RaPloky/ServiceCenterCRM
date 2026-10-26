using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("sale_items")]
    public class SaleItem
    {
        [Key]
        [Column("sale_item_id")]
        public int SaleItemId { get; set; }

        [Column("sale_id")]
        public int SaleId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }
    }
}