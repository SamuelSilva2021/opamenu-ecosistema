import 'package:flutter_pos_printer_platform/flutter_pos_printer_platform.dart';
import 'package:esc_pos_utils_plus/esc_pos_utils.dart' as esc;
import 'package:riverpod_annotation/riverpod_annotation.dart';

part 'printer_service.g.dart';

enum PrinterConnectionType { usb, network }

enum PaperSize {
  mm58,
  mm80;

  int get width {
    switch (this) {
      case PaperSize.mm58:
        return 32;
      case PaperSize.mm80:
        return 42;
    }
  }

  esc.PaperSize get posSize {
    switch (this) {
      case PaperSize.mm58:
        return esc.PaperSize.mm58;
      case PaperSize.mm80:
        return esc.PaperSize.mm80;
    }
  }
}

class PrinterDeviceInfo {
  final String name;
  final String address;
  final PrinterConnectionType type;
  final String? deviceId;

  PrinterDeviceInfo({
    required this.name,
    required this.address,
    required this.type,
    this.deviceId,
  });
}

@riverpod
class PrinterService extends _$PrinterService {
  @override
  void build() {}

  final _printerManager = PrinterManager.instance;

  Future<List<PrinterDeviceInfo>> discoverPrinters(PrinterConnectionType type) async {
    final List<PrinterDeviceInfo> devices = [];

    if (type == PrinterConnectionType.usb) {
      // Listen to the discovery stream for a specific duration
      final usbDevicesStream = _printerManager.discovery(type: PrinterType.usb);
      final subscription = usbDevicesStream.listen((device) {
        devices.add(PrinterDeviceInfo(
          name: device.name,
          address: device.address ?? '',
          type: PrinterConnectionType.usb,
        ));
      });
      
      // Wait for 2 seconds to discover devices
      await Future.delayed(const Duration(seconds: 2));
      await subscription.cancel();
    }
    
    return devices;
  }

  Future<bool> printReceipt(
    PrinterDeviceInfo device,
    List<int> bytes,
  ) async {
    try {
      if (device.type == PrinterConnectionType.network) {
        return await _printerManager.connect(
          type: PrinterType.network,
          model: TcpPrinterInput(ipAddress: device.address),
        );
      } else if (device.type == PrinterConnectionType.usb) {
        return await _printerManager.connect(
          type: PrinterType.usb,
          model: UsbPrinterInput(name: device.name),
        );
      }
      return false;
    } catch (e) {
      return false;
    }
  }

  Future<List<int>> generateTestReceipt(PaperSize paperSize) async {
    final profile = await esc.CapabilityProfile.load();
    final generator = esc.Generator(paperSize.posSize, profile);
    List<int> bytes = [];

    bytes += generator.text('OPAMENU GESTOR',
        styles: const esc.PosStyles(align: esc.PosAlign.center, bold: true, height: esc.PosTextSize.size2, width: esc.PosTextSize.size2));
    bytes += generator.text('Teste de Impressao', styles: const esc.PosStyles(align: esc.PosAlign.center));
    bytes += generator.feed(2);
    bytes += generator.cut();

    return bytes;
  }
}
