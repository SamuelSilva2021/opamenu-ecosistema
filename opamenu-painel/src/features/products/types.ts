export interface ProductAditionalGroupResponse {
  id: string;
  productId: string;
  aditionalGroupId: string;
  aditionalGroup: {
    id: string;
    name: string;
    description?: string;
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
  aditionalGroups: ProductAditionalGroupResponse[];
}

export interface AddProductAditionalGroupRequest {
  aditionalGroupId: string;
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
  aditionalGroups?: AddProductAditionalGroupRequest[];
}

export interface UpdateProductRequest {
  name: string;
  description?: string;
  price: number;
  categoryId: string;
  imageUrl?: string;
  isActive: boolean;
  displayOrder: number;
  aditionalGroups?: AddProductAditionalGroupRequest[];
}

export interface UpdateProductAditionalGroupRequest {
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
