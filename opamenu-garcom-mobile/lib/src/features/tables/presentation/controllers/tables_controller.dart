import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/tables_di.dart';
import '../../../../core/domain/failures/unauthorized_failure.dart';
import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../../../auth/presentation/providers/auth_controller_provider.dart';
import '../../domain/entities/table_entity.dart';

class TablesController extends AsyncNotifier<List<TableEntity>> {
  @override
  Future<List<TableEntity>> build() async {
    final session = await ref.watch(AuthControllerProvider.provider.future);
    if (session == null) return const <TableEntity>[];

    final useCase = ref.read(TablesDi.fetchTablesUseCaseProvider);
    final result = await useCase(accessToken: session.tokens.accessToken);
    if (result is FailureResult<List<TableEntity>>) {
      if (result.failure is UnauthorizedFailure) {
        await ref.read(AuthControllerProvider.provider.notifier).refreshSession();
        final refreshedSession = await ref.read(AuthControllerProvider.provider.future);
        if (refreshedSession == null) return const <TableEntity>[];

        final retryResult = await useCase(accessToken: refreshedSession.tokens.accessToken);
        if (retryResult is FailureResult<List<TableEntity>>) {
          if (retryResult.failure is UnauthorizedFailure) {
            ref.read(AuthControllerProvider.provider.notifier).signOut();
            return const <TableEntity>[];
          }

          throw retryResult.failure;
        }

        return (retryResult as SuccessResult<List<TableEntity>>).value;
      }

      throw result.failure;
    }

    return (result as SuccessResult<List<TableEntity>>).value;
  }

  Future<void> refresh() async {
    state = const AsyncLoading();
    state = await AsyncValue.guard(build);
  }
}
