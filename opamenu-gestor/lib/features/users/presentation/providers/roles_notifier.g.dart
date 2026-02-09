// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'roles_notifier.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(roles)
final rolesProvider = RolesProvider._();

final class RolesProvider
    extends
        $FunctionalProvider<
          AsyncValue<List<RoleModel>>,
          List<RoleModel>,
          FutureOr<List<RoleModel>>
        >
    with $FutureModifier<List<RoleModel>>, $FutureProvider<List<RoleModel>> {
  RolesProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'rolesProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$rolesHash();

  @$internal
  @override
  $FutureProviderElement<List<RoleModel>> $createElement(
    $ProviderPointer pointer,
  ) => $FutureProviderElement(pointer);

  @override
  FutureOr<List<RoleModel>> create(Ref ref) {
    return roles(ref);
  }
}

String _$rolesHash() => r'47ae97f95b80434c811f88832dfc802477e67f71';
