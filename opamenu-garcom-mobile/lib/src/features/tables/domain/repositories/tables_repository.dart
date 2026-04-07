import '../../../../core/domain/result/result.dart';
import '../entities/table_entity.dart';

abstract class TablesRepository {
  Future<Result<List<TableEntity>>> getTables({required String accessToken});
}

