export const AditionalGroupType = {
  Single: 1,
  Multiple: 2,
} as const;

export type AditionalGroupType = (typeof AditionalGroupType)[keyof typeof AditionalGroupType];

export interface Aditional {
  id: string;
  name: string;
  description?: string;
  price: number;
  imageUrl?: string;
  displayOrder: number;
  aditionalGroupId: string;
  isActive: boolean;
}

export interface AditionalGroup {
  id: string;
  name: string;
  description?: string;
  type: AditionalGroupType;
  minSelections?: number;
  maxSelections?: number;
  isRequired: boolean;
  displayOrder: number;
  isActive: boolean;
  aditionals: Aditional[];
}

export interface CreateAditionalGroupRequest {
  name: string;
  description?: string;
  type: AditionalGroupType;
  minSelections?: number;
  maxSelections?: number;
  isRequired: boolean;
  displayOrder: number;
  isActive: boolean;
}

export interface UpdateAditionalGroupRequest {
  name: string;
  description?: string;
  type: AditionalGroupType;
  minSelections?: number;
  maxSelections?: number;
  isRequired: boolean;
  displayOrder: number;
  isActive: boolean;
}

export interface CreateAditionalRequest {
  name: string;
  description?: string;
  price: number;
  aditionalGroupId: string;
  displayOrder: number;
  isActive: boolean;
  imageUrl?: string;
}

export interface UpdateAditionalRequest {
  name: string;
  description?: string;
  price: number;
  aditionalGroupId: string;
  displayOrder: number;
  isActive: boolean;
  imageUrl?: string;
}
