using CommunityToolkit.Mvvm.ComponentModel;
using OpaMenu.Desktop.Models.DTOs.Aditional;

namespace OpaMenu.Desktop.ViewModels.Components;

public partial class SelectableAditional : ObservableObject
{
    public AditionalResponseDto Addon { get; }
    public SelectableAditionalGroup Group { get; }
    
    [ObservableProperty]
    private bool _isSelected;

    public string DisplayText => Addon.Price > 0m ? $"{Addon.Name} (+{Addon.Price:C})" : Addon.Name;

    public SelectableAditional(AditionalResponseDto addon, SelectableAditionalGroup group)
    {
        Addon = addon;
        Group = group;
    }

    partial void OnIsSelectedChanged(bool value)
    {
        Group.HandleSelectionChanged(this, value);
    }
}
