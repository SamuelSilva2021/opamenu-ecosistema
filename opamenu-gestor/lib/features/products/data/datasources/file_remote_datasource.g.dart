// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'file_remote_datasource.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(fileRemoteDataSource)
final fileRemoteDataSourceProvider = FileRemoteDataSourceProvider._();

final class FileRemoteDataSourceProvider
    extends
        $FunctionalProvider<
          FileRemoteDataSource,
          FileRemoteDataSource,
          FileRemoteDataSource
        >
    with $Provider<FileRemoteDataSource> {
  FileRemoteDataSourceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'fileRemoteDataSourceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$fileRemoteDataSourceHash();

  @$internal
  @override
  $ProviderElement<FileRemoteDataSource> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  FileRemoteDataSource create(Ref ref) {
    return fileRemoteDataSource(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(FileRemoteDataSource value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<FileRemoteDataSource>(value),
    );
  }
}

String _$fileRemoteDataSourceHash() =>
    r'd08e5f38b70938d90c9b90e580b7b56227c7c1c4';
