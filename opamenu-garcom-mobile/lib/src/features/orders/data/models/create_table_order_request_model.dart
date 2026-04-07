import 'create_order_item_model.dart';

class CreateTableOrderRequestModel {
  final String tableId;
  final String tabId;
  final List<CreateOrderItemModel> items;

  const CreateTableOrderRequestModel({
    required this.tableId,
    required this.tabId,
    required this.items,
  });

  Map<String, Object?> toJson() {
    return {
      'isDelivery': false,
      'orderType': 'Table',
      'tableId': tableId,
      'tabId': tabId,
      'items': items.map((e) => e.toJson()).toList(growable: false),
    };
  }
}

