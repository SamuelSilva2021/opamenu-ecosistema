// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'collaborator_repository_impl.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(collaboratorRepository)
final collaboratorRepositoryProvider = CollaboratorRepositoryProvider._();

final class CollaboratorRepositoryProvider
    extends
        $FunctionalProvider<
          CollaboratorRepository,
          CollaboratorRepository,
          CollaboratorRepository
        >
    with $Provider<CollaboratorRepository> {
  CollaboratorRepositoryProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'collaboratorRepositoryProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$collaboratorRepositoryHash();

  @$internal
  @override
  $ProviderElement<CollaboratorRepository> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  CollaboratorRepository create(Ref ref) {
    return collaboratorRepository(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(CollaboratorRepository value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<CollaboratorRepository>(value),
    );
  }
}

String _$collaboratorRepositoryHash() =>
    r'a90c2e17d20d4aea34fdd68b512fc104ced49f18';
