export interface Customer {
  id: string;
  name: string;
  phone: string;
  email?: string;
  postalCode?: string;
  street?: string;
  streetNumber?: string;
  neighborhood?: string;
  city?: string;
  state?: string;
  complement?: string;
  createdAt: string;
  updatedAt: string;
}
