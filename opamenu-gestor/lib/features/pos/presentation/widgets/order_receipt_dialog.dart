import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_colors.dart';
import '../../domain/models/order_response_dto.dart';
import '../../domain/models/order_item_response_dto.dart';
import '../../../../core/services/printer_service.dart';
import '../../../../core/utils/receipt_generator.dart';
import 'package:esc_pos_utils_plus/esc_pos_utils.dart' as esc;

class OrderReceiptDialog extends ConsumerWidget {
  final OrderResponseDto order;

  const OrderReceiptDialog({Key? key, required this.order}) : super(key: key);

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final currencyFormat = NumberFormat.currency(symbol: 'R\$', locale: 'pt_BR');
    final printerService = ref.read(printerServiceProvider.notifier);

    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      elevation: 0,
      backgroundColor: Colors.transparent,
      child: Container(
        width: 400,
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.1),
              blurRadius: 10,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // Header
            Padding(
              padding: const EdgeInsets.all(24.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Pedido #${order.id}',
                    style: const TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                      color: AppColors.textPrimary,
                    ),
                  ),
                  const SizedBox(height: 24),
                  _buildInfoRow('Cliente', order.customerName),
                  // Guest count not available in DTO, omitting or could use placeholder
                  // _buildInfoRow('Mesa', '4'), 
                  const SizedBox(height: 8),
                  // Payment method not directly in DTO, assuming standard or omitting
                  _buildInfoRow('Tipo', order.isDelivery ? 'Entrega' : 'Retirada'),
                ],
              ),
            ),
            
            const Divider(height: 1),

            // Items List
            ConstrainedBox(
              constraints: const BoxConstraints(maxHeight: 300),
              child: ListView.builder(
                shrinkWrap: true,
                padding: const EdgeInsets.all(24),
                itemCount: order.items.length,
                itemBuilder: (context, index) {
                  final item = order.items[index];
                  return _buildItemRow(index + 1, item, currencyFormat);
                },
              ),
            ),

            const Divider(height: 1),

            // Totals
            Padding(
              padding: const EdgeInsets.all(24.0),
              child: Column(
                children: [
                  _buildTotalRow('Subtotal', currencyFormat.format(order.subtotal), isBold: true),
                  if (order.deliveryFee > 0)
                    _buildTotalRow('Taxa de Entrega', currencyFormat.format(order.deliveryFee)),
                  if (order.discountAmount > 0)
                    _buildTotalRow('Desconto', '-${currencyFormat.format(order.discountAmount)}', color: Colors.green),
                  
                  const SizedBox(height: 16),
                  _buildTotalRow('Total', currencyFormat.format(order.total), isBold: true, fontSize: 20),
                ],
              ),
            ),

            // Print Button
            Padding(
              padding: const EdgeInsets.fromLTRB(24, 0, 24, 24),
              child: ElevatedButton(
                onPressed: () async {
                  try {
                    // Show loading
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(content: Text('Preparando impressão...')),
                    );

                    final profile = await esc.CapabilityProfile.load();
                    final bytes = await ReceiptGenerator.generateOrderReceipt(
                      order: order,
                      paperSize: PaperSize.mm80, // PaperSize enum from printer_service.dart
                      profile: profile,
                    );

                    // Mock device for testing (Network printer)
                    // TODO: Move this to a settings page
                    final device = PrinterDeviceInfo(
                      name: 'Impressora Cozinha',
                      address: '192.168.1.100', // Typical local IP
                      type: PrinterConnectionType.network,
                    );

                    final success = await printerService.printReceipt(device, bytes);

                    if (context.mounted) {
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(
                          content: Text(success ? 'Cupom enviado para a impressora!' : 'Erro ao conectar na impressora. Verifique a rede.'),
                          backgroundColor: success ? Colors.green : Colors.red,
                        ),
                      );
                      if (success) Navigator.of(context).pop();
                    }
                  } catch (e) {
                    if (context.mounted) {
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(content: Text('Erro: $e'), backgroundColor: Colors.red),
                      );
                    }
                  }
                },
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(8),
                  ),
                ),
                child: const Text(
                  'Imprimir Recibo',
                  style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    color: Colors.white,
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            label,
            style: const TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.bold,
              color: AppColors.textPrimary,
            ),
          ),
          Text(
            value,
            style: const TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w500,
              color: AppColors.textPrimary,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildItemRow(int index, OrderItemResponseDto item, NumberFormat format) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            '$index). ',
            style: const TextStyle(
              fontWeight: FontWeight.bold,
              color: AppColors.textPrimary,
            ),
          ),
          Expanded(
            child: Text(
              item.productName,
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                color: AppColors.textPrimary,
              ),
            ),
          ),
          SizedBox(
            width: 40,
            child: Text(
              '${item.quantity}',
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                color: AppColors.textPrimary,
              ),
            ),
          ),
          SizedBox(
            width: 80,
            child: Text(
              format.format(item.subtotal),
              textAlign: TextAlign.right,
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                color: AppColors.textPrimary,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTotalRow(String label, String value, {bool isBold = false, double fontSize = 14, Color? color}) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            label,
            style: TextStyle(
              fontSize: fontSize,
              fontWeight: isBold ? FontWeight.bold : FontWeight.normal,
              color: color ?? (isBold ? AppColors.textPrimary : AppColors.textSecondary),
            ),
          ),
          Text(
            value,
            style: TextStyle(
              fontSize: fontSize,
              fontWeight: isBold ? FontWeight.bold : FontWeight.normal,
              color: color ?? (isBold ? AppColors.textPrimary : AppColors.textSecondary),
            ),
          ),
        ],
      ),
    );
  }
}
