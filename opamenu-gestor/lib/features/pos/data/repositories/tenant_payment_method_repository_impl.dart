import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/repositories/tenant_payment_method_repository.dart';
import '../datasources/tenant_payment_method_remote_datasource.dart';
import '../../domain/models/tenant_payment_method_model.dart';

part 'tenant_payment_method_repository_impl.g.dart';

@riverpod
TenantPaymentMethodRepository tenantPaymentMethodRepository(Ref ref) {
  return TenantPaymentMethodRepositoryImpl(ref.watch(tenantPaymentMethodRemoteDataSourceProvider));
}

class TenantPaymentMethodRepositoryImpl implements TenantPaymentMethodRepository {
  final TenantPaymentMethodRemoteDataSource _dataSource;

  TenantPaymentMethodRepositoryImpl(this._dataSource);

  @override
  Future<List<TenantPaymentMethodModel>> getPaymentMethods() {
    return _dataSource.getPaymentMethods();
  }
}
