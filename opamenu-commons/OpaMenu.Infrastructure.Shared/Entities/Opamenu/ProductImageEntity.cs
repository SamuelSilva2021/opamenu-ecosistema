using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;

[Table("product_images")]
public class ProductImageEntity : BaseEntity
{
    [Required]
    [Column("product_id")]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("file_name")]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("original_name")]
    public string OriginalName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column("file_path")]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("mime_type")]
    public string MimeType { get; set; } = string.Empty;

    [Required]
    [Column("file_size")]
    public long FileSize { get; set; }

    [Column("width")]
    public int Width { get; set; }

    [Column("height")]
    public int Height { get; set; }

    [MaxLength(50)]
    [Column("aspect_ratio")]
    public string? AspectRatio { get; set; }

    [Required]
    [Column("upload_date")]
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    [Column("is_primary")]
    public bool IsPrimary { get; set; } = false;

    [Column("deleted_date")]
    public DateTime? DeletedDate { get; set; }

    [MaxLength(500)]
    [Column("thumbnail_path")]
    public string? ThumbnailPath { get; set; }

    [MaxLength(500)]
    [Column("medium_path")]
    public string? MediumPath { get; set; }

    [MaxLength(500)]
    [Column("large_path")]
    public string? LargePath { get; set; }

    // Navigation Properties
    [ForeignKey("ProductId")]
    public virtual ProductEntity Product { get; set; } = null!;
}

