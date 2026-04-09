using CommunityToolkit.Mvvm.ComponentModel;
using OpaMenu.Desktop.Models.DTOs;
using OpaMenu.Desktop.Models.DTOs.Product;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace OpaMenu.Desktop.ViewModels.Screens;

public sealed class PdvScreenViewModel : ObservableObject
{
    private readonly MainViewModel _root;

    public PdvScreenViewModel(MainViewModel root)
    {
        _root = root;
        _root.PropertyChanged += RootOnPropertyChanged;
    }

    public ObservableCollection<CategoryDto> Categories => _root.Categories;
    public ObservableCollection<ProductDto> Products => _root.Products;
    public ObservableCollection<CartItemDto> CartItems => _root.CartItems;

    public CategoryDto? SelectedCategory
    {
        get => _root.SelectedCategory;
        set => _root.SelectedCategory = value;
    }

    public string CurrentCategoryTitle => _root.CurrentCategoryTitle;

    public decimal CartTotal => _root.CartTotal;

    public string TableNumber
    {
        get => _root.TableNumber;
        set => _root.TableNumber = value;
    }

    public string CustomerName
    {
        get => _root.CustomerName;
        set => _root.CustomerName = value;
    }

    public string CustomerDocument
    {
        get => _root.CustomerDocument;
        set => _root.CustomerDocument = value;
    }

    public string OrderObservation
    {
        get => _root.OrderObservation;
        set => _root.OrderObservation = value;
    }

    public bool IsCustomerModalOpen => _root.IsCustomerModalOpen;
    public bool IsCheckoutModalOpen => _root.IsCheckoutModalOpen;
    public bool IsAddonSelectionModalOpen => _root.IsAddonSelectionModalOpen;

    public ICommand AddToCartCommand => _root.AddToCartCommand;
    public ICommand RemoveFromCartCommand => _root.RemoveFromCartCommand;
    public ICommand ClearCartCommand => _root.ClearCartCommand;
    public ICommand OpenCustomerModalCommand => _root.OpenCustomerModalCommand;
    public ICommand OpenCheckoutModalCommand => _root.OpenCheckoutModalCommand;
    public ICommand CloseModalsCommand => _root.CloseModalsCommand;

    public ICommand ConfirmPaymentCommand => _root.ConfirmPaymentCommand;

    private void RootOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.PropertyName))
            return;

        if (e.PropertyName is nameof(MainViewModel.SelectedCategory) or nameof(MainViewModel.CurrentCategoryTitle))
            OnPropertyChanged(nameof(CurrentCategoryTitle));

        OnPropertyChanged(e.PropertyName);
    }
}
