using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("repair_statuses")]
    public class RepairStatus
    {
        [Key]
        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;
    }
}