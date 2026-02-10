import 'dart:io';
import 'package:image/image.dart' as img;
import 'package:path_provider/path_provider.dart';

class ImageCompressor {
  /// Comprime o arquivo de imagem mantendo uma boa relação qualidade/tamanho.
  /// Retorna um novo [File] comprimido ou null se falhar.
  static Future<File?> compressFile(File file) async {
    try {
      final tempDir = await getTemporaryDirectory();
      final outPath = "${tempDir.path}/compressed_${DateTime.now().millisecondsSinceEpoch}.jpg";

      // 1. Ler os bytes da imagem
      final bytes = await file.readAsBytes();
      
      // 2. Decodificar a imagem
      final image = img.decodeImage(bytes);
      if (image == null) {
        return file;
      }

      // 3. Redimensionar se necessário
      var resized = image;
      if (image.width > 1024 || image.height > 1024) {
        int? newWidth;
        int? newHeight;

        if (image.width >= image.height) {
          newWidth = 1024;
        } else {
          newHeight = 1024;
        }

        resized = img.copyResize(
          image, 
          width: newWidth,
          height: newHeight,
        );
      }

      // 4. Codificar para JPG com qualidade 80
      final jpgBytes = img.encodeJpg(resized, quality: 80);

      // 5. Salvar o arquivo
      final compressedFile = File(outPath);
      await compressedFile.writeAsBytes(jpgBytes);

      return compressedFile;
    } catch (e) {
      return file;
    }
  }
}