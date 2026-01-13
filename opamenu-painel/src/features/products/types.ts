export interface ProductAddonGroupResponse {
  id: number;
  productId: number;
  addonGroupId: number;
  addonGroup: {
    id: number;
    name: string;
    description?: string;
    // Add other fields as needed based on AddonGroupResponseDto
  };
  displayOrder: number;
  isRequired: boolean;
  minSelectionsOverride?: number;
  maxSelectionsOverride?: number;
}

export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  categoryId: number;
  categoryName: string;
  imageUrl?: string;
  isActive: boolean;
  displayOrder: number;
  createdAt: string;
  updatedAt: string;
  addonGroups: ProductAddonGroupResponse[];
}

export interface AddProductAddonGroupRequest {
  addonGroupId: number;
  displayOrder: number;
  isRequired: boolean;
  minSelectionsOverride?: number;
  maxSelectionsOverride?: number;
}

export interface CreateProductRequest {
  name: string;
  description?: string;
  price: number;
  categoryId: number;
  imageUrl?: string;
  isActive: boolean;
  displayOrder: number;
  addonGroups?: AddProductAddonGroupRequest[];
}

export interface UpdateProductRequest {
  name: string;
  description?: string;
  price: number;
  categoryId: number;
  imageUrl?: string;
  isActive: boolean;
  displayOrder: number;
  addonGroups?: AddProductAddonGroupRequest[];
}

export interface UpdateProductAddonGroupRequest {
  displayOrder: number;
  isRequired: boolean;
  minSelectionsOverride?: number;
  maxSelectionsOverride?: number;
}

export interface ProductSearchRequest {
  searchTerm?: string;
  categoryId?: number;
  minPrice?: number;
  maxPrice?: number;
}
