import '../../../../core/domain/result/result.dart';
import '../models/table_status_model.dart';

abstract class TablesRemoteDataSourceContract {
  Future<Result<List<TableStatusModel>>> getTables({required String accessToken});
}
