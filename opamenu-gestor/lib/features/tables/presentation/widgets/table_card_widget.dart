import 'package:flutter/material.dart';
import '../../data/models/table_response_dto.dart';
import '../../data/models/table_layout_model.dart';
import 'package:go_router/go_router.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class TableCardWidget extends ConsumerWidget {
  final TableResponseDto table;
  final TableLayoutModel layout;
  final VoidCallback onTap;
  final Function(DragUpdateDetails) onDragUpdate;

  const TableCardWidget({
    Key? key,
    required this.table,
    required this.layout,
    required this.onTap,
    required this.onDragUpdate,
  }) : super(key: key);

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Positioned(
      left: layout.x,
      top: layout.y,
      child: GestureDetector(
        onTap: onTap,
        onPanUpdate: onDragUpdate,
        child: AnimatedContainer(
          duration: const Duration(milliseconds: 50), // short duration for smooth drag
          width: layout.width,
          height: layout.height,
          decoration: BoxDecoration(
            color: _colorForStatus(true), // TODO: integrate real status (open/closed)
            borderRadius: BorderRadius.circular(16),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withOpacity(0.1),
                blurRadius: 10,
                offset: const Offset(0, 4),
              ),
            ],
            border: Border.all(color: Colors.white.withOpacity(0.2), width: 1),
          ),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(
                'Mesa ${table.name}',
                style: const TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                  fontSize: 14,
                ),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 4),
              Text(
                '${table.capacity} lugares',
                style: const TextStyle(
                  color: Colors.white70,
                  fontSize: 11,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Color _colorForStatus(bool isFree) {
    // Premium looking colors
    return isFree ? const Color(0xFF10B981) : const Color(0xFFEF4444);
  }
}
