export function Metric({ label, value, icon: Icon }) {
  return (
    <div className="flex items-center justify-between rounded-xl border border-slate-200 p-4 dark:border-slate-800">
      <div className="flex items-center gap-3">
        <Icon className="text-teal-600 dark:text-teal-300" />
        <span className="font-bold">{label}</span>
      </div>
      <span className="text-xl font-black text-slate-950 dark:text-white">{value}</span>
    </div>
  );
}
