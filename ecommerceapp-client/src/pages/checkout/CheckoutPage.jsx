import { useState } from "react";
import { Link, Navigate } from "react-router-dom";
import { CheckCircle2, CreditCard } from "lucide-react";
import { AddressForm } from "../../components/account/AddressForm";
import { Breadcrumbs, Modal } from "../../components/common";
import { OrderSummary } from "../../components/checkout/OrderSummary";
import { useStore } from "../../context/StoreContext";
import { calculateCartTotals, formatCurrency } from "../../utils/pricing";

export function CheckoutPage() {
  const { cartItems, currentUser, actions } = useStore();
  const [step, setStep] = useState(1);
  const [coupon, setCoupon] = useState("");
  const [payment, setPayment] = useState("Credit Card");
  const [selectedAddressId, setSelectedAddressId] = useState(currentUser?.addresses?.[0]?.id || "custom");
  const [address, setAddress] = useState(currentUser?.addresses?.[0] || {});
  const [confirmedOrder, setConfirmedOrder] = useState(null);
  const totals = calculateCartTotals(cartItems, coupon);

  if (!cartItems.length && !confirmedOrder) return <Navigate to="/cart" replace />;

  const placeOrder = () => {
    const shippingAddress = selectedAddressId === "custom" ? address : currentUser.addresses.find((item) => item.id === selectedAddressId);
    const order = actions.placeOrder({
      total: totals.total,
      paymentMethod: payment,
      items: cartItems.map((item) => ({
        productId: item.product.id,
        name: item.product.name,
        price: item.product.price,
        quantity: item.quantity,
        image: item.product.image,
      })),
      shippingAddress,
      tracking: "Order received. Fulfillment will begin shortly.",
    });
    setConfirmedOrder(order);
  };

  return (
    <>
      <Breadcrumbs current="Checkout" />
      <main className="mx-auto grid max-w-7xl gap-8 px-4 py-8 lg:grid-cols-[1fr_360px]">
        <section className="rounded-2xl border border-slate-200 bg-white p-6 dark:border-slate-800 dark:bg-slate-900">
          <div className="mb-6 grid gap-2 sm:grid-cols-3">
            {["Address", "Payment", "Review"].map((label, index) => (
              <div key={label} className={`rounded-xl p-3 text-sm font-bold ${step === index + 1 ? "bg-teal-600 text-white" : "bg-slate-100 text-slate-500 dark:bg-slate-800"}`}>
                Step {index + 1}: {label}
              </div>
            ))}
          </div>

          {step === 1 && (
            <div className="space-y-5">
              <h1 className="text-2xl font-black text-slate-950 dark:text-white">Shipping Address</h1>
              <div className="grid gap-3">
                {currentUser.addresses?.map((item) => (
                  <label key={item.id} className="flex cursor-pointer gap-3 rounded-xl border border-slate-200 p-4 dark:border-slate-700">
                    <input
                      type="radio"
                      checked={selectedAddressId === item.id}
                      onChange={() => {
                        setSelectedAddressId(item.id);
                        setAddress(item);
                      }}
                    />
                    <span>
                      <strong>{item.label}</strong>
                      <span className="block text-sm text-slate-500">{item.line1}, {item.city || item.state}</span>
                    </span>
                  </label>
                ))}
                <label className="flex cursor-pointer gap-3 rounded-xl border border-slate-200 p-4 dark:border-slate-700">
                  <input type="radio" checked={selectedAddressId === "custom"} onChange={() => setSelectedAddressId("custom")} />
                  <span><strong>Use a new address</strong></span>
                </label>
              </div>
              <AddressForm address={address} setAddress={setAddress} />
              <button className="btn-primary" type="button" onClick={() => setStep(2)}>Continue to Payment</button>
            </div>
          )}

          {step === 2 && (
            <div className="space-y-5">
              <h1 className="text-2xl font-black text-slate-950 dark:text-white">Payment Method</h1>
              <div className="grid gap-3 md:grid-cols-3">
                {["Credit Card", "PayPal", "Cash on Delivery"].map((item) => (
                  <button
                    key={item}
                    type="button"
                    onClick={() => setPayment(item)}
                    className={`rounded-2xl border p-5 text-left font-bold ${
                      payment === item ? "border-teal-500 bg-teal-50 text-teal-800 dark:bg-teal-500/15 dark:text-teal-100" : "border-slate-200 dark:border-slate-700"
                    }`}
                  >
                    <CreditCard className="mb-3" /> {item}
                  </button>
                ))}
              </div>
              {payment === "Credit Card" && (
                <div className="grid gap-4 sm:grid-cols-2">
                  <input className="input" placeholder="Card number" />
                  <input className="input" placeholder="Name on card" />
                  <input className="input" placeholder="MM / YY" />
                  <input className="input" placeholder="CVC" />
                </div>
              )}
              <div className="flex gap-3">
                <button className="btn-secondary" type="button" onClick={() => setStep(1)}>Back</button>
                <button className="btn-primary" type="button" onClick={() => setStep(3)}>Review Order</button>
              </div>
            </div>
          )}

          {step === 3 && (
            <div className="space-y-5">
              <h1 className="text-2xl font-black text-slate-950 dark:text-white">Review & Confirm</h1>
              <div className="divide-y divide-slate-100 rounded-2xl border border-slate-200 dark:divide-slate-800 dark:border-slate-700">
                {cartItems.map((item) => (
                  <div key={item.productId} className="flex items-center justify-between gap-4 p-4">
                    <div className="flex items-center gap-3">
                      <img src={item.product.image} alt="" className="h-14 w-14 rounded-lg object-cover" />
                      <span className="font-bold text-slate-950 dark:text-white">{item.product.name} x {item.quantity}</span>
                    </div>
                    <strong>{formatCurrency(item.product.price * item.quantity)}</strong>
                  </div>
                ))}
              </div>
              <div className="rounded-2xl bg-slate-50 p-5 dark:bg-slate-800">
                <p className="font-bold">Payment: {payment}</p>
                <p className="text-sm text-slate-500">Shipping to {address.line1 || currentUser.address}</p>
              </div>
              <div className="flex gap-3">
                <button className="btn-secondary" type="button" onClick={() => setStep(2)}>Back</button>
                <button className="btn-primary" type="button" onClick={placeOrder}>Place Order</button>
              </div>
            </div>
          )}
        </section>
        <OrderSummary totals={totals} coupon={coupon} setCoupon={setCoupon} />
      </main>
      <Modal open={Boolean(confirmedOrder)} onClose={() => setConfirmedOrder(null)} title="Order Confirmed">
        {confirmedOrder && (
          <div className="space-y-4">
            <CheckCircle2 className="text-emerald-500" size={42} />
            <p className="text-slate-600 dark:text-slate-300">Your mock order number is <strong>{confirmedOrder.id}</strong>.</p>
            <div className="flex flex-wrap gap-3">
              <Link className="btn-primary" to="/">Continue Shopping</Link>
              <Link className="btn-secondary" to={`/track-order/${confirmedOrder.id}`}>View Orders</Link>
            </div>
          </div>
        )}
      </Modal>
    </>
  );
}
