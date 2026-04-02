import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../data/models/table_layout_model.dart';
import '../notifiers/table_map_notifier.dart';
import '../widgets/table_card_widget.dart';

class TableMapPage extends ConsumerWidget {
  const TableMapPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final tablesAsync = ref.watch(tableMapProvider);

    return Scaffold(
      backgroundColor: const Color(0xFF0F172A), // Slate 900 for premium dark mode
      appBar: AppBar(
        title: const Text('Visão de Salão', style: TextStyle(fontWeight: FontWeight.w600)),
        backgroundColor: const Color(0xFF1E293B), // Slate 800
        actions: [
          IconButton(
            icon: const Icon(Icons.save_rounded),
            tooltip: 'Salvar Layout',
            onPressed: () {
              ref.read(tableMapProvider.notifier).saveLayout();
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text('Layout salvo com sucesso!')),
              );
            },
          ),
        ],
      ),
      body: tablesAsync.when(
        loading: () => const Center(child: CircularProgressIndicator(color: Colors.white)),
        error: (e, st) => Center(child: Text('Erro ao carregar mesas: $e', style: const TextStyle(color: Colors.redAccent))),
        data: (state) {
          if (state.tables.isEmpty) {
            return const Center(child: Text('Nenhuma mesa encontrada.', style: TextStyle(color: Colors.white70)));
          }
          
          return Stack(
            children: [
              // Grid background pattern
              Positioned.fill(
                child: CustomPaint(
                  painter: _GridPainter(),
                ),
              ),
              
              // Tables
              ...state.tables.map((t) {
                final layout = state.layouts[t.id] ??
                    TableLayoutModel(tableId: t.id, x: 50, y: 50);

                return TableCardWidget(
                  table: t,
                  layout: layout,
                  onTap: () {
                    // TODO: Navigate to table specific orders page
                  },
                  onDragUpdate: (d) => ref
                      .read(tableMapProvider.notifier)
                      .moveTable(t.id, d.delta),
                );
              }),
            ],
          );
        },
      ),
    );
  }
}

class _GridPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = Colors.white.withOpacity(0.05)
      ..strokeWidth = 1;

    const double spacing = 40.0;

    for (double i = 0; i < size.width; i += spacing) {
      canvas.drawLine(Offset(i, 0), Offset(i, size.height), paint);
    }

    for (double i = 0; i < size.height; i += spacing) {
      canvas.drawLine(Offset(0, i), Offset(size.width, i), paint);
    }
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
