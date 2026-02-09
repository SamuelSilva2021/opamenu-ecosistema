// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'additional_remote_datasource.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(additionalRemoteDataSource)
final additionalRemoteDataSourceProvider =
    AdditionalRemoteDataSourceProvider._();

final class AdditionalRemoteDataSourceProvider
    extends
        $FunctionalProvider<
          AdditionalRemoteDataSource,
          AdditionalRemoteDataSource,
          AdditionalRemoteDataSource
        >
    with $Provider<AdditionalRemoteDataSource> {
  AdditionalRemoteDataSourceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'additionalRemoteDataSourceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$additionalRemoteDataSourceHash();

  @$internal
  @override
  $ProviderElement<AdditionalRemoteDataSource> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  AdditionalRemoteDataSource create(Ref ref) {
    return additionalRemoteDataSource(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(AdditionalRemoteDataSource value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<AdditionalRemoteDataSource>(value),
    );
  }
}

String _$additionalRemoteDataSourceHash() =>
    r'a5853b296c767e9e01ec48282e10f42785993130';
