const API_BASE_URL = "http://localhost:5099";

async function parseResponse(response) {
  const contentType = response.headers.get("content-type") || "";
  const hasJson = contentType.includes("application/json");
  const payload = hasJson ? await response.json() : null;

  if (!response.ok) {
    const message = payload?.message || payload?.Message || payload?.title || response.statusText;
    throw new Error(message || "Request failed");
  }

  return payload ?? {};
}

function makeHeaders(token, extraHeaders = {}) {
  return {
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...extraHeaders
  };
}

export async function apiGet(path, token) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: makeHeaders(token)
  });
  return parseResponse(response);
}

export async function apiSend(path, method, body, token) {
  const isFormData = body instanceof FormData;
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers: makeHeaders(token, isFormData ? {} : { "Content-Type": "application/json" }),
    body: isFormData ? body : body === undefined ? undefined : JSON.stringify(body)
  });
  return parseResponse(response);
}

export const api = {
  login: (email, password) => apiSend("/api/auth/login", "POST", { email, password }),
  register: (email, password) => apiSend("/api/auth/register", "POST", { email, password }),
  me: (token) => apiGet("/api/account/me", token),
  updateMe: (payload, token) => apiSend("/api/account/me", "PUT", payload, token),
  categories: () => apiGet("/api/categories"),
  products: (query = {}) => {
    const search = new URLSearchParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== "") search.set(key, value);
    });
    return apiGet(`/api/products?${search.toString()}`);
  },
  product: (id) => apiGet(`/api/products/${id}`),
  createProduct: (formData, token) => apiSend("/api/products", "POST", formData, token),
  updateProduct: (id, formData, token) => apiSend(`/api/products/${id}`, "PUT", formData, token),
  deactivateProduct: (id, token) => apiSend(`/api/products/${id}`, "DELETE", undefined, token),
  cart: (token) => apiGet("/api/cart", token),
  addCartItem: (productId, quantity, token) => apiSend("/api/cart/items", "POST", { productId, quantity }, token),
  updateCartItem: (productId, quantity, token) => apiSend(`/api/cart/items/${productId}`, "PUT", { quantity }, token),
  removeCartItem: (productId, token) => apiSend(`/api/cart/items/${productId}`, "DELETE", undefined, token),
  clearCart: (token) => apiSend("/api/cart", "DELETE", undefined, token),
  wishlist: (token) => apiGet("/api/wishlist", token),
  addWishlist: (productId, token) => apiSend(`/api/wishlist/${productId}`, "POST", undefined, token),
  removeWishlist: (productId, token) => apiSend(`/api/wishlist/${productId}`, "DELETE", undefined, token),
  checkout: (payload, token) => apiSend("/api/orders/checkout", "POST", payload, token),
  myOrders: (token) => apiGet("/api/orders", token),
  dashboard: (token) => apiGet("/api/admin/dashboard", token),
  adminOrders: (token) => apiGet("/api/orders/admin?page=1&pageSize=50", token),
  updateOrderStatus: (id, payload, token) => apiSend(`/api/orders/${id}/status`, "PATCH", payload, token),
  lowStock: (token) => apiGet("/api/admin/inventory/low-stock", token),
  updateInventory: (productId, payload, token) => apiSend(`/api/admin/inventory/products/${productId}`, "PUT", payload, token)
};
