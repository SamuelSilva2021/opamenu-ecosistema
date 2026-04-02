// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'table_map_notifier.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(TableMapNotifier)
final tableMapProvider = TableMapNotifierProvider._();

final class TableMapNotifierProvider
    extends $AsyncNotifierProvider<TableMapNotifier, TableMapState> {
  TableMapNotifierProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'tableMapProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$tableMapNotifierHash();

  @$internal
  @override
  TableMapNotifier create() => TableMapNotifier();
}

String _$tableMapNotifierHash() => r'62b0574a5f6e150e2f8c5731d63a3c564788a3c1';

abstract class _$TableMapNotifier extends $AsyncNotifier<TableMapState> {
  FutureOr<TableMapState> build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref = this.ref as $Ref<AsyncValue<TableMapState>, TableMapState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<AsyncValue<TableMapState>, TableMapState>,
              AsyncValue<TableMapState>,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
