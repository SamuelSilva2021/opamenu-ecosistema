import '../../../../core/domain/result/result.dart';
import '../entities/table_entity.dart';
import '../repositories/tables_repository.dart';

class FetchTablesUseCase {
  final TablesRepository repository;

  const FetchTablesUseCase(this.repository);

  Future<Result<List<TableEntity>>> call({required String accessToken}) {
    return repository.getTables(accessToken: accessToken);
  }
}

