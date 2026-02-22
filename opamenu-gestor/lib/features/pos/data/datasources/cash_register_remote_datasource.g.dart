// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_register_remote_datasource.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(cashRegisterRemoteDataSource)
final cashRegisterRemoteDataSourceProvider =
    CashRegisterRemoteDataSourceProvider._();

final class CashRegisterRemoteDataSourceProvider
    extends
        $FunctionalProvider<
          CashRegisterRemoteDataSource,
          CashRegisterRemoteDataSource,
          CashRegisterRemoteDataSource
        >
    with $Provider<CashRegisterRemoteDataSource> {
  CashRegisterRemoteDataSourceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'cashRegisterRemoteDataSourceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$cashRegisterRemoteDataSourceHash();

  @$internal
  @override
  $ProviderElement<CashRegisterRemoteDataSource> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  CashRegisterRemoteDataSource create(Ref ref) {
    return cashRegisterRemoteDataSource(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(CashRegisterRemoteDataSource value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<CashRegisterRemoteDataSource>(value),
    );
  }
}

String _$cashRegisterRemoteDataSourceHash() =>
    r'a725cc32f705c595b8b1e269d368ff53a2130682';
