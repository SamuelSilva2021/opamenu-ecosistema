import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../datasources/customers_remote_datasource.dart';
import '../../domain/models/customer_response_dto.dart';

part 'customers_repository.g.dart';

@riverpod
CustomersRepository customersRepository(Ref ref) {
  return CustomersRepository(ref.watch(customersRemoteDataSourceProvider));
}

class CustomersRepository {
  final CustomersRemoteDataSource _datasource;

  CustomersRepository(this._datasource);

  Future<CustomerResponseDto?> getCustomerByPhone(String phone) async {
    final data = await _datasource.getCustomerByPhone(phone);
    if (data != null) {
      return CustomerResponseDto.fromJson(data);
    }
    return null;
  }
}
