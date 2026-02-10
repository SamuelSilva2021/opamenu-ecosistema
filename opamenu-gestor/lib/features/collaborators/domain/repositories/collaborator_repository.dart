import 'package:fpdart/fpdart.dart';
import 'package:opamenu_gestor/features/collaborators/domain/models/collaborator_model.dart';

abstract class CollaboratorRepository {
  Future<Either<String, List<CollaboratorModel>>> getCollaborators();
  Future<Either<String, CollaboratorModel>> createCollaborator(Map<String, dynamic> data);
  Future<Either<String, CollaboratorModel>> updateCollaborator(String id, Map<String, dynamic> data);
  Future<Either<String, bool>> deleteCollaborator(String id);
}
