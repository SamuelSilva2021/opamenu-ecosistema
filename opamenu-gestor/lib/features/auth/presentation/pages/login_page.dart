import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/constants/app_strings.dart';
import '../widgets/login_form.dart';
import '../widgets/social_login_buttons.dart';

class LoginPage extends StatelessWidget {
  const LoginPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth > 900) {
            return const _DesktopLayout();
          } else {
            return const _MobileLayout();
          }
        },
      ),
    );
  }
}

class _DesktopLayout extends StatelessWidget {
  const _DesktopLayout();

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        // Left Side - Illustration
        Expanded(
          flex: 5,
          child: Container(
            color: const Color(0xFFFFF0E6),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                // Placeholder for illustration
                Icon(
                  Icons.storefront_rounded,
                  size: 200,
                  color: AppColors.primary.withValues(alpha: 0.5),
                ),
                const SizedBox(height: 32),
                Text(
                  AppStrings.appName,
                  style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                    fontWeight: FontWeight.bold,
                    color: AppColors.primary,
                  ),
                ),
              ],
            ),
          ),
        ),
        // Right Side - Login Form
        const Expanded(
          flex: 4,
          child: Center(
            child: SingleChildScrollView(
              padding: EdgeInsets.symmetric(horizontal: 64),
              child: _LoginContent(),
            ),
          ),
        ),
      ],
    );
  }
}

class _MobileLayout extends StatelessWidget {
  const _MobileLayout();

  @override
  Widget build(BuildContext context) {
    return const Center(
      child: SingleChildScrollView(
        padding: EdgeInsets.all(24),
        child: _LoginContent(),
      ),
    );
  }
}

class _LoginContent extends StatelessWidget {
  const _LoginContent();

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          AppStrings.welcomeBack,
          style: Theme.of(context).textTheme.displaySmall?.copyWith(
            fontWeight: FontWeight.bold,
            color: AppColors.textPrimary,
          ),
        ),
        const SizedBox(height: 8),
        Text(
          AppStrings.loginSubtitle,
          style: Theme.of(context).textTheme.bodyLarge?.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
        const SizedBox(height: 48),
        const LoginForm(),
        const SizedBox(height: 32),
        // Row(
        //   children: [
        //     const Expanded(child: Divider(color: AppColors.inputBorder)),
        //     Padding(
        //       padding: const EdgeInsets.symmetric(horizontal: 16),
        //       child: Text(
        //         AppStrings.or,
        //         style: Theme.of(context).textTheme.bodyMedium?.copyWith(
        //           color: AppColors.textSecondary,
        //         ),
        //       ),
        //     ),
        //     const Expanded(child: Divider(color: AppColors.inputBorder)),
        //   ],
        // ),
        // const SizedBox(height: 32),
        // const SocialLoginButtons(),
      ],
    );
  }
}
