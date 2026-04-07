import '../../../../core/domain/result/result.dart';
import '../models/table_model.dart';

abstract class TablesRemoteDataSourceContract {
  Future<Result<List<TableModel>>> getTables({required String accessToken});
}

