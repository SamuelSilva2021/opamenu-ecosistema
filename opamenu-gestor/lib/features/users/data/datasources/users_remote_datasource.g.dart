// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'users_remote_datasource.dart';

// **************************************************************************
// RiverpodGenerator
// **************************************************************************

// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint, type=warning

@ProviderFor(usersRemoteDataSource)
final usersRemoteDataSourceProvider = UsersRemoteDataSourceProvider._();

final class UsersRemoteDataSourceProvider
    extends
        $FunctionalProvider<
          UsersRemoteDataSource,
          UsersRemoteDataSource,
          UsersRemoteDataSource
        >
    with $Provider<UsersRemoteDataSource> {
  UsersRemoteDataSourceProvider._()
    : super(
        from: null,
        argument: null,
        retry: null,
        name: r'usersRemoteDataSourceProvider',
        isAutoDispose: true,
        dependencies: null,
        $allTransitiveDependencies: null,
      );

  @override
  String debugGetCreateSourceHash() => _$usersRemoteDataSourceHash();

  @$internal
  @override
  $ProviderElement<UsersRemoteDataSource> $createElement(
    $ProviderPointer pointer,
  ) => $ProviderElement(pointer);

  @override
  UsersRemoteDataSource create(Ref ref) {
    return usersRemoteDataSource(ref);
  }

  /// {@macro riverpod.override_with_value}
  Override overrideWithValue(UsersRemoteDataSource value) {
    return $ProviderOverride(
      origin: this,
      providerOverride: $SyncValueProvider<UsersRemoteDataSource>(value),
    );
  }
}

String _$usersRemoteDataSourceHash() =>
    r'ab79f94105048c8357dc4307ef3559387cff4e5d';
