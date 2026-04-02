// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'print_hub_service.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(PrintHubService)
final printHubServiceProvider = PrintHubServiceProvider._();

final class PrintHubServiceProvider
    extends $NotifierProvider<PrintHubService, void> {
  PrintHubServiceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'printHubServiceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$printHubServiceHash();

  @$internal
  @override
  PrintHubService create() => PrintHubService();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(void value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<void>(value),
    );
  }
}

String _$printHubServiceHash() => r'b16f41623e94901f88dd3154f53abecf3b711b49';

abstract class _$PrintHubService extends $Notifier<void> {
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
