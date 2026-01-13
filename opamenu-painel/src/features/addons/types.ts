export const AddonGroupType = {
  Single: 1,
  Multiple: 2,
} as const;

export type AddonGroupType = (typeof AddonGroupType)[keyof typeof AddonGroupType];

export interface Addon {
  id: number;
  name: string;
  description?: string;
  price: number;
  imageUrl?: string;
  displayOrder: number;
  addonGroupId: number;
  isActive: boolean;
}

export interface AddonGroup {
  id: number;
  name: string;
  description?: string;
  type: AddonGroupType;
  minSelections?: number;
  maxSelections?: number;
  isRequired: boolean;
  displayOrder: number;
  isActive: boolean;
  addons: Addon[];
}

export interface CreateAddonGroupRequest {
  name: string;
  description?: string;
  type: AddonGroupType;
  minSelections?: number;
  maxSelections?: number;
  isRequired: boolean;
  displayOrder: number;
  isActive: boolean;
}

export interface UpdateAddonGroupRequest {
  name: string;
  description?: string;
  type: AddonGroupType;
  minSelections?: number;
  maxSelections?: number;
  isRequired: boolean;
  displayOrder: number;
  isActive: boolean;
}

export interface CreateAddonRequest {
  name: string;
  description?: string;
  price: number;
  addonGroupId: number;
  displayOrder: number;
  isActive: boolean;
  imageUrl?: string;
}

export interface UpdateAddonRequest {
  name: string;
  description?: string;
  price: number;
  addonGroupId: number;
  displayOrder: number;
  isActive: boolean;
  imageUrl?: string;
}
