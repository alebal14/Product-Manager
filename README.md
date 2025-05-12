# Product-Manager
A simple REST API with frontend
# Product Management System

A full-stack web application for managing product inventory. This system allows you to create, view, and browse products.

## Features

- **Product Listing**: View all products
- **Product Details**: View detailed information about a specific product
- **Product Creation**: Add new products with name, type, colors, image URL, and description
- **Responsive Design**: Works on desktop and mobile devices

## Technologies Used

### Backend
- **.NET 8** with ASP.NET Core WebAPI
- **Entity Framework Core** for database 
- **PostgreSQL/Supabase** for data storage
- **Swagger/OpenAPI** for API documentation
- **xUnit** and **FluentAssertions** for unit testing

### Frontend
- **Next.js 15** with **React 19**
- **TypeScript** for type safety
- **Tailwind CSS** for styling

## Project Structure

The project is organized into two main directories:

```
/
├── backend/
│   └── ProductAPI/            # .NET WebAPI project
│       ├── Controllers/       # API endpoints
│       ├── Data/              # Database context and migrations
│       ├── DTOs/              # Data Transfer Objects
│       ├── Middlewares/       # Custom middleware components
│       ├── Models/            # Domain entities
│       └── ProductAPI.Tests/  # Unit tests
│
└── frontend/
    ├── app/                   # Next.js app directory
    │   ├── components/        # React components
    │   ├── interfaces/        # TypeScript interfaces
    │   ├── products/          # Product-related pages
    │   ├── create-product/    # Product creation page
    │   └── services/          # API service layer
    └── public/                # Static assets
```

## Setup & Installation

### Prerequisites

- .NET 8 SDK
- Node.js (v18+)
- PostgreSQL

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd backend/ProductAPI
   ```

2. Update the connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DatabaseConnection": "Host=localhost;Database=productdb;Username=your_username;Password=your_password"
   }
   ```

4. Run the backend API:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:7059` with Swagger documentation at `https://localhost:7059/swagger`.

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Create a `.env.local` file with the API URL:
   ```
   NEXT_PUBLIC_API_URL=https://localhost:7059/api
   ```

4. Run the development server:
   ```bash
   npm run dev
   ```

The frontend will be available at `http://localhost:3000`.

## Data Model

The system is built around these core entities:

- **Product**: Represents a product with name, description, image URL, and associated product type and colors
- **ProductType**: Categorizes products (e.g., Sofa, Chair, Table)
- **Color**: Available colors that can be associated with products

## Database Model
![alt text](image-4.png)

## API Endpoints

The API provides the following endpoints:

- `GET /api/products` - Retrieves a paginated list of products
- `GET /api/products/{id}` - Retrieves details for a specific product
- `POST /api/products` - Creates a new product
- `GET /api/product-types` - Retrieves all available product types
- `GET /api/colors` - Retrieves all available colors


## Screenshots

![alt text](image-1.png)
![alt text](image-2.png)
![alt text](image-3.png)

## Running Tests

To run the backend tests:

```bash
cd backend/ProductAPI.Tests
dotnet test
```