import 'package:fpdart/fpdart.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:opamenu_gestor/features/collaborators/data/datasources/collaborator_remote_datasource.dart';
import 'package:opamenu_gestor/features/collaborators/domain/models/collaborator_model.dart';
import 'package:opamenu_gestor/features/collaborators/domain/repositories/collaborator_repository.dart';

part 'collaborator_repository_impl.g.dart';

@riverpod
CollaboratorRepository collaboratorRepository(Ref ref) {
  final dataSource = ref.watch(collaboratorRemoteDataSourceProvider);
  return CollaboratorRepositoryImpl(dataSource);
}

class CollaboratorRepositoryImpl implements CollaboratorRepository {
  final CollaboratorRemoteDataSource _dataSource;

  CollaboratorRepositoryImpl(this._dataSource);

  @override
  Future<Either<String, List<CollaboratorModel>>> getCollaborators() async {
    try {
      final result = await _dataSource.getCollaborators();
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, CollaboratorModel>> createCollaborator(Map<String, dynamic> data) async {
    try {
      final result = await _dataSource.createCollaborator(data);
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, CollaboratorModel>> updateCollaborator(String id, Map<String, dynamic> data) async {
    try {
      final result = await _dataSource.updateCollaborator(id, data);
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, bool>> deleteCollaborator(String id) async {
    try {
      final result = await _dataSource.deleteCollaborator(id);
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }
}
