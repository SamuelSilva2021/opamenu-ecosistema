import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../domain/entities/product_entity.dart';
import '../controllers/catalog_controller.dart';

class CatalogControllerProvider {
  static final AsyncNotifierProvider<CatalogController, List<ProductEntity>> provider =
      AsyncNotifierProvider(CatalogController.new);
}

