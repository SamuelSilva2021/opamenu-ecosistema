import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../pos/data/models/paged_response_model.dart';
import '../../../pos/domain/models/order_response_dto.dart';
import '../../data/models/table_response_dto.dart';
import '../../data/models/create_table_request_dto.dart';
import '../../data/models/update_table_request_dto.dart';
import '../../data/repositories/tables_repository_impl.dart';

part 'tables_controller.g.dart';

@riverpod
class TablesPagination extends _$TablesPagination {
  @override
  int build() => 1;

  void setPage(int page) => state = page;
  
  void nextPage() => state++;
  
  void previousPage() {
    if (state > 1) state--;
  }
}

@riverpod
class TablesController extends _$TablesController {
  @override
  Future<PagedResponseModel<List<TableResponseDto>>> build() async {
    final page = ref.watch(tablesPaginationProvider);
    return _fetchTables(page);
  }

  Future<PagedResponseModel<List<TableResponseDto>>> _fetchTables(int page) async {
    final repository = ref.read(tablesRepositoryProvider);
    return repository.getTables(page: page);
  }

  Future<void> refresh() async {
    ref.invalidateSelf();
    await future;
  }

  Future<void> createTable(String name, int capacity) async {
    final repository = ref.read(tablesRepositoryProvider);
    final dto = CreateTableRequestDto(name: name, capacity: capacity);
    await repository.createTable(dto);
    ref.invalidateSelf();
  }

  Future<void> updateTable(String id, String name, int capacity, bool isActive) async {
    final repository = ref.read(tablesRepositoryProvider);
    final dto = UpdateTableRequestDto(name: name, capacity: capacity, isActive: isActive);
    await repository.updateTable(id, dto);
    ref.invalidateSelf();
  }

  Future<void> deleteTable(String id) async {
    final repository = ref.read(tablesRepositoryProvider);
    await repository.deleteTable(id);
    ref.invalidateSelf();
  }

  Future<String> generateQrCode(String id) async {
    final repository = ref.read(tablesRepositoryProvider);
    final qrCodeUrl = await repository.generateQrCode(id);
    ref.invalidateSelf();
    return qrCodeUrl;
  }

  Future<OrderResponseDto?> checkActiveOrder(String id) async {
    final repository = ref.read(tablesRepositoryProvider);
    return repository.getActiveOrder(id);
  }

  Future<OrderResponseDto> closeAccount(String id) async {
    final repository = ref.read(tablesRepositoryProvider);
    return repository.closeAccount(id);
  }
}
