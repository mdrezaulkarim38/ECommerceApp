export const currency = new Intl.NumberFormat("en-BD", {
  style: "currency",
  currency: "BDT",
  maximumFractionDigits: 0
});

export function formatMoney(value) {
  return currency.format(Number(value || 0));
}

export function formatDate(value) {
  if (!value) return "";
  return new Intl.DateTimeFormat("en-BD", {
    dateStyle: "medium",
    timeStyle: "short"
  }).format(new Date(value));
}

export function imageUrl(path) {
  if (!path) return "/assets/product.png";
  if (path.startsWith("http")) return path;
  const baseUrl = import.meta.env.VITE_API_BASE_URL || "http://localhost:5099";
  return `${baseUrl}${path}`;
}

export function slugify(value) {
  return value
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/(^-|-$)/g, "");
}
