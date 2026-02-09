import 'package:esc_pos_utils_plus/esc_pos_utils.dart' as esc;
import 'package:intl/intl.dart';
import '../../features/pos/domain/models/order_response_dto.dart';
import '../services/printer_service.dart';

class ReceiptGenerator {
  static Future<List<int>> generateOrderReceipt({
    required OrderResponseDto order,
    required PaperSize paperSize,
    required esc.CapabilityProfile profile,
    String? restaurantName,
    String? restaurantAddress,
    String? restaurantPhone,
  }) async {
    final generator = esc.Generator(paperSize.posSize, profile);
    final currencyFormat = NumberFormat.currency(symbol: 'R\$', locale: 'pt_BR');
    final dateFormat = DateFormat('dd/MM/yyyy HH:mm:ss');
    List<int> bytes = [];

    // Header
    bytes += generator.text(restaurantName ?? 'OPAMENU',
        styles: const esc.PosStyles(align: esc.PosAlign.center, bold: true, height: esc.PosTextSize.size2, width: esc.PosTextSize.size2));
    
    if (restaurantAddress != null && restaurantAddress.isNotEmpty) {
      bytes += generator.text(restaurantAddress, styles: const esc.PosStyles(align: esc.PosAlign.center));
    }
    if (restaurantPhone != null && restaurantPhone.isNotEmpty) {
      bytes += generator.text('Tel: $restaurantPhone', styles: const esc.PosStyles(align: esc.PosAlign.center));
    }
    
    bytes += generator.text(order.isDelivery ? 'PEDIDO DELIVERY' : 'PEDIDO LOCAL',
        styles: const esc.PosStyles(align: esc.PosAlign.center, bold: true));
    bytes += generator.hr();

    // Order Info
    bytes += generator.text('Pedido: #${order.id.substring(0, 8)}', styles: const esc.PosStyles(bold: true));
    bytes += generator.text('Data: ${dateFormat.format(order.createdAt.toLocal())}');
    if (order.queuePosition > 0) {
      bytes += generator.text('Mesa/Senha: ${order.queuePosition}');
    }
    bytes += generator.hr();

    // Customer Info
    bytes += generator.text('Cliente: ${order.customerName}');
    bytes += generator.text('Tel: ${order.customerPhone}');
    if (order.isDelivery) {
      bytes += generator.text('End: ${order.deliveryAddress}');
    }
    bytes += generator.hr();

    // Items Header - Manual formatting since row() might be problematic in some v2 versions
    int col1 = (paperSize.width * 0.6).floor();
    int col2 = (paperSize.width * 0.15).floor();
    int col3 = paperSize.width - col1 - col2;

    String header = _pad('Item', col1) + _pad('Qtd', col2, alignRight: true) + _pad('Total', col3, alignRight: true);
    bytes += generator.text(header, styles: const esc.PosStyles(bold: true));
    bytes += generator.hr(ch: '-');

    // Items
    for (var item in order.items) {
      String line = _pad(item.productName, col1) + 
                    _pad(item.quantity.toString(), col2, alignRight: true) + 
                    _pad(currencyFormat.format(item.subtotal), col3, alignRight: true);
      bytes += generator.text(line);
      
      if (item.notes != null && item.notes!.isNotEmpty) {
        bytes += generator.text('  Obs: ${item.notes}', styles: const esc.PosStyles());
      }
    }
    bytes += generator.hr();

    // Totals
    bytes += generator.text(_padLeft('Subtotal: ', paperSize.width - 12) + _pad(currencyFormat.format(order.subtotal), 12, alignRight: true));
    
    if (order.deliveryFee > 0) {
      bytes += generator.text(_padLeft('Taxa Entrega: ', paperSize.width - 12) + _pad(currencyFormat.format(order.deliveryFee), 12, alignRight: true));
    }

    if (order.discountAmount > 0) {
      bytes += generator.text(_padLeft('Desconto: ', paperSize.width - 12) + _pad('- ' + currencyFormat.format(order.discountAmount), 12, alignRight: true));
    }

    bytes += generator.hr(ch: '=');
    bytes += generator.text('TOTAL: ' + currencyFormat.format(order.total), 
        styles: const esc.PosStyles(bold: true, align: esc.PosAlign.right, height: esc.PosTextSize.size2, width: esc.PosTextSize.size2));
    bytes += generator.hr(ch: '=');

    if (order.notes != null && order.notes!.isNotEmpty) {
      bytes += generator.text('OBSERVACOES:', styles: const esc.PosStyles(bold: true));
      bytes += generator.text(order.notes!);
      bytes += generator.hr();
    }

    // Footer
    bytes += generator.text('Obrigado pela preferencia!', styles: const esc.PosStyles(align: esc.PosAlign.center));
    bytes += generator.feed(3);
    bytes += generator.cut();

    return bytes;
  }

  static String _pad(String text, int width, {bool alignRight = false}) {
    if (text.length > width) {
      return text.substring(0, width);
    }
    if (alignRight) {
      return text.padLeft(width);
    }
    return text.padRight(width);
  }

  static String _padLeft(String text, int width) {
    return text.padLeft(width);
  }
}
