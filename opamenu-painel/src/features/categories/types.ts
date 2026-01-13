export interface Category {
  id: number;
  name: string;
  description?: string;
  displayOrder: number;
  isActive: boolean;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string;
  displayOrder: number;
  isActive: boolean;
}

export interface UpdateCategoryRequest {
  name: string;
  description?: string;
  displayOrder: number;
  isActive: boolean;
}
