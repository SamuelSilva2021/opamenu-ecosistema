// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'additional_repository_impl.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(additionalRepository)
final additionalRepositoryProvider = AdditionalRepositoryProvider._();

final class AdditionalRepositoryProvider
    extends
        $FunctionalProvider<
          AdditionalRepository,
          AdditionalRepository,
          AdditionalRepository
        >
    with $Provider<AdditionalRepository> {
  AdditionalRepositoryProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'additionalRepositoryProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$additionalRepositoryHash();

  @$internal
  @override
  $ProviderElement<AdditionalRepository> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  AdditionalRepository create(Ref ref) {
    return additionalRepository(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(AdditionalRepository value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<AdditionalRepository>(value),
    );
  }
}

String _$additionalRepositoryHash() =>
    r'aedb0da0da52a78f782f2e45d8775d5953e812e2';
