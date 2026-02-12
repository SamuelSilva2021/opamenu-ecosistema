// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tenant_payment_method_remote_datasource.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(tenantPaymentMethodRemoteDataSource)
final tenantPaymentMethodRemoteDataSourceProvider =
    TenantPaymentMethodRemoteDataSourceProvider._();

final class TenantPaymentMethodRemoteDataSourceProvider
    extends
        $FunctionalProvider<
          TenantPaymentMethodRemoteDataSource,
          TenantPaymentMethodRemoteDataSource,
          TenantPaymentMethodRemoteDataSource
        >
    with $Provider<TenantPaymentMethodRemoteDataSource> {
  TenantPaymentMethodRemoteDataSourceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'tenantPaymentMethodRemoteDataSourceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() =>
      _$tenantPaymentMethodRemoteDataSourceHash();

  @$internal
  @override
  $ProviderElement<TenantPaymentMethodRemoteDataSource> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  TenantPaymentMethodRemoteDataSource create(Ref ref) {
    return tenantPaymentMethodRemoteDataSource(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(TenantPaymentMethodRemoteDataSource value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<TenantPaymentMethodRemoteDataSource>(
        value,
      ),
    );
  }
}

String _$tenantPaymentMethodRemoteDataSourceHash() =>
    r'957d5f1652c9275337041019ed58869ef115129e';
