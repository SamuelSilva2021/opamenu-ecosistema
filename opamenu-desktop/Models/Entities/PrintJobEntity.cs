using System;
using System.ComponentModel.DataAnnotations;
using OpaMenu.Desktop.Models.Enums;

namespace OpaMenu.Desktop.Models.Entities;

public class PrintJobEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastAttemptAt { get; set; }

    public EPrintDestination Destination { get; set; }
    public string PayloadType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;

    public EPrintJobStatus Status { get; set; } = EPrintJobStatus.Pending;
    public int Attempts { get; set; }
    public string? LastError { get; set; }
}

