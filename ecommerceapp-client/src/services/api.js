import apiClient from './apiClient';

const mapProduct = (p) => ({
  id: `p-${p.id}`,
  name: p.name,
  category: p.categoryName || '',
  brand: p.brandName || '',
  price: p.price,
  originalPrice: p.compareAtPrice || p.price,
  rating: p.averageRating || 0,
  stock: p.stockQuantity,
  sku: p.sku || '',
  createdAt: '', // Not available in list endpoint
  sales: p.salesCount || 0,
  image: p.mainImageUrl || '',
  images: [],
  description: p.description || '',
  specs: {},
  features: [],
});

const mapProductDetail = (p) => ({
  id: `p-${p.id}`,
  name: p.name,
  category: p.categoryName || '',
  brand: p.brandName || '',
  price: p.price,
  originalPrice: p.compareAtPrice || p.price,
  rating: p.averageRating || 0,
  stock: p.stockQuantity,
  sku: p.sku || '',
  createdAt: '',
  sales: p.salesCount || 0,
  image: p.mainImageUrl || '',
  images: (p.images || []).map((img) => img.imageUrl).filter(Boolean),
  description: p.description || '',
  specs: parseSpecs(p.specs),
  features: parseFeatures(p.features),
});

const parseSpecs = (specsJson) => {
  if (!specsJson) return {};
  try {
    return typeof specsJson === 'string' ? JSON.parse(specsJson) : specsJson;
  } catch {
    return {};
  }
};

const parseFeatures = (featuresJson) => {
  if (!featuresJson) return [];
  try {
    const parsed = typeof featuresJson === 'string' ? JSON.parse(featuresJson) : featuresJson;
    return Array.isArray(parsed) ? parsed : [];
  } catch {
    return [];
  }
};

const mapUser = (u) => ({
  id: `u-${u.id}`,
  name: u.name,
  email: u.email,
  password: '',
  phone: u.phoneNumber || '',
  address: '',
  role: u.role?.toLowerCase() || 'user',
  joinedAt: '',
  blocked: false,
  addresses: [],
});

const mapOrder = (o) => ({
  id: o.orderNumber,
  userId: o.userId ? (typeof o.userId === 'number' ? `u-${o.userId}` : o.userId) : '',
  customerName: o.customerName || '',
  date: o.createdAt?.slice(0, 10) || '',
  total: o.totalAmount,
  status: o.status,
  paymentMethod: o.paymentMethod || '',
  items: o.items?.map(mapOrderItem) || [],
  shippingAddress: o.shippingAddress || null,
  tracking: '',
});

const mapOrderDetail = (o) => ({
  id: o.orderNumber,
  userId: '',
  customerName: '',
  date: o.createdAt?.slice(0, 10) || '',
  total: o.totalAmount,
  status: o.status,
  paymentMethod: o.paymentMethod || '',
  items: (o.items || []).map(mapOrderItem),
  shippingAddress: o.shippingAddress || null,
  tracking: o.trackingNumber || '',
});

const mapOrderItem = (i) => ({
  productId: `p-${i.productId}`,
  name: i.productName,
  price: i.unitPrice,
  quantity: i.quantity,
  image: i.productImageUrl || '',
});

export const authService = {
  login: async (email, password) => {
    const { data } = await apiClient.post('/auth/login', { email, password });
    const result = data.data;
    localStorage.setItem('accessToken', result.accessToken);
    localStorage.setItem('refreshToken', result.refreshToken);
    localStorage.setItem('user', JSON.stringify(result.user));
    return { ...mapUser(result.user), password };
  },

  register: async (form) => {
    const { data } = await apiClient.post('/auth/register', {
      name: form.name,
      email: form.email,
      password: form.password,
      confirmPassword: form.password,
      address: form.address || '',
      phoneNumber: form.phone || '',
    });
    const result = data.data;
    localStorage.setItem('accessToken', result.accessToken);
    localStorage.setItem('refreshToken', result.refreshToken);
    localStorage.setItem('user', JSON.stringify(result.user));
    return { ...mapUser(result.user), password: form.password };
  },

  logout: async () => {
    const refreshToken = localStorage.getItem('refreshToken');
    if (refreshToken) {
      try {
        await apiClient.post('/auth/logout', { refreshToken });
      } catch { /* ignore */ }
    }
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  },

  getProfile: async () => {
    const stored = localStorage.getItem('user');
    if (!stored) return null;
    try {
      const { data } = await apiClient.get('/auth/profile');
      localStorage.setItem('user', JSON.stringify(data.data));
      return mapUser(data.data);
    } catch {
      const parsed = JSON.parse(stored);
      return { ...parsed, id: `u-${parsed.id}` };
    }
  },

  changePassword: async (currentPassword, newPassword) => {
    await apiClient.post('/auth/change-password', { currentPassword, newPassword });
  },

  forgotPassword: async (email) => {
    await apiClient.post('/auth/forgot-password', { email });
  },

  resetPassword: async (email, token, newPassword) => {
    await apiClient.post('/auth/reset-password', { email, token, newPassword });
  },
};

export const productService = {
  getProducts: async (params = {}) => {
    const { data } = await apiClient.get('/products', { params });
    const paginated = data.data;
    return {
      items: (paginated.items || []).map(mapProduct),
      page: paginated.page || 1,
      pageSize: paginated.pageSize || 12,
      totalItems: paginated.totalItems || 0,
      totalPages: paginated.totalPages || 1,
    };
  },

  getProduct: async (id) => {
    const numericId = parseInt(id.replace('p-', ''), 10);
    const { data } = await apiClient.get(`/products/${numericId}`);
    return mapProductDetail(data.data);
  },

  getProductsByCategory: async (categoryId) => {
    const { data } = await apiClient.get(`/products/category/${categoryId}`);
    return (data.data || []).map(mapProduct);
  },

  getProductsByBrand: async (brandId) => {
    const { data } = await apiClient.get(`/products/brand/${brandId}`);
    return (data.data || []).map(mapProduct);
  },
};

export const categoryService = {
  getCategories: async () => {
    const { data } = await apiClient.get('/categories');
    return data.data || [];
  },
  getCategory: async (id) => {
    const { data } = await apiClient.get(`/categories/${id}`);
    return data.data;
  },
};

const mapBrand = (b) => ({
  id: b.slug || String(b.id),
  name: b.name,
  logo: b.logoUrl || b.name?.charAt(0).toUpperCase(),
  rating: b.averageRating || 0,
  story: b.description || '',
  contact: b.website || '',
  productCount: b.productCount || 0,
});

export const brandService = {
  getBrands: async () => {
    const { data } = await apiClient.get('/brands');
    return (data.data || []).map(mapBrand);
  },
  getBrand: async (id) => {
    const { data } = await apiClient.get(`/brands/${id}`);
    return mapBrand(data.data);
  },
};

export const cartService = {
  getCart: async () => {
    const { data } = await apiClient.get('/cart');
    const cart = data.data;
    return {
      items: (cart.items || []).map((i) => ({
        productId: `p-${i.productId}`,
        quantity: i.quantity,
        product: {
          id: `p-${i.productId}`,
          name: i.productName,
          price: i.unitPrice,
          originalPrice: i.unitPrice,
          image: i.productImageUrl || '',
          stock: i.maxStock || 999,
          rating: 0,
          category: '',
          brand: '',
        },
      })),
      subtotal: cart.subtotal || 0,
      totalItems: cart.totalItems || 0,
    };
  },

  getCartCount: async () => {
    try {
      const { data } = await apiClient.get('/cart/count');
      return data.data || 0;
    } catch {
      return 0;
    }
  },

  addToCart: async (productId, quantity = 1) => {
    const numericId = parseInt(productId.replace('p-', ''), 10);
    const { data } = await apiClient.post('/cart', { productId: numericId, quantity });
    return data.data;
  },

  updateCartQty: async (productId, quantity) => {
    const numericId = parseInt(productId.replace('p-', ''), 10);
    await apiClient.put(`/cart/${numericId}`, { quantity });
  },

  removeFromCart: async (productId) => {
    const numericId = parseInt(productId.replace('p-', ''), 10);
    await apiClient.delete(`/cart/${numericId}`);
  },

  clearCart: async () => {
    await apiClient.delete('/cart');
  },
};

export const wishlistService = {
  getWishlist: async () => {
    const { data } = await apiClient.get('/wishlist');
    return (data.data || []).map(mapProduct);
  },

  toggleWishlist: async (productId) => {
    const numericId = parseInt(productId.replace('p-', ''), 10);
    const { data } = await apiClient.post(`/wishlist/${numericId}`);
    return data.success;
  },

  isInWishlist: async (productId) => {
    const numericId = parseInt(productId.replace('p-', ''), 10);
    const { data } = await apiClient.get(`/wishlist/check/${numericId}`);
    return data.data || false;
  },
};

export const orderService = {
  getQuote: async (cartItems, couponCode) => {
    const items = cartItems.map((i) => ({
      productId: parseInt(i.productId.replace('p-', ''), 10),
      quantity: i.quantity,
      unitPrice: i.product.price,
    }));
    const { data } = await apiClient.post('/orders/quote', {
      cartItems: items,
      couponCode: couponCode || null,
    });
    return data.data;
  },

  placeOrder: async (request) => {
    const payload = {
      addressId: request.addressId || null,
      shippingAddress: request.shippingAddress || null,
      paymentMethod: request.paymentMethod,
      couponCode: request.couponCode || null,
    };
    const { data } = await apiClient.post('/orders', payload);
    return mapOrderDetail(data.data);
  },

  getOrders: async () => {
    const { data } = await apiClient.get('/orders');
    return (data.data || []).map(mapOrder);
  },

  getOrder: async (orderId) => {
    const { data } = await apiClient.get(`/orders/track/${orderId}`);
    return mapOrderDetail(data.data);
  },
};

export const reviewService = {
  getProductReviews: async (productId) => {
    const numericId = parseInt(productId.replace('p-', ''), 10);
    const { data } = await apiClient.get(`/reviews/product/${numericId}`);
    return (data.data || []).map((r) => ({
      id: `r-${productId}-${r.id}`,
      productId,
      userId: `u-${r.userId}`,
      name: r.userName,
      rating: r.rating,
      date: r.createdAt?.slice(0, 10) || '',
      comment: r.comment,
    }));
  },

  addReview: async (productId, rating, comment) => {
    const numericId = parseInt(productId.replace('p-', ''), 10);
    const { data } = await apiClient.post('/reviews', {
      productId: numericId,
      rating: Number(rating),
      comment,
    });
    return {
      id: `r-${productId}-${data.data.id}`,
      productId,
      userId: `u-${data.data.userId}`,
      name: data.data.userName,
      rating: data.data.rating,
      date: data.data.createdAt?.slice(0, 10) || '',
      comment: data.data.comment,
    };
  },
};

export const addressService = {
  getAddresses: async () => {
    const { data } = await apiClient.get('/addresses');
    return (data.data || []).map((a) => ({
      id: `addr-${a.id}`,
      label: a.isDefault ? 'Default' : 'Address',
      fullName: a.fullName,
      line1: a.street,
      line2: '',
      city: a.city,
      state: a.state,
      zip: a.zipCode,
      country: a.country,
      phone: a.phoneNumber,
    }));
  },

  addAddress: async (address) => {
    const payload = {
      fullName: address.fullName,
      street: address.line1,
      city: address.city,
      state: address.state,
      zipCode: address.zip,
      country: address.country || 'Bangladesh',
      phoneNumber: address.phone || '',
      isDefault: false,
    };
    const { data } = await apiClient.post('/addresses', payload);
    const a = data.data;
    return {
      id: `addr-${a.id}`,
      label: a.isDefault ? 'Default' : 'Address',
      fullName: a.fullName,
      line1: a.street,
      line2: '',
      city: a.city,
      state: a.state,
      zip: a.zipCode,
      country: a.country,
      phone: a.phoneNumber,
    };
  },

  updateAddress: async (address) => {
    const numericId = parseInt(address.id.replace('addr-', ''), 10);
    const payload = {
      fullName: address.fullName,
      street: address.line1,
      city: address.city,
      state: address.state,
      zipCode: address.zip,
      country: address.country || 'Bangladesh',
      phoneNumber: address.phone || '',
      isDefault: false,
    };
    const { data } = await apiClient.put(`/addresses/${numericId}`, payload);
    const a = data.data;
    return {
      id: `addr-${a.id}`,
      label: a.isDefault ? 'Default' : 'Address',
      fullName: a.fullName,
      line1: a.street,
      line2: '',
      city: a.city,
      state: a.state,
      zip: a.zipCode,
      country: a.country,
      phone: a.phoneNumber,
    };
  },

  deleteAddress: async (addressId) => {
    const numericId = parseInt(addressId.replace('addr-', ''), 10);
    await apiClient.delete(`/addresses/${numericId}`);
  },

  setDefaultAddress: async (addressId) => {
    const numericId = parseInt(addressId.replace('addr-', ''), 10);
    await apiClient.put(`/addresses/${numericId}/default`);
  },
};

export const adminService = {
  getDashboard: async () => {
    const { data } = await apiClient.get('/admin/dashboard');
    const d = data.data;
    return {
      revenue: d.revenue || 0,
      totalOrders: d.totalOrders || 0,
      totalUsers: d.totalUsers || 0,
      totalProducts: d.totalProducts || 0,
      lowStock: (d.lowStockProducts || []).map(mapProduct),
      topProducts: (d.topProducts || []).map(mapProduct),
      recentOrders: (d.recentOrders || []).map(mapOrder),
      salesData: (d.salesData || []).map((s) => ({ date: s.date, revenue: s.revenue, orders: s.orders })),
    };
  },

  getOrders: async () => {
    const { data } = await apiClient.get('/admin/orders');
    return (data.data || []).map(mapOrder);
  },

  getUsers: async (page = 1, pageSize = 20, search = '') => {
    const { data } = await apiClient.get('/admin/users', { params: { page, pageSize, search } });
    const d = data.data;
    return {
      users: (d.users || []).map(mapUser),
      totalUsers: d.totalUsers || 0,
      page: d.page || 1,
      pageSize: d.pageSize || 20,
    };
  },

  toggleBlock: async (userId) => {
    const numericId = parseInt(userId.replace('u-', ''), 10);
    await apiClient.put(`/admin/users/${numericId}/toggle-block`);
  },

  toggleRole: async (userId) => {
    const numericId = parseInt(userId.replace('u-', ''), 10);
    await apiClient.put(`/admin/users/${numericId}/toggle-role`);
  },

  deleteUser: async (userId) => {
    const numericId = parseInt(userId.replace('u-', ''), 10);
    await apiClient.delete(`/admin/users/${numericId}`);
  },

  createProduct: async (product) => {
    const payload = {
      name: product.name,
      description: product.description || '',
      price: Number(product.price),
      compareAtPrice: product.originalPrice ? Number(product.originalPrice) : null,
      stockQuantity: Number(product.stock),
      categoryId: product.categoryId || null,
      brandId: product.brandId || null,
      mainImageUrl: product.image || '',
      sku: product.sku || '',
      isFeatured: false,
      specs: typeof product.specs === 'object' ? JSON.stringify(product.specs) : (product.specs || null),
      features: Array.isArray(product.features) ? JSON.stringify(product.features) : (product.features || null),
    };
    const { data } = await apiClient.post('/admin/products', payload);
    return mapProduct(data.data);
  },

  updateProduct: async (product) => {
    const numericId = parseInt(product.id.replace('p-', ''), 10);
    const payload = {
      name: product.name,
      description: product.description || '',
      price: Number(product.price),
      compareAtPrice: product.originalPrice ? Number(product.originalPrice) : null,
      stockQuantity: Number(product.stock),
      categoryId: product.categoryId || null,
      brandId: product.brandId || null,
      mainImageUrl: product.image || '',
      sku: product.sku || '',
      isActive: true,
      isFeatured: false,
      specs: typeof product.specs === 'object' ? JSON.stringify(product.specs) : (product.specs || null),
      features: Array.isArray(product.features) ? JSON.stringify(product.features) : (product.features || null),
    };
    const { data } = await apiClient.put(`/admin/products/${numericId}`, payload);
    return mapProduct(data.data);
  },

  deleteProduct: async (productId) => {
    const numericId = parseInt(productId.replace('p-', ''), 10);
    await apiClient.delete(`/admin/products/${numericId}`);
  },

  updateOrderStatus: async (orderId, status, note) => {
    const { data } = await apiClient.put(`/admin/orders/${orderId}/status`, { status, note });
    return mapOrderDetail(data.data);
  },

  getSettings: async () => {
    const { data } = await apiClient.get('/admin/settings');
    return data.data;
  },

  updateSettings: async (settings) => {
    await apiClient.put('/admin/settings', settings);
  },
};
