// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'production_controller.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(ProductionOrders)
final productionOrdersProvider = ProductionOrdersProvider._();

final class ProductionOrdersProvider
    extends $AsyncNotifierProvider<ProductionOrders, List<OrderResponseDto>> {
  ProductionOrdersProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'productionOrdersProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$productionOrdersHash();

  @$internal
  @override
  ProductionOrders create() => ProductionOrders();
}

String _$productionOrdersHash() => r'698b3d92179a85de2851cef43d14b82c5b2d5c11';

abstract class _$ProductionOrders
    extends $AsyncNotifier<List<OrderResponseDto>> {
  FutureOr<List<OrderResponseDto>> build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref =
        this.ref
            as $Ref<AsyncValue<List<OrderResponseDto>>, List<OrderResponseDto>>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<
                AsyncValue<List<OrderResponseDto>>,
                List<OrderResponseDto>
              >,
              AsyncValue<List<OrderResponseDto>>,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
