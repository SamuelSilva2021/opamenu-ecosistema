// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customers_remote_datasource.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(customersRemoteDataSource)
final customersRemoteDataSourceProvider = CustomersRemoteDataSourceProvider._();

final class CustomersRemoteDataSourceProvider
    extends
        $FunctionalProvider<
          CustomersRemoteDataSource,
          CustomersRemoteDataSource,
          CustomersRemoteDataSource
        >
    with $Provider<CustomersRemoteDataSource> {
  CustomersRemoteDataSourceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'customersRemoteDataSourceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$customersRemoteDataSourceHash();

  @$internal
  @override
  $ProviderElement<CustomersRemoteDataSource> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  CustomersRemoteDataSource create(Ref ref) {
    return customersRemoteDataSource(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(CustomersRemoteDataSource value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<CustomersRemoteDataSource>(value),
    );
  }
}

String _$customersRemoteDataSourceHash() =>
    r'415be0dad7efb635a360fef912ced59e07fcdb0f';
