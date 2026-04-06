import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../domain/entities/auth_session_entity.dart';
import '../controllers/auth_controller.dart';

class AuthControllerProvider {
  static final AsyncNotifierProvider<AuthController, AuthSessionEntity?> provider =
      AsyncNotifierProvider(AuthController.new);
}

