
import 'package:json_annotation/json_annotation.dart';

part 'login_request_model.g.dart';

@JsonSerializable(createFactory: false)
class LoginRequestModel {
  @JsonKey(name: 'usernameOrEmail')
  final String usernameOrEmail;
  final String password;

  const LoginRequestModel({
    required this.usernameOrEmail,
    required this.password,
  });

  Map<String, dynamic> toJson() => _$LoginRequestModelToJson(this);
}
