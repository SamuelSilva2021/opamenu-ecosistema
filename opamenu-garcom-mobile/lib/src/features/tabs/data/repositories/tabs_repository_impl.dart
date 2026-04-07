import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../../domain/entities/tab_item_entity.dart';
import '../../domain/entities/tab_entity.dart';
import '../../domain/repositories/tabs_repository.dart';
import '../datasources/tabs_remote_data_source_contract.dart';
import '../models/create_tab_item_request_model.dart';
import '../models/tab_item_model.dart';
import '../models/tab_model.dart';
import '../models/update_tab_request_model.dart';

class TabsRepositoryImpl implements TabsRepository {
  final TabsRemoteDataSourceContract remoteDataSource;

  const TabsRepositoryImpl(this.remoteDataSource);

  @override
  Future<Result<List<TabEntity>>> getTabs({
    required String accessToken,
    required String tableId,
  }) async {
    final result = await remoteDataSource.getTabs(accessToken: accessToken, tableId: tableId);
    if (result is FailureResult<List<TabModel>>) {
      return FailureResult(result.failure);
    }

    final models = (result as SuccessResult<List<TabModel>>).value;
    final entities = models.map((e) => e.toEntity()).toList(growable: false);
    return SuccessResult<List<TabEntity>>(entities);
  }

  @override
  Future<Result<TabEntity>> openTab({
    required String accessToken,
    required String tableId,
    String? name,
  }) async {
    final result = await remoteDataSource.openTab(
      accessToken: accessToken,
      tableId: tableId,
      name: name,
    );

    if (result is FailureResult<TabModel>) {
      return FailureResult(result.failure);
    }

    final model = (result as SuccessResult<TabModel>).value;
    return SuccessResult<TabEntity>(model.toEntity());
  }

  @override
  Future<Result<bool>> deleteTab({
    required String accessToken,
    required String tabId,
  }) {
    return remoteDataSource.deleteTab(accessToken: accessToken, tabId: tabId);
  }

  @override
  Future<Result<List<TabItemEntity>>> getTabItems({
    required String accessToken,
    required String tabId,
  }) async {
    final result = await remoteDataSource.getTabItems(accessToken: accessToken, tabId: tabId);
    if (result is FailureResult<List<TabItemModel>>) {
      return FailureResult(result.failure);
    }

    final models = (result as SuccessResult<List<TabItemModel>>).value;
    final entities = models.map((e) => e.toEntity()).toList(growable: false);
    return SuccessResult<List<TabItemEntity>>(entities);
  }

  @override
  Future<Result<bool>> addTabItems({
    required String accessToken,
    required String tabId,
    required String productId,
    required int quantity,
    String? notes,
  }) {
    return remoteDataSource.addTabItems(
      accessToken: accessToken,
      tabId: tabId,
      items: [
        CreateTabItemRequestModel(
          productId: productId,
          quantity: quantity,
          notes: notes,
        ),
      ],
    );
  }

  @override
  Future<Result<TabEntity>> updateTab({
    required String accessToken,
    required String tabId,
    String? name,
    String? tableId,
  }) async {
    final result = await remoteDataSource.updateTab(
      accessToken: accessToken,
      tabId: tabId,
      request: UpdateTabRequestModel(name: name, tableId: tableId),
    );

    if (result is FailureResult<TabModel>) {
      return FailureResult(result.failure);
    }

    final model = (result as SuccessResult<TabModel>).value;
    return SuccessResult<TabEntity>(model.toEntity());
  }
}
