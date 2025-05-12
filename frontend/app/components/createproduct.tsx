"use client";

import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { api } from "../services/api";
import { Color, CreateProductDTO, ProductType } from "../interfaces";
import { LoadingSpinner } from "./ui/loadingspinner";
import { ErrorMessage } from "./ui/errormessage";

export default function CreateProductForm() {
  const router = useRouter();
  const [productTypes, setProductTypes] = useState<ProductType[]>([]);
  const [colors, setColors] = useState<Color[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<{ [key: string]: string }>({});

  const [formData, setFormData] = useState<CreateProductDTO>({
    name: "",
    img: "",
    description: "",
    productTypeId: 0,
    colors: [],
  });

  useEffect(() => {
    loadFormData();
  }, []);

  async function loadFormData() {
    try {
      setLoading(true);
      const [types, colorsList] = await Promise.all([
        api.getProductTypes(),
        api.getColors(),
      ]);
      setProductTypes(types);
      setColors(colorsList);
    } catch (err) {
      setError("Failed to load form data");
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  function handleColorChange(colorId: number, checked: boolean) {
    setFormData((prev) => ({
      ...prev,
      colors: checked
        ? [...prev.colors, colorId]
        : prev.colors.filter((id) => id !== colorId),
    }));
  }

  function validateField(name: string, value: string): string | null {
    switch (name) {
      case "name":
        if (!value.trim()) return "Product name is required";
        if (value.length > 255)
          return "Product name must be 255 characters or less";
        return null;
      case "img":
        if (value.length > 500)
          return "Image URL must be 500 characters or less";
        //validate URL format
        if (value && !value.match(/^https?:\/\/.+/))
          return "Please enter a valid URL";
        return null;
      case "description":
        if (value.length > 700)
          return "Description must be 700 characters or less";
        return null;
      default:
        return null;
    }
  }

  function handleInputChange(field: string, value: string) {
    setFormData((prev) => ({ ...prev, [field]: value }));

    const error = validateField(field, value);
    setFieldErrors((prev) => ({
      ...prev,
      [field]: error || "",
    }));
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    setError(null);
    const errors: { [key: string]: string } = {};

    const nameError = validateField("name", formData.name);
    if (nameError) errors.name = nameError;

    const imgError = validateField("img", formData.img);
    if (imgError) errors.img = imgError;

    const descError = validateField("description", formData.description);
    if (descError) errors.description = descError;

    if (!formData.productTypeId) {
      errors.productType = "Please select a product type";
    }

    if (formData.colors.length === 0) {
      errors.colors = "Please select at least one color";
    }

    if (Object.keys(errors).length > 0) {
      setFieldErrors(errors);
      setError("Please fix the errors below");
      return;
    }

    try {
      setSubmitting(true);
      await api.createProduct(formData);
      router.push("/");
    } catch (err) {
      setError("Failed to create product");
      console.error(err);
    } finally {
      setSubmitting(false);
    }
  }

  if (loading) {
    return (
      <LoadingSpinner
        size="lg"
        message="Loading..."
        fullScreen
      ></LoadingSpinner>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="max-w-2xl mx-auto">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold">Create New Product</h1>
          <Link href="/" className="text-black-500 hover:font-bold">
            ← Back to Products
          </Link>
        </div>

        {error && (          
          <ErrorMessage message={error} />         
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label
              htmlFor="name"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              Product Name
            </label>
            <input
              type="text"
              id="name"
              value={formData.name}
              onChange={(e) => handleInputChange("name", e.target.value)}
              maxLength={255}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                fieldErrors.name ? "border-red-500" : "border-gray-300"
              }`}
              required
            />
            {fieldErrors.name && (
              <p className="mt-1 text-sm text-red-600">{fieldErrors.name}</p>
            )}
          </div>

          <div>
            <label
              htmlFor="productType"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              Product Type
            </label>
            <select
              id="productType"
              value={formData.productTypeId}
              onChange={(e) =>
                setFormData((prev) => ({
                  ...prev,
                  productTypeId: parseInt(e.target.value),
                }))
              }
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                fieldErrors.productType ? "border-red-500" : "border-gray-300"
              }`}
              required
            >
              <option value="0">Select a product type</option>
              {productTypes.map((type) => (
                <option key={type.id} value={type.id}>
                  {type.name}
                </option>
              ))}
            </select>
            {fieldErrors.productType && (
              <p className="mt-1 text-sm text-red-600">
                {fieldErrors.productType}
              </p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Colors
            </label>
            <div
              className={`space-y-2 border rounded-md p-4 ${
                fieldErrors.colors ? "border-red-500" : "border-gray-300"
              }`}
            >
              {colors.map((color) => (
                <label key={color.id} className="flex items-center">
                  <input
                    type="checkbox"
                    checked={formData.colors.includes(color.id)}
                    onChange={(e) =>
                      handleColorChange(color.id, e.target.checked)
                    }
                    className="mr-2"
                  />
                  <span>{color.name}</span>
                </label>
              ))}
            </div>
            {fieldErrors.colors && (
              <p className="mt-1 text-sm text-red-600">{fieldErrors.colors}</p>
            )}
          </div>

          <div>
            <label
              htmlFor="img"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              Image URL
            </label>
            <input
              type="url"
              id="img"
              value={formData.img}
              onChange={(e) => handleInputChange("img", e.target.value)}
              maxLength={500}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                fieldErrors.img ? "border-red-500" : "border-gray-300"
              }`}
            />
            {fieldErrors.img && (
              <p className="mt-1 text-sm text-red-600">{fieldErrors.img}</p>
            )}
            <p className="mt-1 text-xs text-gray-500">
              {formData.img.length}/500 characters
            </p>
          </div>

          <div>
            <label
              htmlFor="description"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              Description
            </label>
            <textarea
              rows={4}
              id="description"
              value={formData.description}
              onChange={(e) => handleInputChange("description", e.target.value)}
              maxLength={700}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                fieldErrors.description ? "border-red-500" : "border-gray-300"
              }`}
            />
            {fieldErrors.description && (
              <p className="mt-1 text-sm text-red-600">
                {fieldErrors.description}
              </p>
            )}
            <p className="mt-1 text-xs text-gray-500">
              {formData.description.length}/700 characters
            </p>
          </div>

          <div className="flex justify-end space-x-4">
            <Link
              href="/"
              className="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-50"
            >
              Cancel
            </Link>
            <button
              type="submit"
              disabled={submitting}
              className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {submitting ? "Creating..." : "Create Product"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
