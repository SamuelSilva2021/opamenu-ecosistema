import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/core/presentation/providers/realtime_provider.dart';
import 'package:opamenu_gestor/features/pos/presentation/widgets/pos_sidebar.dart';

class MainLayout extends ConsumerStatefulWidget {
  final Widget child;

  const MainLayout({super.key, required this.child});

  @override
  ConsumerState<MainLayout> createState() => _MainLayoutState();
}

class _MainLayoutState extends ConsumerState<MainLayout> {
  @override
  void initState() {
    super.initState();
    // Inicializa conexão com SignalR ao entrar no layout principal (área logada)
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref.read(signalRServiceProvider).connect();
    });
  }

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
          Expanded(child: widget.child),
        ],
      ),
    );
  }
}
