import { Link } from "react-router-dom";
import toast from "react-hot-toast";
import { formatCurrency } from "../../utils/pricing";

export function OrderSummary({ totals, coupon, setCoupon, checkout = false }) {
  return (
    <aside className="h-max rounded-2xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <h2 className="text-xl font-black text-slate-950 dark:text-white">Order Summary</h2>
      <div className="mt-4 space-y-3 text-sm">
        <div className="flex justify-between"><span>Subtotal</span><strong>{formatCurrency(totals.subtotal)}</strong></div>
        <div className="flex justify-between"><span>Shipping</span><strong>{formatCurrency(totals.shipping)}</strong></div>
        <div className="flex justify-between"><span>Tax</span><strong>{formatCurrency(totals.tax)}</strong></div>
        <div className="flex justify-between text-emerald-600"><span>Discount</span><strong>-{formatCurrency(totals.discount)}</strong></div>
      </div>
      <form
        className="mt-5 flex gap-2"
        onSubmit={(event) => {
          event.preventDefault();
          toast.success(coupon.trim().toUpperCase() === "SMART10" ? "Coupon applied" : "Mock coupon checked");
        }}
      >
        <input className="input" placeholder="SMART10" value={coupon} onChange={(event) => setCoupon(event.target.value)} />
        <button className="btn-secondary" type="submit">Apply</button>
      </form>
      <div className="mt-5 flex items-center justify-between border-t border-slate-200 pt-5 text-lg dark:border-slate-800">
        <span className="font-bold">Total</span>
        <span className="text-2xl font-black text-slate-950 dark:text-white">{formatCurrency(totals.total)}</span>
      </div>
      {checkout && <Link to="/checkout" className="btn-primary mt-5 w-full justify-center">Checkout</Link>}
    </aside>
  );
}
