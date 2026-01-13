// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'active_order_provider.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(ActiveOrder)
final activeOrderProvider = ActiveOrderProvider._();

final class ActiveOrderProvider
    extends $NotifierProvider<ActiveOrder, OrderResponseDto?> {
  ActiveOrderProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'activeOrderProvider',
        isAutoDispose: false,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$activeOrderHash();

  @$internal
  @override
  ActiveOrder create() => ActiveOrder();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(OrderResponseDto? value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<OrderResponseDto?>(value),
    );
  }
}

String _$activeOrderHash() => r'86ebd7861f0aff97711573efd8447ba9fe250aff';

abstract class _$ActiveOrder extends $Notifier<OrderResponseDto?> {
  OrderResponseDto? build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref = this.ref as $Ref<OrderResponseDto?, OrderResponseDto?>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<OrderResponseDto?, OrderResponseDto?>,
              OrderResponseDto?,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}

@ProviderFor(ActiveTable)
final activeTableProvider = ActiveTableProvider._();

final class ActiveTableProvider extends $NotifierProvider<ActiveTable, int?> {
  ActiveTableProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'activeTableProvider',
        isAutoDispose: false,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$activeTableHash();

  @$internal
  @override
  ActiveTable create() => ActiveTable();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(int? value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<int?>(value),
    );
  }
}

String _$activeTableHash() => r'4f94c3c134abc048c4bf174bdce31ba7c4bae1aa';

abstract class _$ActiveTable extends $Notifier<int?> {
  int? build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref = this.ref as $Ref<int?, int?>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<int?, int?>,
              int?,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
