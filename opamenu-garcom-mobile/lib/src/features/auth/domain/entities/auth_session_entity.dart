import 'auth_tokens_entity.dart';
import 'user_info_entity.dart';

class AuthSessionEntity {
  final AuthTokensEntity tokens;
  final UserInfoEntity user;

  const AuthSessionEntity({
    required this.tokens,
    required this.user,
  });
}

