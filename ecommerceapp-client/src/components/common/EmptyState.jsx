import { Sparkles } from "lucide-react";

export function EmptyState({ icon: Icon = Sparkles, title, message, action }) {
  return (
    <div className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center dark:border-slate-700 dark:bg-slate-900">
      <Icon className="mx-auto mb-3 text-teal-600 dark:text-teal-300" size={36} />
      <h3 className="text-xl font-bold text-slate-950 dark:text-white">{title}</h3>
      <p className="mx-auto mt-2 max-w-xl text-slate-600 dark:text-slate-300">{message}</p>
      {action && <div className="mt-5">{action}</div>}
    </div>
  );
}
