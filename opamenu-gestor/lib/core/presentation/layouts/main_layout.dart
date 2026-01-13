import 'package:flutter/material.dart';
import 'package:opamenu_gestor/features/pos/presentation/widgets/pos_sidebar.dart';

class MainLayout extends StatelessWidget {
  final Widget child;

  const MainLayout({super.key, required this.child});

  @override
  Widget build(BuildContext context) {
    // Define breakpoint para desktop vs tablet/mobile
    final isDesktop = MediaQuery.of(context).size.width >= 900;

    return Scaffold(
      backgroundColor: const Color(0xFFF5F5F5),
      // No mobile, usamos um Drawer para a navegação
      drawer: !isDesktop ? const Drawer(child: PosSidebar()) : null,
      appBar: !isDesktop
          ? AppBar(
              backgroundColor: Colors.white,
              elevation: 0,
              iconTheme: const IconThemeData(color: Colors.black),
              title: const Text(
                'Opamenu Gestor',
                style: TextStyle(color: Colors.black),
              ),
            )
          : null,
      body: Row(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // No desktop, a sidebar é fixa
          if (isDesktop) const PosSidebar(),
          // O conteúdo principal ocupa o resto
          Expanded(child: child),
        ],
      ),
    );
  }
}
