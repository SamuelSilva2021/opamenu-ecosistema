// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'orders_controller.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(OrdersController)
final ordersControllerProvider = OrdersControllerProvider._();

final class OrdersControllerProvider
    extends
        $AsyncNotifierProvider<
          OrdersController,
          PagedResponseModel<List<OrderResponseDto>>
        > {
  OrdersControllerProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'ordersControllerProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$ordersControllerHash();

  @$internal
  @override
  OrdersController create() => OrdersController();
}

String _$ordersControllerHash() => r'cf7bbad23c321191660c584f665717b7e662229a';

abstract class _$OrdersController
    extends $AsyncNotifier<PagedResponseModel<List<OrderResponseDto>>> {
  FutureOr<PagedResponseModel<List<OrderResponseDto>>> build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref =
        this.ref
            as $Ref<
              AsyncValue<PagedResponseModel<List<OrderResponseDto>>>,
              PagedResponseModel<List<OrderResponseDto>>
            >;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<
                AsyncValue<PagedResponseModel<List<OrderResponseDto>>>,
                PagedResponseModel<List<OrderResponseDto>>
              >,
              AsyncValue<PagedResponseModel<List<OrderResponseDto>>>,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}

@ProviderFor(OrdersPagination)
final ordersPaginationProvider = OrdersPaginationProvider._();

final class OrdersPaginationProvider
    extends $NotifierProvider<OrdersPagination, int> {
  OrdersPaginationProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'ordersPaginationProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$ordersPaginationHash();

  @$internal
  @override
  OrdersPagination create() => OrdersPagination();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(int value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<int>(value),
    );
  }
}

String _$ordersPaginationHash() => r'457c76b2745f92dd342e27cbb28390fefcadd85e';

abstract class _$OrdersPagination extends $Notifier<int> {
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

@ProviderFor(totalOrdersCount)
final totalOrdersCountProvider = TotalOrdersCountProvider._();

final class TotalOrdersCountProvider extends $FunctionalProvider<int, int, int>
    with $Provider<int> {
  TotalOrdersCountProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'totalOrdersCountProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$totalOrdersCountHash();

  @$internal
  @override
  $ProviderElement<int> $createElement($ProviderPointer pointer) =>
      $ProviderElement(pointer);

  @override
  int create(Ref ref) {
    return totalOrdersCount(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(int value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<int>(value),
    );
  }
}

String _$totalOrdersCountHash() => r'53fab7f4fb9ac3d32757e98152f24965b609a709';

@ProviderFor(SelectedOrderId)
final selectedOrderIdProvider = SelectedOrderIdProvider._();

final class SelectedOrderIdProvider
    extends $NotifierProvider<SelectedOrderId, int?> {
  SelectedOrderIdProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'selectedOrderIdProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$selectedOrderIdHash();

  @$internal
  @override
  SelectedOrderId create() => SelectedOrderId();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(int? value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<int?>(value),
    );
  }
}

String _$selectedOrderIdHash() => r'a5863c147c712280addcfb5e9aa150f522591a70';

abstract class _$SelectedOrderId extends $Notifier<int?> {
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
