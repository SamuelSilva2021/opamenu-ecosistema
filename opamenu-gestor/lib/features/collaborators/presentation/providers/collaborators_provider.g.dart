// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'collaborators_provider.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(CollaboratorsNotifier)
final collaboratorsProvider = CollaboratorsNotifierProvider._();

final class CollaboratorsNotifierProvider
    extends
        $AsyncNotifierProvider<CollaboratorsNotifier, List<CollaboratorModel>> {
  CollaboratorsNotifierProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'collaboratorsProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$collaboratorsNotifierHash();

  @$internal
  @override
  CollaboratorsNotifier create() => CollaboratorsNotifier();
}

String _$collaboratorsNotifierHash() =>
    r'7e3aee5065f03d10ec9cb9911c58614c4b270b6e';

abstract class _$CollaboratorsNotifier
    extends $AsyncNotifier<List<CollaboratorModel>> {
  FutureOr<List<CollaboratorModel>> build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref =
        this.ref
            as $Ref<
              AsyncValue<List<CollaboratorModel>>,
              List<CollaboratorModel>
            >;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<
                AsyncValue<List<CollaboratorModel>>,
                List<CollaboratorModel>
              >,
              AsyncValue<List<CollaboratorModel>>,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
