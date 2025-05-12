export interface PaginatedProducts {
  data: Product[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface Product {
  id: number;
  name: string;
  img: string;
  productType: ProductTypeDisplay;
  colors: ColorDisplay[];
}

export interface Color {
  id: number;
  name: string;
  hex: string;
}

export interface ProductType {
  id: number;
  name: string;
}

export interface ProductDetails extends Product {
  description: string;
}

export interface CreateProductDTO {
  name: string;
  img: string;
  description: string;
  productTypeId: number;
  colors: number[];
}

export type ProductTypeDisplay = Omit<ProductType, "id">;
export type ColorDisplay = Omit<Color, "id">;