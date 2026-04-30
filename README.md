# ECommerceApp

Backend-first rewrite for an AI-powered ecommerce platform.

Current status:

- Backend: ASP.NET Core 10 Web API
- Database: SQL Server with Windows authentication
- Frontend: React 19 with Vite
- AI/recommendation/forecasting: planned for the final phase

## Backend Stack

- ASP.NET Core 10
- Entity Framework Core 10
- SQL Server
- ASP.NET Core Identity API endpoints
- Swashbuckle Swagger

## Frontend Stack

- React 19
- Vite
- Lucide React icons
- Plain CSS modules in `src/styles.css`

## SQL Server

The API uses Windows authentication and creates/updates the database through EF Core migrations.

Connection string:

```json
"DefaultConnection": "Data Source=DESKTOP-1N9N76K;Initial Catalog=ECommerceAppDb;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"ECommerceApp API\";Command Timeout=0"
```

Database name:

```text
ECommerceAppDb
```

## Seeded Admin

The development seed creates:

```text
Email: admin@ecommerce.local
Password: Admin@12345
Role: Admin
```

Change this before production.

## Backend Features

- Auth/register/login through `/api/auth`
- Account/profile endpoints
- Admin user role management
- Product catalog with categories, search, filters, sort, pagination
- Product images under `wwwroot/uploads/products`
- Inventory and low-stock management
- Wishlist
- Cart
- Checkout and order history
- Admin order management
- Reviews and ratings
- Admin dashboard summary
- Seed data for categories and sample products
- JSON console logging for application and HTTP request logs

## Logging

The API writes structured JSON logs to the console. Each request log includes:

```text
traceId, requestMethod, requestPath, statusCode, elapsedMilliseconds, remoteIpAddress, userId, userName
```

Sensitive request bodies, passwords, and bearer tokens are not logged.

## Run Backend

```bash
cd ECommerceApp.API
dotnet run --urls http://localhost:5099
```

Swagger:

```text
http://localhost:5099/swagger
```

## Useful Endpoints

```text
GET    /api/categories
GET    /api/products
POST   /api/auth/login
GET    /api/account/me
GET    /api/cart
POST   /api/orders/checkout
GET    /api/admin/dashboard
```

Most customer endpoints require a bearer token. Admin endpoints require the `Admin` role.

## Run Frontend

Open a second terminal:

```bash
cd ECommerceApp.Client
npm install
npm run dev
```

Frontend:

```text
http://localhost:5173
```

Production build:

```bash
npm run build
```
