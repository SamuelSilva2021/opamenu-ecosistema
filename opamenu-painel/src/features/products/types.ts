export interface ProductAddonGroupResponse {
  id: string;
  productId: string;
  addonGroupId: string;
  addonGroup: {
    id: string;
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
  id: string;
  name: string;
  description?: string;
  price: number;
  categoryId: string;
  categoryName: string;
  imageUrl?: string;
  isActive: boolean;
  displayOrder: number;
  createdAt: string;
  updatedAt: string;
  addonGroups: ProductAddonGroupResponse[];
}

export interface AddProductAddonGroupRequest {
  addonGroupId: string;
  displayOrder: number;
  isRequired: boolean;
  minSelectionsOverride?: number;
  maxSelectionsOverride?: number;
}

export interface CreateProductRequest {
  name: string;
  description?: string;
  price: number;
  categoryId: string;
  imageUrl?: string;
  isActive: boolean;
  displayOrder: number;
  addonGroups?: AddProductAddonGroupRequest[];
}

export interface UpdateProductRequest {
  name: string;
  description?: string;
  price: number;
  categoryId: string;
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
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
}
