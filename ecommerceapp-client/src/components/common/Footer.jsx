import { Link } from "react-router-dom";
import toast from "react-hot-toast";
import { useStore } from "../../context/StoreContext";

export function Footer() {
  const { actions } = useStore();
  return (
    <footer className="mt-16 border-t border-slate-200 bg-slate-950 text-white dark:border-slate-800">
      <div className="mx-auto grid max-w-7xl gap-8 px-4 py-10 md:grid-cols-[1.3fr_1fr_1fr_1.2fr]">
        <div>
          <div className="flex items-center gap-2">
            <span className="grid h-10 w-10 place-items-center rounded-xl bg-teal-500 font-black text-slate-950">AI</span>
            <span className="text-xl font-black">SmartShop</span>
          </div>
          <p className="mt-4 text-sm leading-6 text-slate-300">
            AI-powered ecommerce demo with predictive analytics, personalization, and business intelligence.
          </p>
        </div>
        <div>
          <h4 className="font-bold">Company</h4>
          <div className="mt-3 grid gap-2 text-sm text-slate-300">
            <Link to="/support">About Us</Link>
            <Link to="/support">Contact</Link>
            <Link to="/support">FAQs</Link>
          </div>
        </div>
        <div>
          <h4 className="font-bold">Legal</h4>
          <div className="mt-3 grid gap-2 text-sm text-slate-300">
            <Link to="/support">Privacy Policy</Link>
            <Link to="/support">Terms of Service</Link>
            <Link to="/support">Returns</Link>
          </div>
        </div>
        <div>
          <h4 className="font-bold">Newsletter</h4>
          <form
            className="mt-3 flex overflow-hidden rounded-full bg-white"
            onSubmit={(event) => {
              event.preventDefault();
              toast.success("Newsletter subscription saved");
              event.currentTarget.reset();
            }}
          >
            <input className="min-w-0 flex-1 px-4 text-sm text-slate-950 outline-none" placeholder="Email address" />
            <button className="bg-teal-500 px-4 text-sm font-bold text-slate-950" type="submit">
              Join
            </button>
          </form>
          <button
            type="button"
            onClick={actions.resetDemoData}
            className="mt-4 rounded-full border border-white/20 px-4 py-2 text-sm font-bold text-white transition hover:bg-white/10"
          >
            Reset Demo Data
          </button>
        </div>
      </div>
    </footer>
  );
}
