
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../../data/models/login_request_model.dart';
import '../../domain/usecases/login_usecase.dart';

part 'auth_notifier.g.dart';

@riverpod
class AuthNotifier extends _$AuthNotifier {
  final _storage = const FlutterSecureStorage();

  @override
  AsyncValue<void> build() {
    return const AsyncValue.data(null);
  }

  Future<void> login(String email, String password) async {
    state = const AsyncValue.loading();

    final useCase = ref.read(loginUseCaseProvider);
    final result = await useCase(
      LoginRequestModel(usernameOrEmail: email, password: password),
    );

    result.fold(
      (error) {
        state = AsyncValue.error(error, StackTrace.current);
      },
      (response) async {
        await _storage.write(key: 'access_token', value: response.accessToken);
        await _storage.write(key: 'refresh_token', value: response.refreshToken);
        state = const AsyncValue.data(null);
      },
    );
  }

  Future<void> logout() async {
    await _storage.deleteAll();
    state = const AsyncValue.data(null);
  }
}
