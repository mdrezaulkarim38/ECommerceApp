import { useEffect, useState } from "react";
import { Timer } from "lucide-react";
import toast from "react-hot-toast";
import { Breadcrumbs, ProductCard } from "../../components/common";
import { useStore } from "../../context/StoreContext";

export function DealsPage() {
  const { state } = useStore();
  const [dealType, setDealType] = useState("Today's Deals");
  const [seconds, setSeconds] = useState(21600);
  useEffect(() => {
    const timer = setInterval(() => setSeconds((value) => Math.max(0, value - 1)), 1000);
    return () => clearInterval(timer);
  }, []);
  const deals = state.products.filter((product) => product.originalPrice > product.price).slice(0, dealType === "Clearance" ? 8 : 12);
  const time = [Math.floor(seconds / 3600), Math.floor((seconds % 3600) / 60), seconds % 60].map((part) => String(part).padStart(2, "0")).join(":");

  return (
    <>
      <Breadcrumbs current="Deals" />
      <main className="mx-auto max-w-7xl px-4 py-8">
        <section className="overflow-hidden rounded-3xl bg-slate-950 text-white">
          <div className="grid gap-8 p-8 md:grid-cols-[1fr_320px] md:items-center">
            <div>
              <span className="inline-flex items-center gap-2 rounded-full bg-white/15 px-4 py-2 text-sm font-bold"><Timer size={16} /> Flash sale starts in</span>
              <h1 className="mt-5 text-5xl font-black">{time}</h1>
              <p className="mt-3 max-w-xl text-slate-300">Discounted products with stock hints, sold-out badges, and mock alert signup.</p>
            </div>
            <form
              className="rounded-2xl bg-white/10 p-5 backdrop-blur"
              onSubmit={(event) => {
                event.preventDefault();
                toast.success("Deal alert subscription saved");
                event.currentTarget.reset();
              }}
            >
              <p className="font-bold">Email alert signup</p>
              <input className="mt-3 h-11 w-full rounded-full px-4 text-slate-950 outline-none" placeholder="you@example.com" type="email" required />
              <button className="mt-3 w-full rounded-full bg-teal-400 px-4 py-2 font-bold text-slate-950" type="submit">Notify Me</button>
            </form>
          </div>
        </section>
        <div className="my-6 flex flex-wrap gap-2">
          {["Today's Deals", "Lightning Deals", "Clearance"].map((item) => (
            <button key={item} className={`rounded-full px-4 py-2 text-sm font-bold ${dealType === item ? "bg-teal-600 text-white" : "bg-white ring-1 ring-slate-200 dark:bg-slate-900 dark:ring-slate-700"}`} type="button" onClick={() => setDealType(item)}>
              {item}
            </button>
          ))}
        </div>
        <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
          {deals.map((product, index) => (
            <div key={product.id} className="relative">
              <ProductCard product={product} />
              {index === 3 && <span className="absolute inset-x-4 top-28 rounded-xl bg-slate-950/80 py-2 text-center font-bold text-white">Sold Out</span>}
              <div className="mt-2 text-sm font-bold text-amber-600">{Math.max(0, product.stock - 3)} stock remaining</div>
            </div>
          ))}
        </div>
      </main>
    </>
  );
}
