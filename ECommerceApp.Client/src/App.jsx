import {
  BarChart3,
  Box,
  CheckCircle2,
  ChevronLeft,
  ChevronRight,
  Heart,
  Home,
  LayoutDashboard,
  LogIn,
  LogOut,
  Package,
  PackageCheck,
  Search,
  Settings,
  ShieldCheck,
  ShoppingBag,
  ShoppingCart,
  Star,
  Trash2,
  User,
  X
} from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { api } from "./api/client";
import { formatDate, formatMoney, imageUrl, slugify } from "./utils/format";

const tokenKey = "ecommerceapp.accessToken";

const orderStatuses = [
  ["Pending", 0],
  ["Confirmed", 1],
  ["Processing", 2],
  ["Shipped", 3],
  ["Delivered", 4],
  ["Cancelled", 5],
  ["Refunded", 6]
];

const paymentStatuses = [
  ["Pending", 0],
  ["Paid", 1],
  ["Failed", 2],
  ["Refunded", 3]
];

const fallbackImages = ["/assets/product.png", "/assets/uniqueProduct.png", "/assets/vendor.png"];

function App() {
  const [view, setView] = useState("shop");
  const [adminTab, setAdminTab] = useState("dashboard");
  const [token, setToken] = useState(() => localStorage.getItem(tokenKey));
  const [account, setAccount] = useState(null);
  const [authMode, setAuthMode] = useState("login");
  const [authForm, setAuthForm] = useState({ email: "", password: "" });
  const [profileForm, setProfileForm] = useState({
    fullName: "",
    phoneNumber: "",
    addressLine1: "",
    city: "",
    country: ""
  });
  const [query, setQuery] = useState({ search: "", categoryId: "", sort: "newest", page: 1, pageSize: 12 });
  const [categories, setCategories] = useState([]);
  const [products, setProducts] = useState({ items: [], page: 1, pageSize: 12, totalItems: 0, totalPages: 1 });
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [cart, setCart] = useState({ items: [], totalQuantity: 0, subtotal: 0 });
  const [wishlist, setWishlist] = useState([]);
  const [orders, setOrders] = useState([]);
  const [dashboard, setDashboard] = useState(null);
  const [adminOrders, setAdminOrders] = useState({ items: [] });
  const [lowStock, setLowStock] = useState([]);
  const [productForm, setProductForm] = useState(emptyProductForm);
  const [checkoutForm, setCheckoutForm] = useState({
    shippingName: "",
    shippingPhone: "",
    shippingAddressLine1: "",
    shippingCity: "",
    shippingCountry: "Bangladesh",
    paymentMethod: 0,
    notes: ""
  });
  const [loading, setLoading] = useState(false);
  const [toast, setToast] = useState(null);

  const isAdmin = account?.roles?.includes("Admin");
  const wishlistIds = useMemo(() => new Set(wishlist.map((item) => item.productId)), [wishlist]);

  useEffect(() => {
    refreshCatalog();
  }, [query]);

  useEffect(() => {
    loadCategories();
  }, []);

  useEffect(() => {
    if (!token) {
      setAccount(null);
      setCart({ items: [], totalQuantity: 0, subtotal: 0 });
      setWishlist([]);
      setOrders([]);
      return;
    }

    loadSession(token);
  }, [token]);

  useEffect(() => {
    if (isAdmin && token) {
      refreshAdmin();
    }
  }, [isAdmin, token]);

  useEffect(() => {
    if (account) {
      setProfileForm({
        fullName: account.fullName || "",
        phoneNumber: account.phoneNumber || "",
        addressLine1: account.addressLine1 || "",
        city: account.city || "",
        country: account.country || ""
      });
      setCheckoutForm((current) => ({
        ...current,
        shippingName: current.shippingName || account.fullName || "",
        shippingPhone: current.shippingPhone || account.phoneNumber || "",
        shippingAddressLine1: current.shippingAddressLine1 || account.addressLine1 || "",
        shippingCity: current.shippingCity || account.city || "",
        shippingCountry: current.shippingCountry || account.country || "Bangladesh"
      }));
    }
  }, [account]);

  function notify(message, tone = "success") {
    setToast({ message, tone });
    window.clearTimeout(notify.timeoutId);
    notify.timeoutId = window.setTimeout(() => setToast(null), 3500);
  }

  async function run(action, successMessage) {
    setLoading(true);
    try {
      const result = await action();
      if (successMessage) notify(successMessage);
      return result;
    } catch (error) {
      notify(error.message || "Something went wrong", "error");
      return null;
    } finally {
      setLoading(false);
    }
  }

  async function loadCategories() {
    const result = await run(() => api.categories());
    if (result) setCategories(result);
  }

  async function refreshCatalog() {
    const result = await run(() => api.products(query));
    if (result) setProducts(result);
  }

  async function loadSession(accessToken) {
    const me = await run(() => api.me(accessToken));
    if (!me) {
      localStorage.removeItem(tokenKey);
      setToken(null);
      return;
    }

    setAccount(me);
    await Promise.all([loadCart(accessToken), loadWishlist(accessToken), loadOrders(accessToken)]);
  }

  async function loadCart(accessToken = token) {
    if (!accessToken) return;
    const result = await run(() => api.cart(accessToken));
    if (result) setCart(result);
  }

  async function loadWishlist(accessToken = token) {
    if (!accessToken) return;
    const result = await run(() => api.wishlist(accessToken));
    if (result) setWishlist(result);
  }

  async function loadOrders(accessToken = token) {
    if (!accessToken) return;
    const result = await run(() => api.myOrders(accessToken));
    if (result) setOrders(result);
  }

  async function refreshAdmin() {
    if (!token) return;
    const [summary, orderPage, stockItems] = await Promise.all([
      api.dashboard(token).catch((error) => ({ error })),
      api.adminOrders(token).catch((error) => ({ error })),
      api.lowStock(token).catch((error) => ({ error }))
    ]);

    if (!summary.error) setDashboard(summary);
    if (!orderPage.error) setAdminOrders(orderPage);
    if (!stockItems.error) setLowStock(stockItems);
  }

  async function handleLogin(event) {
    event.preventDefault();
    const response = await run(() => api.login(authForm.email, authForm.password), "Signed in");
    if (response?.accessToken) {
      localStorage.setItem(tokenKey, response.accessToken);
      setToken(response.accessToken);
      setView("shop");
    }
  }

  async function handleRegister(event) {
    event.preventDefault();
    const registered = await run(() => api.register(authForm.email, authForm.password), "Account created");
    if (registered !== null) {
      const response = await run(() => api.login(authForm.email, authForm.password), "Signed in");
      if (response?.accessToken) {
        localStorage.setItem(tokenKey, response.accessToken);
        setToken(response.accessToken);
        setView("profile");
      }
    }
  }

  function signOut() {
    localStorage.removeItem(tokenKey);
    setToken(null);
    setAccount(null);
    setView("shop");
    notify("Signed out");
  }

  function requireAuth(nextView) {
    if (!token) {
      setAuthMode("login");
      setView("auth");
      notify("Sign in to continue", "warning");
      return false;
    }
    if (nextView) setView(nextView);
    return true;
  }

  async function addToCart(productId, quantity = 1) {
    if (!requireAuth()) return;
    const result = await run(() => api.addCartItem(productId, quantity, token), "Added to cart");
    if (result) setCart(result);
  }

  async function toggleWishlist(productId) {
    if (!requireAuth()) return;
    if (wishlistIds.has(productId)) {
      await run(() => api.removeWishlist(productId, token), "Removed from wishlist");
    } else {
      await run(() => api.addWishlist(productId, token), "Added to wishlist");
    }
    await loadWishlist();
  }

  async function loadProductDetails(productId) {
    const product = await run(() => api.product(productId));
    if (product) setSelectedProduct(product);
  }

  async function updateQuantity(productId, quantity) {
    const result = quantity <= 0
      ? await run(() => api.removeCartItem(productId, token), "Removed item")
      : await run(() => api.updateCartItem(productId, quantity, token));
    if (result) setCart(result);
  }

  async function checkout(event) {
    event.preventDefault();
    const order = await run(() => api.checkout(checkoutForm, token), "Order placed");
    if (order) {
      await Promise.all([loadCart(), loadOrders(), refreshCatalog()]);
      setView("orders");
    }
  }

  async function updateProfile(event) {
    event.preventDefault();
    const updated = await run(() => api.updateMe(profileForm, token), "Profile saved");
    if (updated) setAccount(updated);
  }

  async function submitProduct(event) {
    event.preventDefault();
    if (!productForm.categoryId && categories[0]) {
      productForm.categoryId = categories[0].id;
    }

    const formData = toProductFormData(productForm);
    const result = await run(() => api.createProduct(formData, token), "Product saved");
    if (result) {
      setProductForm(emptyProductForm);
      await Promise.all([refreshCatalog(), refreshAdmin()]);
    }
  }

  async function deactivateProduct(productId) {
    const result = await run(() => api.deactivateProduct(productId, token), "Product deactivated");
    if (result) await refreshCatalog();
  }

  async function updateOrderStatus(orderId, status, paymentStatus) {
    const result = await run(
      () => api.updateOrderStatus(orderId, { status: Number(status), paymentStatus: Number(paymentStatus) }, token),
      "Order updated"
    );
    if (result) await refreshAdmin();
  }

  async function restockProduct(productId) {
    const quantity = window.prompt("Quantity available");
    if (quantity === null) return;
    const value = Number(quantity);
    if (!Number.isFinite(value) || value < 0) {
      notify("Enter a valid quantity", "error");
      return;
    }

    await run(() => api.updateInventory(productId, { quantityAvailable: value, lowStockThreshold: 5 }, token), "Inventory updated");
    await refreshAdmin();
  }

  const navItems = [
    ["shop", "Shop", Home],
    ["cart", `Cart ${cart.totalQuantity ? `(${cart.totalQuantity})` : ""}`, ShoppingCart],
    ["orders", "Orders", ShoppingBag],
    ["profile", "Profile", User]
  ];

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <button className="brand" onClick={() => setView("shop")} aria-label="Open storefront">
          <img src="/assets/logo.png" alt="" />
          <span>ECommerceApp</span>
        </button>

        <nav className="nav-list" aria-label="Primary navigation">
          {navItems.map(([id, label, Icon]) => (
            <button
              key={id}
              className={view === id ? "active" : ""}
              onClick={() => (id === "shop" ? setView(id) : requireAuth(id))}
            >
              <Icon size={18} />
              <span>{label}</span>
            </button>
          ))}

          {isAdmin && (
            <button className={view === "admin" ? "active" : ""} onClick={() => setView("admin")}>
              <LayoutDashboard size={18} />
              <span>Admin</span>
            </button>
          )}
        </nav>

        <div className="sidebar-account">
          {account ? (
            <>
              <div className="account-pill">
                <ShieldCheck size={16} />
                <span>{account.fullName || account.email}</span>
              </div>
              <button className="ghost-button full" onClick={signOut}>
                <LogOut size={16} />
                <span>Sign out</span>
              </button>
            </>
          ) : (
            <button className="primary-button full" onClick={() => setView("auth")}>
              <LogIn size={16} />
              <span>Sign in</span>
            </button>
          )}
        </div>
      </aside>

      <main className="workspace">
        <Topbar
          query={query}
          setQuery={setQuery}
          categories={categories}
          loading={loading}
          account={account}
          onAuth={() => setView("auth")}
        />

        {view === "shop" && (
          <ShopView
            products={products}
            query={query}
            setQuery={setQuery}
            wishlistIds={wishlistIds}
            onDetails={loadProductDetails}
            onCart={addToCart}
            onWishlist={toggleWishlist}
            selectedProduct={selectedProduct}
            onCloseDetails={() => setSelectedProduct(null)}
          />
        )}

        {view === "auth" && (
          <AuthView
            mode={authMode}
            setMode={setAuthMode}
            form={authForm}
            setForm={setAuthForm}
            onLogin={handleLogin}
            onRegister={handleRegister}
          />
        )}

        {view === "cart" && (
          <CartView
            cart={cart}
            checkoutForm={checkoutForm}
            setCheckoutForm={setCheckoutForm}
            onQuantity={updateQuantity}
            onCheckout={checkout}
          />
        )}

        {view === "orders" && <OrdersView orders={orders} />}

        {view === "profile" && (
          <ProfileView
            account={account}
            form={profileForm}
            setForm={setProfileForm}
            onSubmit={updateProfile}
          />
        )}

        {view === "admin" && isAdmin && (
          <AdminView
            tab={adminTab}
            setTab={setAdminTab}
            dashboard={dashboard}
            products={products.items}
            categories={categories}
            productForm={productForm}
            setProductForm={setProductForm}
            onProductSubmit={submitProduct}
            onDeactivateProduct={deactivateProduct}
            orders={adminOrders.items || []}
            lowStock={lowStock}
            onOrderStatus={updateOrderStatus}
            onRestock={restockProduct}
          />
        )}
      </main>

      {toast && (
        <div className={`toast ${toast.tone}`}>
          {toast.tone === "error" ? <X size={16} /> : <CheckCircle2 size={16} />}
          <span>{toast.message}</span>
        </div>
      )}
    </div>
  );
}

function Topbar({ query, setQuery, categories, loading, account, onAuth }) {
  return (
    <header className="topbar">
      <div className="searchbar">
        <Search size={18} />
        <input
          value={query.search}
          onChange={(event) => setQuery((current) => ({ ...current, search: event.target.value, page: 1 }))}
          placeholder="Search products"
        />
      </div>
      <select
        value={query.categoryId}
        onChange={(event) => setQuery((current) => ({ ...current, categoryId: event.target.value, page: 1 }))}
        aria-label="Category"
      >
        <option value="">All categories</option>
        {categories.map((category) => (
          <option key={category.id} value={category.id}>
            {category.name}
          </option>
        ))}
      </select>
      <select
        value={query.sort}
        onChange={(event) => setQuery((current) => ({ ...current, sort: event.target.value, page: 1 }))}
        aria-label="Sort"
      >
        <option value="newest">Newest</option>
        <option value="featured">Featured</option>
        <option value="price_asc">Price low to high</option>
        <option value="price_desc">Price high to low</option>
        <option value="name">Name</option>
      </select>
      <div className="topbar-user">
        {loading && <span className="sync-dot" />}
        {account ? <span>{account.email}</span> : <button onClick={onAuth}>Sign in</button>}
      </div>
    </header>
  );
}

function ShopView({ products, query, setQuery, wishlistIds, onDetails, onCart, onWishlist, selectedProduct, onCloseDetails }) {
  return (
    <>
      <section className="section-heading">
        <div>
          <p>Product Catalog</p>
          <h1>Smart Ecommerce Storefront</h1>
        </div>
        <span>{products.totalItems} products</span>
      </section>

      <section className="product-grid">
        {products.items.map((product, index) => (
          <article className="product-card" key={product.id}>
            <button className="image-button" onClick={() => onDetails(product.id)} aria-label={`Open ${product.name}`}>
              <img src={imageUrl(product.primaryImageUrl) || fallbackImages[index % fallbackImages.length]} alt={product.name} />
            </button>
            <div className="product-card-body">
              <div className="product-meta">
                <span>{product.categoryName}</span>
                {product.isFeatured && <span className="badge">Featured</span>}
              </div>
              <h2>{product.name}</h2>
              <p>{product.brand || product.sku}</p>
              <div className="rating-line">
                <Star size={15} fill="currentColor" />
                <span>{product.averageRating || 0}</span>
                <span>{product.quantityAvailable > 0 ? `${product.quantityAvailable} in stock` : "Out of stock"}</span>
              </div>
              <div className="price-row">
                <strong>{formatMoney(product.effectivePrice)}</strong>
                {product.discountPercent > 0 && <small>{product.discountPercent}% off</small>}
              </div>
              <div className="button-row">
                <button className="icon-button" onClick={() => onWishlist(product.id)} title="Wishlist" aria-label="Wishlist">
                  <Heart size={18} fill={wishlistIds.has(product.id) ? "currentColor" : "none"} />
                </button>
                <button className="ghost-button" onClick={() => onDetails(product.id)}>
                  <Box size={16} />
                  <span>Details</span>
                </button>
                <button className="primary-button" onClick={() => onCart(product.id)} disabled={product.quantityAvailable <= 0}>
                  <ShoppingCart size={16} />
                  <span>Add</span>
                </button>
              </div>
            </div>
          </article>
        ))}
      </section>

      {products.items.length === 0 && <EmptyState icon={Package} title="No products found" />}

      <div className="pager">
        <button
          className="icon-button"
          onClick={() => setQuery((current) => ({ ...current, page: Math.max(1, current.page - 1) }))}
          disabled={query.page <= 1}
          aria-label="Previous page"
        >
          <ChevronLeft size={18} />
        </button>
        <span>
          Page {products.page} of {products.totalPages || 1}
        </span>
        <button
          className="icon-button"
          onClick={() => setQuery((current) => ({ ...current, page: current.page + 1 }))}
          disabled={products.page >= products.totalPages}
          aria-label="Next page"
        >
          <ChevronRight size={18} />
        </button>
      </div>

      {selectedProduct && (
        <section className="detail-panel">
          <button className="icon-button close-button" onClick={onCloseDetails} aria-label="Close product details">
            <X size={18} />
          </button>
          <img src={imageUrl(selectedProduct.images?.[0]?.url)} alt={selectedProduct.name} />
          <div>
            <span className="badge">{selectedProduct.category?.name}</span>
            <h2>{selectedProduct.name}</h2>
            <p>{selectedProduct.description}</p>
            <div className="metric-row">
              <strong>{formatMoney(selectedProduct.effectivePrice)}</strong>
              <span>{selectedProduct.inventory?.quantityAvailable ?? 0} available</span>
              <span>{selectedProduct.reviewCount} reviews</span>
            </div>
          </div>
        </section>
      )}
    </>
  );
}

function AuthView({ mode, setMode, form, setForm, onLogin, onRegister }) {
  const isLogin = mode === "login";
  return (
    <section className="auth-layout">
      <div className="auth-media">
        <img src="/assets/uniqueProduct.png" alt="" />
      </div>
      <form className="form-panel" onSubmit={isLogin ? onLogin : onRegister}>
        <p>{isLogin ? "Welcome Back" : "Create Account"}</p>
        <h1>{isLogin ? "Sign in" : "Register"}</h1>
        <label>
          Email
          <input
            type="email"
            value={form.email}
            onChange={(event) => setForm((current) => ({ ...current, email: event.target.value }))}
            required
          />
        </label>
        <label>
          Password
          <input
            type="password"
            value={form.password}
            onChange={(event) => setForm((current) => ({ ...current, password: event.target.value }))}
            required
            minLength={8}
          />
        </label>
        <button className="primary-button full" type="submit">
          <LogIn size={16} />
          <span>{isLogin ? "Sign in" : "Register"}</span>
        </button>
        <button className="ghost-button full" type="button" onClick={() => setMode(isLogin ? "register" : "login")}>
          {isLogin ? "Create an account" : "Use existing account"}
        </button>
      </form>
    </section>
  );
}

function CartView({ cart, checkoutForm, setCheckoutForm, onQuantity, onCheckout }) {
  return (
    <section className="two-column">
      <div className="panel">
        <div className="panel-title">
          <h1>Cart</h1>
          <span>{cart.totalQuantity} items</span>
        </div>
        <div className="line-list">
          {cart.items.map((item) => (
            <div className="cart-line" key={item.productId}>
              <img src={imageUrl(item.imageUrl)} alt={item.productName} />
              <div>
                <strong>{item.productName}</strong>
                <span>{formatMoney(item.unitPrice)}</span>
              </div>
              <div className="quantity-control">
                <button onClick={() => onQuantity(item.productId, item.quantity - 1)} aria-label="Decrease quantity">
                  -
                </button>
                <span>{item.quantity}</span>
                <button onClick={() => onQuantity(item.productId, item.quantity + 1)} aria-label="Increase quantity">
                  +
                </button>
              </div>
              <strong>{formatMoney(item.lineTotal)}</strong>
            </div>
          ))}
          {cart.items.length === 0 && <EmptyState icon={ShoppingCart} title="Your cart is empty" />}
        </div>
        <div className="total-line">
          <span>Subtotal</span>
          <strong>{formatMoney(cart.subtotal)}</strong>
        </div>
      </div>

      <form className="form-panel" onSubmit={onCheckout}>
        <p>Checkout</p>
        <h1>Shipping</h1>
        <Input label="Name" value={checkoutForm.shippingName} onChange={(value) => setCheckoutForm((current) => ({ ...current, shippingName: value }))} />
        <Input label="Phone" value={checkoutForm.shippingPhone} onChange={(value) => setCheckoutForm((current) => ({ ...current, shippingPhone: value }))} />
        <Input label="Address" value={checkoutForm.shippingAddressLine1} onChange={(value) => setCheckoutForm((current) => ({ ...current, shippingAddressLine1: value }))} />
        <Input label="City" value={checkoutForm.shippingCity} onChange={(value) => setCheckoutForm((current) => ({ ...current, shippingCity: value }))} />
        <Input label="Country" value={checkoutForm.shippingCountry} onChange={(value) => setCheckoutForm((current) => ({ ...current, shippingCountry: value }))} />
        <label>
          Payment
          <select
            value={checkoutForm.paymentMethod}
            onChange={(event) => setCheckoutForm((current) => ({ ...current, paymentMethod: Number(event.target.value) }))}
          >
            <option value={0}>Cash on delivery</option>
            <option value={1}>Card</option>
            <option value={2}>Mobile banking</option>
          </select>
        </label>
        <button className="primary-button full" type="submit" disabled={cart.items.length === 0}>
          <ShoppingBag size={16} />
          <span>Place order</span>
        </button>
      </form>
    </section>
  );
}

function OrdersView({ orders }) {
  return (
    <section className="panel">
      <div className="panel-title">
        <h1>Orders</h1>
        <span>{orders.length} records</span>
      </div>
      <DataTable
        columns={["Order", "Status", "Payment", "Date", "Total"]}
        rows={orders.map((order) => [
          order.orderNumber,
          statusLabel(order.status, orderStatuses),
          statusLabel(order.paymentStatus, paymentStatuses),
          formatDate(order.createdAt),
          formatMoney(order.total)
        ])}
      />
      {orders.length === 0 && <EmptyState icon={ShoppingBag} title="No orders yet" />}
    </section>
  );
}

function ProfileView({ account, form, setForm, onSubmit }) {
  return (
    <section className="two-column compact">
      <form className="form-panel" onSubmit={onSubmit}>
        <p>Account</p>
        <h1>Profile</h1>
        <Input label="Full name" value={form.fullName} onChange={(value) => setForm((current) => ({ ...current, fullName: value }))} />
        <Input label="Phone" value={form.phoneNumber} required={false} onChange={(value) => setForm((current) => ({ ...current, phoneNumber: value }))} />
        <Input label="Address" value={form.addressLine1} required={false} onChange={(value) => setForm((current) => ({ ...current, addressLine1: value }))} />
        <Input label="City" value={form.city} required={false} onChange={(value) => setForm((current) => ({ ...current, city: value }))} />
        <Input label="Country" value={form.country} required={false} onChange={(value) => setForm((current) => ({ ...current, country: value }))} />
        <button className="primary-button full" type="submit">
          <CheckCircle2 size={16} />
          <span>Save profile</span>
        </button>
      </form>
      <div className="panel identity-panel">
        <User size={44} />
        <h2>{account?.fullName || "Customer"}</h2>
        <p>{account?.email}</p>
        <div className="chip-row">
          {account?.roles?.map((role) => <span key={role}>{role}</span>)}
        </div>
      </div>
    </section>
  );
}

function AdminView({
  tab,
  setTab,
  dashboard,
  products,
  categories,
  productForm,
  setProductForm,
  onProductSubmit,
  onDeactivateProduct,
  orders,
  lowStock,
  onOrderStatus,
  onRestock
}) {
  const tabs = [
    ["dashboard", "Dashboard", BarChart3],
    ["products", "Products", Package],
    ["orders", "Orders", ShoppingBag],
    ["inventory", "Inventory", PackageCheck]
  ];

  return (
    <section className="admin-shell">
      <div className="tab-list">
        {tabs.map(([id, label, Icon]) => (
          <button key={id} className={tab === id ? "active" : ""} onClick={() => setTab(id)}>
            <Icon size={16} />
            <span>{label}</span>
          </button>
        ))}
      </div>

      {tab === "dashboard" && <DashboardPanel dashboard={dashboard} />}
      {tab === "products" && (
        <ProductsAdminPanel
          products={products}
          categories={categories}
          form={productForm}
          setForm={setProductForm}
          onSubmit={onProductSubmit}
          onDeactivate={onDeactivateProduct}
        />
      )}
      {tab === "orders" && <OrdersAdminPanel orders={orders} onOrderStatus={onOrderStatus} />}
      {tab === "inventory" && <InventoryPanel lowStock={lowStock} onRestock={onRestock} />}
    </section>
  );
}

function DashboardPanel({ dashboard }) {
  const metrics = [
    ["Products", dashboard?.totalProducts || 0, Package],
    ["Customers", dashboard?.totalCustomers || 0, User],
    ["Orders", dashboard?.totalOrders || 0, ShoppingBag],
    ["30 day revenue", formatMoney(dashboard?.revenueLast30Days || 0), BarChart3],
    ["Pending orders", dashboard?.pendingOrders || 0, Settings],
    ["Low stock", dashboard?.lowStockProducts || 0, PackageCheck]
  ];

  return (
    <>
      <section className="metric-grid">
        {metrics.map(([label, value, Icon]) => (
          <div className="metric-tile" key={label}>
            <Icon size={20} />
            <span>{label}</span>
            <strong>{value}</strong>
          </div>
        ))}
      </section>
      <section className="two-column">
        <div className="panel">
          <div className="panel-title">
            <h2>Top Products</h2>
          </div>
          <DataTable
            columns={["Product", "Units", "Revenue"]}
            rows={(dashboard?.topProducts || []).map((item) => [item.productName, item.unitsSold, formatMoney(item.revenue)])}
          />
        </div>
        <div className="panel">
          <div className="panel-title">
            <h2>Low Stock</h2>
          </div>
          <DataTable
            columns={["Product", "SKU", "Qty"]}
            rows={(dashboard?.lowStockItems || []).map((item) => [item.productName, item.sku, item.quantityAvailable])}
          />
        </div>
      </section>
    </>
  );
}

function ProductsAdminPanel({ products, categories, form, setForm, onSubmit, onDeactivate }) {
  return (
    <section className="two-column wide-left">
      <div className="panel">
        <div className="panel-title">
          <h2>Products</h2>
          <span>{products.length} active</span>
        </div>
        <div className="admin-product-list">
          {products.map((product) => (
            <div className="admin-product-row" key={product.id}>
              <img src={imageUrl(product.primaryImageUrl)} alt={product.name} />
              <div>
                <strong>{product.name}</strong>
                <span>{product.sku} · {product.categoryName}</span>
              </div>
              <strong>{formatMoney(product.effectivePrice)}</strong>
              <button className="icon-button danger" onClick={() => onDeactivate(product.id)} aria-label="Deactivate product">
                <Trash2 size={17} />
              </button>
            </div>
          ))}
        </div>
      </div>

      <form className="form-panel" onSubmit={onSubmit}>
        <p>Catalog</p>
        <h2>New Product</h2>
        <Input label="Name" value={form.name} onChange={(value) => setForm((current) => ({ ...current, name: value, slug: slugify(value) }))} />
        <Input label="Slug" value={form.slug} onChange={(value) => setForm((current) => ({ ...current, slug: value }))} />
        <Input label="Brand" value={form.brand} required={false} onChange={(value) => setForm((current) => ({ ...current, brand: value }))} />
        <Input label="SKU" value={form.sku} required={false} onChange={(value) => setForm((current) => ({ ...current, sku: value }))} />
        <label>
          Category
          <select value={form.categoryId || categories[0]?.id || ""} onChange={(event) => setForm((current) => ({ ...current, categoryId: event.target.value }))}>
            {categories.map((category) => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>
        </label>
        <div className="form-grid">
          <Input label="Price" type="number" value={form.price} onChange={(value) => setForm((current) => ({ ...current, price: value }))} />
          <Input label="Stock" type="number" value={form.quantityAvailable} onChange={(value) => setForm((current) => ({ ...current, quantityAvailable: value }))} />
        </div>
        <label>
          Description
          <textarea value={form.description} onChange={(event) => setForm((current) => ({ ...current, description: event.target.value }))} required />
        </label>
        <label>
          Image
          <input type="file" accept="image/*" onChange={(event) => setForm((current) => ({ ...current, imageFile: event.target.files?.[0] || null }))} />
        </label>
        <label className="switch-line">
          <input type="checkbox" checked={form.isFeatured} onChange={(event) => setForm((current) => ({ ...current, isFeatured: event.target.checked }))} />
          Featured
        </label>
        <button className="primary-button full" type="submit">
          <Package size={16} />
          <span>Create product</span>
        </button>
      </form>
    </section>
  );
}

function OrdersAdminPanel({ orders, onOrderStatus }) {
  return (
    <section className="panel">
      <div className="panel-title">
        <h2>Order Management</h2>
        <span>{orders.length} records</span>
      </div>
      <div className="order-admin-list">
        {orders.map((order) => (
          <div className="order-admin-row" key={order.id}>
            <div>
              <strong>{order.orderNumber}</strong>
              <span>{formatDate(order.createdAt)} · {formatMoney(order.total)}</span>
            </div>
            <select defaultValue={order.status} onChange={(event) => onOrderStatus(order.id, event.target.value, order.paymentStatus)}>
              {orderStatuses.map(([label, value]) => (
                <option key={value} value={value}>{label}</option>
              ))}
            </select>
            <select defaultValue={order.paymentStatus} onChange={(event) => onOrderStatus(order.id, order.status, event.target.value)}>
              {paymentStatuses.map(([label, value]) => (
                <option key={value} value={value}>{label}</option>
              ))}
            </select>
          </div>
        ))}
        {orders.length === 0 && <EmptyState icon={ShoppingBag} title="No orders yet" />}
      </div>
    </section>
  );
}

function InventoryPanel({ lowStock, onRestock }) {
  return (
    <section className="panel">
      <div className="panel-title">
        <h2>Inventory</h2>
        <span>{lowStock.length} low-stock items</span>
      </div>
      <div className="line-list">
        {lowStock.map((item) => (
          <div className="inventory-row" key={item.productId}>
            <PackageCheck size={18} />
            <div>
              <strong>{item.productName}</strong>
              <span>{item.sku}</span>
            </div>
            <span>{item.quantityAvailable} / {item.lowStockThreshold}</span>
            <button className="ghost-button" onClick={() => onRestock(item.productId)}>
              <PackageCheck size={16} />
              <span>Restock</span>
            </button>
          </div>
        ))}
        {lowStock.length === 0 && <EmptyState icon={PackageCheck} title="Inventory is healthy" />}
      </div>
    </section>
  );
}

function Input({ label, value, onChange, type = "text", required = true }) {
  return (
    <label>
      {label}
      <input type={type} value={value} onChange={(event) => onChange(event.target.value)} required={required} />
    </label>
  );
}

function EmptyState({ icon: Icon, title }) {
  return (
    <div className="empty-state">
      <Icon size={28} />
      <span>{title}</span>
    </div>
  );
}

function DataTable({ columns, rows }) {
  return (
    <div className="table-wrap">
      <table>
        <thead>
          <tr>
            {columns.map((column) => <th key={column}>{column}</th>)}
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => (
            <tr key={index}>
              {row.map((cell, cellIndex) => <td key={cellIndex}>{cell}</td>)}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function statusLabel(value, list) {
  return list.find(([, statusValue]) => Number(statusValue) === Number(value))?.[0] || value;
}

function toProductFormData(form) {
  const formData = new FormData();
  formData.append("Name", form.name);
  formData.append("Slug", form.slug || slugify(form.name));
  formData.append("Description", form.description);
  formData.append("Brand", form.brand || "");
  formData.append("Sku", form.sku || "");
  formData.append("Price", form.price || 0);
  formData.append("CategoryId", form.categoryId);
  formData.append("QuantityAvailable", form.quantityAvailable || 0);
  formData.append("LowStockThreshold", form.lowStockThreshold || 5);
  formData.append("IsActive", form.isActive);
  formData.append("IsFeatured", form.isFeatured);
  if (form.discountPercent) formData.append("DiscountPercent", form.discountPercent);
  if (form.imageFile) formData.append("ImageFile", form.imageFile);
  return formData;
}

const emptyProductForm = {
  name: "",
  slug: "",
  description: "",
  brand: "",
  sku: "",
  price: "",
  categoryId: "",
  quantityAvailable: 0,
  lowStockThreshold: 5,
  discountPercent: "",
  isActive: true,
  isFeatured: false,
  imageFile: null
};

export default App;
