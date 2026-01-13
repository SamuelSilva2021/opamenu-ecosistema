import 'package:fpdart/fpdart.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/models/dashboard_summary_dto.dart';
import '../datasources/dashboard_remote_datasource.dart';

part 'dashboard_repository.g.dart';

@riverpod
DashboardRepository dashboardRepository(Ref ref) {
  return DashboardRepository(ref.watch(dashboardRemoteDatasourceProvider));
}

class DashboardRepository {
  final DashboardRemoteDatasource _dataSource;

  DashboardRepository(this._dataSource);

  Future<Either<String, DashboardSummaryDto>> getSummary() async {
    try {
      final summary = await _dataSource.getSummary();
      return Right(summary);
    } catch (e) {
      return Left(e.toString());
    }
  }
}
