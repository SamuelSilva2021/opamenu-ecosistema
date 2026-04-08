using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.Models.DTOs;
using OpaMenu.Desktop.Models.DTOs.Requests;
using OpaMenu.Desktop.Models.Enums;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Desktop.Services;
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
using OpaMenu.Desktop.Services.Interfaces;
using OpaMenu.Desktop.Models.DTOs.Aditional;
using OpaMenu.Desktop.Models.DTOs.Product;
using OpaMenu.Desktop.Models.DTOs.Pdv;
using OpaMenu.Desktop.Models.Data;
using OpaMenu.Desktop.Models.Entities;
using OpaMenu.Desktop.Models.DTOs.Tables;

namespace OpaMenu.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public enum MainSection
    {
        Pdv = 0,
        Mesas = 1,
        MesaDetalhe = 2
    }

    private readonly ICatalogService _catalogService;
    private readonly AppDbContext _dbContext;
    private readonly ICashRegisterService _cashRegisterService;
    private readonly IAuthService _authService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UserStore _userStore;
    private readonly ITablesService _tablesService;

    [ObservableProperty]
    private string _title = "Opamenu - Frente de Caixa (PDV)";

    [ObservableProperty]
    private string _operatorName = "Operador";

    [ObservableProperty]
    private MainSection _currentSection = MainSection.Pdv;

    public bool IsPdvSection => CurrentSection == MainSection.Pdv;
    public bool IsTablesSection => CurrentSection is MainSection.Mesas or MainSection.MesaDetalhe;
    public bool IsTablesListSection => CurrentSection == MainSection.Mesas;
    public bool IsTableDetailsSection => CurrentSection == MainSection.MesaDetalhe;

    [ObservableProperty]
    private bool _isCashModalOpen;

    [ObservableProperty]
    private bool _isCashShiftOpen;

    [ObservableProperty]
    private bool _isClosingCashShift;

    [ObservableProperty]
    private bool _isCashMovementModal;

    [ObservableProperty]
    private ECashMovementType _cashMovementType;

    [ObservableProperty]
    private decimal _cashMovementAmount;

    [ObservableProperty]
    private string _cashMovementDescription = string.Empty;

    [ObservableProperty]
    private decimal _openingCashBalance;

    [ObservableProperty]
    private decimal _closingCashBalance;

    [ObservableProperty]
    private decimal _expectedCashBalance;

    [ObservableProperty]
    private string _closingDiscrepancyJustification = string.Empty;

    [ObservableProperty]
    private bool _hasClosingDiscrepancy;

    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<PaymentMethodSummaryDto> _closingSalesByPaymentMethod = new();

    [ObservableProperty]
    private decimal _closingTotalSales;

    [ObservableProperty]
    private decimal _closingTotalInflows;

    [ObservableProperty]
    private decimal _closingTotalOutflows;

    [ObservableProperty]
    private decimal _closingDiscrepancy;

    [ObservableProperty]
    private System.DateTime? _cashOpenedAt;

    [ObservableProperty]
    private string _cashShiftStatusText = "Caixa: --";

    public bool IsCashShiftClosed => !IsCashShiftOpen;

    public bool IsOpeningCashShift => !IsClosingCashShift && !IsCashMovementModal;

    public string CashModalTitle =>
        IsCashMovementModal
            ? CashMovementType == ECashMovementType.Inbound ? "Suprimento" : "Sangria"
            : "Caixa";

    // Coleções reais vindas da API
    public ObservableCollection<CategoryDto> Categories { get; } = new();
    public ObservableCollection<ProductDto> Products { get; } = new();
    
    // Todos os produtos salvos em memória para filtrar rapidamente ao clicar nas categorias
    private List<ProductDto> _allProducts = new();

    public ObservableCollection<CartItemDto> CartItems { get; } = new();

    public sealed class TableListItem
    {
        public TableListItem(Guid id, string name, string status)
        {
            Id = id;
            Name = name;
            Status = status;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Status { get; }
        public string DisplayName => Name;
    }

    public ObservableCollection<TableListItem> Tables { get; } = new();

    [ObservableProperty]
    private TableFullDto? _selectedTableDetails;

    public sealed class TabSummaryItem
    {
        public TabSummaryItem(TabDto tab, decimal total, int ordersCount)
        {
            Tab = tab;
            Total = total;
            OrdersCount = ordersCount;
        }

        public TabDto Tab { get; }
        public Guid Id => Tab.Id;
        public string Name => string.IsNullOrWhiteSpace(Tab.Name) ? "Comanda" : Tab.Name!;
        public int Status => Tab.Status;
        public bool IsOpen => Status == 1;
        public string StatusText => IsOpen ? "Aberta" : "Fechada";
        public decimal Total { get; }
        public int OrdersCount { get; }
        public IReadOnlyList<OrderDto> Orders => (Tab.Orders ?? new List<OrderDto>()).AsReadOnly();
    }

    public ObservableCollection<TabSummaryItem> SelectedTableTabs { get; } = new();

    [ObservableProperty]
    private TabSummaryItem? _selectedTab;

    public decimal SelectedTabTotal => SelectedTab?.Total ?? 0m;
    public bool CanCheckoutSelectedTab => SelectedTab?.IsOpen == true;

    public sealed class PaymentMethodOption
    {
        public PaymentMethodOption(EPaymentMethod method, string name)
        {
            Method = method;
            Name = name;
        }

        public EPaymentMethod Method { get; }
        public string Name { get; }
    }

    public IReadOnlyList<PaymentMethodOption> TabPaymentMethodOptions { get; } = new[]
    {
        new PaymentMethodOption(EPaymentMethod.Pix, "Pix"),
        new PaymentMethodOption(EPaymentMethod.CreditCard, "Cartão de Crédito"),
        new PaymentMethodOption(EPaymentMethod.DebitCard, "Cartão de Débito"),
        new PaymentMethodOption(EPaymentMethod.Cash, "Dinheiro")
    };

    [ObservableProperty]
    private PaymentMethodOption? _selectedTabPaymentMethod;

    [ObservableProperty]
    private bool _isTabCheckoutModalOpen;

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

    [ObservableProperty]
    private bool _isAddonSelectionModalOpen;

    [ObservableProperty]
    private ProductDto? _productToConfigure;

    // --- Propriedades de Checkout ---
    [ObservableProperty]
    private decimal _totalPaid;

    [ObservableProperty]
    private decimal _changeAmount;

    [ObservableProperty]
    private decimal _remainingAmount;

    [ObservableProperty]
    private string _itemNotes = string.Empty;

    [ObservableProperty]
    private string _orderObservation = string.Empty;

    public ObservableCollection<SelectableAditionalGroup> AddonGroups { get; } = new();

    public ObservableCollection<PaymentEntry> Payments { get; } = new();

    public IReadOnlyList<string> PaymentMethodOptions { get; } = new[]
    {
        "Dinheiro",
        "Pix",
        "Cartão de Crédito",
        "Cartão de Débito"
    };

    public MainViewModel(
        ICatalogService catalogService,
        AppDbContext dbContext,
        ICashRegisterService cashRegisterService,
        IAuthService authService,
        IHttpClientFactory httpClientFactory,
        UserStore userStore,
        ITablesService tablesService)
    {
        _catalogService = catalogService;
        _dbContext = dbContext;
        _cashRegisterService = cashRegisterService;
        _authService = authService;
        _httpClientFactory = httpClientFactory;
        _userStore = userStore;
        _tablesService = tablesService;

        Payments.CollectionChanged += Payments_CollectionChanged;

        SelectedTabPaymentMethod = TabPaymentMethodOptions.FirstOrDefault();
    }

    partial void OnCurrentSectionChanged(MainSection value)
    {
        OnPropertyChanged(nameof(IsPdvSection));
        OnPropertyChanged(nameof(IsTablesSection));
        OnPropertyChanged(nameof(IsTablesListSection));
        OnPropertyChanged(nameof(IsTableDetailsSection));
    }

    [RelayCommand]
    private void NavigateToPdv()
    {
        CurrentSection = MainSection.Pdv;
    }

    [RelayCommand]
    private async Task NavigateToTablesAsync()
    {
        CurrentSection = MainSection.Mesas;
        await LoadTablesAsync();
    }

    [RelayCommand]
    private async Task OpenTableDetailsAsync(TableListItem table)
    {
        try
        {
            var details = await _tablesService.GetTableFullByIdAsync(table.Id);
            if (details == null)
            {
                MessageBox.Show("Não foi possível carregar os detalhes da mesa.", "Mesas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedTableDetails = details;
            BuildSelectedTableTabs(details);
            CurrentSection = MainSection.MesaDetalhe;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Mesas", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void BackToTables()
    {
        SelectedTableDetails = null;
        SelectedTableTabs.Clear();
        SelectedTab = null;
        CurrentSection = MainSection.Mesas;
    }

    private void BuildSelectedTableTabs(TableFullDto details)
    {
        SelectedTableTabs.Clear();

        foreach (var tab in details.Tabs.OrderByDescending(t => t.OpenedAt))
        {
            var orders = (tab.Orders ?? new List<OrderDto>())
                .Where(o => o.Status is not 5 and not 6)
                .ToList();

            var total = orders.Sum(o => o.Total);
            SelectedTableTabs.Add(new TabSummaryItem(tab, total, orders.Count));
        }

        SelectedTab = SelectedTableTabs.FirstOrDefault(t => t.IsOpen) ?? SelectedTableTabs.FirstOrDefault();
    }

    partial void OnSelectedTabChanged(TabSummaryItem? value)
    {
        OnPropertyChanged(nameof(SelectedTabTotal));
        OnPropertyChanged(nameof(CanCheckoutSelectedTab));
    }

    [RelayCommand]
    private void OpenTabCheckoutModal()
    {
        if (SelectedTableDetails == null || SelectedTab == null)
            return;

        if (!IsCashShiftOpen)
        {
            MessageBox.Show("Não é possível fechar comanda com o caixa fechado.", "Mesas", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!CanCheckoutSelectedTab)
        {
            MessageBox.Show("Selecione uma comanda aberta para fechar.", "Mesas", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsTabCheckoutModalOpen = true;
        IsAnyModalOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmTabCheckoutAsync()
    {
        try
        {
            if (SelectedTableDetails == null || SelectedTab == null || SelectedTabPaymentMethod == null)
                return;

            await _tablesService.CheckoutTabAsync(SelectedTableDetails.Id, SelectedTab.Id, SelectedTabPaymentMethod.Method);
            CloseModals();

            var details = await _tablesService.GetTableFullByIdAsync(SelectedTableDetails.Id);
            if (details != null)
            {
                SelectedTableDetails = details;
                BuildSelectedTableTabs(details);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Fechamento de Comanda", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task LoadTablesAsync()
    {
        Tables.Clear();

        var tables = await _tablesService.GetTablesFullAsync();

        foreach (var table in tables.OrderBy(t => t.Name))
        {
            var status = GetTableStatus(table);
            Tables.Add(new TableListItem(table.Id, table.Name, status));
        }
    }

    private static string GetTableStatus(TableFullDto table)
    {
        var openTabs = table.Tabs.Where(t => t.Status == 1).ToList();
        if (openTabs.Count == 0)
            return "Livre";

        var orders = openTabs
            .SelectMany(t => t.Orders ?? Enumerable.Empty<OrderDto>())
            .ToList();

        if (orders.Count == 0)
            return "Ocupada";

        var anyNonDelivered = orders.Any(o => o.Status is not 4 and not 5 and not 6);
        return anyNonDelivered ? "Ocupada" : "Aguardando conta";
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
            UpdateOperatorFromUserStore();

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

            await LoadCashShiftAsync();
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
    private void AddToCartFromModal()
    {
        if (ProductToConfigure == null) return;

        foreach (var group in AddonGroups)
        {
            if (!group.IsSelectionValid())
            {
                var rangeText = group.GetSelectionRangeText();
                MessageBox.Show($"Seleção inválida em \"{group.Name}\". {rangeText}", "Adicionais", MessageBoxButton.OK, MessageBoxImage.Warning);
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
    private void AddToCart(ProductDto product)
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
            IsAddonSelectionModalOpen = true;
            IsAnyModalOpen = true;
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
        ResetPaymentsForCheckout();
        IsCheckoutModalOpen = true;
        IsAnyModalOpen = true;
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
    private async Task ConfirmPaymentAsync()
    {
        try
        {
            if (!IsCashShiftOpen)
            {
                MessageBox.Show("Abra o caixa antes de realizar vendas.", "Caixa", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var validationError = ValidatePayments();
            if (!string.IsNullOrWhiteSpace(validationError))
            {
                MessageBox.Show(validationError, "Pagamento", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var paymentBreakdownText = BuildPaymentBreakdownText();
            var notes = string.IsNullOrWhiteSpace(OrderObservation) ? null : OrderObservation.Trim();
            if (!string.IsNullOrWhiteSpace(paymentBreakdownText))
            {
                notes = string.IsNullOrWhiteSpace(notes) ? paymentBreakdownText : $"{notes} | {paymentBreakdownText}";
            }

            var paymentMethod = ResolvePaymentMethodForApi();

            // 1. Preparar o DTO para enviar pra API depois
            var requestDto = new CreateOrderRequestDto
            {
                CustomerName = CustomerName == "Não Informado" ? "Cliente Balcão" : CustomerName,
                CustomerPhone = "11999999999",
                IsDelivery = false,
                OrderType = EOrderType.Counter, // Venda no balcão
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

            // 2. Criar o Pedido Local
            var orderId = Guid.NewGuid();
            var localOrder = new LocalOrderEntity
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
                AmountPaid = TotalPaid,
                ChangeAmount = ChangeAmount,
                PaymentMethod = Payments.Count == 1 ? Payments[0].Method : "Dividido",
                PaymentBreakdownJson = BuildPaymentBreakdownJson(),
                SyncStatus = Models.Enums.ESyncStatus.PendingSync,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PayloadJson = payloadJson // Payload montado corretamente!
            };

            _dbContext.LocalOrders.Add(localOrder);

            // 3. Criar os Itens do Pedido no SQLite
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

            // 4. Salvar no SQLite
            await _dbContext.SaveChangesAsync();

            // 5. Já tentar sincronizar imediatamente
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
            MessageBox.Show($"Erro ao salvar a venda: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                localOrder.SyncStatus = Models.Enums.ESyncStatus.Synced;
                localOrder.SyncErrorMessage = null;
                await _dbContext.SaveChangesAsync();
                return new SyncAttemptResult(true, null);
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            localOrder.SyncStatus = Models.Enums.ESyncStatus.Error;
            localOrder.SyncErrorMessage = $"HTTP {(int)response.StatusCode} - {errorResponse}";
            await _dbContext.SaveChangesAsync();
            return new SyncAttemptResult(false, localOrder.SyncErrorMessage);
        }
        catch (Exception ex)
        {
            localOrder.SyncStatus = Models.Enums.ESyncStatus.Error;
            localOrder.SyncErrorMessage = ex.Message;
            await _dbContext.SaveChangesAsync();
            return new SyncAttemptResult(false, ex.Message);
        }
    }

    private static string NormalizeOrderPayloadJson(string payloadJson)
    {
        try
        {
            var requestDto = JsonSerializer.Deserialize<CreateOrderRequestDto>(payloadJson, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                Converters = { new JsonStringEnumConverter() }
            });

            if (requestDto is null)
                return payloadJson;

            var changed = false;

            if (string.IsNullOrWhiteSpace(requestDto.CustomerName))
            {
                requestDto.CustomerName = "Cliente Balcão";
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(requestDto.CustomerPhone))
            {
                requestDto.CustomerPhone = "11999999999";
                changed = true;
            }

            if (requestDto.Items is null)
            {
                requestDto.Items = new();
                changed = true;
            }

            return changed
                ? JsonSerializer.Serialize(requestDto, new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    Converters = { new JsonStringEnumConverter() }
                })
                : payloadJson;
        }
        catch
        {
            return payloadJson;
        }
    }

    private string? ValidatePayments()
    {
        if (Payments.Count == 0)
            return "Informe pelo menos uma forma de pagamento.";

        foreach (var payment in Payments)
        {
            if (string.IsNullOrWhiteSpace(payment.Method))
                return "Selecione a forma de pagamento em todos os lançamentos.";

            if (payment.Amount <= 0m)
                return "Informe valores maiores que zero para todos os pagamentos.";
        }

        var total = Payments.Sum(p => p.Amount);
        if (total < CartTotal)
            return "O total pago não pode ser menor que o total do pedido.";

        var hasCash = Payments.Any(p => string.Equals(p.Method, "Dinheiro", StringComparison.Ordinal));
        if (total > CartTotal && !hasCash)
            return "Troco só é permitido quando houver pagamento em Dinheiro.";

        return null;
    }

    private string BuildPaymentBreakdownText()
    {
        if (Payments.Count == 0)
            return string.Empty;

        var parts = Payments
            .Where(p => !string.IsNullOrWhiteSpace(p.Method) && p.Amount > 0m)
            .Select(p => $"{p.Method} {p.Amount:C}")
            .ToList();

        return parts.Count == 0 ? string.Empty : string.Join(", ", parts);
    }

    private string? BuildPaymentBreakdownJson()
    {
        if (Payments.Count == 0)
            return null;

        var items = Payments
            .Where(p => !string.IsNullOrWhiteSpace(p.Method) && p.Amount > 0m)
            .Select(p => new PaymentBreakdownItem(p.Method.Trim(), p.Amount))
            .ToList();

        if (items.Count == 0)
            return null;

        return JsonSerializer.Serialize(items, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }

    private static string? SerializeItemAditionals(List<AditionalSelectionDto> aditionals)
    {
        if (aditionals.Count == 0)
            return null;

        return JsonSerializer.Serialize(
            aditionals.Select(a => new { a.AditionalId, a.Name, a.Quantity, a.Price }),
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }

    private EPaymentMethod? ResolvePaymentMethodForApi()
    {
        if (Payments.Count != 1)
            return null;

        return Payments[0].Method switch
        {
            "Dinheiro" => EPaymentMethod.Cash,
            "Cartão de Crédito" => EPaymentMethod.CreditCard,
            "Cartão de Débito" => EPaymentMethod.DebitCard,
            "Pix" => EPaymentMethod.Pix,
            _ => null
        };
    }

    [RelayCommand]
    private void CloseModals()
    {
        IsCustomerModalOpen = false;
        IsCheckoutModalOpen = false;
        IsTabCheckoutModalOpen = false;
        IsCashModalOpen = false;
        IsCashMovementModal = false;
        IsAddonSelectionModalOpen = false;
        IsAnyModalOpen = false;
        ProductToConfigure = null;
        ItemNotes = string.Empty;
        CashMovementAmount = 0m;
        CashMovementDescription = string.Empty;
    }

    partial void OnIsCashShiftOpenChanged(bool value)
    {
        OnPropertyChanged(nameof(IsCashShiftClosed));
    }

    partial void OnIsClosingCashShiftChanged(bool value)
    {
        OnPropertyChanged(nameof(IsOpeningCashShift));
    }

    partial void OnIsCashMovementModalChanged(bool value)
    {
        OnPropertyChanged(nameof(IsOpeningCashShift));
        OnPropertyChanged(nameof(CashModalTitle));
    }

    partial void OnCashMovementTypeChanged(ECashMovementType value)
    {
        OnPropertyChanged(nameof(CashModalTitle));
    }

    private void UpdateOperatorFromUserStore()
    {
        if (!string.IsNullOrWhiteSpace(_userStore.Name))
        {
            OperatorName = _userStore.Name;
            return;
        }

        if (!string.IsNullOrWhiteSpace(_userStore.Email))
        {
            OperatorName = _userStore.Email;
        }
    }

    [RelayCommand]
    private void OpenCashShiftModal()
    {
        IsCashMovementModal = false;
        IsClosingCashShift = false;
        OpeningCashBalance = 0m;
        IsCashModalOpen = true;
        IsAnyModalOpen = true;
    }

    [RelayCommand]
    private async Task OpenCloseCashShiftModal()
    {
        var hasPendingOrders = await _dbContext.LocalOrders.AnyAsync(o =>
            o.SyncStatus == Models.Enums.ESyncStatus.PendingSync || o.SyncStatus == Models.Enums.ESyncStatus.Error);

        if (hasPendingOrders)
        {
            MessageBox.Show("Existem vendas pendentes de sincronização. Sincronize antes de fechar o caixa.", "Caixa", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsCashMovementModal = false;
        IsClosingCashShift = true;
        ClosingCashBalance = 0m;
        ClosingDiscrepancyJustification = string.Empty;
        IsCashModalOpen = true;
        IsAnyModalOpen = true;
        await LoadActiveShiftSummaryIntoModalAsync();
    }

    [RelayCommand]
    private void OpenCashInboundMovementModal()
    {
        if (!IsCashShiftOpen)
        {
            MessageBox.Show("Não é possível registrar suprimento com o caixa fechado.", "Caixa", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsClosingCashShift = false;
        IsCashMovementModal = true;
        CashMovementType = ECashMovementType.Inbound;
        CashMovementAmount = 0m;
        CashMovementDescription = string.Empty;
        IsCashModalOpen = true;
        IsAnyModalOpen = true;
    }

    [RelayCommand]
    private void OpenCashOutboundMovementModal()
    {
        if (!IsCashShiftOpen)
        {
            MessageBox.Show("Não é possível registrar sangria com o caixa fechado.", "Caixa", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsClosingCashShift = false;
        IsCashMovementModal = true;
        CashMovementType = ECashMovementType.Outbound;
        CashMovementAmount = 0m;
        CashMovementDescription = string.Empty;
        IsCashModalOpen = true;
        IsAnyModalOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmCashShiftAsync()
    {
        try
        {
            if (IsCashMovementModal)
            {
                if (CashMovementAmount <= 0m)
                    throw new InvalidOperationException("Informe um valor maior que zero.");

                var description = CashMovementDescription?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(description))
                    throw new InvalidOperationException("Informe uma descrição para a movimentação.");

                await _cashRegisterService.AddMovementAsync(CashMovementType, CashMovementAmount, description);
                var shift = await _cashRegisterService.GetActiveShiftAsync();
                ApplyCashShiftState(shift);
            }
            else if (IsClosingCashShift)
            {
                var diff = Math.Abs(ClosingCashBalance - ExpectedCashBalance);
                if (diff >= 0.01m && string.IsNullOrWhiteSpace(ClosingDiscrepancyJustification))
                    throw new System.InvalidOperationException("Informe uma justificativa para a diferença no fechamento.");

                var summary = await _cashRegisterService.CloseShiftWithSummaryAsync(ClosingCashBalance, ClosingDiscrepancyJustification?.Trim());
                //MessageBox.Show(BuildCloseSummaryText(summary), "Fechamento de Caixa", MessageBoxButton.OK, MessageBoxImage.Information);
                ApplyCashShiftState(summary.Shift);
            }
            else
            {
                var shift = await _cashRegisterService.OpenShiftAsync(OpeningCashBalance);
                ApplyCashShiftState(shift);
            }

            CloseModals();
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.Message, "Caixa", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task LoadCashShiftAsync()
    {
        try
        {
            var shift = await _cashRegisterService.GetActiveShiftAsync();
            ApplyCashShiftState(shift);
        }
        catch (System.Exception ex)
        {
            CashShiftStatusText = $"Caixa: erro ({ex.Message})";
            IsCashShiftOpen = false;
        }
    }

    private void ApplyCashShiftState(CashShiftDto? shift)
    {
        if (shift == null)
        {
            IsCashShiftOpen = false;
            ExpectedCashBalance = 0m;
            CashOpenedAt = null;
            CashShiftStatusText = "Caixa: fechado";
            return;
        }

        IsCashShiftOpen = shift.Status == ECashShiftStatus.Open;
        ExpectedCashBalance = shift.ExpectedBalance;
        CashOpenedAt = shift.OpenedAt;
        CashShiftStatusText = IsCashShiftOpen ? "Caixa: aberto" : "Caixa: fechado";
    }

    private static string BuildCloseSummaryText(CashShiftCloseSummaryDto summary)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Resumo do caixa:");
        sb.AppendLine();

        if (summary.SalesByPaymentMethod is { Count: > 0 })
        {
            sb.AppendLine("Vendas por forma de pagamento:");
            foreach (var p in summary.SalesByPaymentMethod.OrderBy(p => p.PaymentMethodName))
                sb.AppendLine($"- {p.PaymentMethodName}: {p.TotalAmount:C}");
            sb.AppendLine();
        }

        sb.AppendLine($"Total de vendas: {summary.TotalSales:C}");
        sb.AppendLine($"Entradas: {summary.TotalInflows:C}");
        sb.AppendLine($"Saídas: {summary.TotalOutflows:C}");
        sb.AppendLine();
        sb.AppendLine($"Saldo esperado (dinheiro): {summary.ExpectedCashBalance:C}");
        sb.AppendLine($"Saldo informado: {summary.ClosingBalance:C}");
        sb.AppendLine($"Diferença: {summary.Discrepancy:C}");

        if (!string.IsNullOrWhiteSpace(summary.DiscrepancyJustification))
        {
            sb.AppendLine();
            sb.AppendLine("Justificativa:");
            sb.AppendLine(summary.DiscrepancyJustification);
        }

        return sb.ToString();
    }

    partial void OnClosingCashBalanceChanged(decimal value)
    {
        HasClosingDiscrepancy = Math.Abs(value - ExpectedCashBalance) >= 0.01m;
        ClosingDiscrepancy = value - ExpectedCashBalance;
    }

    partial void OnExpectedCashBalanceChanged(decimal value)
    {
        HasClosingDiscrepancy = Math.Abs(ClosingCashBalance - value) >= 0.01m;
        ClosingDiscrepancy = ClosingCashBalance - value;
    }

    private async Task LoadActiveShiftSummaryIntoModalAsync()
    {
        try
        {
            var summary = await _cashRegisterService.GetActiveShiftSummaryAsync();
            ClosingSalesByPaymentMethod.Clear();
            ClosingTotalSales = 0m;
            ClosingTotalInflows = 0m;
            ClosingTotalOutflows = 0m;

            if (summary != null)
            {
                foreach (var s in summary.SalesByPaymentMethod)
                    ClosingSalesByPaymentMethod.Add(s);
                ClosingTotalSales = summary.TotalSales;
                ClosingTotalInflows = summary.TotalInflows;
                ClosingTotalOutflows = summary.TotalOutflows;
                ExpectedCashBalance = summary.Shift.ExpectedBalance;
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"Falha ao carregar resumo do caixa: {ex.Message}", "Caixa", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}

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

public partial class PaymentEntry : ObservableObject
{
    [ObservableProperty]
    private string _method = "Pix";

    [ObservableProperty]
    private decimal _amount;
}

public record PaymentBreakdownItem(string Method, decimal Amount);
