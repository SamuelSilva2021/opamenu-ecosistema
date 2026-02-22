import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:dio/dio.dart';
import '../../../../core/theme/app_colors.dart';
import '../providers/cash_register_notifier.dart';
import '../../domain/enums/cash_shift_status.dart';
import 'open_shift_dialog.dart';

class CashRegisterGatekeeper extends ConsumerWidget {
  final Widget child;

  const CashRegisterGatekeeper({super.key, required this.child});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final cashRegisterState = ref.watch(cashRegisterProvider);

    return cashRegisterState.when(
      data: (shift) {
        if (shift != null && shift.status == CashShiftStatus.open) {
          return child;
        }

        return _buildLockedScreen(context, ref);
      },
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (error, stack) => Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.error_outline, size: 48, color: Colors.red),
            const SizedBox(height: 16),
            Text(
              'Erro ao carregar estado do caixa',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 8),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 32),
              child: Text(
                _formatError(error),
                textAlign: TextAlign.center,
                style: const TextStyle(color: AppColors.textSecondary),
              ),
            ),
            const SizedBox(height: 24),
            ElevatedButton(
              onPressed: () => ref.refresh(cashRegisterProvider),
              child: const Text('Tentar Novamente'),
            ),
          ],
        ),
      ),
    );
  }

  String _formatError(Object error) {
    if (error is DioException) {
      if (error.response?.data is List) {
        final errors = error.response?.data as List;
        if (errors.isNotEmpty && errors.first is Map) {
          return errors.first['message'] ?? error.message;
        }
      }
      return error.message ?? 'Erro de conexão com o servidor';
    }
    return error.toString();
  }

  Widget _buildLockedScreen(BuildContext context, WidgetRef ref) {
    return Container(
      color: const Color(0xFFF9FAFB),
      child: Center(
        child: SingleChildScrollView(
          child: Padding(
            padding: const EdgeInsets.all(24.0),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Container(
                  padding: const EdgeInsets.all(32),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(24),
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withOpacity(0.05),
                        blurRadius: 20,
                        offset: const Offset(0, 10),
                      ),
                    ],
                  ),
                  child: Column(
                    children: [
                      Container(
                        padding: const EdgeInsets.all(20),
                        decoration: BoxDecoration(
                          color: AppColors.primary.withOpacity(0.1),
                          shape: BoxScheme.circle,
                        ),
                        child: const Icon(
                          Icons.lock_outline,
                          size: 64,
                          color: AppColors.primary,
                        ),
                      ),
                      const SizedBox(height: 32),
                      const Text(
                        'CAIXA FECHADO',
                        style: TextStyle(
                          fontSize: 28,
                          fontWeight: FontWeight.w900,
                          letterSpacing: 1.2,
                        ),
                      ),
                      const SizedBox(height: 16),
                      const Text(
                        'O acesso ao PDV está bloqueado. Para começar a vender e gerenciar o fluxo de dinheiro, abra um novo turno.',
                        textAlign: TextAlign.center,
                        style: TextStyle(
                          fontSize: 16,
                          color: AppColors.textSecondary,
                          height: 1.5,
                        ),
                      ),
                      const SizedBox(height: 40),
                      SizedBox(
                        width: double.infinity,
                        height: 60,
                        child: ElevatedButton.icon(
                          onPressed: () => _showOpenShiftDialog(context, ref),
                          icon: const Icon(Icons.lock_open),
                          label: const Text(
                            'ABRIR TURNO AGORA',
                            style: TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppColors.primary,
                            foregroundColor: Colors.white,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(16),
                            ),
                            elevation: 0,
                          ),
                        ),
                      ),
                      const SizedBox(height: 24),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Container(
                            width: 6,
                            height: 6,
                            decoration: const BoxDecoration(
                              color: Color(0xFFD1D5DB),
                              shape: BoxScheme.circle,
                            ),
                          ),
                          const SizedBox(width: 8),
                          const Text(
                            'Controle total de entradas e saídas',
                            style: TextStyle(
                              fontSize: 14,
                              color: Color(0xFF9CA3AF),
                            ),
                          ),
                          const SizedBox(width: 8),
                          Container(
                            width: 6,
                            height: 6,
                            decoration: const BoxDecoration(
                              color: Color(0xFFD1D5DB),
                              shape: BoxScheme.circle,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  void _showOpenShiftDialog(BuildContext context, WidgetRef ref) {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => Consumer(
        builder: (context, ref, _) {
          final state = ref.watch(cashRegisterProvider);
          return OpenShiftDialog(
            isLoading: state.isLoading,
            onSubmit: (balance) async {
              try {
                await ref.read(cashRegisterProvider.notifier).openShift(balance);
                if (context.mounted) Navigator.pop(context);
              } catch (e) {
                // O erro será exibido pelo Gatekeeper se o Notifier atualizar o estado para AsyncError
                // Mas queremos que o diálogo também saiba se deu erro para parar o loading internally se necessário
                // Como o Notifier já atualiza o estado WATCHED pelo consumer do diálogo, 
                // o widget reconstruirá com state.hasError == true.
              }
            },
          );
        },
      ),
    );
  }
}

// Helper for Circle shape in decoration
class BoxScheme {
  static const BoxShape circle = BoxShape.circle;
}
