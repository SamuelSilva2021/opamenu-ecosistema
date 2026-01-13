import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:opamenu_gestor/core/constants/app_strings.dart';
import 'package:opamenu_gestor/features/auth/presentation/pages/login_page.dart';

void main() {
  testWidgets('LoginPage renders correctly', (WidgetTester tester) async {
    // Build our app and trigger a frame.
    await tester.pumpWidget(
      const ProviderScope(
        child: MaterialApp(
          home: LoginPage(),
        ),
      ),
    );

    // Verify that login form is present
    expect(find.byType(LoginPage), findsOneWidget);
    expect(find.text(AppStrings.loginButton), findsOneWidget);
    expect(find.text(AppStrings.welcomeBack), findsOneWidget);
    expect(find.byIcon(Icons.email_outlined), findsOneWidget);
    expect(find.byIcon(Icons.lock_outline), findsOneWidget);
  });
}
