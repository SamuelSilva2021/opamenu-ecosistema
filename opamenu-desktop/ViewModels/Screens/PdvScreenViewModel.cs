using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.Models.Data;
using OpaMenu.Desktop.Models.DTOs;
using OpaMenu.Desktop.Models.DTOs.Aditional;
using OpaMenu.Desktop.Models.DTOs.Product;
using OpaMenu.Desktop.Models.DTOs.Requests;
using OpaMenu.Desktop.Models.Entities;
using OpaMenu.Desktop.Models.Enums;
using OpaMenu.Desktop.Services.Interfaces;
using OpaMenu.Desktop.ViewModels.Components;
using OpaMenu.Desktop.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace OpaMenu.Desktop.ViewModels.Screens;

public sealed partial class PdvScreenViewModel : ObservableObject
{
    private readonly ICatalogService _catalogService;
    private readonly AppDbContext _dbContext;
    private readonly IAuthService _authService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Func<bool> _isCashShiftOpen;
    private readonly IDialogService _dialogService;

    private List<ProductDto> _allProducts = new();

    public PdvScreenViewModel(
        ICatalogService catalogService,
        AppDbContext dbContext,
        IAuthService authService,
        IHttpClientFactory httpClientFactory,
        Func<bool> isCashShiftOpen,
        IDialogService dialogService)
    {
        _catalogService = catalogService;
        _dbContext = dbContext;
        _authService = authService;
        _httpClientFactory = httpClientFactory;
        _isCashShiftOpen = isCashShiftOpen;
        _dialogService = dialogService;

        Payments.CollectionChanged += Payments_CollectionChanged;
        CustomerName = "Não Informado";
        TableNumber = "00";
    }

    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<CategoryDto> Categories { get; } = new();
    public ObservableCollection<ProductDto> Products { get; } = new();
    public ObservableCollection<CartItemDto> CartItems { get; } = new();

    [ObservableProperty]
    private CategoryDto? _selectedCategory;

    public string CurrentCategoryTitle => SelectedCategory?.Name ?? "Todos";

    [ObservableProperty]
    private decimal _cartTotal;

    [ObservableProperty]
    private string _tableNumber = "00";

    [ObservableProperty]
    private string _customerName = "Não Informado";

    [ObservableProperty]
    private string _customerDocument = string.Empty;

    [ObservableProperty]
    private string _orderObservation = string.Empty;

    [ObservableProperty]
    private bool _isCustomerModalOpen;

    [ObservableProperty]
    private bool _isCheckoutModalOpen;

    [ObservableProperty]
    private bool _isAddonSelectionModalOpen;

    [ObservableProperty]
    private ProductDto? _productToConfigure;

    public ObservableCollection<SelectableAditionalGroup> AddonGroups { get; } = new();

    [ObservableProperty]
    private string _itemNotes = string.Empty;

    public ObservableCollection<PaymentEntry> Payments { get; } = new();

    [ObservableProperty]
    private decimal _totalPaid;

    [ObservableProperty]
    private decimal _remainingAmount;

    [ObservableProperty]
    private decimal _changeAmount;

    public IReadOnlyList<string> PaymentMethodOptions { get; } = new[] { "Pix", "Cartão", "Dinheiro" };

    partial void OnSelectedCategoryChanged(CategoryDto? value)
    {
        ApplyCategoryFilter(value);
        OnPropertyChanged(nameof(CurrentCategoryTitle));
    }

    partial void OnProductToConfigureChanged(ProductDto? value)
    {
        AddonGroups.Clear();
        if (value?.AditionalGroups == null)
            return;

        var groups = value.AditionalGroups
            .Where(g => g.AditionalGroup?.IsActive == true)
            .OrderBy(g => g.DisplayOrder)
            .ToList();

        foreach (var group in groups)
        {
            var groupDto = group.AditionalGroup;
            if (groupDto == null)
                continue;

            var effectiveIsRequired = group.IsRequired;
            var effectiveMin = group.MinSelectionsOverride ?? groupDto.MinSelections;
            var effectiveMax = group.MaxSelectionsOverride ?? groupDto.MaxSelections;

            var groupVm = new SelectableAditionalGroup(groupDto, effectiveIsRequired, effectiveMin, effectiveMax);
            foreach (var addon in groupDto.Aditionals.Where(a => a.IsActive).OrderBy(a => a.DisplayOrder))
            {
                groupVm.Options.Add(new SelectableAditional(addon, groupVm));
            }
            AddonGroups.Add(groupVm);
        }
    }

    public async Task LoadStorefrontAsync()
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
                SelectedCategory = Categories.FirstOrDefault();
                if (SelectedCategory == null)
                {
                    ApplyCategoryFilter(null);
                }
            }
            else
            {
                await _dialogService.ShowMessageAsync("PDV", "Não foi possível carregar os dados. Verifique a conexão com a API e o login.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Erro", $"Erro ao carregar dados: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyCategoryFilter(CategoryDto? category)
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
    private async Task AddToCartFromModalAsync()
    {
        if (ProductToConfigure == null) return;

        foreach (var group in AddonGroups)
        {
            if (!group.IsSelectionValid())
            {
                var rangeText = group.GetSelectionRangeText();
                await _dialogService.ShowMessageAsync("Adicionais", $"Seleção inválida em \"{group.Name}\". {rangeText}");
                return;
            }
        }

        var selectedAddons = AddonGroups
            .SelectMany(g => g.Options)
            .Where(a => a.IsSelected)
            .Select(a => new AditionalSelectionDto
            {
                AditionalId = a.Addon.Id,
                Name = a.Addon.Name,
                Quantity = 1,
                Price = a.Addon.Price
            })
            .ToList();

        AddToCartConfirmed(ProductToConfigure, selectedAddons, ItemNotes);
    }

    [RelayCommand]
    private async Task AddToCartAsync(ProductDto product)
    {
        if (product == null) return;

        var hasAddons = product.AditionalGroups != null &&
                        product.AditionalGroups.Any(g =>
                            g.AditionalGroup?.IsActive == true &&
                            g.AditionalGroup.Aditionals != null &&
                            g.AditionalGroup.Aditionals.Any(a => a.IsActive));

        if (hasAddons)
        {
            ProductToConfigure = product;
            ItemNotes = string.Empty;
            await _dialogService.ShowAsync(new PdvAddonsDialogViewModel(this));
            return;
        }

        AddToCartConfirmed(product);
    }

    private void AddToCartConfirmed(ProductDto product, List<AditionalSelectionDto>? selectedAddons = null, string? notes = null)
    {
        var normalizedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        var normalizedAddons = NormalizeSelectedAddons(selectedAddons);

        var existingItem = CartItems.FirstOrDefault(c =>
            c.ProductId == product.Id &&
            string.Equals(c.Notes, normalizedNotes, StringComparison.Ordinal) &&
            AreSameSelections(c.SelectedAditionals, normalizedAddons));

        if (existingItem != null)
        {
            existingItem.Quantity++;
            existingItem.TotalPrice = existingItem.Quantity * GetUnitTotal(existingItem.UnitPrice, existingItem.SelectedAditionals);
        }
        else
        {
            var unitTotal = GetUnitTotal(product.Price, normalizedAddons);

            var item = new CartItemDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = 1,
                UnitPrice = product.Price,
                TotalPrice = unitTotal,
                SelectedAditionals = normalizedAddons,
                Notes = normalizedNotes
            };
            CartItems.Add(item);
        }

        UpdateTotal();
        CloseModals();
    }

    private static List<AditionalSelectionDto> NormalizeSelectedAddons(List<AditionalSelectionDto>? selectedAddons)
    {
        if (selectedAddons == null || selectedAddons.Count == 0)
            return new();

        return selectedAddons
            .Where(a => a.Quantity > 0)
            .GroupBy(a => a.AditionalId)
            .Select(g =>
            {
                var first = g.First();
                return new AditionalSelectionDto
                {
                    AditionalId = first.AditionalId,
                    Name = first.Name,
                    Price = first.Price,
                    Quantity = g.Sum(x => x.Quantity)
                };
            })
            .OrderBy(a => a.AditionalId)
            .ToList();
    }

    private static bool AreSameSelections(List<AditionalSelectionDto> left, List<AditionalSelectionDto> right)
    {
        if (left.Count != right.Count)
            return false;

        var leftOrdered = left.OrderBy(a => a.AditionalId).ThenBy(a => a.Quantity).ToList();
        var rightOrdered = right.OrderBy(a => a.AditionalId).ThenBy(a => a.Quantity).ToList();

        for (var i = 0; i < leftOrdered.Count; i++)
        {
            if (leftOrdered[i].AditionalId != rightOrdered[i].AditionalId)
                return false;
            if (leftOrdered[i].Quantity != rightOrdered[i].Quantity)
                return false;
        }

        return true;
    }

    private static decimal GetUnitTotal(decimal unitPrice, List<AditionalSelectionDto> addons)
    {
        var addonsTotal = addons.Sum(a => a.Price * a.Quantity);
        return unitPrice + addonsTotal;
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

    [RelayCommand]
    private void OpenCustomerModal()
    {
        _ = _dialogService.ShowAsync(new PdvCustomerDialogViewModel(this));
    }

    [RelayCommand]
    private async Task OpenCheckoutModalAsync()
    {
        if (CartItems.Count == 0)
        {
            await _dialogService.ShowMessageAsync("PDV", "O carrinho está vazio!");
            return;
        }
        ResetPaymentsForCheckout();
        await _dialogService.ShowAsync(new PdvCheckoutDialogViewModel(this));
    }

    [RelayCommand]
    private void AddPayment()
    {
        Payments.Add(new PaymentEntry
        {
            Method = "Dinheiro",
            Amount = 0m
        });
        RecalculatePaymentSummary();
    }

    [RelayCommand]
    private void RemovePayment(PaymentEntry entry)
    {
        if (Payments.Count <= 1)
            return;

        Payments.Remove(entry);
        RecalculatePaymentSummary();
    }

    private void ResetPaymentsForCheckout()
    {
        foreach (var payment in Payments.ToList())
        {
            payment.PropertyChanged -= Payment_PropertyChanged;
        }

        Payments.Clear();
        Payments.Add(new PaymentEntry
        {
            Method = "Pix",
            Amount = CartTotal
        });

        RecalculatePaymentSummary();
    }

    private void Payments_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (PaymentEntry item in e.OldItems)
            {
                item.PropertyChanged -= Payment_PropertyChanged;
            }
        }

        if (e.NewItems != null)
        {
            foreach (PaymentEntry item in e.NewItems)
            {
                item.PropertyChanged += Payment_PropertyChanged;
            }
        }

        RecalculatePaymentSummary();
    }

    private void Payment_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(PaymentEntry.Amount) or nameof(PaymentEntry.Method))
        {
            RecalculatePaymentSummary();
        }
    }

    private void RecalculatePaymentSummary()
    {
        TotalPaid = Payments.Sum(p => p.Amount);

        var change = TotalPaid - CartTotal;
        var hasCash = Payments.Any(p => string.Equals(p.Method, "Dinheiro", StringComparison.Ordinal));

        if (change > 0m && !hasCash)
        {
            ChangeAmount = 0m;
            RemainingAmount = 0m;
            return;
        }

        ChangeAmount = change > 0m ? change : 0m;
        RemainingAmount = CartTotal - TotalPaid;
        if (RemainingAmount < 0m)
            RemainingAmount = 0m;
    }

    [RelayCommand]
    internal async Task ConfirmPaymentAsync()
    {
        try
        {
            if (!_isCashShiftOpen())
            {
                await _dialogService.ShowMessageAsync("Caixa", "Abra o caixa antes de realizar vendas.");
                return;
            }

            var validationError = ValidatePayments();
            if (!string.IsNullOrWhiteSpace(validationError))
            {
                await _dialogService.ShowMessageAsync("Pagamento", validationError);
                return;
            }

            var paymentBreakdownText = BuildPaymentBreakdownText();
            var notes = string.IsNullOrWhiteSpace(OrderObservation) ? null : OrderObservation.Trim();
            if (!string.IsNullOrWhiteSpace(paymentBreakdownText))
            {
                notes = string.IsNullOrWhiteSpace(notes) ? paymentBreakdownText : $"{notes} | {paymentBreakdownText}";
            }

            var paymentMethod = ResolvePaymentMethodForApi();

            var requestDto = new CreateOrderRequestDto
            {
                CustomerName = CustomerName == "Não Informado" ? "Cliente Balcão" : CustomerName,
                CustomerPhone = "11999999999",
                IsDelivery = false,
                OrderType = EOrderType.Counter,
                TableId = TableNumber == "00" ? null : TableNumber,
                Notes = notes,
                PaymentMethod = paymentMethod,
                Items = CartItems.Select(c => new CreateOrderItemRequestDto
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Notes = c.Notes,
                    Aditionals = c.SelectedAditionals.Select(a => new CreateOrderItemAditionalRequestDto
                    {
                        AditionalId = a.AditionalId,
                        Quantity = a.Quantity
                    }).ToList()
                }).ToList()
            };

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            var payloadJson = JsonSerializer.Serialize(requestDto, options);

            var orderId = Guid.NewGuid();
            var localOrder = new LocalOrderEntity
            {
                Id = orderId,
                LocalId = Guid.NewGuid(),
                TenantId = Guid.Empty,
                OrderType = "Balcao",
                CustomerName = requestDto.CustomerName,
                CustomerDocument = string.IsNullOrWhiteSpace(CustomerDocument) ? null : CustomerDocument,
                TableNumber = requestDto.TableId,
                OrderObservation = requestDto.Notes,
                TotalAmount = CartTotal,
                AmountPaid = TotalPaid,
                ChangeAmount = ChangeAmount,
                PaymentMethod = Payments.Count == 1 ? Payments[0].Method : "Dividido",
                PaymentBreakdownJson = BuildPaymentBreakdownJson(),
                SyncStatus = ESyncStatus.PendingSync,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PayloadJson = payloadJson
            };

            _dbContext.LocalOrders.Add(localOrder);

            var localItems = CartItems.Select(c => new LocalOrderItemEntity
            {
                Id = Guid.NewGuid(),
                LocalOrderId = orderId,
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Quantity = c.Quantity,
                UnitPrice = c.UnitPrice,
                TotalPrice = c.TotalPrice,
                Notes = c.Notes,
                AditionalsJson = SerializeItemAditionals(c.SelectedAditionals)
            }).ToList();

            _dbContext.LocalOrderItems.AddRange(localItems);

            await _dbContext.SaveChangesAsync();

            await TrySyncLocalOrderImmediatelyAsync(localOrder);

            ClearCart();
            CustomerName = "Não Informado";
            CustomerDocument = string.Empty;
            TableNumber = "00";
            OrderObservation = string.Empty;
            CloseModals();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Erro", $"Erro ao salvar a venda: {ex.Message}");
        }
    }

    private sealed record SyncAttemptResult(bool Succeeded, string? ErrorMessage);

    private async Task<SyncAttemptResult> TrySyncLocalOrderImmediatelyAsync(LocalOrderEntity localOrder)
    {
        var token = _authService.GetCurrentToken();
        if (string.IsNullOrWhiteSpace(token))
            return new SyncAttemptResult(false, "Usuário não autenticado.");

        if (string.IsNullOrWhiteSpace(localOrder.PayloadJson))
            return new SyncAttemptResult(false, "PayloadJson vazio.");

        try
        {
            localOrder.LastSyncAttempt = DateTime.UtcNow;

            var payloadJson = NormalizeOrderPayloadJson(localOrder.PayloadJson);
            if (!string.Equals(payloadJson, localOrder.PayloadJson, StringComparison.Ordinal))
                localOrder.PayloadJson = payloadJson;

            var httpClient = _httpClientFactory.CreateClient("CoreApi");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PostAsync("/api/orders", new StringContent(payloadJson, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                localOrder.SyncStatus = ESyncStatus.Synced;
                localOrder.SyncErrorMessage = null;
                await _dbContext.SaveChangesAsync();
                return new SyncAttemptResult(true, null);
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            localOrder.SyncStatus = ESyncStatus.Error;
            localOrder.SyncErrorMessage = $"HTTP {(int)response.StatusCode} - {errorResponse}";
            await _dbContext.SaveChangesAsync();
            return new SyncAttemptResult(false, localOrder.SyncErrorMessage);
        }
        catch (Exception ex)
        {
            localOrder.SyncStatus = ESyncStatus.Error;
            localOrder.SyncErrorMessage = ex.Message;
            await _dbContext.SaveChangesAsync();
            return new SyncAttemptResult(false, localOrder.SyncErrorMessage);
        }
    }

    private static string SerializeItemAditionals(List<AditionalSelectionDto> aditionals)
    {
        if (aditionals == null || aditionals.Count == 0)
            return "[]";

        return JsonSerializer.Serialize(aditionals);
    }

    private string? ValidatePayments()
    {
        if (Payments.Count == 0)
            return "Adicione ao menos um pagamento.";

        if (CartTotal <= 0m)
            return "Total inválido.";

        if (Payments.Any(p => p.Amount <= 0m))
            return "Todos os pagamentos precisam ter valor maior que zero.";

        var total = Payments.Sum(p => p.Amount);
        if (total < CartTotal - 0.01m)
            return "Valor pago insuficiente.";

        var hasCash = Payments.Any(p => string.Equals(p.Method, "Dinheiro", StringComparison.Ordinal));
        if (total > CartTotal + 0.01m && !hasCash)
            return "Troco só é permitido quando houver pagamento em dinheiro.";

        return null;
    }

    private string BuildPaymentBreakdownText()
    {
        var items = BuildPaymentBreakdownItems();
        if (items.Count == 0)
            return string.Empty;

        return string.Join(", ", items.Select(i => $"{i.Method}:{i.Amount:C}"));
    }

    private string BuildPaymentBreakdownJson()
    {
        var items = BuildPaymentBreakdownItems();
        return JsonSerializer.Serialize(items);
    }

    private List<PaymentBreakdownItem> BuildPaymentBreakdownItems()
    {
        return Payments
            .Where(p => p.Amount > 0m)
            .GroupBy(p => p.Method)
            .Select(g => new PaymentBreakdownItem(g.Key, g.Sum(x => x.Amount)))
            .ToList();
    }

    private EPaymentMethod ResolvePaymentMethodForApi()
    {
        if (Payments.Count == 1)
        {
            return Payments[0].Method switch
            {
                "Pix" => EPaymentMethod.Pix,
                "Dinheiro" => EPaymentMethod.Cash,
                _ => EPaymentMethod.CreditCard
            };
        }

        if (Payments.Any(p => string.Equals(p.Method, "Pix", StringComparison.Ordinal)))
            return EPaymentMethod.Pix;

        if (Payments.Any(p => string.Equals(p.Method, "Dinheiro", StringComparison.Ordinal)))
            return EPaymentMethod.Cash;

        return EPaymentMethod.CreditCard;
    }

    private static string NormalizeOrderPayloadJson(string payloadJson)
    {
        var doc = JsonDocument.Parse(payloadJson);
        return JsonSerializer.Serialize(doc, new JsonSerializerOptions
        {
            WriteIndented = false,
            Converters = { new JsonStringEnumConverter() }
        });
    }

    [RelayCommand]
    private void CloseModals()
    {
        IsCustomerModalOpen = false;
        IsCheckoutModalOpen = false;
        IsAddonSelectionModalOpen = false;
        ProductToConfigure = null;
        ItemNotes = string.Empty;
    }

    public void ClearAddonSelection()
    {
        ProductToConfigure = null;
        ItemNotes = string.Empty;
        AddonGroups.Clear();
    }

    public void ResetCheckout()
    {
        Payments.Clear();
        TotalPaid = 0m;
        RemainingAmount = 0m;
        ChangeAmount = 0m;
    }
}
