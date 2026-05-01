export function StatusPill({ tone, label }) {
  const styles = {
    ok: "bg-emerald-100 text-emerald-700 dark:bg-emerald-500/15 dark:text-emerald-200",
    warn: "bg-amber-100 text-amber-700 dark:bg-amber-500/15 dark:text-amber-200",
    info: "bg-blue-100 text-blue-700 dark:bg-blue-500/15 dark:text-blue-200",
  };
  return <span className={`rounded-full px-2.5 py-1 text-xs font-bold capitalize ${styles[tone]}`}>{label}</span>;
}
