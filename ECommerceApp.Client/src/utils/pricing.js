export const formatCurrency = (value) =>
  new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" }).format(Number(value || 0));

export const calculateCartTotals = (cartItems, coupon = "") => {
  const subtotal = cartItems.reduce((sum, item) => sum + item.product.price * item.quantity, 0);
  const shipping = subtotal > 200 || subtotal === 0 ? 0 : 8.5;
  const tax = subtotal * 0.08;
  const discount = coupon.trim().toUpperCase() === "SMART10" ? subtotal * 0.1 : 0;
  return {
    subtotal,
    shipping,
    tax,
    discount,
    total: Math.max(0, subtotal + shipping + tax - discount),
  };
};
