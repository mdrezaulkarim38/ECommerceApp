import { Link } from "react-router-dom";
import { Breadcrumbs, Stars } from "../../components/common";
import { useStore } from "../../context/StoreContext";

export function BrandsPage() {
  const { state, getBrandProducts } = useStore();
  return (
    <>
      <Breadcrumbs current="Brands" />
      <main className="mx-auto max-w-7xl px-4 py-8">
        <h1 className="text-3xl font-black text-slate-950 dark:text-white">Brands & Sellers</h1>
        <div className="mt-6 grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
          {state.brands.map((brand) => (
            <Link key={brand.id} to={`/brands/${brand.id}`} className="rounded-2xl border border-slate-200 bg-white p-6 transition hover:-translate-y-1 hover:shadow-xl dark:border-slate-800 dark:bg-slate-900">
              <div className="flex items-center gap-4">
                <span className="grid h-16 w-16 place-items-center rounded-2xl bg-teal-600 text-xl font-black text-white">{brand.logo}</span>
                <div>
                  <h2 className="text-xl font-black text-slate-950 dark:text-white">{brand.name}</h2>
                  <Stars value={brand.rating} />
                </div>
              </div>
              <p className="mt-4 text-slate-600 dark:text-slate-300">{brand.story}</p>
              <p className="mt-4 text-sm font-bold text-teal-700 dark:text-teal-300">{getBrandProducts(brand.name).length} products</p>
            </Link>
          ))}
        </div>
      </main>
    </>
  );
}
