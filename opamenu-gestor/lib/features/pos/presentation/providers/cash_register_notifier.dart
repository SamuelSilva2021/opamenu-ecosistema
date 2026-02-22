import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:dio/dio.dart';
import '../../data/repositories/cash_register_repository.dart';
import '../../domain/models/cash_shift_response_dto.dart';
import '../../domain/models/cash_register_requests.dart';
import '../../domain/enums/cash_shift_status.dart';

part 'cash_register_notifier.g.dart';

@riverpod
class CashRegisterNotifier extends _$CashRegisterNotifier {
  @override
  FutureOr<CashShiftResponseDto?> build() async {
    return _fetchActiveShift();
  }

  Future<CashShiftResponseDto?> _fetchActiveShift() async {
    final repository = ref.read(cashRegisterRepositoryProvider);
    return await repository.getActiveShift();
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(() => _fetchActiveShift());
  }

  Future<void> openShift(double openingBalance) async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(() async {
      final repository = ref.read(cashRegisterRepositoryProvider);
      return await repository.openShift(
        OpenCashShiftRequestDto(openingBalance: openingBalance),
      );
    });
  }

  Future<void> closeShift(double closingBalance) async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(() async {
      final repository = ref.read(cashRegisterRepositoryProvider);
      final result = await repository.closeShift(
        CloseCashShiftRequestDto(closingBalance: closingBalance),
      );
      return result;
    });
  }

  bool get isShiftOpen {
    final shift = state.value;
    return shift != null && shift.status == CashShiftStatus.open;
  }
}
