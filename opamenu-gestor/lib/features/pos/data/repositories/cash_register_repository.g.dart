// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_register_repository.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(cashRegisterRepository)
final cashRegisterRepositoryProvider = CashRegisterRepositoryProvider._();

final class CashRegisterRepositoryProvider
    extends
        $FunctionalProvider<
          CashRegisterRepository,
          CashRegisterRepository,
          CashRegisterRepository
        >
    with $Provider<CashRegisterRepository> {
  CashRegisterRepositoryProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'cashRegisterRepositoryProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$cashRegisterRepositoryHash();

  @$internal
  @override
  $ProviderElement<CashRegisterRepository> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  CashRegisterRepository create(Ref ref) {
    return cashRegisterRepository(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(CashRegisterRepository value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<CashRegisterRepository>(value),
    );
  }
}

String _$cashRegisterRepositoryHash() =>
    r'c810eb01314eeb9b419e0a6effd902770e9af7d8';
