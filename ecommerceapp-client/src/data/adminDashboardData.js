export const salesData = [
  { day: "Mon", last7: 1240, last30: 980 },
  { day: "Tue", last7: 1480, last30: 1100 },
  { day: "Wed", last7: 1725, last30: 1260 },
  { day: "Thu", last7: 1560, last30: 1180 },
  { day: "Fri", last7: 2100, last30: 1450 },
  { day: "Sat", last7: 2450, last30: 1680 },
  { day: "Sun", last7: 2310, last30: 1590 },
];

export const forecastData = [
  { month: "May", predicted: 24200, upper: 27600, lower: 21800 },
  { month: "Jun", predicted: 26800, upper: 30400, lower: 23300 },
  { month: "Jul", predicted: 29300, upper: 33700, lower: 25000 },
];

export const demandData = [
  { category: "Electronics", demand: 860 },
  { category: "Clothing", demand: 520 },
  { category: "Books", demand: 430 },
  { category: "Home", demand: 610 },
  { category: "Sports", demand: 690 },
];

export const seasonalData = [
  { week: "W1", Electronics: 64, Clothing: 48, Sports: 58 },
  { week: "W2", Electronics: 72, Clothing: 55, Sports: 63 },
  { week: "W3", Electronics: 78, Clothing: 61, Sports: 70 },
  { week: "W4", Electronics: 83, Clothing: 74, Sports: 76 },
];

export const emptyProduct = {
  name: "",
  description: "",
  price: 0,
  originalPrice: 0,
  stock: 0,
  category: "Electronics",
  brand: "NovaTech",
  image: "https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&w=900&q=80",
  sku: "",
  rating: 4.5,
  sales: 0,
  featuresText: "",
};
