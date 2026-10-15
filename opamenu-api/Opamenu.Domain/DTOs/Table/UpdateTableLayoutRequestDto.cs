using System;
using System.ComponentModel.DataAnnotations;

namespace Opamenu.Domain.DTOs.Table;

public record UpdateTableLayoutRequestDto(
    [Required]
    Guid TableId,
    
    [Required]
    double LayoutX,
    
    [Required]
    double LayoutY,
    
    [Required]
    double LayoutWidth,
    
    [Required]
    double LayoutHeight,
    
    string? Floor
);
