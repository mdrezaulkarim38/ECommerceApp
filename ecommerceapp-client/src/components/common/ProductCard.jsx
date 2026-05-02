import { Link } from "react-router-dom";
import { motion } from "framer-motion";
import { Heart, ShoppingCart } from "lucide-react";
import { useStore } from "../../context/StoreContext";
import { formatCurrency } from "../../utils/pricing";
import { Stars } from "./Stars";

export function ProductCard({ product, compact = false }) {
  const { isAuthenticated, wishlistProducts, actions } = useStore();
  const wished = wishlistProducts.some((item) => item.id === product.id);
  const discount = Math.round(((product.originalPrice - product.price) / product.originalPrice) * 100);

  return (
    <motion.article
      layout
      whileHover={{ y: -4 }}
      className="group overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm transition hover:shadow-xl dark:border-slate-800 dark:bg-slate-900"
    >
      <Link to={`/products/${product.id}`} className="relative block aspect-[4/3] overflow-hidden bg-slate-100">
        <img
          src={product.image}
          alt={product.name}
          className="h-full w-full object-cover transition duration-500 group-hover:scale-105"
          loading="lazy"
        />
        {discount > 0 && (
          <span className="absolute left-3 top-3 rounded-full bg-rose-600 px-2.5 py-1 text-xs font-bold text-white">
            {discount}% OFF
          </span>
        )}
        {product.stock <= 6 && (
          <span className="absolute bottom-3 left-3 rounded-full bg-amber-500 px-2.5 py-1 text-xs font-bold text-white">
            Low stock
          </span>
        )}
      </Link>
      <div className="space-y-3 p-4">
        <div>
          <div className="flex items-center justify-between gap-2 text-xs font-semibold uppercase tracking-wide text-teal-700 dark:text-teal-300">
            <span>{product.category}</span>
            <span>{product.brand}</span>
          </div>
          <Link
            to={`/products/${product.id}`}
            className="mt-1 line-clamp-2 min-h-[44px] text-base font-bold text-slate-950 hover:text-teal-700 dark:text-white dark:hover:text-teal-300"
          >
            {product.name}
          </Link>
        </div>
        {!compact && <Stars value={product.rating} showValue />}
        <div className="flex items-end justify-between gap-3">
          <div>
            <div className="text-lg font-black text-slate-950 dark:text-white">{formatCurrency(product.price)}</div>
            <div className="text-xs text-slate-500 line-through">{formatCurrency(product.originalPrice)}</div>
          </div>
          {isAuthenticated ? (
            <div className="flex gap-2">
              <button
                type="button"
                onClick={() => actions.toggleWishlist(product.id)}
                className={`grid h-10 w-10 place-items-center rounded-full border transition ${
                  wished
                    ? "border-rose-200 bg-rose-50 text-rose-600 dark:border-rose-500/30 dark:bg-rose-500/15"
                    : "border-slate-200 text-slate-500 hover:border-rose-300 hover:text-rose-600 dark:border-slate-700"
                }`}
                aria-label="Add to wishlist"
              >
                <Heart size={18} className={wished ? "fill-current" : ""} />
              </button>
              <button
                type="button"
                onClick={() => actions.addToCart(product.id)}
                className="inline-flex items-center gap-2 rounded-full bg-slate-950 px-4 py-2 text-sm font-bold text-white transition hover:bg-teal-700 dark:bg-teal-500 dark:text-slate-950"
              >
                <ShoppingCart size={17} /> Add
              </button>
            </div>
          ) : (
            <Link
              to="/auth"
              className="rounded-full bg-slate-950 px-4 py-2 text-sm font-bold text-white transition hover:bg-teal-700 dark:bg-teal-500 dark:text-slate-950"
            >
              Login to Buy
            </Link>
          )}
        </div>
      </div>
    </motion.article>
  );
}
