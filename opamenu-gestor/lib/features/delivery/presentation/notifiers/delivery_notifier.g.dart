// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'delivery_notifier.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(DeliveryNotifier)
final deliveryProvider = DeliveryNotifierProvider._();

final class DeliveryNotifierProvider
    extends $AsyncNotifierProvider<DeliveryNotifier, DeliveryState> {
  DeliveryNotifierProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'deliveryProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$deliveryNotifierHash();

  @$internal
  @override
  DeliveryNotifier create() => DeliveryNotifier();
}

String _$deliveryNotifierHash() => r'e2a88ba644fb6228c6a7556b3031644c324aa327';

abstract class _$DeliveryNotifier extends $AsyncNotifier<DeliveryState> {
  FutureOr<DeliveryState> build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref = this.ref as $Ref<AsyncValue<DeliveryState>, DeliveryState>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<AsyncValue<DeliveryState>, DeliveryState>,
              AsyncValue<DeliveryState>,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
