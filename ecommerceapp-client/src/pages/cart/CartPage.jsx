import { useState } from "react";
import { Link } from "react-router-dom";
import { Minus, Plus, ShoppingCart } from "lucide-react";
import { Breadcrumbs, EmptyState } from "../../components/common";
import { OrderSummary } from "../../components/checkout/OrderSummary";
import { useStore } from "../../context/StoreContext";
import { calculateCartTotals, formatCurrency } from "../../utils/pricing";

export function CartPage() {
  const { cartItems, actions } = useStore();
  const [coupon, setCoupon] = useState("");
  const totals = calculateCartTotals(cartItems, coupon);

  if (!cartItems.length) {
    return (
      <main className="mx-auto max-w-7xl px-4 py-12">
        <EmptyState
          icon={ShoppingCart}
          title="Your cart is empty"
          message="Add a few products and the checkout simulation will appear here."
          action={<Link className="btn-primary" to="/">Continue Shopping</Link>}
        />
      </main>
    );
  }

  return (
    <>
      <Breadcrumbs current="Cart" />
      <main className="mx-auto grid max-w-7xl gap-8 px-4 py-8 lg:grid-cols-[1fr_360px]">
        <section className="overflow-hidden rounded-2xl border border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900">
          <div className="border-b border-slate-200 p-5 dark:border-slate-800">
            <h1 className="text-2xl font-black text-slate-950 dark:text-white">Shopping Cart</h1>
          </div>
          <div className="divide-y divide-slate-100 dark:divide-slate-800">
            {cartItems.map((item) => (
              <div key={item.productId} className="grid gap-4 p-5 md:grid-cols-[90px_1fr_150px_130px] md:items-center">
                <img src={item.product.image} alt={item.product.name} className="h-24 w-24 rounded-xl object-cover" />
                <div>
                  <Link to={`/products/${item.product.id}`} className="font-bold text-slate-950 hover:text-teal-700 dark:text-white">
                    {item.product.name}
                  </Link>
                  <p className="mt-1 text-sm text-slate-500">{item.product.category}</p>
                  <div className="mt-3 flex flex-wrap gap-2">
                    <button className="text-sm font-bold text-teal-700 dark:text-teal-300" onClick={() => actions.saveForLater(item.productId)} type="button">
                      Save for later
                    </button>
                    <button className="text-sm font-bold text-rose-600" onClick={() => actions.removeFromCart(item.productId)} type="button">
                      Remove
                    </button>
                  </div>
                </div>
                <div className="inline-flex w-max items-center rounded-full border border-slate-200 dark:border-slate-700">
                  <button className="p-3" type="button" onClick={() => actions.updateCartQty(item.productId, item.quantity - 1)}>
                    <Minus size={16} />
                  </button>
                  <span className="w-10 text-center font-bold">{item.quantity}</span>
                  <button className="p-3" type="button" onClick={() => actions.updateCartQty(item.productId, item.quantity + 1)}>
                    <Plus size={16} />
                  </button>
                </div>
                <div className="font-black text-slate-950 dark:text-white">{formatCurrency(item.product.price * item.quantity)}</div>
              </div>
            ))}
          </div>
        </section>
        <OrderSummary totals={totals} coupon={coupon} setCoupon={setCoupon} checkout />
      </main>
    </>
  );
}
