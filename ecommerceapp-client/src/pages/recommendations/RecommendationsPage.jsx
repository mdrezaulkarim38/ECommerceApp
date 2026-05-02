import { useState } from "react";
import { Link } from "react-router-dom";
import { RefreshCw, Sparkles } from "lucide-react";
import { RecommendationSection } from "../../components/recommendations/RecommendationSection";
import { Breadcrumbs } from "../../components/common";
import { useStore } from "../../context/StoreContext";

const randomize = (items, salt = 0) =>
  [...items].sort((a, b) => ((a.id.charCodeAt(2) + salt) % 7) - ((b.id.charCodeAt(2) + salt) % 7));

export function RecommendationsPage() {
  const { state, getRecommendations, recentlyViewedProducts } = useStore();
  const [seed, setSeed] = useState(1);
  const justForYou = randomize(getRecommendations(null, 8), seed);
  const trending = [...state.products].sort((a, b) => b.sales - a.sales).slice(0, 8);
  const seasonal = state.products.filter((product) => ["Sports", "Home & Living", "Clothing"].includes(product.category)).slice(0, 8);
  const bundles = [
    ["p-1002", "p-1005", "p-1010"],
    ["p-1004", "p-1014", "p-1016"],
    ["p-1001", "p-1006", "p-1011"],
  ].map((ids) => ids.map((id) => state.products.find((product) => product.id === id)).filter(Boolean));

  return (
    <>
      <Breadcrumbs current="AI Recommendations" />
      <main className="mx-auto max-w-7xl px-4 py-8">
        <div className="mb-8 flex flex-wrap items-end justify-between gap-4">
          <div>
            <span className="inline-flex items-center gap-2 rounded-full bg-teal-50 px-4 py-2 text-sm font-bold text-teal-700 dark:bg-teal-500/15 dark:text-teal-200">
              <Sparkles size={16} /> AI Recommendation Hub
            </span>
            <h1 className="mt-4 text-4xl font-black text-slate-950 dark:text-white">Personalized product discovery</h1>
          </div>
          <button className="btn-primary" type="button" onClick={() => setSeed((value) => value + 1)}>
            <RefreshCw size={18} /> Refresh Recommendations
          </button>
        </div>
        <RecommendationSection title="Just For You" note="Because of your browsing and wishlist signals" products={justForYou} />
        <RecommendationSection title="Trending Now" note="Popular products this week" products={trending} />
        <RecommendationSection title="Seasonal Picks" note="Relevant for current shopping patterns" products={seasonal} />
        <section className="py-8">
          <h2 className="text-2xl font-black text-slate-950 dark:text-white">Complete The Look</h2>
          <div className="mt-5 grid gap-5 lg:grid-cols-3">
            {bundles.map((bundle, index) => (
              <div key={index} className="rounded-2xl border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
                <p className="mb-4 font-bold text-teal-700 dark:text-teal-300">Bundle {index + 1}</p>
                <div className="grid gap-3">
                  {bundle.map((product) => (
                    <Link key={product.id} to={`/products/${product.id}`} className="flex items-center gap-3 rounded-xl bg-slate-50 p-3 dark:bg-slate-800">
                      <img src={product.image} alt="" className="h-14 w-14 rounded-lg object-cover" />
                      <span className="font-bold text-slate-950 dark:text-white">{product.name}</span>
                    </Link>
                  ))}
                </div>
              </div>
            ))}
          </div>
        </section>
        <RecommendationSection title="Recently Viewed" note="Continue browsing from your history" products={recentlyViewedProducts} />
      </main>
    </>
  );
}
