// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'connectivity_monitor.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(ConnectivityMonitor)
final connectivityMonitorProvider = ConnectivityMonitorProvider._();

final class ConnectivityMonitorProvider
    extends $NotifierProvider<ConnectivityMonitor, bool> {
  ConnectivityMonitorProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'connectivityMonitorProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$connectivityMonitorHash();

  @$internal
  @override
  ConnectivityMonitor create() => ConnectivityMonitor();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(bool value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<bool>(value),
    );
  }
}

String _$connectivityMonitorHash() =>
    r'0cdaeaff6fbb44f630e98eb7b378a7e7166e4d76';

abstract class _$ConnectivityMonitor extends $Notifier<bool> {
  bool build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref = this.ref as $Ref<bool, bool>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<bool, bool>,
              bool,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
