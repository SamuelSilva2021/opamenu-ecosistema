import '../models/tenant_payment_method_model.dart';

abstract class TenantPaymentMethodRepository {
  Future<List<TenantPaymentMethodModel>> getPaymentMethods();
}
