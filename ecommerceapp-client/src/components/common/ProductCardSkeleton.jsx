export function ProductCardSkeleton() {
  return (
    <div className="animate-pulse rounded-2xl border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900">
      <div className="aspect-square rounded-xl bg-slate-200 dark:bg-slate-800" />
      <div className="mt-4 h-4 w-3/4 rounded bg-slate-200 dark:bg-slate-800" />
      <div className="mt-2 h-3 w-1/2 rounded bg-slate-200 dark:bg-slate-800" />
      <div className="mt-4 flex items-center justify-between">
        <div className="h-5 w-20 rounded bg-slate-200 dark:bg-slate-800" />
        <div className="h-8 w-24 rounded-full bg-slate-200 dark:bg-slate-800" />
      </div>
    </div>
  );
}

export function ProductCardSkeletons({ count = 8 }) {
  return (
    <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
      {Array.from({ length: count }).map((_, i) => (
        <ProductCardSkeleton key={i} />
      ))}
    </div>
  );
}
