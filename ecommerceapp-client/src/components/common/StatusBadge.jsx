export function StatusBadge({ status }) {
  const styles = {
    Pending: "bg-amber-100 text-amber-700 dark:bg-amber-500/15 dark:text-amber-200",
    Processing: "bg-blue-100 text-blue-700 dark:bg-blue-500/15 dark:text-blue-200",
    Shipped: "bg-indigo-100 text-indigo-700 dark:bg-indigo-500/15 dark:text-indigo-200",
    Delivered: "bg-emerald-100 text-emerald-700 dark:bg-emerald-500/15 dark:text-emerald-200",
    Cancelled: "bg-rose-100 text-rose-700 dark:bg-rose-500/15 dark:text-rose-200",
    Refunded: "bg-slate-100 text-slate-700 dark:bg-slate-800 dark:text-slate-300",
  };
  return (
    <span className={`rounded-full px-2.5 py-1 text-xs font-semibold ${styles[status] || styles.Pending}`}>
      {status}
    </span>
  );
}
