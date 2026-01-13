using OpaMenu.Application.Common.Interfaces;

namespace OpaMenu.Application.Common.Models;

public class FileUploadResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? FileName { get; set; }
    public string? OriginalFileName { get; set; }
    public string? FilePath { get; set; }
    public string? FileUrl { get; set; }
    public long FileSize { get; set; }
    public string? ThumbnailUrl { get; set; } = null!;
    public string? MediumUrl { get; set; } = null!;
    public string? LargeUrl { get; set; } = null!;
    public string? MimeType { get; set; }
    public DateTime UploadDate { get; set; }
    public List<string> ImageVariants { get; set; } = new();
    public ImageMetadata? ImageMetadata { get; set; }
    public List<string> Variants { get; set; } = new();
}