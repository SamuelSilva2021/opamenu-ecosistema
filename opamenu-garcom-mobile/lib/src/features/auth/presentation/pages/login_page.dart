import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/app_colors.dart';
import '../../../../core/domain/failures/failure.dart';
import '../providers/auth_controller_provider.dart';

final _usernameControllerProvider = Provider.autoDispose<TextEditingController>((ref) {
  final controller = TextEditingController();
  ref.onDispose(controller.dispose);
  return controller;
});

final _passwordControllerProvider = Provider.autoDispose<TextEditingController>((ref) {
  final controller = TextEditingController();
  ref.onDispose(controller.dispose);
  return controller;
});

final _passwordVisibleNotifierProvider = Provider.autoDispose<ValueNotifier<bool>>((ref) {
  final notifier = ValueNotifier(false);
  ref.onDispose(notifier.dispose);
  return notifier;
});

class LoginPage extends ConsumerWidget {
  const LoginPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final usernameController = ref.watch(_usernameControllerProvider);
    final passwordController = ref.watch(_passwordControllerProvider);
    final passwordVisibleNotifier = ref.watch(_passwordVisibleNotifierProvider);

    final authState = ref.watch(AuthControllerProvider.provider);
    final isLoading = authState.isLoading;
    final errorMessage = authState.whenOrNull(
      error: (error, _) {
        if (error is Failure) return error.message;
        return 'Falha ao autenticar';
      },
    );

    return Scaffold(
      backgroundColor: AppColors.surface,
      body: Center(
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 420),
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Card(
              child: Padding(
                padding: const EdgeInsets.all(20),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Text(
                      'Entrar',
                      style: Theme.of(context).textTheme.headlineSmall,
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 16),
                    if (errorMessage != null && errorMessage.isNotEmpty) ...[
                      Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: Colors.red.withValues(alpha: 0.08),
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: Text(
                          errorMessage,
                          style: const TextStyle(color: Colors.red),
                        ),
                      ),
                      const SizedBox(height: 12),
                    ],
                    TextField(
                      controller: usernameController,
                      enabled: !isLoading,
                      decoration: const InputDecoration(
                        labelText: 'Usuário ou e-mail',
                        border: OutlineInputBorder(),
                      ),
                      textInputAction: TextInputAction.next,
                      autofillHints: const [AutofillHints.username, AutofillHints.email],
                    ),
                    const SizedBox(height: 12),
                    ValueListenableBuilder<bool>(
                      valueListenable: passwordVisibleNotifier,
                      builder: (context, passwordVisible, child) {
                        return TextField(
                          controller: passwordController,
                          enabled: !isLoading,
                          decoration: InputDecoration(
                            labelText: 'Senha',
                            border: const OutlineInputBorder(),
                            suffixIcon: IconButton(
                              onPressed: isLoading
                                  ? null
                                  : () => passwordVisibleNotifier.value = !passwordVisible,
                              icon: Icon(
                                passwordVisible ? Icons.visibility_off : Icons.visibility,
                              ),
                            ),
                          ),
                          obscureText: !passwordVisible,
                          textInputAction: TextInputAction.done,
                          onSubmitted: isLoading
                              ? null
                              : (_) => _submit(
                                    ref,
                                    usernameController.text,
                                    passwordController.text,
                                  ),
                          autofillHints: const [AutofillHints.password],
                        );
                      },
                    ),
                    const SizedBox(height: 16),
                    FilledButton(
                      onPressed: isLoading
                          ? null
                          : () => _submit(ref, usernameController.text, passwordController.text),
                      child: isLoading
                          ? const SizedBox(
                              height: 18,
                              width: 18,
                              child: CircularProgressIndicator(strokeWidth: 2),
                            )
                          : const Text('Entrar'),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  void _submit(WidgetRef ref, String usernameOrEmail, String password) {
    ref.read(AuthControllerProvider.provider.notifier).signIn(
          usernameOrEmail: usernameOrEmail,
          password: password,
        );
  }
}
