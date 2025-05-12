import React from "react";
import { Product } from "../interfaces";
import Link from "next/link";
import { ColorSwatch } from "./ui/colorswatch";

interface ProductCardProps {
  product: Product;
}

export function ProductCard({ product }: ProductCardProps) {
  return (
    <Link href={`/products/${product.id}`} className="block">
      <div className="w-full max-w-sm border border-gray-200 rounded-lg shadow-md hover:shadow-2xl transition-shadow duration-300 bg-white dark:bg-gray-800 dark:border-gray-70">
        <div className="w-full h-60 bg-gray-200 dark:bg-gray-700 rounded-t-lg overflow-hidden">
          {product.img ? (
            <img
              src={product.img}
              alt={product.name}
              className="w-full h-full object-cover"
            />
          ) : (
            <div className="w-full h-full flex items-center justify-center text-gray-400">
              <svg
                className="w-20 h-20"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  fillRule="evenodd"
                  d="M4 3a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V5a2 2 0 00-2-2H4zm12 12H4l4-8 3 6 2-4 3 6z"
                  clipRule="evenodd"
                />
              </svg>
            </div>
          )}
        </div>

        {/* Product Info */}
        <div className="p-5">
          <h5 className="text-xl font-semibold tracking-tight text-gray-900 dark:text-white mb-2">
            {product.name}
          </h5>

          <p className="text-sm text-gray-600 dark:text-gray-400 mb-3">
            {product.productType.name}
          </p>

          {/* Colors */}
          <div className="flex flex-wrap gap-2">
            {product.colors.map((color) => (
              <ColorSwatch color={color} key={color.hex} />
            ))}
          </div>
        </div>
      </div>
    </Link>
  );
}
