import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/tabs_di.dart';
import '../../../../core/domain/failures/unauthorized_failure.dart';
import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../../../auth/presentation/providers/auth_controller_provider.dart';
import '../../domain/entities/tab_entity.dart';

class TabsController extends AsyncNotifier<List<TabEntity>> {
  String? _tableId;
  Object? _activeRequest;

  @override
  Future<List<TabEntity>> build() async {
    return const <TabEntity>[];
  }

  Future<void> load({required String tableId, bool force = false}) async {
    if (!force && _tableId == tableId && state.hasValue) return;

    _tableId = tableId;
    final requestId = Object();
    _activeRequest = requestId;
    state = const AsyncLoading();

    final session = await ref.watch(AuthControllerProvider.provider.future);
    if (_activeRequest != requestId) return;
    if (session == null) {
      state = const AsyncData(<TabEntity>[]);
      return;
    }

    final useCase = ref.read(TabsDi.fetchTabsUseCaseProvider);
    final result = await useCase(accessToken: session.tokens.accessToken, tableId: tableId);
    if (_activeRequest != requestId) return;

    if (result is FailureResult<List<TabEntity>>) {
      if (result.failure is UnauthorizedFailure) {
        await ref.read(AuthControllerProvider.provider.notifier).refreshSession();
        if (_activeRequest != requestId) return;

        final refreshedSession = await ref.read(AuthControllerProvider.provider.future);
        if (_activeRequest != requestId) return;
        if (refreshedSession == null) {
          state = const AsyncData(<TabEntity>[]);
          return;
        }

        final retryResult =
            await useCase(accessToken: refreshedSession.tokens.accessToken, tableId: tableId);
        if (_activeRequest != requestId) return;

        if (retryResult is FailureResult<List<TabEntity>>) {
          if (retryResult.failure is UnauthorizedFailure) {
            ref.read(AuthControllerProvider.provider.notifier).signOut();
            state = const AsyncData(<TabEntity>[]);
            return;
          }

          state = AsyncError(retryResult.failure, StackTrace.current);
          return;
        }

        state = AsyncData((retryResult as SuccessResult<List<TabEntity>>).value);
        return;
      }

      state = AsyncError(result.failure, StackTrace.current);
      return;
    }

    state = AsyncData((result as SuccessResult<List<TabEntity>>).value);
  }

  Future<void> openTab({required String tableId, String? name}) async {
    final requestId = Object();
    _activeRequest = requestId;
    state = const AsyncLoading();

    final session = await ref.read(AuthControllerProvider.provider.future);
    if (_activeRequest != requestId) return;
    if (session == null) {
      state = const AsyncData(<TabEntity>[]);
      return;
    }

    final useCase = ref.read(TabsDi.openTabUseCaseProvider);
    final result = await useCase(
      accessToken: session.tokens.accessToken,
      tableId: tableId,
      name: name,
    );

    if (_activeRequest != requestId) return;

    if (result is FailureResult<TabEntity>) {
      if (result.failure is UnauthorizedFailure) {
        await ref.read(AuthControllerProvider.provider.notifier).refreshSession();
        if (_activeRequest != requestId) return;

        final refreshedSession = await ref.read(AuthControllerProvider.provider.future);
        if (_activeRequest != requestId) return;
        if (refreshedSession == null) {
          state = const AsyncData(<TabEntity>[]);
          return;
        }

        final retryResult = await useCase(
          accessToken: refreshedSession.tokens.accessToken,
          tableId: tableId,
          name: name,
        );

        if (_activeRequest != requestId) return;

        if (retryResult is FailureResult<TabEntity>) {
          if (retryResult.failure is UnauthorizedFailure) {
            ref.read(AuthControllerProvider.provider.notifier).signOut();
            state = const AsyncData(<TabEntity>[]);
            return;
          }

          state = AsyncError(retryResult.failure, StackTrace.current);
          return;
        }

        final created = (retryResult as SuccessResult<TabEntity>).value;
        final current = state.asData?.value ?? const <TabEntity>[];
        state = AsyncData([...current, created]);
        await load(tableId: tableId, force: true);
        return;
      }

      state = AsyncError(result.failure, StackTrace.current);
      return;
    }

    final created = (result as SuccessResult<TabEntity>).value;
    final current = state.asData?.value ?? const <TabEntity>[];
    state = AsyncData([...current, created]);
    await load(tableId: tableId, force: true);
  }

  Future<bool> updateTab({
    required String tabId,
    String? name,
    String? tableId,
  }) async {
    final currentTableId = _tableId;
    if (currentTableId == null) return false;

    final requestId = Object();
    _activeRequest = requestId;
    state = const AsyncLoading();

    final session = await ref.read(AuthControllerProvider.provider.future);
    if (_activeRequest != requestId) return false;
    if (session == null) {
      state = const AsyncData(<TabEntity>[]);
      return false;
    }

    final useCase = ref.read(TabsDi.updateTabUseCaseProvider);
    final result = await useCase(
      accessToken: session.tokens.accessToken,
      tabId: tabId,
      name: name,
      tableId: tableId,
    );

    if (_activeRequest != requestId) return false;

    if (result is FailureResult<TabEntity>) {
      if (result.failure is UnauthorizedFailure) {
        await ref.read(AuthControllerProvider.provider.notifier).refreshSession();
        if (_activeRequest != requestId) return false;

        final refreshedSession = await ref.read(AuthControllerProvider.provider.future);
        if (_activeRequest != requestId) return false;
        if (refreshedSession == null) {
          state = const AsyncData(<TabEntity>[]);
          return false;
        }

        final retryResult = await useCase(
          accessToken: refreshedSession.tokens.accessToken,
          tabId: tabId,
          name: name,
          tableId: tableId,
        );

        if (_activeRequest != requestId) return false;

        if (retryResult is FailureResult<TabEntity>) {
          if (retryResult.failure is UnauthorizedFailure) {
            ref.read(AuthControllerProvider.provider.notifier).signOut();
            state = const AsyncData(<TabEntity>[]);
            return false;
          }

          state = AsyncError(retryResult.failure, StackTrace.current);
          return false;
        }

        await load(tableId: currentTableId, force: true);
        return true;
      }

      state = AsyncError(result.failure, StackTrace.current);
      return false;
    }

    await load(tableId: currentTableId);
    return true;
  }

  Future<bool> deleteTab({required String tabId}) async {
    final currentTableId = _tableId;
    if (currentTableId == null) return false;

    final requestId = Object();
    _activeRequest = requestId;
    state = const AsyncLoading();

    final session = await ref.read(AuthControllerProvider.provider.future);
    if (_activeRequest != requestId) return false;
    if (session == null) {
      state = const AsyncData(<TabEntity>[]);
      return false;
    }

    final useCase = ref.read(TabsDi.deleteTabUseCaseProvider);
    final result = await useCase(accessToken: session.tokens.accessToken, tabId: tabId);

    if (_activeRequest != requestId) return false;

    if (result is FailureResult<bool>) {
      if (result.failure is UnauthorizedFailure) {
        await ref.read(AuthControllerProvider.provider.notifier).refreshSession();
        if (_activeRequest != requestId) return false;

        final refreshedSession = await ref.read(AuthControllerProvider.provider.future);
        if (_activeRequest != requestId) return false;
        if (refreshedSession == null) {
          state = const AsyncData(<TabEntity>[]);
          return false;
        }

        final retryResult =
            await useCase(accessToken: refreshedSession.tokens.accessToken, tabId: tabId);

        if (_activeRequest != requestId) return false;

        if (retryResult is FailureResult<bool>) {
          if (retryResult.failure is UnauthorizedFailure) {
            ref.read(AuthControllerProvider.provider.notifier).signOut();
            state = const AsyncData(<TabEntity>[]);
            return false;
          }

          state = AsyncError(retryResult.failure, StackTrace.current);
          return false;
        }

        await load(tableId: currentTableId, force: true);
        return true;
      }

      state = AsyncError(result.failure, StackTrace.current);
      return false;
    }

    await load(tableId: currentTableId);
    return true;
  }
}
