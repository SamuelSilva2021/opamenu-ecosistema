// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_register_notifier.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(CashRegisterNotifier)
final cashRegisterProvider = CashRegisterNotifierProvider._();

final class CashRegisterNotifierProvider
    extends
        $AsyncNotifierProvider<CashRegisterNotifier, CashShiftResponseDto?> {
  CashRegisterNotifierProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'cashRegisterProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$cashRegisterNotifierHash();

  @$internal
  @override
  CashRegisterNotifier create() => CashRegisterNotifier();
}

String _$cashRegisterNotifierHash() =>
    r'9025a564cbef9646f923ddf0ac6d7a5315afe3d6';

abstract class _$CashRegisterNotifier
    extends $AsyncNotifier<CashShiftResponseDto?> {
  FutureOr<CashShiftResponseDto?> build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref =
        this.ref
            as $Ref<AsyncValue<CashShiftResponseDto?>, CashShiftResponseDto?>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<
                AsyncValue<CashShiftResponseDto?>,
                CashShiftResponseDto?
              >,
              AsyncValue<CashShiftResponseDto?>,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
