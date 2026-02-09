// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'additional_notifier.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(AdditionalNotifier)
final additionalProvider = AdditionalNotifierProvider._();

final class AdditionalNotifierProvider
    extends
        $AsyncNotifierProvider<AdditionalNotifier, List<AdditionalGroupModel>> {
  AdditionalNotifierProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'additionalProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$additionalNotifierHash();

  @$internal
  @override
  AdditionalNotifier create() => AdditionalNotifier();
}

String _$additionalNotifierHash() =>
    r'143d86b7610e8c1e54fa2b1f99f8b0db5af6acab';

abstract class _$AdditionalNotifier
    extends $AsyncNotifier<List<AdditionalGroupModel>> {
  FutureOr<List<AdditionalGroupModel>> build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref =
        this.ref
            as $Ref<
              AsyncValue<List<AdditionalGroupModel>>,
              List<AdditionalGroupModel>
            >;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<
                AsyncValue<List<AdditionalGroupModel>>,
                List<AdditionalGroupModel>
              >,
              AsyncValue<List<AdditionalGroupModel>>,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
