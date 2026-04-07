import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/tabs_di.dart';
import '../../../../core/domain/failures/unauthorized_failure.dart';
import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../../../auth/presentation/providers/auth_controller_provider.dart';
import '../../domain/entities/tab_item_entity.dart';

class TabItemsController extends AsyncNotifier<List<TabItemEntity>> {
  Object? _activeRequest;
  String? _tabId;

  @override
  Future<List<TabItemEntity>> build() async {
    return const <TabItemEntity>[];
  }

  Future<void> load({required String tabId}) async {
    _tabId = tabId;
    final requestId = Object();
    _activeRequest = requestId;
    state = const AsyncLoading();

    final session = await ref.read(AuthControllerProvider.provider.future);
    if (_activeRequest != requestId) return;
    if (session == null) {
      state = const AsyncData(<TabItemEntity>[]);
      return;
    }

    final useCase = ref.read(TabsDi.fetchTabItemsUseCaseProvider);
    final result = await useCase(accessToken: session.tokens.accessToken, tabId: tabId);
    if (_activeRequest != requestId) return;

    if (result is FailureResult<List<TabItemEntity>>) {
      if (result.failure is UnauthorizedFailure) {
        await ref.read(AuthControllerProvider.provider.notifier).refreshSession();
        if (_activeRequest != requestId) return;

        final refreshedSession = await ref.read(AuthControllerProvider.provider.future);
        if (_activeRequest != requestId) return;
        if (refreshedSession == null) {
          state = const AsyncData(<TabItemEntity>[]);
          return;
        }

        final retryResult =
            await useCase(accessToken: refreshedSession.tokens.accessToken, tabId: tabId);
        if (_activeRequest != requestId) return;

        if (retryResult is FailureResult<List<TabItemEntity>>) {
          if (retryResult.failure is UnauthorizedFailure) {
            ref.read(AuthControllerProvider.provider.notifier).signOut();
            state = const AsyncData(<TabItemEntity>[]);
            return;
          }

          state = AsyncError(retryResult.failure, StackTrace.current);
          return;
        }

        state = AsyncData((retryResult as SuccessResult<List<TabItemEntity>>).value);
        return;
      }

      state = AsyncError(result.failure, StackTrace.current);
      return;
    }

    state = AsyncData((result as SuccessResult<List<TabItemEntity>>).value);
  }

  Future<void> refresh() async {
    final tabId = _tabId;
    if (tabId == null) return;
    await load(tabId: tabId);
  }
}
