
import 'package:fpdart/fpdart.dart';
import '../../data/models/login_request_model.dart';
import '../../data/models/login_response_model.dart';

abstract class AuthRepository {
  Future<Either<String, LoginResponseModel>> login(LoginRequestModel request);
}
