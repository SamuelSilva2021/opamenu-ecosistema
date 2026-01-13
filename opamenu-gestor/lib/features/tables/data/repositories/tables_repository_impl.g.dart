// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tables_repository_impl.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(tablesRepository)
final tablesRepositoryProvider = TablesRepositoryProvider._();

final class TablesRepositoryProvider
    extends
        $FunctionalProvider<
          ITablesRepository,
          ITablesRepository,
          ITablesRepository
        >
    with $Provider<ITablesRepository> {
  TablesRepositoryProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'tablesRepositoryProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$tablesRepositoryHash();

  @$internal
  @override
  $ProviderElement<ITablesRepository> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  ITablesRepository create(Ref ref) {
    return tablesRepository(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(ITablesRepository value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<ITablesRepository>(value),
    );
  }
}

String _$tablesRepositoryHash() => r'279a33d2cbf942492565d75aa6fdc74d1bd9c81f';
