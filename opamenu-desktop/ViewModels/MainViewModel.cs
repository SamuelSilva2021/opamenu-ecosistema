using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.Models.DTOs;
using OpaMenu.Desktop.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OpaMenu.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ICatalogService _catalogService;

    [ObservableProperty]
    private string _title = "Opamenu - Frente de Caixa (PDV)";

    [ObservableProperty]
    private string _operatorName = "Caixa 01 - João";

    // Coleções reais vindas da API
    public ObservableCollection<CategoryDto> Categories { get; } = new();
    public ObservableCollection<ProductDto> Products { get; } = new();
    
    // Todos os produtos salvos em memória para filtrar rapidamente ao clicar nas categorias
    private List<ProductDto> _allProducts = new();

    public ObservableCollection<CartItemDto> CartItems { get; } = new();

    [ObservableProperty]
    private decimal _cartTotal;

    [ObservableProperty]
    private bool _isLoading;

    public MainViewModel(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    /// <summary>
    /// Chamado pela View quando ela termina de carregar.
    /// Busca os dados reais da opamenu-api através das rotas protegidas.
    /// </summary>
    [RelayCommand]
    private async Task LoadStorefrontAsync()
    {
        if (IsLoading) return;
        IsLoading = true;

        try
        {
            var categoriesTask = _catalogService.GetCategoriesAsync();
            var productsTask = _catalogService.GetProductsAsync();

            await Task.WhenAll(categoriesTask, productsTask);

            var categories = await categoriesTask;
            var products = await productsTask;

            if (categories != null && products != null)
            {
                Categories.Clear();
                foreach (var cat in categories.OrderBy(c => c.DisplayOrder))
                {
                    Categories.Add(cat);
                }

                _allProducts = products.ToList();
                FilterProductsByCategory(null); // Mostra todos inicialmente
            }
            else
            {
                MessageBox.Show("Não foi possível carregar os dados. Verifique a conexão com a API e o login.");
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"Erro ao carregar dados: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void FilterProductsByCategory(CategoryDto? category)
    {
        Products.Clear();
        
        var filtered = category == null 
            ? _allProducts 
            : _allProducts.Where(p => p.CategoryId == category.Id);

        foreach (var p in filtered)
        {
            Products.Add(p);
        }
    }

    [RelayCommand]
    private void AddToCart(ProductDto product)
    {
        if (product == null) return;

        var existingItem = CartItems.FirstOrDefault(c => c.ProductName == product.Name);
        if (existingItem != null)
        {
            existingItem.Quantity++;
            existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;
        }
        else
        {
            CartItems.Add(new CartItemDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = 1,
                UnitPrice = product.Price,
                TotalPrice = product.Price
            });
        }

        UpdateTotal();
    }

    [RelayCommand]
    private void RemoveFromCart(CartItemDto item)
    {
        if (item == null) return;
        CartItems.Remove(item);
        UpdateTotal();
    }

    [RelayCommand]
    private void ClearCart()
    {
        CartItems.Clear();
        UpdateTotal();
    }

    private void UpdateTotal()
    {
        CartTotal = CartItems.Sum(i => i.TotalPrice);
    }
}