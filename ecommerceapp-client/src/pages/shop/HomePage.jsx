import { useEffect, useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { motion } from "framer-motion";
import { Filter, SlidersHorizontal, Sparkles } from "lucide-react";
import { ProductCard, ProductCardSkeletons, ProductRow, SearchInput } from "../../components/common";
import { useStore } from "../../context/StoreContext";
import { productService } from "../../services/api";

const sortMap = {
  Newest: { sortBy: "newest", sortOrder: "desc" },
  "Price: Low to High": { sortBy: "price", sortOrder: "asc" },
  "Price: High to Low": { sortBy: "price", sortOrder: "desc" },
  Rating: { sortBy: "rating", sortOrder: "desc" },
};

const heroSlides = [
  {
    title: "AI-picked deals for smarter shopping",
    copy: "Browse recommendations, compare products, and discover the highest-value offers from one polished storefront.",
    image: "https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?auto=format&fit=crop&w=1600&q=80",
    cta: "Explore Products",
    href: "#products",
  },
  {
    title: "Forecast-ready retail intelligence",
    copy: "A modern ecommerce front end with admin analytics, demand forecasting, and mock BI metrics built in.",
    image: "https://images.unsplash.com/photo-1551288049-bebda4e38f71?auto=format&fit=crop&w=1600&q=80",
    cta: "View AI Hub",
    href: "/recommendations",
  },
  {
    title: "Flash sales with live urgency",
    copy: "Discount badges, countdowns, inventory hints, and seasonal promotions make the demo feel alive.",
    image: "https://images.unsplash.com/photo-1607083206968-13611e3d76db?auto=format&fit=crop&w=1600&q=80",
    cta: "Shop Deals",
    href: "/deals",
  },
];

export function HomePage() {
  const { state, recentlyViewedProducts, loading: storeLoading } = useStore();
  const navigate = useNavigate();
  const [slide, setSlide] = useState(0);
  const [query, setQuery] = useState("");
  const [category, setCategory] = useState("All");
  const [sort, setSort] = useState("Newest");
  const [page, setPage] = useState(1);
  const [products, setProducts] = useState([]);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);
  const perPage = 8;

  const catNames = useMemo(() => (state.categories || []).map((c) => c.name || c), [state.categories]);
  const catByName = useMemo(() => {
    const map = {};
    (state.categories || []).forEach((c) => { map[c.name || c] = c.id; });
    return map;
  }, [state.categories]);

  useEffect(() => {
    const timer = setInterval(() => setSlide((value) => (value + 1) % heroSlides.length), 4500);
    return () => clearInterval(timer);
  }, []);

  useEffect(() => {
    const fetchProducts = async () => {
      setLoading(true);
      try {
        const categoryId = category === "All" ? null : catByName[category];
        const { sortBy, sortOrder } = sortMap[sort] || {};
        const result = await productService.getProducts({
          search: query || undefined,
          categoryId,
          sortBy,
          sortOrder,
          page,
          pageSize: perPage,
        });
        setProducts(result.items);
        setTotalPages(result.totalPages);
      } catch {
        setProducts([]);
        setTotalPages(1);
      }
      setLoading(false);
    };
    fetchProducts();
  }, [query, category, sort, page, catByName]);

  const activeHero = heroSlides[slide];
  const showLoading = storeLoading || loading;

  return (
    <main>
      <section className="relative min-h-[520px] overflow-hidden bg-slate-950 text-white">
        {heroSlides.map((item, index) => (
          <img
            key={item.title}
            src={item.image}
            alt=""
            className={`absolute inset-0 h-full w-full object-cover transition-opacity duration-700 ${
              index === slide ? "opacity-45" : "opacity-0"
            }`}
          />
        ))}
        <div className="relative z-10 mx-auto flex min-h-[520px] max-w-7xl flex-col justify-center px-4 pb-20 pt-16">
          <motion.div key={activeHero.title} initial={{ opacity: 0, y: 18 }} animate={{ opacity: 1, y: 0 }} className="max-w-2xl">
            <span className="inline-flex items-center gap-2 rounded-full bg-white/15 px-4 py-2 text-sm font-bold backdrop-blur">
              <Sparkles size={16} /> Predictive commerce experience
            </span>
            <h1 className="mt-6 text-4xl font-black leading-tight md:text-6xl">{activeHero.title}</h1>
            <p className="mt-5 max-w-xl text-lg text-slate-200">{activeHero.copy}</p>
            <div className="mt-8 flex flex-wrap gap-3">
              <Link className="btn-primary" to={activeHero.href}>
                {activeHero.cta}
              </Link>
              <Link className="btn-ghost-light" to="/compare">
                Compare Products
              </Link>
            </div>
          </motion.div>
          <div className="absolute bottom-5 left-1/2 flex -translate-x-1/2 gap-2">
            {heroSlides.map((item, index) => (
              <button
                key={item.title}
                type="button"
                onClick={() => setSlide(index)}
                className={`h-2.5 rounded-full transition-all ${index === slide ? "w-10 bg-teal-400" : "w-2.5 bg-white/60"}`}
                aria-label={`Show offer ${index + 1}`}
              />
            ))}
          </div>
        </div>
      </section>

      <section id="products" className="mx-auto max-w-7xl px-4 py-10">
        <div className="mb-6 grid gap-4 lg:grid-cols-[1fr_260px_220px]">
          <SearchInput
            value={query}
            onChange={(value) => {
              setQuery(value);
              setPage(1);
            }}
            suggestions={state.products.filter((product) => product.name.toLowerCase().includes(query.toLowerCase()))}
            onSelect={(product) => navigate(`/products/${product.id}`)}
          />
          <label className="relative">
            <Filter className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
            <select
              className="h-12 w-full appearance-none rounded-full border border-slate-200 bg-white pl-11 pr-4 text-sm font-semibold outline-none dark:border-slate-700 dark:bg-slate-900 dark:text-white"
              value={category}
              onChange={(event) => {
                setCategory(event.target.value);
                setPage(1);
              }}
            >
              <option>All</option>
              {catNames.map((item) => (
                <option key={item}>{item}</option>
              ))}
            </select>
          </label>
          <label className="relative">
            <SlidersHorizontal className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
            <select
              className="h-12 w-full appearance-none rounded-full border border-slate-200 bg-white pl-11 pr-4 text-sm font-semibold outline-none dark:border-slate-700 dark:bg-slate-900 dark:text-white"
              value={sort}
              onChange={(event) => {
                setSort(event.target.value);
                setPage(1);
              }}
            >
              {Object.keys(sortMap).map((item) => (
                <option key={item}>{item}</option>
              ))}
            </select>
          </label>
        </div>

        <div className="mb-6 flex flex-wrap gap-2">
          {["All", ...catNames].map((item) => (
            <button
              type="button"
              key={item}
              onClick={() => {
                setCategory(item);
                setPage(1);
              }}
              className={`rounded-full px-4 py-2 text-sm font-bold transition ${
                category === item
                  ? "bg-teal-600 text-white"
                  : "bg-white text-slate-600 ring-1 ring-slate-200 hover:bg-slate-50 dark:bg-slate-900 dark:text-slate-200 dark:ring-slate-700"
              }`}
            >
              {item}
            </button>
          ))}
        </div>

        {showLoading ? (
          <ProductCardSkeletons count={8} />
        ) : products.length === 0 ? (
          <div className="py-20 text-center">
            <p className="text-xl font-bold text-slate-400">No products found</p>
            <p className="mt-2 text-slate-500">Try adjusting your search or filters</p>
          </div>
        ) : (
          <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
            {products.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
        )}

        {totalPages > 1 && (
          <div className="mt-8 flex items-center justify-center gap-2">
            {Array.from({ length: totalPages }).map((_, index) => (
              <button
                key={index}
                type="button"
                onClick={() => setPage(index + 1)}
                className={`grid h-10 w-10 place-items-center rounded-full text-sm font-bold ${
                  page === index + 1
                    ? "bg-slate-950 text-white dark:bg-teal-500 dark:text-slate-950"
                    : "bg-white text-slate-600 ring-1 ring-slate-200 dark:bg-slate-900 dark:text-slate-200 dark:ring-slate-700"
                }`}
              >
                {index + 1}
              </button>
            ))}
          </div>
        )}
      </section>

      <ProductRow
        products={recentlyViewedProducts}
        title="Recently Viewed"
        subtitle="Products you have browsed recently."
      />
    </main>
  );
}
