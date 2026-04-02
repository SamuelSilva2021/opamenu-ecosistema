import 'dart:async';
import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../data/models/table_response_dto.dart';
import '../../data/models/table_layout_model.dart';
import '../controllers/tables_controller.dart'; 
import '../../data/repositories/tables_repository_impl.dart';

part 'table_map_notifier.g.dart';

class TableMapState {
  final List<TableResponseDto> tables;
  final Map<String, TableLayoutModel> layouts;

  TableMapState({required this.tables, required this.layouts});

  TableMapState copyWith({
    List<TableResponseDto>? tables,
    Map<String, TableLayoutModel>? layouts,
  }) {
    return TableMapState(
      tables: tables ?? this.tables,
      layouts: layouts ?? this.layouts,
    );
  }
}

@riverpod
class TableMapNotifier extends _$TableMapNotifier {
  static const _layoutKey = 'table_layouts_v1';

  @override
  FutureOr<TableMapState> build() async {
    final response = await ref.watch(tablesControllerProvider.future);
    final tables = response.data ?? [];

    Map<String, TableLayoutModel> layouts = {};
    for (var table in tables) {
      layouts[table.id] = TableLayoutModel(
        tableId: table.id,
        x: table.layoutX ?? 50,
        y: table.layoutY ?? 50,
        width: table.layoutWidth ?? 80,
        height: table.layoutHeight ?? 80,
        floor: table.floor,
      );
    }

    return TableMapState(tables: tables, layouts: layouts);
  }

  void moveTable(String tableId, Offset delta) {
    if (state.value == null) return;
    
    final currentLayouts = Map<String, TableLayoutModel>.from(state.value!.layouts);
    final layout = currentLayouts[tableId] ?? TableLayoutModel(tableId: tableId, x: 50, y: 50);

    // Simple clamping to prevent losing tables off-screen completely
    final newX = (layout.x + delta.dx).clamp(0.0, 3000.0);
    final newY = (layout.y + delta.dy).clamp(0.0, 3000.0);

    currentLayouts[tableId] = TableLayoutModel(
      tableId: tableId,
      x: newX,
      y: newY,
      width: layout.width,
      height: layout.height,
      floor: layout.floor,
    );

    state = AsyncData(state.value!.copyWith(layouts: currentLayouts));
  }

  Future<void> saveLayout() async {
    if (state.value == null) return;
    
    final payload = state.value!.layouts.values.map((layout) => {
      'tableId': layout.tableId,
      'layoutX': layout.x,
      'layoutY': layout.y,
      'layoutWidth': layout.width,
      'layoutHeight': layout.height,
      'floor': layout.floor,
    }).toList();

    try {
      final repository = ref.read(tablesRepositoryProvider);
      await repository.updateLayouts(payload);
    } catch (e) {
      // Falha ao salvar no backend, poderia fazer um fallback offline aqui
      debugPrint('Erro ao salvar layout no servidor: $e');
    }
  }
}
