using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.Models;
using OpaMenu.Desktop.Models.DTOs;
using OpaMenu.Desktop.Models.DTOs.Requests;
using OpaMenu.Desktop.Models.Enums;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Desktop.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OpaMenu.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ICatalogService _catalogService;
    private readonly AppDbContext _dbContext;

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

    // --- Identificação do Cliente ---
    [ObservableProperty]
    private string _customerName = "Não Informado";

    [ObservableProperty]
    private string _customerDocument = string.Empty;

    [ObservableProperty]
    private string _tableNumber = "00";

    // --- Modais (Controle de Visibilidade) ---
    [ObservableProperty]
    private bool _isAnyModalOpen;

    [ObservableProperty]
    private bool _isCustomerModalOpen;

    [ObservableProperty]
    private bool _isCheckoutModalOpen;

    // --- Propriedades de Checkout ---
    [ObservableProperty]
    private decimal _amountPaid;

    [ObservableProperty]
    private decimal _changeAmount;

    [ObservableProperty]
    private string _paymentMethod = "Pix";

    [ObservableProperty]
    private string _orderObservation = string.Empty;

    public MainViewModel(ICatalogService catalogService, AppDbContext dbContext)
    {
        _catalogService = catalogService;
        _dbContext = dbContext;
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

        var existingItem = CartItems.FirstOrDefault(c => c.ProductId == product.Id);
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

    // --- Comandos do Modal de Identificação ---
    [RelayCommand]
    private void OpenCustomerModal()
    {
        IsCustomerModalOpen = true;
        IsAnyModalOpen = true;
    }

    // --- Comandos de Checkout ---
    [RelayCommand]
    private void OpenCheckoutModal()
    {
        if (CartItems.Count == 0)
        {
            MessageBox.Show("O carrinho está vazio!");
            return;
        }
        AmountPaid = CartTotal;
        CalculateChange();
        IsCheckoutModalOpen = true;
        IsAnyModalOpen = true;
    }

    [RelayCommand]
    private void CalculateChange()
    {
        ChangeAmount = AmountPaid >= CartTotal ? AmountPaid - CartTotal : 0;
    }

    [RelayCommand]
    private async Task ConfirmPaymentAsync()
    {
        try
        {
            // 1. Preparar o DTO para enviar pra API depois
            var requestDto = new CreateOrderRequestDto
            {
                CustomerName = CustomerName == "Não Informado" ? "Cliente Balcão" : CustomerName,
                CustomerPhone = "0000000000", // Preenchimento obrigatório para a API não dar erro 400
                IsDelivery = false,
                OrderType = EOrderType.Counter, // Venda no balcão
                TableId = TableNumber == "00" ? null : TableNumber,
                Notes = string.IsNullOrWhiteSpace(OrderObservation) ? null : OrderObservation,
                PaymentMethod = PaymentMethod switch
                {
                    "Dinheiro" => EPaymentMethod.Cash,
                    "Cartão de Crédito" => EPaymentMethod.CreditCard,
                    "Cartão de Débito" => EPaymentMethod.DebitCard,
                    "Pix" => EPaymentMethod.Pix,
                    _ => EPaymentMethod.Cash
                },
                Items = CartItems.Select(c => new CreateOrderItemRequestDto
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Notes = null // Observação individual já foi removida
                }).ToList()
            };

            var options = new System.Text.Json.JsonSerializerOptions
            {
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };
            var payloadJson = System.Text.Json.JsonSerializer.Serialize(requestDto, options);

            // 2. Criar o Pedido Local
            var orderId = Guid.NewGuid();
            var localOrder = new LocalOrder
            {
                Id = orderId,
                LocalId = Guid.NewGuid(),
                TenantId = Guid.Empty, // TODO: Pegar do token ou login logado
                OrderType = "Balcao",
                CustomerName = requestDto.CustomerName,
                CustomerDocument = string.IsNullOrWhiteSpace(CustomerDocument) ? null : CustomerDocument,
                TableNumber = requestDto.TableId,
                OrderObservation = requestDto.Notes,
                TotalAmount = CartTotal,
                AmountPaid = AmountPaid,
                ChangeAmount = ChangeAmount,
                PaymentMethod = PaymentMethod,
                SyncStatus = Models.Enums.ESyncStatus.PendingSync,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PayloadJson = payloadJson // Payload montado corretamente!
            };

            _dbContext.LocalOrders.Add(localOrder);

            // 3. Criar os Itens do Pedido no SQLite
            var localItems = CartItems.Select(c => new LocalOrderItem
            {
                Id = Guid.NewGuid(),
                LocalOrderId = orderId,
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Quantity = c.Quantity,
                UnitPrice = c.UnitPrice,
                TotalPrice = c.TotalPrice
            }).ToList();

            _dbContext.LocalOrderItems.AddRange(localItems);

            // 4. Salvar no SQLite
            await _dbContext.SaveChangesAsync();

            MessageBox.Show($"Venda finalizada e salva offline com sucesso!\n\nCliente: {CustomerName}\nTotal: {CartTotal:C}\nPagamento: {PaymentMethod}\nTroco: {ChangeAmount:C}\nObs: {OrderObservation}", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            
            ClearCart();
            CustomerName = "Não Informado";
            CustomerDocument = string.Empty;
            TableNumber = "00";
            OrderObservation = string.Empty;
            CloseModals();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao salvar a venda: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void CloseModals()
    {
        IsCustomerModalOpen = false;
        IsCheckoutModalOpen = false;
        IsAnyModalOpen = false;
    }
}