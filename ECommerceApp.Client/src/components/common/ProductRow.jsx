import { ProductCard } from "./ProductCard";

export function ProductRow({ products, title, subtitle }) {
  if (!products?.length) return null;
  return (
    <section className="mx-auto w-full max-w-7xl px-4 py-10">
      <div className="mb-5 flex flex-wrap items-end justify-between gap-3">
        <div>
          <h2 className="text-2xl font-black text-slate-950 dark:text-white">{title}</h2>
          {subtitle && <p className="mt-1 text-slate-600 dark:text-slate-300">{subtitle}</p>}
        </div>
      </div>
      <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
        {products.slice(0, 4).map((product) => (
          <ProductCard key={product.id} product={product} compact />
        ))}
      </div>
    </section>
  );
}
