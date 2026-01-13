import { httpClient } from './http-client';
import { API_ENDPOINTS } from '@/config/api';
import { ApiResponse, CustomerResponseDto } from '@/types/api';

export interface CreateCustomerRequestDto {
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
}

export interface UpdateCustomerRequestDto {
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
}

export const customerService = {
  getCustomer: async (slug: string, phoneNumber: string) => {
    // Note: API route is [HttpGet("customer/{phoneNumber}")]
    const url = `${API_ENDPOINTS.PUBLIC.CUSTOMER(slug)}/${encodeURIComponent(phoneNumber)}`;
    
    // Changing to get as per user instruction
    return httpClient.get<ApiResponse<CustomerResponseDto>>(url);
  },

  createCustomer: async (slug: string, data: CreateCustomerRequestDto) => {
    // Note: API route is [HttpPost("customer/create")]
    const url = `${API_ENDPOINTS.PUBLIC.CUSTOMER(slug)}/create`;
    return httpClient.post<ApiResponse<CustomerResponseDto>>(url, data);
  },

  updateCustomer: async (slug: string, data: UpdateCustomerRequestDto) => {
    // Note: API route assumed to be [HttpPut("customer/update")]
    const url = `${API_ENDPOINTS.PUBLIC.CUSTOMER(slug)}/update`;
    return httpClient.put<ApiResponse<CustomerResponseDto>>(url, data);
  }
};
