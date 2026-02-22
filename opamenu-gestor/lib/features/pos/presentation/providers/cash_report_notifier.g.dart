// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_report_notifier.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(CashReportNotifier)
final cashReportProvider = CashReportNotifierProvider._();

final class CashReportNotifierProvider
    extends $AsyncNotifierProvider<CashReportNotifier, CashRegisterReportDto?> {
  CashReportNotifierProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'cashReportProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$cashReportNotifierHash();

  @$internal
  @override
  CashReportNotifier create() => CashReportNotifier();
}

String _$cashReportNotifierHash() =>
    r'd0d7c1750b3af52daf69ac1819b39c6b11997879';

abstract class _$CashReportNotifier
    extends $AsyncNotifier<CashRegisterReportDto?> {
  FutureOr<CashRegisterReportDto?> build();
  @$mustCallSuper
  @override
  void runBuild() {
    final ref =
        this.ref
            as $Ref<AsyncValue<CashRegisterReportDto?>, CashRegisterReportDto?>;
    final element =
        ref.element
            as $ClassProviderElement<
              AnyNotifier<
                AsyncValue<CashRegisterReportDto?>,
                CashRegisterReportDto?
              >,
              AsyncValue<CashRegisterReportDto?>,
              Object?,
              Object?
            >;
    element.handleCreate(ref, build);
  }
}
