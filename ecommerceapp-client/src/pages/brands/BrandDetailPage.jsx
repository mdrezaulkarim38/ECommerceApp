import { useParams } from "react-router-dom";
import toast from "react-hot-toast";
import { Breadcrumbs, EmptyState, ProductCard } from "../../components/common";
import { useStore } from "../../context/StoreContext";

export function BrandDetailPage() {
  const { brandId } = useParams();
  const { state, getBrandProducts } = useStore();
  const brand = state.brands.find((item) => item.id === brandId);
  if (!brand) return <EmptyState title="Brand not found" message="This seller does not exist in demo data." />;
  const products = getBrandProducts(brand.name);
  return (
    <>
      <Breadcrumbs current={brand.name} />
      <main className="mx-auto max-w-7xl px-4 py-8">
        <section className="rounded-3xl bg-slate-950 p-8 text-white">
          <div className="flex flex-wrap items-center justify-between gap-6">
            <div className="flex items-center gap-5">
              <span className="grid h-20 w-20 place-items-center rounded-3xl bg-teal-500 text-2xl font-black text-slate-950">{brand.logo}</span>
              <div>
                <h1 className="text-4xl font-black">{brand.name}</h1>
                <p className="mt-2 max-w-2xl text-slate-300">{brand.story}</p>
              </div>
            </div>
            <button className="btn-ghost-light" type="button" onClick={() => toast.success(`Following ${brand.name}`)}>Follow Brand</button>
          </div>
        </section>
        <div className="mt-8 grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
          {products.map((product) => <ProductCard key={product.id} product={product} />)}
        </div>
      </main>
    </>
  );
}
