import { useState } from "react";
import { Info } from "../../../components/admin";
import { Modal, StatusBadge } from "../../../components/common";
import { useStore } from "../../../context/StoreContext";
import { formatCurrency } from "../../../utils/pricing";

const orderStatuses = ["Pending", "Processing", "Shipped", "Delivered", "Cancelled", "Refunded"];

export function OrdersManagement() {
  const { state, actions } = useStore();
  const [filter, setFilter] = useState("All");
  const [details, setDetails] = useState(null);
  const orders = filter === "All" ? state.orders : state.orders.filter((order) => order.status === filter);
  return (
    <div className="space-y-5">
      <div className="flex flex-wrap gap-2">
        {["All", ...orderStatuses].map((item) => (
          <button key={item} className={`rounded-full px-4 py-2 text-sm font-bold ${filter === item ? "bg-teal-600 text-white" : "bg-white ring-1 ring-slate-200 dark:bg-slate-900 dark:ring-slate-700"}`} type="button" onClick={() => setFilter(item)}>
            {item}
          </button>
        ))}
      </div>
      <div className="overflow-auto rounded-2xl border border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900">
        <table className="w-full min-w-[980px] text-left text-sm">
          <thead className="bg-slate-50 text-slate-500 dark:bg-slate-800">
            <tr><th className="p-4">Order ID</th><th className="p-4">Customer</th><th className="p-4">Date</th><th className="p-4">Total</th><th className="p-4">Status</th><th className="p-4">Payment</th><th className="p-4">Actions</th></tr>
          </thead>
          <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
            {orders.map((order) => (
              <tr key={order.id}>
                <td className="p-4 font-bold text-slate-950 dark:text-white">{order.id}</td>
                <td className="p-4">{order.customerName}</td>
                <td className="p-4">{order.date}</td>
                <td className="p-4">{formatCurrency(order.total)}</td>
                <td className="p-4"><StatusBadge status={order.status} /></td>
                <td className="p-4">{order.paymentMethod}</td>
                <td className="p-4">
                  <div className="flex gap-2">
                    <select className="rounded-lg border border-slate-200 bg-white px-2 py-1 dark:border-slate-700 dark:bg-slate-900" value={order.status} onChange={(event) => actions.adminUpdateOrderStatus(order.id, event.target.value)}>
                      {orderStatuses.map((status) => <option key={status}>{status}</option>)}
                    </select>
                    <button className="btn-mini" type="button" onClick={() => setDetails(order)}>View</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <Modal open={Boolean(details)} onClose={() => setDetails(null)} title={`Order ${details?.id || ""}`} wide>
        {details && (
          <div className="space-y-4">
            <div className="grid gap-3 md:grid-cols-3">
              <Info label="Customer" value={details.customerName} />
              <Info label="Payment" value={details.paymentMethod} />
              <Info label="Total" value={formatCurrency(details.total)} />
            </div>
            <div className="rounded-2xl bg-slate-50 p-4 dark:bg-slate-800">
              <p className="font-bold">Shipping Address</p>
              <p className="text-sm text-slate-500">{details.shippingAddress?.line1}, {details.shippingAddress?.city}, {details.shippingAddress?.country}</p>
            </div>
            <div className="divide-y divide-slate-100 rounded-2xl border border-slate-200 dark:divide-slate-800 dark:border-slate-700">
              {details.items.map((item) => (
                <div key={item.productId} className="flex items-center justify-between p-4">
                  <span className="font-bold">{item.name} x {item.quantity}</span>
                  <span>{formatCurrency(item.price * item.quantity)}</span>
                </div>
              ))}
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}
