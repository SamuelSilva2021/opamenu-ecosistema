using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpaMenu.Desktop.Models.DTOs.Tables;
using OpaMenu.Desktop.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.ViewModels.Screens;

public sealed partial class TablesScreenViewModel : ObservableObject
{
    private readonly ITablesService _tablesService;
    private readonly Func<Guid, Task> _openTableDetailsAsync;

    public TablesScreenViewModel(ITablesService tablesService, Func<Guid, Task> openTableDetailsAsync)
    {
        _tablesService = tablesService;
        _openTableDetailsAsync = openTableDetailsAsync;
    }

    public ObservableCollection<TableListItem> Tables { get; } = new();

    [ObservableProperty]
    private bool _isLoading;

    public async Task LoadAsync()
    {
        if (IsLoading)
            return;

        IsLoading = true;

        try
        {
            Tables.Clear();
            var tables = await _tablesService.GetTablesFullAsync();

            foreach (var table in tables.OrderBy(t => t.Name))
            {
                var status = GetTableStatus(table);
                Tables.Add(new TableListItem(table.Id, table.Name, status));
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadTablesAsync()
    {
        await LoadAsync();
    }

    [RelayCommand]
    private async Task OpenTableDetailsAsync(TableListItem table)
    {
        if (table == null)
            return;

        await _openTableDetailsAsync(table.Id);
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
}
