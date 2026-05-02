import { Star } from "lucide-react";

export function Stars({ value = 0, size = 16, showValue = false }) {
  return (
    <div className="flex items-center gap-1 text-amber-500">
      {Array.from({ length: 5 }).map((_, index) => (
        <Star
          key={index}
          size={size}
          className={index < Math.round(value) ? "fill-amber-400 stroke-amber-400" : "stroke-slate-300"}
        />
      ))}
      {showValue && <span className="ml-1 text-sm font-semibold text-slate-700 dark:text-slate-200">{value}</span>}
    </div>
  );
}
