
import 'dart:convert';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:opamenu_gestor/core/presentation/providers/permissions_provider.dart';
import '../../data/models/login_request_model.dart';
import '../../data/models/user_info_model.dart';
import '../../data/repositories/auth_repository_impl.dart';
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
    print('Tentando login com: $email');
    final result = await useCase(
      LoginRequestModel(usernameOrEmail: email, password: password),
    );

    result.fold(
      (error) {
        print('Erro no login: $error');
        state = AsyncValue.error(error, StackTrace.current);
      },
      (response) async {
        print('Login bem-sucedido! Buscando permissões...');
        await _storage.write(key: 'access_token', value: response.accessToken);
        await _storage.write(key: 'refresh_token', value: response.refreshToken);
        
        final repository = ref.read(authRepositoryProvider);
        final userInfoResult = await repository.getUserInfo(token: response.accessToken);

        userInfoResult.fold(
          (error) {
            print('Erro ao buscar UserInfo: $error');
            state = AsyncValue.error('Erro ao obter permissões: $error', StackTrace.current);
          },
          (userInfo) async {
            final permissions = _flattenPermissions(userInfo);
            print('Permissões mapeadas: $permissions');
            
            await _storage.write(
              key: 'user_permissions', 
              value: jsonEncode(permissions),
            );

            print('Invalidando permissionsProvider...');
            ref.invalidate(permissionsProvider);
            state = const AsyncValue.data(null);
          },
        );
      },
    );
  }

  List<String> _flattenPermissions(UserInfoModel userInfo) {
    final permissions = <String>{};
    for (final group in userInfo.permissions.accessGroups) {
      for (final role in group.roles) {
        for (final module in role.modules) {
          if (module.operations.isEmpty) {
            permissions.add(module.key);
          } else {
            for (final op in module.operations) {
              permissions.add('${module.key}:$op');
            }
          }
        }
      }
    }
    return permissions.toList();
  }

  Future<void> logout() async {
    await _storage.deleteAll();
    state = const AsyncValue.data(null);
  }
}
