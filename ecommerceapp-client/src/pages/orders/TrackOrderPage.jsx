import { useParams } from "react-router-dom";
import { CheckCircle2, MapPin, MessageCircle, Truck } from "lucide-react";
import toast from "react-hot-toast";
import { Breadcrumbs, EmptyState } from "../../components/common";
import { useStore } from "../../context/StoreContext";

export function TrackOrderPage() {
  const { orderId } = useParams();
  const { getOrder, currentUser, isAdmin } = useStore();
  const order = getOrder(orderId);
  const isOwner = !order || !order.userId || order.userId === currentUser?.id || order.userId === currentUser?.id?.replace("u-", "");
  if (!order || (!isAdmin && !isOwner)) {
    return <main className="mx-auto max-w-7xl px-4 py-12"><EmptyState icon={Truck} title="Order not found" message="The order ID is unavailable for this account." /></main>;
  }
  const steps = ["Pending", "Confirmed", "Processing", "Shipped", "Out for Delivery", "Delivered"];
  const indexMap = { Pending: 0, Processing: 2, Shipped: 3, Delivered: 5, Cancelled: 0 };
  const current = indexMap[order.status] ?? 0;
  return (
    <>
      <Breadcrumbs current={order.id} />
      <main className="mx-auto max-w-7xl px-4 py-8">
        <h1 className="text-3xl font-black text-slate-950 dark:text-white">Track Order {order.id}</h1>
        <section className="mt-6 rounded-2xl border border-slate-200 bg-white p-6 dark:border-slate-800 dark:bg-slate-900">
          <div className="grid gap-4 md:grid-cols-6">
            {steps.map((step, index) => (
              <div key={step} className={`rounded-2xl p-4 text-center ${index <= current ? "bg-teal-50 text-teal-800 dark:bg-teal-500/15 dark:text-teal-100" : "bg-slate-50 text-slate-400 dark:bg-slate-800"}`}>
                <CheckCircle2 className="mx-auto mb-2" />
                <p className="text-sm font-bold">{step}</p>
              </div>
            ))}
          </div>
          <div className="mt-6 grid gap-5 lg:grid-cols-[1fr_340px]">
            <div className="grid min-h-[260px] place-items-center rounded-2xl bg-slate-100 dark:bg-slate-800">
              <div className="text-center">
                <MapPin className="mx-auto mb-2 text-teal-600" size={42} />
                <p className="font-bold text-slate-950 dark:text-white">Live tracking simulation</p>
                <p className="text-sm text-slate-500">{order.tracking}</p>
              </div>
            </div>
            <div className="rounded-2xl bg-slate-50 p-5 dark:bg-slate-800">
              <p className="font-bold">Estimated delivery</p>
              <p className="text-2xl font-black text-slate-950 dark:text-white">May 5, 2026</p>
              <p className="mt-4 text-sm text-slate-500">{order.shippingAddress?.line1}, {order.shippingAddress?.city}</p>
              <button className="btn-secondary mt-5" type="button" onClick={() => toast.success("Support chat opened")}>
                <MessageCircle size={18} /> Contact Support
              </button>
            </div>
          </div>
        </section>
      </main>
    </>
  );
}
