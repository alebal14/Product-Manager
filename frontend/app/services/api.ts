import { Color, CreateProductDTO, PaginatedProducts, ProductDetails, ProductType } from "../interfaces";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL;

/**
 * Custom fetch wrapper with error handling
 */
async function fetchApi<T>(
  endpoint: string, 
  options?: RequestInit
): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;
  
  try {
    const response = await fetch(url, {
      ...options,
      headers: {
        "Content-Type": "application/json",
        ...options?.headers,
      },
    });

    if (!response.ok) {
      // Get error message from the response body
      let errorMessage;
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorData.error || `Error: ${response.status}`;
      } catch {
        errorMessage = `Error: ${response.status} ${response.statusText}`;
      }
      
      throw new Error(errorMessage);
    }

    return await response.json();
  } catch (error) {
    console.error(`API request failed for ${endpoint}:`, error);
    throw error;
  }
}

export const api = {
  async createProduct(product: CreateProductDTO): Promise<ProductDetails> {
    return fetchApi<ProductDetails>("/products", {
      method: "POST",
      body: JSON.stringify(product),
    });
  },

  async getProducts(page: number = 1, pageSize: number = 12): Promise<PaginatedProducts> {
    return fetchApi<PaginatedProducts>(`/products?page=${page}&pageSize=${pageSize}`);
  },

  async getProductById(id: number): Promise<ProductDetails> {
    return fetchApi<ProductDetails>(`/products/${id}`);
  },

  async getProductTypes(): Promise<ProductType[]> {
    return fetchApi<ProductType[]>("/product-types");
  },

  async getColors(): Promise<Color[]> {
    return fetchApi<Color[]>("/colors");
  },
};