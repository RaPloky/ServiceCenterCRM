using Service_Center.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Store.Models
{
    [Table("repair_attachments")]
    public class RepairAttachment
    {
        [Key]
        [Column("attachment_id")]
        public int AttachmentId { get; set; }

        [Column("repair_id")]
        public int RepairId { get; set; }

        [Column("file_path")]
        public string FilePath { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("uploaded_by")]
        public int UploadedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Навігаційні властивості
        [ForeignKey("RepairId")]
        public virtual Repair Repair { get; set; }

        [ForeignKey("UploadedBy")]
        public virtual Employee Employee { get; set; }
    }
}