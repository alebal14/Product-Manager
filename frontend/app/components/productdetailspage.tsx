"use client";

import React, { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import Link from "next/link";
import { api } from "../services/api";
import { ProductDetails } from "../interfaces";
import { LoadingSpinner } from "./ui/loadingspinner";
import { ColorSwatch } from "./ui/colorswatch";
import { ErrorMessage } from "./ui/errormessage";

export default function ProductDetailsPage() {
  const params = useParams();
  const productId = params.id as string;

  const [product, setProduct] = useState<ProductDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (productId) {
      loadProduct(parseInt(productId));
    }
  }, [productId]);

  async function loadProduct(id: number) {
    try {
      setLoading(true);
      const productData = await api.getProductById(id);
      setProduct(productData);
    } catch (err) {
      
      if (err instanceof Error && err.message.includes("404")) {
        setError(
          "Product not found. The product may have been deleted or never existed."
        );
      } else {
        setError("Failed to load product details. Please try again later.");
      }

      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  if (loading) {
    return (
      <LoadingSpinner
        size="lg"
        message="Loading product details..."
        fullScreen
      ></LoadingSpinner>
    );
  }

  if (error) {
    return <ErrorMessage message={error} />;
  }

  if (!product) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="text-xl text-red-600 mb-4">{"Product not found"}</div>
          <Link href="/" className="text-black-500 hover:font-bold">
            ← Back to Products
          </Link>
          <span className="text-gray-300">|</span>
          <Link
            href="/create-product"
            className="text-black-500 hover:font-bold"
          >
            + Create New Product
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="max-w-3xl mx-auto">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold">Product Details</h1>
          <div className="flex items-center gap-4">
            <Link href="/" className="text-black-500 hover:font-bold">
              ← Back to Products
            </Link>
            <span className="text-gray-300">|</span>
            <Link
              href="/create-product"
              className="text-black-500 hover:font-bold"
            >
              + Create New Product
            </Link>
          </div>
        </div>

        <div className="bg-white shadow-lg rounded-lg overflow-hidden">
          <div className="w-full h-96">
            {product.img ? (
              <img
                src={product.img}
                alt={product.name}
                className="w-full h-full object-cover"
              />
            ) : (
              <div className="w-full h-full flex items-center justify-center bg-gray-100">
                <svg
                  className="w-20 h-20 text-gray-400"
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

          <div className="p-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <h2 className="text-xl font-semibold mb-4">
                  Basic Information
                </h2>
                <div className="space-y-3">
                  <div>
                    <label className="block text-sm font-medium text-gray-600">
                      Product ID
                    </label>
                    <p className="text-lg">{product.id}</p>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-600">
                      Product Name
                    </label>
                    <p className="text-lg">{product.name}</p>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-600">
                      Product Type
                    </label>
                    <p className="text-lg">{product.productType.name}</p>
                  </div>
                </div>
              </div>

              <div>
                <h2 className="text-xl font-semibold mb-4">Colors</h2>
                <div className="flex flex-wrap gap-2">
                  {product.colors.map((color) => (
                    <ColorSwatch color={color} key={color.hex} />
                  ))}
                </div>
                <div className="mt-6">
                  <h2 className="text-xl font-semibold mb-4">Description</h2>
                  <p className="text-gray-700">{product.description}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
