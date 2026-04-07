import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/products_di.dart';
import '../../../../core/domain/failures/unauthorized_failure.dart';
import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../../../auth/presentation/providers/auth_controller_provider.dart';
import '../../domain/entities/product_entity.dart';

class CatalogController extends AsyncNotifier<List<ProductEntity>> {
  @override
  Future<List<ProductEntity>> build() async {
    final session = await ref.watch(AuthControllerProvider.provider.future);
    if (session == null) return const <ProductEntity>[];

    final useCase = ref.read(ProductsDi.fetchMenuProductsUseCaseProvider);
    final result = await useCase(accessToken: session.tokens.accessToken);
    if (result is FailureResult<List<ProductEntity>>) {
      if (result.failure is UnauthorizedFailure) {
        await ref.read(AuthControllerProvider.provider.notifier).refreshSession();
        final refreshedSession = await ref.read(AuthControllerProvider.provider.future);
        if (refreshedSession == null) return const <ProductEntity>[];

        final retryResult = await useCase(accessToken: refreshedSession.tokens.accessToken);
        if (retryResult is FailureResult<List<ProductEntity>>) {
          if (retryResult.failure is UnauthorizedFailure) {
            ref.read(AuthControllerProvider.provider.notifier).signOut();
            return const <ProductEntity>[];
          }

          throw retryResult.failure;
        }

        return (retryResult as SuccessResult<List<ProductEntity>>).value;
      }

      throw result.failure;
    }

    return (result as SuccessResult<List<ProductEntity>>).value;
  }

  Future<void> refresh() async {
    state = const AsyncLoading();
    state = await AsyncValue.guard(build);
  }
}
