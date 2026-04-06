import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:opamenu_garcom_mobile/src/app/opamenu_garcom_app.dart';

void main() {
  testWidgets('Mostra a tela de login', (tester) async {
    await tester.pumpWidget(const ProviderScope(child: OpaMenuGarcomApp()));
    await tester.pumpAndSettle();

    expect(find.text('Entrar'), findsAtLeastNWidgets(1));
    expect(find.text('Usuário ou e-mail'), findsOneWidget);
    expect(find.text('Senha'), findsOneWidget);
  });
}
