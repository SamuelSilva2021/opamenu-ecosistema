// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'collaborator_remote_datasource.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(collaboratorRemoteDataSource)
final collaboratorRemoteDataSourceProvider =
    CollaboratorRemoteDataSourceProvider._();

final class CollaboratorRemoteDataSourceProvider
    extends
        $FunctionalProvider<
          CollaboratorRemoteDataSource,
          CollaboratorRemoteDataSource,
          CollaboratorRemoteDataSource
        >
    with $Provider<CollaboratorRemoteDataSource> {
  CollaboratorRemoteDataSourceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'collaboratorRemoteDataSourceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$collaboratorRemoteDataSourceHash();

  @$internal
  @override
  $ProviderElement<CollaboratorRemoteDataSource> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  CollaboratorRemoteDataSource create(Ref ref) {
    return collaboratorRemoteDataSource(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(CollaboratorRemoteDataSource value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<CollaboratorRemoteDataSource>(value),
    );
  }
}

String _$collaboratorRemoteDataSourceHash() =>
    r'd1bf23795b87b00aef187af3ec3ce4893882f6c9';
