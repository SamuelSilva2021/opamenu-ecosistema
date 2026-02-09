import '../../../pos/data/models/paged_response_model.dart';
import '../../../pos/domain/models/order_response_dto.dart';
import '../../data/models/create_table_request_dto.dart';
import '../../data/models/table_response_dto.dart';
import '../../data/models/update_table_request_dto.dart';

abstract class ITablesRepository {
  Future<PagedResponseModel<List<TableResponseDto>>> getTables({int page = 1, int pageSize = 10});
  Future<TableResponseDto> getTable(String id);
  Future<TableResponseDto> createTable(CreateTableRequestDto dto);
  Future<TableResponseDto> updateTable(String id, UpdateTableRequestDto dto);
  Future<bool> deleteTable(String id);
  Future<String> generateQrCode(String id);
  Future<OrderResponseDto?> getActiveOrder(String id);
  Future<OrderResponseDto> closeAccount(String id);
}
