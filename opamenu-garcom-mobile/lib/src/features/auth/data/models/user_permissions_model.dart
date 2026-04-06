import '../../../../core/data/json/json_map_reader.dart';
import '../../domain/entities/user_permissions_entity.dart';
import 'access_group_basic_model.dart';

class UserPermissionsModel {
  final String userId;
  final List<AccessGroupBasicModel> accessGroups;

  const UserPermissionsModel({
    required this.userId,
    required this.accessGroups,
  });

  factory UserPermissionsModel.fromJson(Map<String, Object?> json) {
    final reader = JsonMapReader(json);
    return UserPermissionsModel(
      userId: reader.string('userId'),
      accessGroups: reader
          .listOrEmpty('accessGroups')
          .whereType<Map>()
          .map((e) => AccessGroupBasicModel.fromJson(e.cast<String, Object?>()))
          .toList(growable: false),
    );
  }

  UserPermissionsEntity toEntity() {
    return UserPermissionsEntity(
      userId: userId,
      accessGroups: accessGroups.map((e) => e.toEntity()).toList(growable: false),
    );
  }
}

