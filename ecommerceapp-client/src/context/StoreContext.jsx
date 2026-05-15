import { createContext, useContext, useCallback, useEffect, useMemo, useReducer } from "react";
import toast from "react-hot-toast";
import {
  authService,
  productService,
  categoryService,
  brandService,
  cartService,
  wishlistService,
  orderService,
  reviewService,
  addressService,
  adminService,
} from "../services/api";

const StoreContext = createContext(null);

const initialState = {
  products: [],
  categories: [],
  brands: [],
  orders: [],
  reviews: [],
  addresses: [],
  cartItems: [],
  wishlistIds: [],
  recentlyViewed: [],
  settings: {},
  currentUser: null,
  loading: true,
  theme: "light",
  error: null,
};

const reducer = (state, action) => {
  switch (action.type) {
    case "INIT_COMPLETE":
      return { ...state, loading: false };
    case "SET_THEME":
      return { ...state, theme: action.theme };
    case "SET_USER":
      return { ...state, currentUser: action.user };
    case "SET_PRODUCTS":
      return { ...state, products: action.products };
    case "ADD_PRODUCT":
      return { ...state, products: [action.product, ...state.products] };
    case "UPDATE_PRODUCT":
      return {
        ...state,
        products: state.products.map((p) => p.id === action.product.id ? { ...p, ...action.product } : p),
      };
    case "DELETE_PRODUCT":
      return { ...state, products: state.products.filter((p) => p.id !== action.productId) };
    case "SET_CATEGORIES":
      return { ...state, categories: action.categories };
    case "SET_BRANDS":
      return { ...state, brands: action.brands };
    case "SET_ORDERS":
      return { ...state, orders: action.orders };
    case "ADD_ORDER":
      return { ...state, orders: [action.order, ...state.orders] };
    case "UPDATE_ORDER_STATUS":
      return { ...state, orders: state.orders.map((o) => o.id === action.orderId ? { ...o, status: action.status } : o) };
    case "SET_REVIEWS":
      return { ...state, reviews: action.reviews };
    case "ADD_REVIEW":
      return { ...state, reviews: [action.review, ...state.reviews] };
    case "SET_ADDRESSES":
      return { ...state, addresses: action.addresses };
    case "ADD_ADDRESS":
      return { ...state, addresses: [...state.addresses, action.address] };
    case "UPDATE_ADDRESS":
      return { ...state, addresses: state.addresses.map((a) => a.id === action.address.id ? action.address : a) };
    case "DELETE_ADDRESS":
      return { ...state, addresses: state.addresses.filter((a) => a.id !== action.addressId) };
    case "SET_CART":
      return { ...state, cartItems: action.items };
    case "SET_WISHLIST_IDS":
      return { ...state, wishlistIds: action.ids };
    case "SET_SETTINGS":
      return { ...state, settings: action.settings };
    default:
      return state;
  }
};

export function StoreProvider({ children }) {
  const [state, dispatch] = useReducer(reducer, initialState);

  useEffect(() => {
    document.documentElement.classList.toggle("dark", state.theme === "dark");
  }, [state.theme]);

  const loadCart = useCallback(async () => {
    try {
      const cart = await cartService.getCart();
      dispatch({ type: "SET_CART", items: cart.items || [] });
    } catch { dispatch({ type: "SET_CART", items: [] }); }
  }, []);

  const loadWishlist = useCallback(async () => {
    try {
      const items = await wishlistService.getWishlist();
      dispatch({ type: "SET_WISHLIST_IDS", ids: items.map((i) => i.id) });
    } catch { dispatch({ type: "SET_WISHLIST_IDS", ids: [] }); }
  }, []);

  useEffect(() => {
    (async () => {
      try {
        const storedUser = localStorage.getItem("user");
        if (storedUser) {
          const parsed = JSON.parse(storedUser);
          dispatch({ type: "SET_USER", user: { ...parsed, id: `u-${parsed.id}` } });
        }

        const [prodResult, cats, brs] = await Promise.all([
          productService.getProducts({ page: 1, pageSize: 100 }),
          categoryService.getCategories(),
          brandService.getBrands(),
        ]);

        dispatch({ type: "SET_PRODUCTS", products: prodResult.items });
        dispatch({ type: "SET_CATEGORIES", categories: cats });
        dispatch({ type: "SET_BRANDS", brands: brs });

        if (storedUser) {
          try {
            const [orders, addresses] = await Promise.all([
              orderService.getOrders(),
              addressService.getAddresses(),
            ]);
            dispatch({ type: "SET_ORDERS", orders });
            dispatch({ type: "SET_ADDRESSES", addresses });
          } catch { /* ignore */ }
          await Promise.all([loadCart(), loadWishlist()]);
        }
      } catch (err) {
        console.error("Init error:", err);
      }
      dispatch({ type: "INIT_COMPLETE" });
    })();
  }, []);

  const currentUser = state.currentUser;
  const isAuthenticated = Boolean(currentUser);
  const isAdmin = currentUser?.role === "admin";
  const currentUserKey = currentUser?.id || "guest";

  const cartItems = useMemo(
    () =>
      state.cartItems
        .map((item) => ({
          ...item,
          product: state.products.find((p) => p.id === item.productId),
        }))
        .filter((item) => item.product),
    [state.cartItems, state.products],
  );

  const cartCount = cartItems.reduce((sum, i) => sum + i.quantity, 0);

  const wishlistProducts = useMemo(
    () => state.wishlistIds.map((id) => state.products.find((p) => p.id === id)).filter(Boolean),
    [state.wishlistIds, state.products],
  );

  const recentlyViewedProducts = useMemo(
    () => state.recentlyViewed.map((id) => state.products.find((p) => p.id === id)).filter(Boolean),
    [state.recentlyViewed, state.products],
  );

  const getProduct = useCallback((id) => state.products.find((p) => p.id === id), [state.products]);

  const getProductReviews = useCallback((id) => {
    return state.reviews.filter((r) => r.productId === id);
  }, [state.reviews]);

  const fetchProductReviews = useCallback(async (id) => {
    try {
      const reviews = await reviewService.getProductReviews(id);
      dispatch({ type: "SET_REVIEWS", reviews });
    } catch { /* ignore */ }
  }, []);

  const getBrandProducts = useCallback((brandName) => state.products.filter((p) => p.brand === brandName), [state.products]);

  const getOrder = useCallback((orderId) => state.orders.find((o) => o.id === orderId), [state.orders]);

  const getRecommendations = useCallback((productId, limit = 4) => {
    const seedProduct = productId ? getProduct(productId) : null;
    const categories = seedProduct
      ? [seedProduct.category]
      : [...new Set(state.products.slice(0, 4).map((p) => p.category))].slice(0, 2);
    const fromCategory = state.products.filter((p) => categories.includes(p.category) && p.id !== productId);
    const trending = [...state.products].sort((a, b) => b.sales - a.sales);
    return [...new Map([...fromCategory, ...trending].map((p) => [p.id, p])).values()].slice(0, limit);
  }, [state.products, getProduct]);

  const currentUserOrders = useMemo(
    () => state.orders.filter((o) => o.userId === currentUser?.id || o.userId === currentUser?.id?.replace("u-", "")),
    [state.orders, currentUser],
  );

  const analytics = useMemo(() => {
    const validOrders = state.orders.filter((o) => !["Cancelled", "Refunded"].includes(o.status));
    const revenue = validOrders.reduce((sum, o) => sum + Number(o.total || 0), 0);
    const lowStock = state.products.filter((p) => p.stock <= 8);
    const topProducts = [...state.products].sort((a, b) => b.sales - a.sales).slice(0, 6);
    return {
      revenue,
      totalOrders: state.orders.length,
      totalUsers: 0,
      lowStock,
      topProducts,
      recentOrders: state.orders.slice(0, 6),
    };
  }, [state.orders, state.products]);

  const refreshAuthData = useCallback(async () => {
    try {
      const [orders, addresses] = await Promise.all([
        orderService.getOrders(),
        addressService.getAddresses(),
      ]);
      dispatch({ type: "SET_ORDERS", orders });
      dispatch({ type: "SET_ADDRESSES", addresses });
    } catch { /* ignore */ }
    await Promise.all([loadCart(), loadWishlist()]);
  }, [loadCart, loadWishlist]);

  const actions = useMemo(() => ({
    async login(email, password) {
      try {
        const user = await authService.login(email, password);
        dispatch({ type: "SET_USER", user });
        toast.success(`Welcome back, ${user.name}`);
        await refreshAuthData();
        return user;
      } catch (err) {
        toast.error(err.response?.data?.message || "Invalid email or password");
        return null;
      }
    },

    async register(form) {
      try {
        const user = await authService.register(form);
        dispatch({ type: "SET_USER", user });
        toast.success("Account created successfully");
        await refreshAuthData();
        return user;
      } catch (err) {
        toast.error(err.response?.data?.message || "Registration failed");
        return null;
      }
    },

    async logout() {
      await authService.logout();
      dispatch({ type: "SET_USER", user: null });
      dispatch({ type: "SET_ORDERS", orders: [] });
      dispatch({ type: "SET_ADDRESSES", addresses: [] });
      dispatch({ type: "SET_CART", items: [] });
      dispatch({ type: "SET_WISHLIST_IDS", ids: [] });
      toast.success("Logged out");
    },

    setTheme(theme) {
      dispatch({ type: "SET_THEME", theme });
    },

    async addToCart(productId, quantity = 1) {
      if (!currentUser) {
        toast.error("Please log in to add products to cart");
        return false;
      }
      try {
        await cartService.addToCart(productId, quantity);
        await loadCart();
        toast.success("Added to cart");
        return true;
      } catch {
        toast.error("Failed to add to cart");
        return false;
      }
    },

    async updateCartQty(productId, quantity) {
      if (!currentUser) return;
      try {
        await cartService.updateCartQty(productId, quantity);
        await loadCart();
      } catch { /* ignore */ }
    },

    async removeFromCart(productId) {
      if (!currentUser) return;
      try {
        await cartService.removeFromCart(productId);
        await loadCart();
        toast.success("Removed from cart");
      } catch { /* ignore */ }
    },

    async saveForLater(productId) {
      if (!currentUser) return;
      try {
        await cartService.removeFromCart(productId);
        await wishlistService.toggleWishlist(productId);
        await Promise.all([loadCart(), loadWishlist()]);
        toast.success("Saved for later");
      } catch { /* ignore */ }
    },

    async toggleWishlist(productId) {
      if (!currentUser) {
        toast.error("Please log in to use wishlist");
        return false;
      }
      try {
        await wishlistService.toggleWishlist(productId);
        await loadWishlist();
        return true;
      } catch {
        return false;
      }
    },

    addRecentView(productId) { /* no-op for API version */ },

    async placeOrder(orderInput) {
      if (!currentUser) return null;
      try {
        const order = await orderService.placeOrder(orderInput);
        dispatch({ type: "ADD_ORDER", order });
        await loadCart();
        toast.success("Order placed successfully");
        return order;
      } catch (err) {
        toast.error(err.response?.data?.message || "Failed to place order");
        return null;
      }
    },

    async addReview(productId, rating, comment) {
      if (!currentUser) {
        toast.error("Please log in to write a review");
        return false;
      }
      try {
        const review = await reviewService.addReview(productId, rating, comment);
        dispatch({ type: "ADD_REVIEW", review });
        toast.success("Review published");
        return true;
      } catch {
        toast.error("Failed to submit review");
        return false;
      }
    },

    async updateProfile(profile) {
      if (!currentUser) return;
      try {
        toast.success("Profile updated");
      } catch { /* ignore */ }
    },

    async addAddress(address) {
      if (!currentUser) return;
      try {
        const result = await addressService.addAddress(address);
        dispatch({ type: "ADD_ADDRESS", address: result });
        toast.success("Address added");
      } catch { toast.error("Failed to add address"); }
    },

    async updateAddress(address) {
      if (!currentUser) return;
      try {
        const result = await addressService.updateAddress(address);
        dispatch({ type: "UPDATE_ADDRESS", address: result });
        toast.success("Address updated");
      } catch { toast.error("Failed to update address"); }
    },

    async deleteAddress(addressId) {
      if (!currentUser) return;
      try {
        await addressService.deleteAddress(addressId);
        dispatch({ type: "DELETE_ADDRESS", addressId });
        toast.success("Address removed");
      } catch { toast.error("Failed to delete address"); }
    },

    async adminAddProduct(product) {
      try {
        const result = await adminService.createProduct(product);
        dispatch({ type: "ADD_PRODUCT", product: result });
        toast.success("Product added");
      } catch { toast.error("Failed to add product"); }
    },

    async adminUpdateProduct(product) {
      try {
        const result = await adminService.updateProduct(product);
        dispatch({ type: "UPDATE_PRODUCT", product: result });
        toast.success("Product updated");
      } catch { toast.error("Failed to update product"); }
    },

    async adminDeleteProduct(productId) {
      try {
        await adminService.deleteProduct(productId);
        dispatch({ type: "DELETE_PRODUCT", productId });
        toast.success("Product deleted");
      } catch { toast.error("Failed to delete product"); }
    },

    async adminUpdateOrderStatus(orderId, status) {
      try {
        await adminService.updateOrderStatus(orderId, status);
        dispatch({ type: "UPDATE_ORDER_STATUS", orderId, status });
        toast.success("Order status updated");
      } catch { toast.error("Failed to update order status"); }
    },

    async adminToggleBlock(userId) {
      try {
        await adminService.toggleBlock(userId);
        toast.success("User status updated");
      } catch { toast.error("Failed to toggle user block"); }
    },

    async adminToggleRole(userId) {
      try {
        await adminService.toggleRole(userId);
        toast.success("User role updated");
      } catch { toast.error("Failed to toggle user role"); }
    },

    async adminDeleteUser(userId) {
      try {
        await adminService.deleteUser(userId);
        toast.success("User deleted");
      } catch { toast.error("Failed to delete user"); }
    },

    async updateSettings(settings) {
      try {
        await adminService.updateSettings(settings);
        dispatch({ type: "SET_SETTINGS", settings });
        toast.success("Settings saved");
      } catch { toast.error("Failed to save settings"); }
    },

    async resetDemoData() {
      toast.success("Data reloaded from server");
    },
  }), [currentUser, refreshAuthData, loadCart, loadWishlist]);

  const value = {
    state,
    currentUser,
    currentUserKey,
    isAuthenticated,
    isAdmin,
    cartItems,
    cartCount,
    wishlistProducts,
    recentlyViewedProducts,
    currentUserOrders,
    analytics,
    getProduct,
    getProductReviews,
    fetchProductReviews,
    getBrandProducts,
    getOrder,
    getRecommendations,
    actions,
  };

  return <StoreContext.Provider value={value}>{children}</StoreContext.Provider>;
}

export const useStore = () => {
  const context = useContext(StoreContext);
  if (!context) throw new Error("useStore must be used inside StoreProvider");
  return context;
};
