
import 'package:fpdart/fpdart.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../datasources/additional_remote_datasource.dart';
import '../../domain/models/additional_group_model.dart';

part 'additional_repository_impl.g.dart';

@riverpod
AdditionalRepository additionalRepository(Ref ref) {
  return AdditionalRepositoryImpl(ref.watch(additionalRemoteDataSourceProvider));
}

abstract class AdditionalRepository {
  Future<Either<String, List<AdditionalGroupModel>>> getAdditionalGroups();
  Future<Either<String, AdditionalGroupModel>> createAdditionalGroup(Map<String, dynamic> data);
  Future<Either<String, AdditionalGroupModel>> updateAdditionalGroup(String id, Map<String, dynamic> data);
  Future<Either<String, void>> deleteAdditionalGroup(String id);
}

class AdditionalRepositoryImpl implements AdditionalRepository {
  final AdditionalRemoteDataSource _dataSource;

  AdditionalRepositoryImpl(this._dataSource);

  @override
  Future<Either<String, List<AdditionalGroupModel>>> getAdditionalGroups() async {
    try {
      final result = await _dataSource.getAdditionalGroups();
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, AdditionalGroupModel>> createAdditionalGroup(Map<String, dynamic> data) async {
    try {
      final result = await _dataSource.createAdditionalGroup(data);
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, AdditionalGroupModel>> updateAdditionalGroup(String id, Map<String, dynamic> data) async {
    try {
      final result = await _dataSource.updateAdditionalGroup(id, data);
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, void>> deleteAdditionalGroup(String id) async {
    try {
      await _dataSource.deleteAdditionalGroup(id);
      return const Right(null);
    } catch (e) {
      return Left(e.toString());
    }
  }
}
