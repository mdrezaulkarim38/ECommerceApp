import { Fragment } from "react";
import { Link, useLocation } from "react-router-dom";

export function Breadcrumbs({ current }) {
  const location = useLocation();
  const parts = location.pathname.split("/").filter(Boolean);
  if (location.pathname === "/") return null;
  return (
    <nav className="mx-auto flex w-full max-w-7xl items-center gap-2 px-4 pt-6 text-sm text-slate-500 dark:text-slate-400">
      <Link to="/" className="hover:text-teal-600 dark:hover:text-teal-300">
        Home
      </Link>
      {parts.map((part, index) => {
        const to = `/${parts.slice(0, index + 1).join("/")}`;
        const label = index === parts.length - 1 && current ? current : part.replaceAll("-", " ");
        return (
          <Fragment key={to}>
            <span>/</span>
            {index === parts.length - 1 ? (
              <span className="truncate font-medium capitalize text-slate-800 dark:text-slate-100">{label}</span>
            ) : (
              <Link to={to} className="capitalize hover:text-teal-600 dark:hover:text-teal-300">
                {label}
              </Link>
            )}
          </Fragment>
        );
      })}
    </nav>
  );
}
