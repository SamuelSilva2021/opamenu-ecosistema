import '../../../../core/domain/result/result.dart';
import '../models/create_tab_item_request_model.dart';
import '../models/tab_model.dart';
import '../models/tab_item_model.dart';
import '../models/update_tab_request_model.dart';

abstract class TabsRemoteDataSourceContract {
  Future<Result<List<TabModel>>> getTabs({
    required String accessToken,
    required String tableId,
  });

  Future<Result<TabModel>> openTab({
    required String accessToken,
    required String tableId,
    String? name,
  });

  Future<Result<TabModel>> updateTab({
    required String accessToken,
    required String tabId,
    required UpdateTabRequestModel request,
  });

  Future<Result<bool>> deleteTab({
    required String accessToken,
    required String tabId,
  });

  Future<Result<List<TabItemModel>>> getTabItems({
    required String accessToken,
    required String tabId,
  });

  Future<Result<bool>> addTabItems({
    required String accessToken,
    required String tabId,
    required List<CreateTabItemRequestModel> items,
  });
}
