// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tenant_payment_method_repository_impl.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(tenantPaymentMethodRepository)
final tenantPaymentMethodRepositoryProvider =
    TenantPaymentMethodRepositoryProvider._();

final class TenantPaymentMethodRepositoryProvider
    extends
        $FunctionalProvider<
          TenantPaymentMethodRepository,
          TenantPaymentMethodRepository,
          TenantPaymentMethodRepository
        >
    with $Provider<TenantPaymentMethodRepository> {
  TenantPaymentMethodRepositoryProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'tenantPaymentMethodRepositoryProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$tenantPaymentMethodRepositoryHash();

  @$internal
  @override
  $ProviderElement<TenantPaymentMethodRepository> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  TenantPaymentMethodRepository create(Ref ref) {
    return tenantPaymentMethodRepository(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(TenantPaymentMethodRepository value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<TenantPaymentMethodRepository>(
        value,
      ),
    );
  }
}

String _$tenantPaymentMethodRepositoryHash() =>
    r'1b76092e0354637bd466a52437548733d5f21dd2';
