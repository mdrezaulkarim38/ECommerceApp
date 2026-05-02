export function Panel({ title, children }) {
  return (
    <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <h2 className="mb-5 text-xl font-black text-slate-950 dark:text-white">{title}</h2>
      {children}
    </section>
  );
}
