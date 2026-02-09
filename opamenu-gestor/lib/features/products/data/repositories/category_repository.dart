
import 'package:fpdart/fpdart.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../datasources/category_remote_datasource.dart';
import '../../domain/models/category_model.dart';

part 'category_repository.g.dart';

@riverpod
CategoryRepository categoryRepository(Ref ref) {
  return CategoryRepository(ref.watch(categoryRemoteDataSourceProvider));
}

class CategoryRepository {
  final CategoryRemoteDataSource _dataSource;

  CategoryRepository(this._dataSource);

  Future<Either<String, List<CategoryModel>>> getCategories() async {
    try {
      final categories = await _dataSource.getCategories();
      return Right(categories);
    } catch (e) {
      return Left(e.toString());
    }
  }

  Future<Either<String, CategoryModel>> createCategory(Map<String, dynamic> data) async {
    try {
      final category = await _dataSource.createCategory(data);
      return Right(category);
    } catch (e) {
      return Left(e.toString());
    }
  }

  Future<Either<String, CategoryModel>> updateCategory(String id, Map<String, dynamic> data) async {
    try {
      final category = await _dataSource.updateCategory(id, data);
      return Right(category);
    } catch (e) {
      return Left(e.toString());
    }
  }

  Future<Either<String, void>> deleteCategory(String id) async {
    try {
      await _dataSource.deleteCategory(id);
      return const Right(null);
    } catch (e) {
      return Left(e.toString());
    }
  }
}
