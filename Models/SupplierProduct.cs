using Service_Center.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("supplier_products")]
    public class SupplierProduct
    {
        [Key]
        [Column("supplier_product_id")]
        public int SupplierProductid { get; set; }

        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("purchase_price")]
        public decimal PurchasePrice { get; set; }

        [Column("delivery_days")]
        public int DeliveryDays { get; set; }

        [Column("is_available")]
        public bool IsAvailable { get; set; }

        // Навігаційні властивості
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}