import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../../domain/entities/table_entity.dart';
import '../../domain/repositories/tables_repository.dart';
import '../datasources/tables_remote_data_source_contract.dart';
import '../models/table_status_model.dart';

class TablesRepositoryImpl implements TablesRepository {
  final TablesRemoteDataSourceContract remoteDataSource;

  const TablesRepositoryImpl(this.remoteDataSource);

  @override
  Future<Result<List<TableEntity>>> getTables({required String accessToken}) async {
    final result = await remoteDataSource.getTables(accessToken: accessToken);
    if (result is FailureResult<List<TableStatusModel>>) {
      return FailureResult(result.failure);
    }

    final models = (result as SuccessResult<List<TableStatusModel>>).value;
    final entities = models.map((e) => e.toEntity()).toList(growable: false);
    return SuccessResult<List<TableEntity>>(entities);
  }
}
