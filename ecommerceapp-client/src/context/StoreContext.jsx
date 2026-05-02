import { createContext, useContext, useEffect, useMemo, useReducer } from "react";
import toast from "react-hot-toast";
import { cloneInitialData } from "../data/mockData";

const STORAGE_KEY = "smartshop_ai_demo_state_v1";
const StoreContext = createContext(null);

const makeId = (prefix) => `${prefix}-${Date.now()}-${Math.random().toString(16).slice(2, 7)}`;

const loadState = () => {
  try {
    const saved = localStorage.getItem(STORAGE_KEY);
    if (saved) return JSON.parse(saved);
  } catch (error) {
    console.warn("Could not load SmartShop demo state", error);
  }
  return cloneInitialData();
};

const unique = (items) => [...new Set(items)];

const reducer = (state, action) => {
  switch (action.type) {
    case "LOGIN":
      return { ...state, currentUserId: action.userId };
    case "LOGOUT":
      return { ...state, currentUserId: null };
    case "SET_THEME":
      return { ...state, theme: action.theme };
    case "REGISTER":
      return {
        ...state,
        users: [...state.users, action.user],
        currentUserId: action.user.id,
        carts: { ...state.carts, [action.user.id]: [] },
        wishlist: { ...state.wishlist, [action.user.id]: [] },
        recentlyViewed: { ...state.recentlyViewed, [action.user.id]: [] },
      };
    case "ADD_TO_CART": {
      const current = state.carts[action.userId] || [];
      const found = current.find((item) => item.productId === action.productId);
      const next = found
        ? current.map((item) =>
            item.productId === action.productId
              ? { ...item, quantity: Math.min(99, item.quantity + action.quantity) }
              : item,
          )
        : [...current, { productId: action.productId, quantity: action.quantity }];
      return { ...state, carts: { ...state.carts, [action.userId]: next } };
    }
    case "UPDATE_CART_QTY": {
      const current = state.carts[action.userId] || [];
      const next = current
        .map((item) =>
          item.productId === action.productId
            ? { ...item, quantity: Math.max(1, Math.min(99, action.quantity)) }
            : item,
        )
        .filter((item) => item.quantity > 0);
      return { ...state, carts: { ...state.carts, [action.userId]: next } };
    }
    case "REMOVE_FROM_CART": {
      const current = state.carts[action.userId] || [];
      return {
        ...state,
        carts: {
          ...state.carts,
          [action.userId]: current.filter((item) => item.productId !== action.productId),
        },
      };
    }
    case "CLEAR_CART":
      return { ...state, carts: { ...state.carts, [action.userId]: [] } };
    case "TOGGLE_WISHLIST": {
      const current = state.wishlist[action.userId] || [];
      const next = current.includes(action.productId)
        ? current.filter((id) => id !== action.productId)
        : [...current, action.productId];
      return { ...state, wishlist: { ...state.wishlist, [action.userId]: next } };
    }
    case "MOVE_CART_TO_WISHLIST": {
      const currentCart = state.carts[action.userId] || [];
      const currentWishlist = state.wishlist[action.userId] || [];
      return {
        ...state,
        carts: {
          ...state.carts,
          [action.userId]: currentCart.filter((item) => item.productId !== action.productId),
        },
        wishlist: {
          ...state.wishlist,
          [action.userId]: unique([...currentWishlist, action.productId]),
        },
      };
    }
    case "ADD_RECENT_VIEW": {
      const current = state.recentlyViewed[action.userId] || [];
      const next = [action.productId, ...current.filter((id) => id !== action.productId)].slice(0, 8);
      return { ...state, recentlyViewed: { ...state.recentlyViewed, [action.userId]: next } };
    }
    case "PLACE_ORDER":
      return {
        ...state,
        orders: [action.order, ...state.orders],
        carts: { ...state.carts, [action.order.userId]: [] },
      };
    case "ADD_REVIEW": {
      const nextReviews = [action.review, ...state.reviews];
      const productReviews = nextReviews.filter((review) => review.productId === action.review.productId);
      const average =
        productReviews.reduce((sum, review) => sum + Number(review.rating), 0) / productReviews.length;
      return {
        ...state,
        reviews: nextReviews,
        products: state.products.map((product) =>
          product.id === action.review.productId
            ? { ...product, rating: Number(average.toFixed(1)) }
            : product,
        ),
      };
    }
    case "UPDATE_PROFILE":
      return {
        ...state,
        users: state.users.map((user) =>
          user.id === action.userId
            ? {
                ...user,
                ...action.profile,
                password: action.profile.password || user.password,
              }
            : user,
        ),
      };
    case "ADD_ADDRESS":
      return {
        ...state,
        users: state.users.map((user) =>
          user.id === action.userId
            ? { ...user, addresses: [...(user.addresses || []), action.address] }
            : user,
        ),
      };
    case "UPDATE_ADDRESS":
      return {
        ...state,
        users: state.users.map((user) =>
          user.id === action.userId
            ? {
                ...user,
                addresses: (user.addresses || []).map((address) =>
                  address.id === action.address.id ? action.address : address,
                ),
              }
            : user,
        ),
      };
    case "DELETE_ADDRESS":
      return {
        ...state,
        users: state.users.map((user) =>
          user.id === action.userId
            ? {
                ...user,
                addresses: (user.addresses || []).filter((address) => address.id !== action.addressId),
              }
            : user,
        ),
      };
    case "ADMIN_ADD_PRODUCT":
      return { ...state, products: [action.product, ...state.products] };
    case "ADMIN_UPDATE_PRODUCT":
      return {
        ...state,
        products: state.products.map((product) =>
          product.id === action.product.id ? { ...product, ...action.product } : product,
        ),
      };
    case "ADMIN_DELETE_PRODUCT":
      return {
        ...state,
        products: state.products.filter((product) => product.id !== action.productId),
        carts: Object.fromEntries(
          Object.entries(state.carts).map(([userId, items]) => [
            userId,
            items.filter((item) => item.productId !== action.productId),
          ]),
        ),
        wishlist: Object.fromEntries(
          Object.entries(state.wishlist).map(([userId, items]) => [
            userId,
            items.filter((id) => id !== action.productId),
          ]),
        ),
      };
    case "ADMIN_UPDATE_ORDER_STATUS":
      return {
        ...state,
        orders: state.orders.map((order) =>
          order.id === action.orderId ? { ...order, status: action.status } : order,
        ),
      };
    case "ADMIN_TOGGLE_BLOCK":
      return {
        ...state,
        users: state.users.map((user) =>
          user.id === action.userId ? { ...user, blocked: !user.blocked } : user,
        ),
      };
    case "ADMIN_TOGGLE_ROLE":
      return {
        ...state,
        users: state.users.map((user) =>
          user.id === action.userId
            ? { ...user, role: user.role === "admin" ? "user" : "admin" }
            : user,
        ),
      };
    case "ADMIN_DELETE_USER":
      return {
        ...state,
        users: state.users.filter((user) => user.id !== action.userId),
        orders: state.orders.filter((order) => order.userId !== action.userId),
        carts: Object.fromEntries(Object.entries(state.carts).filter(([id]) => id !== action.userId)),
        wishlist: Object.fromEntries(Object.entries(state.wishlist).filter(([id]) => id !== action.userId)),
      };
    case "UPDATE_SETTINGS":
      return { ...state, settings: { ...state.settings, ...action.settings } };
    case "RESET_DEMO":
      return cloneInitialData();
    default:
      return state;
  }
};

export function StoreProvider({ children }) {
  const [state, dispatch] = useReducer(reducer, undefined, loadState);

  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
  }, [state]);

  useEffect(() => {
    document.documentElement.classList.toggle("dark", state.theme === "dark");
  }, [state.theme]);

  const currentUser = state.users.find((user) => user.id === state.currentUserId) || null;
  const isAuthenticated = Boolean(currentUser);
  const isAdmin = currentUser?.role === "admin";
  const currentUserKey = currentUser?.id || "guest";

  const cartItems = (state.carts[currentUser?.id] || [])
    .map((item) => ({
      ...item,
      product: state.products.find((product) => product.id === item.productId),
    }))
    .filter((item) => item.product);

  const wishlistProducts = (state.wishlist[currentUser?.id] || [])
    .map((id) => state.products.find((product) => product.id === id))
    .filter(Boolean);

  const recentlyViewedProducts = (state.recentlyViewed[currentUserKey] || [])
    .map((id) => state.products.find((product) => product.id === id))
    .filter(Boolean);

  const currentUserOrders = state.orders.filter((order) => order.userId === currentUser?.id);

  const analytics = useMemo(() => {
    const validOrders = state.orders.filter((order) => !["Cancelled", "Refunded"].includes(order.status));
    const revenue = validOrders.reduce((sum, order) => sum + Number(order.total), 0);
    const lowStock = state.products.filter((product) => product.stock <= 8);
    const topProducts = [...state.products].sort((a, b) => b.sales - a.sales).slice(0, 6);
    return {
      revenue,
      totalOrders: state.orders.length,
      totalUsers: state.users.filter((user) => user.role === "user").length,
      lowStock,
      topProducts,
      recentOrders: state.orders.slice(0, 6),
    };
  }, [state.orders, state.products, state.users]);

  const actions = useMemo(
    () => ({
      login(email, password) {
        const user = state.users.find(
          (item) => item.email.toLowerCase() === email.trim().toLowerCase() && item.password === password,
        );
        if (!user) {
          toast.error("Invalid email or password");
          return null;
        }
        if (user.blocked) {
          toast.error("This account is blocked. Contact support.");
          return null;
        }
        dispatch({ type: "LOGIN", userId: user.id });
        toast.success(`Welcome back, ${user.name}`);
        return user;
      },
      register(form) {
        const exists = state.users.some((user) => user.email.toLowerCase() === form.email.trim().toLowerCase());
        if (exists) {
          toast.error("An account with this email already exists");
          return null;
        }
        const user = {
          id: makeId("u"),
          name: form.name,
          email: form.email,
          password: form.password,
          phone: form.phone,
          address: form.address,
          role: "user",
          joinedAt: new Date().toISOString().slice(0, 10),
          blocked: false,
          addresses: [
            {
              id: makeId("addr"),
              label: "Default",
              fullName: form.name,
              line1: form.address,
              line2: "",
              city: "",
              state: "",
              zip: "",
              country: "Bangladesh",
            },
          ],
        };
        dispatch({ type: "REGISTER", user });
        toast.success("Account created successfully");
        return user;
      },
      logout() {
        dispatch({ type: "LOGOUT" });
        toast.success("Logged out");
      },
      setTheme(theme) {
        dispatch({ type: "SET_THEME", theme });
      },
      addToCart(productId, quantity = 1) {
        if (!currentUser) {
          toast.error("Please log in to add products to cart");
          return false;
        }
        dispatch({ type: "ADD_TO_CART", userId: currentUser.id, productId, quantity });
        toast.success("Added to cart");
        return true;
      },
      updateCartQty(productId, quantity) {
        if (!currentUser) return;
        dispatch({ type: "UPDATE_CART_QTY", userId: currentUser.id, productId, quantity });
      },
      removeFromCart(productId) {
        if (!currentUser) return;
        dispatch({ type: "REMOVE_FROM_CART", userId: currentUser.id, productId });
        toast.success("Removed from cart");
      },
      saveForLater(productId) {
        if (!currentUser) return;
        dispatch({ type: "MOVE_CART_TO_WISHLIST", userId: currentUser.id, productId });
        toast.success("Saved for later");
      },
      toggleWishlist(productId) {
        if (!currentUser) {
          toast.error("Please log in to use wishlist");
          return false;
        }
        dispatch({ type: "TOGGLE_WISHLIST", userId: currentUser.id, productId });
        return true;
      },
      addRecentView(productId) {
        dispatch({ type: "ADD_RECENT_VIEW", userId: currentUserKey, productId });
      },
      placeOrder(orderInput) {
        if (!currentUser) return null;
        const order = {
          ...orderInput,
          id: `ORD-${Math.floor(10000 + Math.random() * 89999)}`,
          userId: currentUser.id,
          customerName: currentUser.name,
          date: new Date().toISOString().slice(0, 10),
          status: "Pending",
        };
        dispatch({ type: "PLACE_ORDER", order });
        toast.success("Order placed successfully");
        return order;
      },
      addReview(productId, rating, comment) {
        if (!currentUser) {
          toast.error("Please log in to write a review");
          return false;
        }
        const review = {
          id: makeId("review"),
          productId,
          userId: currentUser.id,
          name: currentUser.name,
          rating: Number(rating),
          date: new Date().toISOString().slice(0, 10),
          comment,
        };
        dispatch({ type: "ADD_REVIEW", review });
        toast.success("Review published");
        return true;
      },
      updateProfile(profile) {
        if (!currentUser) return;
        dispatch({ type: "UPDATE_PROFILE", userId: currentUser.id, profile });
        toast.success("Profile updated");
      },
      addAddress(address) {
        if (!currentUser) return;
        dispatch({ type: "ADD_ADDRESS", userId: currentUser.id, address: { ...address, id: makeId("addr") } });
        toast.success("Address added");
      },
      updateAddress(address) {
        if (!currentUser) return;
        dispatch({ type: "UPDATE_ADDRESS", userId: currentUser.id, address });
        toast.success("Address updated");
      },
      deleteAddress(addressId) {
        if (!currentUser) return;
        dispatch({ type: "DELETE_ADDRESS", userId: currentUser.id, addressId });
        toast.success("Address removed");
      },
      adminAddProduct(product) {
        dispatch({
          type: "ADMIN_ADD_PRODUCT",
          product: {
            ...product,
            id: makeId("p"),
            price: Number(product.price),
            originalPrice: Number(product.originalPrice || product.price),
            stock: Number(product.stock),
            rating: Number(product.rating || 4.5),
            sales: Number(product.sales || 0),
            images: product.images?.length ? product.images : [product.image],
            specs: product.specs || {},
            features: product.features || [],
            createdAt: new Date().toISOString().slice(0, 10),
          },
        });
        toast.success("Product added");
      },
      adminUpdateProduct(product) {
        dispatch({
          type: "ADMIN_UPDATE_PRODUCT",
          product: {
            ...product,
            price: Number(product.price),
            originalPrice: Number(product.originalPrice || product.price),
            stock: Number(product.stock),
            rating: Number(product.rating || 4.5),
          },
        });
        toast.success("Product updated");
      },
      adminDeleteProduct(productId) {
        dispatch({ type: "ADMIN_DELETE_PRODUCT", productId });
        toast.success("Product deleted");
      },
      adminUpdateOrderStatus(orderId, status) {
        dispatch({ type: "ADMIN_UPDATE_ORDER_STATUS", orderId, status });
        toast.success("Order status updated");
      },
      adminToggleBlock(userId) {
        dispatch({ type: "ADMIN_TOGGLE_BLOCK", userId });
        toast.success("User status updated");
      },
      adminToggleRole(userId) {
        dispatch({ type: "ADMIN_TOGGLE_ROLE", userId });
        toast.success("User role updated");
      },
      adminDeleteUser(userId) {
        if (userId === "u-admin") {
          toast.error("Primary admin cannot be deleted in demo mode");
          return;
        }
        dispatch({ type: "ADMIN_DELETE_USER", userId });
        toast.success("User deleted");
      },
      updateSettings(settings) {
        dispatch({ type: "UPDATE_SETTINGS", settings });
        toast.success("Settings saved");
      },
      resetDemoData() {
        dispatch({ type: "RESET_DEMO" });
        toast.success("Demo data reset");
      },
    }),
    [currentUser, currentUserKey, state.users],
  );

  const getProduct = (id) => state.products.find((product) => product.id === id);
  const getProductReviews = (id) => state.reviews.filter((review) => review.productId === id);
  const getBrandProducts = (brandName) => state.products.filter((product) => product.brand === brandName);
  const getOrder = (orderId) => state.orders.find((order) => order.id === orderId);

  const getRecommendations = (productId, limit = 4) => {
    const seedProduct = productId ? getProduct(productId) : recentlyViewedProducts[0];
    const userSignals = [
      ...(state.recentlyViewed[currentUserKey] || []),
      ...(state.wishlist[currentUser?.id] || []),
      ...currentUserOrders.flatMap((order) => order.items.map((item) => item.productId)),
    ];
    const signalProducts = userSignals.map(getProduct).filter(Boolean);
    const categories = seedProduct
      ? [seedProduct.category]
      : unique(signalProducts.map((product) => product.category)).slice(0, 2);
    const fromCategory = state.products.filter(
      (product) => categories.includes(product.category) && product.id !== productId,
    );
    const trending = [...state.products].sort((a, b) => b.sales - a.sales);
    return unique([...fromCategory, ...trending].map((product) => product.id))
      .map(getProduct)
      .filter(Boolean)
      .slice(0, limit);
  };

  const value = {
    state,
    currentUser,
    currentUserKey,
    isAuthenticated,
    isAdmin,
    cartItems,
    cartCount: cartItems.reduce((sum, item) => sum + item.quantity, 0),
    wishlistProducts,
    recentlyViewedProducts,
    currentUserOrders,
    analytics,
    getProduct,
    getProductReviews,
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
