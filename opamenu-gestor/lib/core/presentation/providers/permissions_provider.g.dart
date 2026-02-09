// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'permissions_provider.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(permissions)
final permissionsProvider = PermissionsProvider._();

final class PermissionsProvider
    extends
        $FunctionalProvider<
          AsyncValue<List<PermissionModel>>,
          List<PermissionModel>,
          FutureOr<List<PermissionModel>>
        >
    with
        $FutureModifier<List<PermissionModel>>,
        $FutureProvider<List<PermissionModel>> {
  PermissionsProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'permissionsProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$permissionsHash();

  @$internal
  @override
  $FutureProviderElement<List<PermissionModel>> $createElement(
    $ProviderPointer pointer,
  ) => $FutureProviderElement(pointer);

  @override
  FutureOr<List<PermissionModel>> create(Ref ref) {
    return permissions(ref);
  }
}

String _$permissionsHash() => r'784345f41c967f5da82f0014d702a31b0817deea';

@ProviderFor(hasPermission)
final hasPermissionProvider = HasPermissionFamily._();

final class HasPermissionProvider extends $FunctionalProvider<bool, bool, bool>
    with $Provider<bool> {
  HasPermissionProvider._({
    required HasPermissionFamily super.from,
    required String super.argument,
  }) : super(
         retry: null,
         name: r'hasPermissionProvider',
         isAutoDispose: true,
         dependencies: null,
         $allTransitiveDependencies: null,
       );

  @override
  String debugGetCreateSourceHash() => _$hasPermissionHash();

  @override
  String toString() {
    return r'hasPermissionProvider'
        ''
        '($argument)';
  }

  @$internal
  @override
  $ProviderElement<bool> $createElement($ProviderPointer pointer) =>
      $ProviderElement(pointer);

  @override
  bool create(Ref ref) {
    final argument = this.argument as String;
    return hasPermission(ref, argument);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(bool value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<bool>(value),
    );
  }

  @override
  bool operator ==(Object other) {
    return other is HasPermissionProvider && other.argument == argument;
  }

  @override
  int get hashCode {
    return argument.hashCode;
  }
}

String _$hasPermissionHash() => r'ff591c6a78059fb2cda26190818f7370c110ac02';

final class HasPermissionFamily extends $Family
    with $FunctionalFamilyOverride<bool, String> {
  HasPermissionFamily._()
    : super(
        retry: null,
        name: r'hasPermissionProvider',
        dependencies: null,
        $allTransitiveDependencies: null,
        isAutoDispose: true,
      );

  HasPermissionProvider call(String permissionKey) =>
      HasPermissionProvider._(argument: permissionKey, from: this);

  @override
  String toString() => r'hasPermissionProvider';
}
