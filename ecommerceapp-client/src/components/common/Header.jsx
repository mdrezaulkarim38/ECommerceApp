import { useState } from "react";
import { Link, NavLink, useNavigate } from "react-router-dom";
import { BarChart3, ChevronDown, Heart, LogOut, Menu, Moon, ShoppingCart, Sun, User, X } from "lucide-react";
import { useStore } from "../../context/StoreContext";

export function Header() {
  const { currentUser, isAuthenticated, isAdmin, cartCount, state, actions } = useStore();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [profileOpen, setProfileOpen] = useState(false);
  const navigate = useNavigate();

  const links = isAuthenticated
    ? [
        ["Shop", "/"],
        ["Deals", "/deals"],
        ["AI Recommendations", "/recommendations"],
        ["Brands", "/brands"],
        ["Support", "/support"],
      ]
    : [
        ["Shop", "/"],
        ["Deals", "/deals"],
        ["Brands", "/brands"],
        ["Support", "/support"],
      ];

  const logout = () => {
    actions.logout();
    setProfileOpen(false);
    navigate("/");
  };

  return (
    <header className="sticky top-0 z-40 border-b border-slate-200 bg-white/90 backdrop-blur-xl dark:border-slate-800 dark:bg-slate-950/90">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between gap-4 px-4">
        <Link to="/" className="flex items-center gap-2">
          <span className="grid h-10 w-10 place-items-center rounded-xl bg-teal-600 font-black text-white shadow-lg shadow-teal-600/20">
            AI
          </span>
          <span className="text-xl font-black text-slate-950 dark:text-white">SmartShop</span>
        </Link>

        <nav className="hidden items-center gap-1 lg:flex">
          {links.map(([label, href]) => (
            <NavLink
              key={href}
              to={href}
              className={({ isActive }) =>
                `rounded-full px-4 py-2 text-sm font-semibold transition ${
                  isActive
                    ? "bg-teal-50 text-teal-700 dark:bg-teal-500/15 dark:text-teal-200"
                    : "text-slate-600 hover:bg-slate-100 hover:text-slate-950 dark:text-slate-300 dark:hover:bg-slate-900 dark:hover:text-white"
                }`
              }
            >
              {label}
            </NavLink>
          ))}
          {isAdmin && (
            <NavLink
              to="/admin"
              className="rounded-full bg-slate-950 px-4 py-2 text-sm font-bold text-white dark:bg-white dark:text-slate-950"
            >
              Admin
            </NavLink>
          )}
        </nav>

        <div className="flex items-center gap-2">
          <button
            type="button"
            onClick={() => actions.setTheme(state.theme === "dark" ? "light" : "dark")}
            className="grid h-10 w-10 place-items-center rounded-full border border-slate-200 text-slate-600 transition hover:bg-slate-100 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-900"
            aria-label="Toggle dark mode"
          >
            {state.theme === "dark" ? <Sun size={18} /> : <Moon size={18} />}
          </button>

          {isAuthenticated ? (
            <>
              <Link
                to="/cart"
                className="relative grid h-10 w-10 place-items-center rounded-full border border-slate-200 text-slate-700 transition hover:bg-slate-100 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-900"
                aria-label="Cart"
              >
                <ShoppingCart size={18} />
                {cartCount > 0 && (
                  <span className="absolute -right-1 -top-1 grid h-5 min-w-5 place-items-center rounded-full bg-rose-600 px-1 text-[11px] font-bold text-white">
                    {cartCount}
                  </span>
                )}
              </Link>
              <div className="relative">
                <button
                  type="button"
                  onClick={() => setProfileOpen((value) => !value)}
                  className="flex h-10 items-center gap-2 rounded-full border border-slate-200 px-2 pr-3 text-sm font-bold text-slate-700 dark:border-slate-700 dark:text-slate-100"
                >
                  <span className="grid h-7 w-7 place-items-center rounded-full bg-slate-950 text-xs text-white dark:bg-teal-400 dark:text-slate-950">
                    {currentUser.name
                      .split(" ")
                      .map((part) => part[0])
                      .slice(0, 2)
                      .join("")}
                  </span>
                  <ChevronDown size={15} />
                </button>
                {profileOpen && (
                  <div className="absolute right-0 mt-3 w-56 rounded-xl border border-slate-200 bg-white p-2 shadow-xl dark:border-slate-800 dark:bg-slate-900">
                    <div className="px-3 py-2">
                      <p className="font-bold text-slate-950 dark:text-white">{currentUser.name}</p>
                      <p className="text-xs text-slate-500">{currentUser.email}</p>
                    </div>
                    <Link className="menu-link" to="/profile" onClick={() => setProfileOpen(false)}>
                      <User size={16} /> Profile
                    </Link>
                    <Link className="menu-link" to="/wishlist" onClick={() => setProfileOpen(false)}>
                      <Heart size={16} /> Wishlist
                    </Link>
                    <Link className="menu-link" to="/profile?tab=orders" onClick={() => setProfileOpen(false)}>
                      <BarChart3 size={16} /> Orders
                    </Link>
                    <button type="button" className="menu-link w-full" onClick={logout}>
                      <LogOut size={16} /> Logout
                    </button>
                  </div>
                )}
              </div>
            </>
          ) : (
            <Link
              to="/auth"
              className="hidden rounded-full bg-slate-950 px-4 py-2 text-sm font-bold text-white transition hover:bg-teal-700 dark:bg-teal-500 dark:text-slate-950 sm:inline-flex"
            >
              Login / Register
            </Link>
          )}
          <button
            type="button"
            onClick={() => setMobileOpen((value) => !value)}
            className="grid h-10 w-10 place-items-center rounded-full border border-slate-200 text-slate-700 lg:hidden dark:border-slate-700 dark:text-slate-100"
            aria-label="Open menu"
          >
            {mobileOpen ? <X size={18} /> : <Menu size={18} />}
          </button>
        </div>
      </div>

      {mobileOpen && (
        <div className="border-t border-slate-200 bg-white p-4 lg:hidden dark:border-slate-800 dark:bg-slate-950">
          <div className="grid gap-2">
            {links.map(([label, href]) => (
              <NavLink
                key={href}
                to={href}
                onClick={() => setMobileOpen(false)}
                className="rounded-xl px-4 py-3 font-semibold text-slate-700 hover:bg-slate-100 dark:text-slate-200 dark:hover:bg-slate-900"
              >
                {label}
              </NavLink>
            ))}
            {isAdmin && (
              <Link to="/admin" onClick={() => setMobileOpen(false)} className="rounded-xl bg-slate-950 px-4 py-3 font-bold text-white">
                Admin Panel
              </Link>
            )}
            {!isAuthenticated && (
              <Link to="/auth" onClick={() => setMobileOpen(false)} className="rounded-xl bg-teal-600 px-4 py-3 font-bold text-white">
                Login / Register
              </Link>
            )}
          </div>
        </div>
      )}
    </header>
  );
}
