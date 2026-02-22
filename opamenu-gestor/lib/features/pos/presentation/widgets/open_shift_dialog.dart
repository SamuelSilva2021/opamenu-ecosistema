import 'package:flutter/material.dart';
import 'package:dio/dio.dart';
import '../../../../core/theme/app_colors.dart';

class OpenShiftDialog extends StatefulWidget {
  final Future<void> Function(double) onSubmit;
  final bool isLoading;

  const OpenShiftDialog({
    super.key,
    required this.onSubmit,
    this.isLoading = false,
  });

  @override
  State<OpenShiftDialog> createState() => _OpenShiftDialogState();
}

class _OpenShiftDialogState extends State<OpenShiftDialog> {
  final _controller = TextEditingController();
  final _formKey = GlobalKey<FormState>();
  String? _errorMessage;

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Abrir Novo Turno'),
      content: Form(
        key: _formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Text(
              'Informe o valor disponível em dinheiro na gaveta para iniciar o turno.',
              style: TextStyle(color: AppColors.textSecondary),
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _controller,
              keyboardType: const TextInputType.numberWithOptions(decimal: true),
              decoration: const InputDecoration(
                labelText: 'Fundo de Troco (R\$)',
                hintText: '0,00',
                border: OutlineInputBorder(),
                prefixText: 'R\$ ',
              ),
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Informe o valor inicial';
                }
                if (double.tryParse(value.replaceAll(',', '.')) == null) {
                  return 'Valor inválido';
                }
                return null;
              },
            ),
            if (_errorMessage != null) ...[
              const SizedBox(height: 16),
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.red.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.error_outline, color: Colors.red, size: 20),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        _errorMessage!,
                        style: const TextStyle(color: Colors.red, fontSize: 13),
                      ),
                    ),
                  ],
                ),
              ),
            ],
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
              : () async {
                  if (_formKey.currentState!.validate()) {
                    setState(() => _errorMessage = null);
                    final value = double.parse(_controller.text.replaceAll(',', '.'));
                    try {
                      await widget.onSubmit(value);
                    } catch (e) {
                      if (context.mounted) {
                        setState(() {
                          if (e is DioException) {
                            if (e.response?.data is List) {
                              final errors = e.response?.data as List;
                              if (errors.isNotEmpty && errors.first is Map) {
                                _errorMessage = errors.first['message'];
                              }
                            }
                            _errorMessage ??= e.message;
                          } else {
                            _errorMessage = e.toString();
                          }
                        });
                      }
                    }
                  }
                },
          style: ElevatedButton.styleFrom(
            backgroundColor: AppColors.primary,
            foregroundColor: Colors.white,
          ),
          child: widget.isLoading
              ? const SizedBox(
                  height: 20,
                  width: 20,
                  child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                )
              : const Text('ABRIR TURNO'),
        ),
      ],
    );
  }
}
