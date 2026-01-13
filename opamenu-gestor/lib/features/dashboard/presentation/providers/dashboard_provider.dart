import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../data/repositories/dashboard_repository.dart';
import '../../domain/models/dashboard_summary_dto.dart';

part 'dashboard_provider.g.dart';

@riverpod
Future<DashboardSummaryDto> dashboardSummary(Ref ref) async {
  final repository = ref.watch(dashboardRepositoryProvider);
  final result = await repository.getSummary();
  
  return result.fold(
    (error) => throw Exception(error),
    (summary) => summary,
  );
}
