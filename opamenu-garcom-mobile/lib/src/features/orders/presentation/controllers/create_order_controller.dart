import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/orders_di.dart';
import '../../../../core/domain/failures/unauthorized_failure.dart';
import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../../../auth/presentation/providers/auth_controller_provider.dart';
import '../../domain/entities/order_entity.dart';

class CreateOrderController extends AsyncNotifier<OrderEntity?> {
  Object? _activeRequest;

  @override
  Future<OrderEntity?> build() async {
    return null;
  }

  Future<void> createTableOrder({
    required String tableId,
    required String tabId,
    required String productId,
    required int quantity,
    String? notes,
  }) async {
    final requestId = Object();
    _activeRequest = requestId;
    state = const AsyncLoading();

    final session = await ref.read(AuthControllerProvider.provider.future);
    if (_activeRequest != requestId) return;
    if (session == null) {
      state = const AsyncData(null);
      return;
    }

    final useCase = ref.read(OrdersDi.createTableOrderUseCaseProvider);
    final result = await useCase(
      accessToken: session.tokens.accessToken,
      tableId: tableId,
      tabId: tabId,
      productId: productId,
      quantity: quantity,
      notes: notes,
    );

    if (_activeRequest != requestId) return;

    if (result is FailureResult<OrderEntity>) {
      if (result.failure is UnauthorizedFailure) {
        await ref.read(AuthControllerProvider.provider.notifier).refreshSession();
        if (_activeRequest != requestId) return;

        final refreshedSession = await ref.read(AuthControllerProvider.provider.future);
        if (_activeRequest != requestId) return;
        if (refreshedSession == null) {
          state = const AsyncData(null);
          return;
        }

        final retryResult = await useCase(
          accessToken: refreshedSession.tokens.accessToken,
          tableId: tableId,
          tabId: tabId,
          productId: productId,
          quantity: quantity,
          notes: notes,
        );

        if (_activeRequest != requestId) return;

        if (retryResult is FailureResult<OrderEntity>) {
          if (retryResult.failure is UnauthorizedFailure) {
            ref.read(AuthControllerProvider.provider.notifier).signOut();
            state = const AsyncData(null);
            return;
          }

          state = AsyncError(retryResult.failure, StackTrace.current);
          return;
        }

        state = AsyncData((retryResult as SuccessResult<OrderEntity>).value);
        return;
      }

      state = AsyncError(result.failure, StackTrace.current);
      return;
    }

    state = AsyncData((result as SuccessResult<OrderEntity>).value);
  }

  void clear() {
    _activeRequest = null;
    state = const AsyncData(null);
  }
}
