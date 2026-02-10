import 'dart:io';
import 'package:image/image.dart' as img;
import 'package:path_provider/path_provider.dart';

class ImageCompressor {
  static Future<File?> compressFile(
    File file, {
    int maxBytes = 5 * 1024 * 1024,
    int initialJpegQuality = 95,
    int minJpegQuality = 60,
    int maxDimension = 4096,
  }) async {
    try {
      final originalSize = await file.length();
      if (originalSize <= maxBytes) return file;

      final bytes = await file.readAsBytes();
      final decoded = img.decodeImage(bytes);
      if (decoded == null) return file;

      final image = img.bakeOrientation(decoded);

      final hasAlpha = image.channels == img.Channels.rgba;

      img.Image working = image;
      int longestSide = working.width > working.height ? working.width : working.height;
      int targetLongest = longestSide.clamp(1, maxDimension);

      List<int> outBytes;
      String ext;

      if (!hasAlpha) {
        var quality = initialJpegQuality;
        outBytes = img.encodeJpg(working, quality: quality);
        while (outBytes.length > maxBytes) {
          if (quality > minJpegQuality) {
            quality = (quality - 10).clamp(minJpegQuality, initialJpegQuality);
            outBytes = img.encodeJpg(working, quality: quality);
          } else {
            if (targetLongest > 1024) {
              targetLongest = (targetLongest * 0.85).round();
              working = _resizeToLongest(working, targetLongest);
              outBytes = img.encodeJpg(working, quality: quality);
            } else {
              break;
            }
          }
        }
        ext = 'jpg';
      } else {
        outBytes = img.encodePng(working, level: 6);
        while (outBytes.length > maxBytes && targetLongest > 512) {
          targetLongest = (targetLongest * 0.85).round();
          working = _resizeToLongest(working, targetLongest);
          outBytes = img.encodePng(working, level: 6);
        }

        if (outBytes.length > maxBytes) {
          final flattened = _flattenOnColor(working, img.getColor(255, 255, 255));
          var quality = initialJpegQuality;
          outBytes = img.encodeJpg(flattened, quality: quality);
          while (outBytes.length > maxBytes) {
            if (quality > minJpegQuality) {
              quality = (quality - 10).clamp(minJpegQuality, initialJpegQuality);
              outBytes = img.encodeJpg(flattened, quality: quality);
            } else {
              if (targetLongest > 512) {
                targetLongest = (targetLongest * 0.85).round();
                final resized = _resizeToLongest(flattened, targetLongest);
                outBytes = img.encodeJpg(resized, quality: quality);
              } else {
                break;
              }
            }
          }
          ext = 'jpg';
        } else {
          ext = 'png';
        }
      }

      final tempDir = await getTemporaryDirectory();
      final outFile = File("${tempDir.path}/compressed_${DateTime.now().millisecondsSinceEpoch}.$ext");
      await outFile.writeAsBytes(outBytes);
      return outFile;
    } catch (_) {
      return file;
    }
  }

  static img.Image _resizeToLongest(img.Image src, int targetLongestSide) {
    final w = src.width;
    final h = src.height;
    if (w >= h) {
      return img.copyResize(src, width: targetLongestSide);
    } else {
      return img.copyResize(src, height: targetLongestSide);
    }
  }

  static img.Image _flattenOnColor(img.Image src, int color) {
    final canvas = img.Image(src.width, src.height);
    img.fillRect(canvas, 0, 0, src.width, src.height, color);
    img.copyInto(canvas, src);
    return canvas;
  }
}
