using CommunityToolkit.Mvvm.ComponentModel;
using OpaMenu.Desktop.Models.DTOs.Aditional;
using OpaMenu.Desktop.Models.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace OpaMenu.Desktop.ViewModels.Components;

public partial class SelectableAditionalGroup : ObservableObject
{
    private bool _isHandling;

    public Guid Id { get; }
    public string Name { get; }
    public string? Description { get; }
    public EAditionalGroupType Type { get; }
    public bool IsRequired { get; }
    public int? MinSelections { get; }
    public int? MaxSelections { get; }

    public ObservableCollection<SelectableAditional> Options { get; } = new();

    public SelectableAditionalGroup(AditionalGroupResponseDto dto, bool isRequired, int? minSelections, int? maxSelections)
    {
        Id = dto.Id;
        Name = dto.Name;
        Description = dto.Description;
        Type = dto.Type;
        IsRequired = isRequired;
        MinSelections = minSelections;
        MaxSelections = maxSelections;
    }

    public void HandleSelectionChanged(SelectableAditional option, bool isSelected)
    {
        if (_isHandling)
            return;

        try
        {
            _isHandling = true;

            if (Type == EAditionalGroupType.Single && isSelected)
            {
                foreach (var other in Options.Where(o => !ReferenceEquals(o, option) && o.IsSelected))
                {
                    other.IsSelected = false;
                }
                return;
            }

            var effectiveMax = GetEffectiveMax();
            if (effectiveMax.HasValue && SelectedCount() > effectiveMax.Value)
            {
                option.IsSelected = false;
            }
        }
        finally
        {
            _isHandling = false;
        }
    }

    public bool IsSelectionValid()
    {
        var count = SelectedCount();
        var min = GetEffectiveMin();
        var max = GetEffectiveMax();

        if (count < min)
            return false;

        if (max.HasValue && count > max.Value)
            return false;

        return true;
    }

    public string GetSelectionRangeText()
    {
        var min = GetEffectiveMin();
        var max = GetEffectiveMax();
        if (max.HasValue)
            return $"Selecione entre {min} e {max.Value}.";

        return min > 0 ? $"Selecione no mínimo {min}." : "Seleção livre.";
    }

    private int GetEffectiveMin()
    {
        if (IsRequired && (!MinSelections.HasValue || MinSelections.Value < 1))
            return 1;

        if (Type == EAditionalGroupType.Single)
            return MinSelections.HasValue ? Math.Clamp(MinSelections.Value, 0, 1) : (IsRequired ? 1 : 0);

        return MinSelections ?? 0;
    }

    private int? GetEffectiveMax()
    {
        if (Type == EAditionalGroupType.Single)
            return 1;

        return MaxSelections;
    }

    private int SelectedCount() => Options.Count(o => o.IsSelected);
}
