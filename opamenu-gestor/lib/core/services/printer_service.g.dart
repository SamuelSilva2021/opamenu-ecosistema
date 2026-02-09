// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'printer_service.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(PrinterService)
final printerServiceProvider = PrinterServiceProvider._();

final class PrinterServiceProvider
    extends $NotifierProvider<PrinterService, void> {
  PrinterServiceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'printerServiceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$printerServiceHash();

  @$internal
  @override
  PrinterService create() => PrinterService();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(void value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<void>(value),
    );
  }
}

String _$printerServiceHash() => r'60283bb678495b968e4e8e594a68ba082aa8d3ec';

abstract class _$PrinterService extends $Notifier<void> {
  void build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref = this.ref as $Ref<void, void>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<void, void>,
              void,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
