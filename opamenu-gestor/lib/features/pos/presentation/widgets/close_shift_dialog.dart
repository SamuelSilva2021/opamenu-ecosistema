import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';

class CloseShiftDialog extends StatefulWidget {
  final double expectedBalance;
  final Function(double) onSubmit;
  final bool isLoading;

  const CloseShiftDialog({
    super.key,
    required this.expectedBalance,
    required this.onSubmit,
    this.isLoading = false,
  });

  @override
  State<CloseShiftDialog> createState() => _CloseShiftDialogState();
}

class _CloseShiftDialogState extends State<CloseShiftDialog> {
  final _controller = TextEditingController();
  final _formKey = GlobalKey<FormState>();

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Fechar Turno'),
      content: Form(
        key: _formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: Colors.blue.withOpacity(0.1),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Column(
                children: [
                  const Text(
                    'Saldo Esperado',
                    style: TextStyle(fontSize: 14, color: Colors.blue),
                  ),
                  Text(
                    'R\$ ${widget.expectedBalance.toStringAsFixed(2)}',
                    style: const TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                      color: Colors.blue,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 16),
            const Text(
              'Conte o dinheiro físico na gaveta e informe o valor final.',
              style: TextStyle(color: AppColors.textSecondary),
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _controller,
              keyboardType: const TextInputType.numberWithOptions(decimal: true),
              autofocus: true,
              decoration: const InputDecoration(
                labelText: 'Saldo Final (R\$)',
                hintText: '0,00',
                border: OutlineInputBorder(),
                prefixText: 'R\$ ',
              ),
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Informe o valor real em caixa';
                }
                if (double.tryParse(value.replaceAll(',', '.')) == null) {
                  return 'Valor inválido';
                }
                return null;
              },
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: widget.isLoading ? null : () => Navigator.pop(context),
          child: const Text('CANCELAR'),
        ),
        ElevatedButton(
          onPressed: widget.isLoading
              ? null
              : () {
                  if (_formKey.currentState!.validate()) {
                    final value = double.parse(_controller.text.replaceAll(',', '.'));
                    widget.onSubmit(value);
                  }
                },
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.redAccent,
            foregroundColor: Colors.white,
          ),
          child: widget.isLoading
              ? const SizedBox(
                  height: 20,
                  width: 20,
                  child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                )
              : const Text('FECHAR CAIXA'),
        ),
      ],
    );
  }
}
