import Link from "next/link";
import ProductsListPage from "./components/productlistpage";

export default function Home() {
  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-4xl font-bold">Product Management System</h1>
        <Link
          href="/create-product"
          className="bg-blue-500 hover:bg-blue-600 text-white font-medium px-4 py-2 rounded-md transition-colors duration-200"
        >
          + Add New Product
        </Link>
      </div>
      <ProductsListPage />
    </div>
  );
}
