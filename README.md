# 🛒 ECommerceApp

This is a simple full-stack E-Commerce application built using **ASP.NET Core 8**, **Entity Framework Core**, and **SQLite**. It consists of two parts:

* **Backend API** (`ECommerceApp.API`)
* **Frontend Razor Pages App** (`ECommerceApp.Web`)

---

## 🗂️ Project Structure

```
ECommerceApp/
│
├── ECommerceApp.API/              → Backend (Web API)
│   ├── Controllers/
│   ├── Data/
│   ├── Dtos/
│   ├── Interfaces/
│   ├── Middleware/
│   ├── Migrations/
│   ├── Models/
│   ├── Services/
│   ├── wwwroot/images/            → Uploaded product images
│   ├── ecommerce.db               → SQLite Database
│   └── Program.cs
│
├── ECommerceApp.Web/              → Frontend (Razor Pages)
│   ├── Pages/
│   │   ├── Shared/_Layout.cshtml  → Layout (Navbar, Sidebar)
│   │   └── Index.cshtml           → Main page (Product display)
│   └── wwwroot/images/            → Static logos/images
```

---

## ⚙️ Technologies Used

* ASP.NET Core 8
* Entity Framework Core
* SQLite 3
* Razor Pages
* Tailwind CSS (via CDN)

---

## 🚀 Features

### 🧩 Frontend (`ECommerceApp.Web`)

* **Product Listing:** 4 products per page in two rows with pagination and dropdown to adjust page size.
* **Search:** Live search by partial product name — no button click needed.
* **Add Product Modal:**

  * Includes fields for **Discount Percentage** and **Product Image** (not in the original requirements but necessary for UI).
  * Includes a **Slug Generator** — auto-generates from the name or manually with the click of a button.
* **Layout Components:**

  * Navbar
  * Sidebar for Shopping Cart
* **Static Vendor/User ID:** Set statically for demo purposes.

### 🔗 Backend (`ECommerceApp.API`)

* **Products Manage**
* **Cart Information**
* **Image Upload Support**
* **Discount Price Calculation**
* **CORS Configuration** (see below if you use a different port)

---

## 🏁 Getting Started

### 📦 Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [SQLite 3](https://www.sqlite.org/download.html)

### 🛠️ Running the App

1. **Start the API:**

   ```bash
   cd ECommerceApp.API
   dotnet run
   ```

2. **Start the Frontend:**

   Open a new terminal:

   ```bash
   cd ECommerceApp.Web
   dotnet run
   ```

3. **Navigate to:**

   * Frontend: [http://localhost:5026](http://localhost:5026)
   * Backend API: [http://localhost:5099](http://localhost:5099)

---

## 🌐 CORS Configuration

If your frontend runs on a different port than `5026`, update the CORS policy in `ECommerceApp.API/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5026", policy =>
    {
        policy.WithOrigins("http://localhost:<your-new-port>")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

---

## 📁 File Uploads

Uploaded product images are saved to:

```
ECommerceApp.API/wwwroot/images
```

---

## 📞 Support

If you face any issues or need help, feel free to contact:

**MD Rezaul Karim Shuvo**
📧 [mdrezaulkarim31295@gmail.com](mailto:mdrezaulkarim31295@gmail.com)
📞 +8801303316865
