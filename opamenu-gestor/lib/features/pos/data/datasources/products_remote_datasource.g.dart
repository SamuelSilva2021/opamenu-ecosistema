// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'products_remote_datasource.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(productsRemoteDataSource)
final productsRemoteDataSourceProvider = ProductsRemoteDataSourceProvider._();

final class ProductsRemoteDataSourceProvider
    extends
        $FunctionalProvider<
          ProductsRemoteDataSource,
          ProductsRemoteDataSource,
          ProductsRemoteDataSource
        >
    with $Provider<ProductsRemoteDataSource> {
  ProductsRemoteDataSourceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'productsRemoteDataSourceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$productsRemoteDataSourceHash();

  @$internal
  @override
  $ProviderElement<ProductsRemoteDataSource> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  ProductsRemoteDataSource create(Ref ref) {
    return productsRemoteDataSource(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(ProductsRemoteDataSource value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<ProductsRemoteDataSource>(value),
    );
  }
}

String _$productsRemoteDataSourceHash() =>
    r'c27becbb6c8936704b204afd6bd91ba9f6795aa7';
