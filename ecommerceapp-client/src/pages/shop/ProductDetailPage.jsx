import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { Heart, Minus, Plus, ShoppingCart } from "lucide-react";
import { Breadcrumbs, EmptyState, ProductRow, Stars } from "../../components/common";
import { useStore } from "../../context/StoreContext";
import { formatCurrency } from "../../utils/pricing";

export function ProductDetailPage() {
  const { productId } = useParams();
  const { getProduct, getProductReviews, fetchProductReviews, getRecommendations, isAuthenticated, wishlistProducts, actions } = useStore();
  const product = getProduct(productId);
  const reviews = getProductReviews(productId);
  const [image, setImage] = useState(product?.image);
  const [quantity, setQuantity] = useState(1);
  const [review, setReview] = useState({ rating: 5, comment: "" });

  useEffect(() => {
    if (productId) fetchProductReviews(productId);
  }, [productId, fetchProductReviews]);

  if (!product) return <EmptyState title="Product not found" message="This item may have been removed from the demo catalog." />;

  const wished = wishlistProducts.some((item) => item.id === product.id);
  const breakdown = [5, 4, 3, 2, 1].map((rating) => ({
    rating,
    count: reviews.filter((item) => Math.round(item.rating) === rating).length,
  }));

  return (
    <>
      <Breadcrumbs current={product.name} />
      <main className="mx-auto max-w-7xl px-4 py-8">
        <section className="grid gap-8 lg:grid-cols-[1fr_0.9fr]">
          <div className="grid gap-4 md:grid-cols-[100px_1fr]">
            <div className="order-2 flex gap-3 md:order-1 md:flex-col">
              {product.images.map((item) => (
                <button
                  key={item}
                  type="button"
                  onClick={() => setImage(item)}
                  className={`aspect-square overflow-hidden rounded-xl border ${image === item ? "border-teal-500" : "border-slate-200 dark:border-slate-700"}`}
                >
                  <img src={item} alt="" className="h-full w-full object-cover" />
                </button>
              ))}
            </div>
            <div className="order-1 aspect-square overflow-hidden rounded-3xl bg-slate-100 md:order-2 dark:bg-slate-900">
              <img src={image || product.image} alt={product.name} className="h-full w-full object-cover transition duration-700 hover:scale-110" />
            </div>
          </div>
          <div className="space-y-6">
            <div>
              <p className="text-sm font-bold uppercase tracking-wide text-teal-700 dark:text-teal-300">
                {product.category} / {product.brand}
              </p>
              <h1 className="mt-2 text-4xl font-black text-slate-950 dark:text-white">{product.name}</h1>
              <div className="mt-3 flex flex-wrap items-center gap-3">
                <Stars value={product.rating} showValue />
                <span className="text-sm text-slate-500">{reviews.length} reviews</span>
                <span className={`rounded-full px-3 py-1 text-sm font-bold ${product.stock > 0 ? "bg-emerald-100 text-emerald-700" : "bg-rose-100 text-rose-700"}`}>
                  {product.stock > 0 ? `${product.stock} in stock` : "Out of stock"}
                </span>
              </div>
            </div>
            <div className="flex items-end gap-3">
              <span className="text-4xl font-black text-slate-950 dark:text-white">{formatCurrency(product.price)}</span>
              <span className="text-lg text-slate-400 line-through">{formatCurrency(product.originalPrice)}</span>
            </div>
            <p className="leading-7 text-slate-600 dark:text-slate-300">{product.description}</p>
            <div className="grid gap-3 sm:grid-cols-2">
              <div className="rounded-2xl bg-slate-50 p-4 dark:bg-slate-900">
                <p className="text-sm font-bold text-slate-500">SKU</p>
                <p className="font-bold text-slate-950 dark:text-white">{product.sku}</p>
              </div>
              <div className="rounded-2xl bg-slate-50 p-4 dark:bg-slate-900">
                <p className="text-sm font-bold text-slate-500">AI Signal</p>
                <p className="font-bold text-slate-950 dark:text-white">High demand, stable margin</p>
              </div>
            </div>
            <div>
              <p className="mb-2 text-sm font-bold text-slate-600 dark:text-slate-300">Quantity</p>
              <div className="inline-flex items-center rounded-full border border-slate-200 dark:border-slate-700">
                <button className="p-3" type="button" onClick={() => setQuantity(Math.max(1, quantity - 1))}>
                  <Minus size={16} />
                </button>
                <span className="w-12 text-center font-bold">{quantity}</span>
                <button className="p-3" type="button" onClick={() => setQuantity(Math.min(99, quantity + 1))}>
                  <Plus size={16} />
                </button>
              </div>
            </div>
            {isAuthenticated ? (
              <div className="flex flex-wrap gap-3">
                <button className="btn-primary" type="button" onClick={() => actions.addToCart(product.id, quantity)}>
                  <ShoppingCart size={18} /> Add to Cart
                </button>
                <Link className="btn-secondary" to="/checkout" onClick={() => actions.addToCart(product.id, quantity)}>
                  Buy Now
                </Link>
                <button className="btn-secondary" type="button" onClick={() => actions.toggleWishlist(product.id)}>
                  <Heart size={18} className={wished ? "fill-current text-rose-600" : ""} /> Wishlist
                </button>
              </div>
            ) : (
              <Link className="btn-primary" to="/auth">
                Login to Buy
              </Link>
            )}
          </div>
        </section>

        <section className="mt-12 grid gap-8 lg:grid-cols-[0.8fr_1.2fr]">
          <div className="rounded-2xl border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
            <h2 className="text-xl font-black text-slate-950 dark:text-white">Specifications</h2>
            <div className="mt-4 divide-y divide-slate-100 dark:divide-slate-800">
              {Object.entries(product.specs).map(([key, value]) => (
                <div key={key} className="flex justify-between gap-4 py-3 text-sm">
                  <span className="font-bold text-slate-500">{key}</span>
                  <span className="text-right text-slate-800 dark:text-slate-100">{value}</span>
                </div>
              ))}
            </div>
          </div>
          <div className="rounded-2xl border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
            <h2 className="text-xl font-black text-slate-950 dark:text-white">Rating Breakdown</h2>
            <div className="mt-4 space-y-3">
              {breakdown.map((item) => (
                <div key={item.rating} className="flex items-center gap-3">
                  <span className="w-10 text-sm font-bold">{item.rating} star</span>
                  <div className="h-2 flex-1 overflow-hidden rounded-full bg-slate-100 dark:bg-slate-800">
                    <div className="h-full rounded-full bg-amber-400" style={{ width: `${reviews.length ? (item.count / reviews.length) * 100 : 0}%` }} />
                  </div>
                  <span className="w-8 text-right text-sm text-slate-500">{item.count}</span>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="mt-12 grid gap-8 lg:grid-cols-[1fr_360px]">
          <div>
            <h2 className="text-2xl font-black text-slate-950 dark:text-white">Customer Reviews</h2>
            <div className="mt-4 grid gap-4">
              {reviews.map((item) => (
                <article key={item.id} className="rounded-2xl border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
                  <div className="flex flex-wrap items-center justify-between gap-3">
                    <div>
                      <p className="font-bold text-slate-950 dark:text-white">{item.name}</p>
                      <p className="text-sm text-slate-500">{item.date}</p>
                    </div>
                    <Stars value={item.rating} />
                  </div>
                  <p className="mt-3 text-slate-600 dark:text-slate-300">{item.comment}</p>
                </article>
              ))}
            </div>
          </div>
          <aside className="rounded-2xl border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
            <h3 className="text-xl font-black text-slate-950 dark:text-white">Write a Review</h3>
            {isAuthenticated ? (
              <form
                className="mt-4 space-y-4"
                onSubmit={(event) => {
                  event.preventDefault();
                  if (actions.addReview(product.id, review.rating, review.comment)) setReview({ rating: 5, comment: "" });
                }}
              >
                <select className="input" value={review.rating} onChange={(event) => setReview({ ...review, rating: event.target.value })}>
                  {[5, 4, 3, 2, 1].map((item) => (
                    <option key={item} value={item}>
                      {item} stars
                    </option>
                  ))}
                </select>
                <textarea
                  className="input min-h-28"
                  required
                  value={review.comment}
                  onChange={(event) => setReview({ ...review, comment: event.target.value })}
                  placeholder="Share your experience"
                />
                <button className="btn-primary w-full" type="submit">
                  Publish Review
                </button>
              </form>
            ) : (
              <Link className="btn-primary mt-4" to="/auth">
                Login to Review
              </Link>
            )}
          </aside>
        </section>
      </main>
      <ProductRow
        products={getRecommendations(product.id, 4)}
        title="Customers who bought this also bought"
        subtitle="Simulated using category similarity and collaborative purchase signals."
      />
    </>
  );
}
