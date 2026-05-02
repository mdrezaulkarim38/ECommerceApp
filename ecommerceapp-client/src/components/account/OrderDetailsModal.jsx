import { Modal, StatusBadge } from "../common";
import { formatCurrency } from "../../utils/pricing";

export function OrderDetailsModal({ order, onClose }) {
  return (
    <Modal open={Boolean(order)} onClose={onClose} title={`Order ${order?.id || ""}`} wide>
      {order && (
        <div className="space-y-5">
          <div className="grid gap-3 sm:grid-cols-3">
            <div className="rounded-xl bg-slate-50 p-4 dark:bg-slate-800"><p className="text-sm text-slate-500">Status</p><StatusBadge status={order.status} /></div>
            <div className="rounded-xl bg-slate-50 p-4 dark:bg-slate-800"><p className="text-sm text-slate-500">Payment</p><p className="font-bold">{order.paymentMethod}</p></div>
            <div className="rounded-xl bg-slate-50 p-4 dark:bg-slate-800"><p className="text-sm text-slate-500">Total</p><p className="font-bold">{formatCurrency(order.total)}</p></div>
          </div>
          <div className="divide-y divide-slate-100 rounded-2xl border border-slate-200 dark:divide-slate-800 dark:border-slate-700">
            {order.items.map((item) => (
              <div key={item.productId} className="flex items-center justify-between gap-3 p-4">
                <div className="flex items-center gap-3">
                  <img src={item.image} alt="" className="h-14 w-14 rounded-lg object-cover" />
                  <span className="font-bold">{item.name} x {item.quantity}</span>
                </div>
                <span>{formatCurrency(item.price * item.quantity)}</span>
              </div>
            ))}
          </div>
          <p className="rounded-xl bg-teal-50 p-4 text-sm text-teal-900 dark:bg-teal-500/15 dark:text-teal-100">{order.tracking}</p>
        </div>
      )}
    </Modal>
  );
}
