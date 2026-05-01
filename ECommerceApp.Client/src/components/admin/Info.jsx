export function Info({ label, value }) {
  return (
    <div className="rounded-xl bg-slate-50 p-4 dark:bg-slate-800">
      <p className="text-sm text-slate-500">{label}</p>
      <p className="font-bold text-slate-950 dark:text-white">{value}</p>
    </div>
  );
}
