// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tables_controller.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(TablesPagination)
final tablesPaginationProvider = TablesPaginationProvider._();

final class TablesPaginationProvider
    extends $NotifierProvider<TablesPagination, int> {
  TablesPaginationProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'tablesPaginationProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$tablesPaginationHash();

  @$internal
  @override
  TablesPagination create() => TablesPagination();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(int value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<int>(value),
    );
  }
}

String _$tablesPaginationHash() => r'8e554c87ae64b60ae163a123134cf51d1311cce9';

abstract class _$TablesPagination extends $Notifier<int> {
  int build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref = this.ref as $Ref<int, int>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<int, int>,
              int,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}

@ProviderFor(TablesController)
final tablesControllerProvider = TablesControllerProvider._();

final class TablesControllerProvider
    extends
        $AsyncNotifierProvider<
          TablesController,
          PagedResponseModel<List<TableResponseDto>>
        > {
  TablesControllerProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'tablesControllerProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$tablesControllerHash();

  @$internal
  @override
  TablesController create() => TablesController();
}

String _$tablesControllerHash() => r'f6c85cff1033104acbeb3e43bc692a7464127266';

abstract class _$TablesController
    extends $AsyncNotifier<PagedResponseModel<List<TableResponseDto>>> {
  FutureOr<PagedResponseModel<List<TableResponseDto>>> build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref =
        this.ref
            as $Ref<
              AsyncValue<PagedResponseModel<List<TableResponseDto>>>,
              PagedResponseModel<List<TableResponseDto>>
            >;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<
                AsyncValue<PagedResponseModel<List<TableResponseDto>>>,
                PagedResponseModel<List<TableResponseDto>>
              >,
              AsyncValue<PagedResponseModel<List<TableResponseDto>>>,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
