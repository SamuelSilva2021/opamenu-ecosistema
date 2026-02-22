import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../datasources/cash_register_remote_datasource.dart';
import '../../domain/models/cash_shift_response_dto.dart';
import '../../domain/models/cash_movement_response_dto.dart';
import '../../domain/models/cash_register_requests.dart';
import '../../domain/models/cash_register_report_dto.dart';

part 'cash_register_repository.g.dart';

@riverpod
CashRegisterRepository cashRegisterRepository(Ref ref) {
  return CashRegisterRepository(ref.watch(cashRegisterRemoteDataSourceProvider));
}

class CashRegisterRepository {
  final CashRegisterRemoteDataSource _dataSource;

  CashRegisterRepository(this._dataSource);

  Future<CashShiftResponseDto?> getActiveShift() => _dataSource.getActiveShift();

  Future<CashShiftResponseDto> openShift(OpenCashShiftRequestDto request) => 
      _dataSource.openShift(request);

  Future<CashShiftResponseDto> closeShift(CloseCashShiftRequestDto request) => 
      _dataSource.closeShift(request);

  Future<CashMovementResponseDto> addMovement(AddCashMovementRequestDto request) => 
      _dataSource.addMovement(request);

  Future<CashRegisterReportDto> getReport(DateTime startDate, DateTime endDate) =>
      _dataSource.getReport(startDate, endDate);
}
