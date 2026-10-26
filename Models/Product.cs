using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("sku")]
        public string Sku { get; set; } = string.Empty;

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("sale_price")]
        public decimal SalePrice { get; set; }

        [Column("warranty_months")]
        public int WarrantyMonths { get; set; }
    }
}