import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../data/repositories/cash_register_repository.dart';
import '../../domain/models/cash_register_report_dto.dart';
import 'dart:developer' as developer;

part 'cash_report_notifier.g.dart';

@riverpod
class CashReportNotifier extends _$CashReportNotifier {
  @override
  FutureOr<CashRegisterReportDto?> build() async {
    // Default period: Today
    final now = DateTime.now();
    final start = DateTime(now.year, now.month, now.day);
    final end = DateTime(now.year, now.month, now.day, 23, 59, 59);
    
    return fetchReport(start, end);
  }

  Future<CashRegisterReportDto?> fetchReport(DateTime start, DateTime end) async {
    state = const AsyncLoading();
    
    state = await AsyncValue.guard(() async {
      try {
        final repository = ref.read(cashRegisterRepositoryProvider);
        return await repository.getReport(start, end);
      } catch (e, stack) {
        developer.log('Error fetching cash report', error: e, stackTrace: stack);
        rethrow;
      }
    });

    return state.value;
  }
}
