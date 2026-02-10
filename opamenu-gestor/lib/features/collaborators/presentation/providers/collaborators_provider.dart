import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:opamenu_gestor/features/collaborators/domain/models/collaborator_model.dart';
import 'package:opamenu_gestor/features/collaborators/data/repositories/collaborator_repository_impl.dart';

part 'collaborators_provider.g.dart';

@riverpod
class CollaboratorsNotifier extends _$CollaboratorsNotifier {
  @override
  FutureOr<List<CollaboratorModel>> build() async {
    final repository = ref.watch(collaboratorRepositoryProvider);
    final result = await repository.getCollaborators();
    return result.fold(
      (error) => throw Exception(error),
      (data) => data,
    );
  }

  Future<void> createCollaborator(Map<String, dynamic> data) async {
    state = const AsyncValue.loading();
    final repository = ref.read(collaboratorRepositoryProvider);
    final result = await repository.createCollaborator(data);
    
    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (data) => ref.invalidateSelf(),
    );
  }

  Future<void> updateCollaborator(String id, Map<String, dynamic> data) async {
    state = const AsyncValue.loading();
    final repository = ref.read(collaboratorRepositoryProvider);
    final result = await repository.updateCollaborator(id, data);
    
    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (data) => ref.invalidateSelf(),
    );
  }
  
  Future<void> deleteCollaborator(String id) async {
    state = const AsyncValue.loading();
    final repository = ref.read(collaboratorRepositoryProvider);
    final result = await repository.deleteCollaborator(id);
    
    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (success) => ref.invalidateSelf(),
    );
  }
}
