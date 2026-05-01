import { ProductCard } from "../common";

export function RecommendationSection({ title, note, products }) {
  return (
    <section className="py-8">
      <div className="mb-5 flex flex-wrap items-end justify-between gap-3">
        <div>
          <h2 className="text-2xl font-black text-slate-950 dark:text-white">{title}</h2>
          <p className="text-slate-600 dark:text-slate-300">{note}</p>
        </div>
      </div>
      <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
        {products.map((product) => (
          <div key={product.id} className="relative">
            <ProductCard product={product} compact />
            <span className="absolute right-3 top-3 rounded-full bg-teal-600 px-2.5 py-1 text-xs font-bold text-white">Because you viewed similar items</span>
          </div>
        ))}
      </div>
    </section>
  );
}
