using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("repair_parts")]
    public class RepairPart
    {
        [Key]
        [Column("repair_part_id")]
        public int RepairPartId { get; set; }

        [Column("repair_id")]
        public int RepairId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("supplier_id")]
        public int? SupplierId { get; set; }

        [ForeignKey("RepairId")]
        public virtual Repair Repair { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}