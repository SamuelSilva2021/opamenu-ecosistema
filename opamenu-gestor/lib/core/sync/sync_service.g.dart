// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'sync_service.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(SyncService)
final syncServiceProvider = SyncServiceProvider._();

final class SyncServiceProvider
    extends $NotifierProvider<SyncService, AsyncValue<SyncStatus>> {
  SyncServiceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'syncServiceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$syncServiceHash();

  @$internal
  @override
  SyncService create() => SyncService();

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(AsyncValue<SyncStatus> value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<AsyncValue<SyncStatus>>(value),
    );
  }
}

String _$syncServiceHash() => r'cca88a716537c364fec8a4d306fc3049a33f8e63';

abstract class _$SyncService extends $Notifier<AsyncValue<SyncStatus>> {
  AsyncValue<SyncStatus> build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref =
        this.ref as $Ref<AsyncValue<SyncStatus>, AsyncValue<SyncStatus>>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<AsyncValue<SyncStatus>, AsyncValue<SyncStatus>>,
              AsyncValue<SyncStatus>,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
