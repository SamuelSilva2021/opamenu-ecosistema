// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'collaborator_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CollaboratorModel _$CollaboratorModelFromJson(Map<String, dynamic> json) =>
    CollaboratorModel(
      id: json['id'] as String,
      name: json['name'] as String,
      phone: json['phone'] as String?,
      type: (json['type'] as num).toInt(),
      role: json['role'] as String?,
      active: json['active'] as bool,
      userAccountId: json['userAccountId'] as String?,
      tenantId: json['tenantId'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
    );

Map<String, dynamic> _$CollaboratorModelToJson(CollaboratorModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'phone': instance.phone,
      'type': instance.type,
      'role': instance.role,
      'active': instance.active,
      'userAccountId': instance.userAccountId,
      'tenantId': instance.tenantId,
      'createdAt': instance.createdAt.toIso8601String(),
    };
