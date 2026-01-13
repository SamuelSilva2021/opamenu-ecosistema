import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../pos/data/models/paged_response_model.dart';
import '../../../pos/domain/models/order_response_dto.dart';
import '../../domain/repositories/tables_repository.dart';
import '../datasources/tables_remote_datasource.dart';
import '../models/create_table_request_dto.dart';
import '../models/table_response_dto.dart';
import '../models/update_table_request_dto.dart';

part 'tables_repository_impl.g.dart';

@riverpod
ITablesRepository tablesRepository(Ref ref) {
  return TablesRepositoryImpl(ref.watch(tablesRemoteDataSourceProvider));
}

class TablesRepositoryImpl implements ITablesRepository {
  final TablesRemoteDataSource _dataSource;

  TablesRepositoryImpl(this._dataSource);

  @override
  Future<PagedResponseModel<List<TableResponseDto>>> getTables({int page = 1, int pageSize = 10}) {
    return _dataSource.getTables(page: page, pageSize: pageSize);
  }

  @override
  Future<TableResponseDto> getTable(int id) {
    return _dataSource.getTable(id);
  }

  @override
  Future<TableResponseDto> createTable(CreateTableRequestDto dto) {
    return _dataSource.createTable(dto);
  }

  @override
  Future<TableResponseDto> updateTable(int id, UpdateTableRequestDto dto) {
    return _dataSource.updateTable(id, dto);
  }

  @override
  Future<bool> deleteTable(int id) {
    return _dataSource.deleteTable(id);
  }

  @override
  Future<String> generateQrCode(int id) {
    return _dataSource.generateQrCode(id);
  }

  @override
  Future<OrderResponseDto?> getActiveOrder(int id) {
    return _dataSource.getActiveOrder(id);
  }

  @override
  Future<OrderResponseDto> closeAccount(int id) {
    return _dataSource.closeAccount(id);
  }
}
