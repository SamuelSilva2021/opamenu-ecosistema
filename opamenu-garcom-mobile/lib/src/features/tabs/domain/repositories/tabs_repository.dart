import '../../../../core/domain/result/result.dart';
import '../entities/tab_item_entity.dart';
import '../entities/tab_entity.dart';

abstract class TabsRepository {
  Future<Result<List<TabEntity>>> getTabs({
    required String accessToken,
    required String tableId,
  });

  Future<Result<TabEntity>> openTab({
    required String accessToken,
    required String tableId,
    String? name,
  });

  Future<Result<TabEntity>> updateTab({
    required String accessToken,
    required String tabId,
    String? name,
    String? tableId,
  });

  Future<Result<bool>> deleteTab({
    required String accessToken,
    required String tabId,
  });

  Future<Result<List<TabItemEntity>>> getTabItems({
    required String accessToken,
    required String tabId,
  });

  Future<Result<bool>> addTabItems({
    required String accessToken,
    required String tabId,
    required String productId,
    required int quantity,
    String? notes,
  });
}
