import { useState } from "react";
import { Search } from "lucide-react";

export function SearchInput({ value, onChange, suggestions = [], onSelect }) {
  const [focused, setFocused] = useState(false);
  return (
    <div className="relative">
      <Search className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
      <input
        value={value}
        onChange={(event) => onChange(event.target.value)}
        onFocus={() => setFocused(true)}
        onBlur={() => setTimeout(() => setFocused(false), 150)}
        className="h-12 w-full rounded-full border border-slate-200 bg-white pl-11 pr-4 text-sm outline-none ring-teal-500 transition focus:ring-2 dark:border-slate-700 dark:bg-slate-900 dark:text-white"
        placeholder="Search products, brands, categories..."
      />
      {focused && value && suggestions.length > 0 && (
        <div className="absolute z-20 mt-2 w-full overflow-hidden rounded-xl border border-slate-200 bg-white shadow-xl dark:border-slate-700 dark:bg-slate-900">
          {suggestions.slice(0, 6).map((product) => (
            <button
              key={product.id}
              type="button"
              onMouseDown={() => onSelect(product)}
              className="flex w-full items-center gap-3 px-4 py-3 text-left transition hover:bg-slate-50 dark:hover:bg-slate-800"
            >
              <img src={product.image} alt="" className="h-10 w-10 rounded-lg object-cover" />
              <span className="text-sm font-semibold text-slate-800 dark:text-slate-100">{product.name}</span>
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
