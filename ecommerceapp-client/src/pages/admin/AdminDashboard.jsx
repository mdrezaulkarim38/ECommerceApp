import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { BarChart3, LayoutDashboard, LogOut, Package, Settings, Sparkles, Users } from "lucide-react";
import { useStore } from "../../context/StoreContext";
import { AdminSettings } from "./tabs/AdminSettings";
import { AnalyticsForecasting } from "./tabs/AnalyticsForecasting";
import { DashboardHome } from "./tabs/DashboardHome";
import { OrdersManagement } from "./tabs/OrdersManagement";
import { ProductsManagement } from "./tabs/ProductsManagement";
import { UsersManagement } from "./tabs/UsersManagement";

const adminTabs = [
  ["Dashboard", LayoutDashboard],
  ["Products", Package],
  ["Orders", BarChart3],
  ["Users", Users],
  ["Analytics", Sparkles],
  ["Settings", Settings],
];

export default function AdminDashboard() {
  const { currentUser, actions } = useStore();
  const [tab, setTab] = useState("Dashboard");
  const navigate = useNavigate();

  const logout = () => {
    actions.logout();
    navigate("/");
  };

  return (
    <main className="min-h-screen bg-slate-100 text-slate-800 dark:bg-slate-950 dark:text-slate-100">
      <aside className="fixed inset-y-0 left-0 z-40 hidden w-72 border-r border-slate-200 bg-white p-5 lg:block dark:border-slate-800 dark:bg-slate-900">
        <Link to="/" className="mb-8 flex items-center gap-3">
          <span className="grid h-11 w-11 place-items-center rounded-xl bg-teal-600 font-black text-white">AI</span>
          <span>
            <span className="block text-xl font-black text-slate-950 dark:text-white">SmartShop</span>
            <span className="text-sm text-slate-500">Admin Panel</span>
          </span>
        </Link>
        <nav className="grid gap-2">
          {adminTabs.map(([label, Icon]) => (
            <button
              key={label}
              type="button"
              onClick={() => setTab(label)}
              className={`flex items-center gap-3 rounded-xl px-4 py-3 text-left text-sm font-bold transition ${
                tab === label
                  ? "bg-teal-600 text-white shadow-lg shadow-teal-600/20"
                  : "text-slate-600 hover:bg-slate-100 dark:text-slate-300 dark:hover:bg-slate-800"
              }`}
            >
              <Icon size={18} /> {label}
            </button>
          ))}
        </nav>
      </aside>

      <section className="lg:pl-72">
        <header className="sticky top-0 z-30 border-b border-slate-200 bg-white/90 px-4 py-4 backdrop-blur-xl dark:border-slate-800 dark:bg-slate-900/90">
          <div className="mx-auto flex max-w-7xl flex-wrap items-center justify-between gap-4">
            <div>
              <p className="text-sm font-bold text-teal-700 dark:text-teal-300">Admin Dashboard</p>
              <h1 className="text-2xl font-black text-slate-950 dark:text-white">{tab}</h1>
            </div>
            <div className="flex items-center gap-3">
              <div className="hidden rounded-full bg-slate-100 px-4 py-2 text-sm font-bold dark:bg-slate-800 sm:block">
                {currentUser.name}
              </div>
              <button className="btn-secondary" type="button" onClick={logout}>
                <LogOut size={18} /> Logout
              </button>
            </div>
            <div className="flex w-full gap-2 overflow-x-auto lg:hidden">
              {adminTabs.map(([label, Icon]) => (
                <button
                  key={label}
                  type="button"
                  onClick={() => setTab(label)}
                  className={`inline-flex items-center gap-2 rounded-full px-4 py-2 text-sm font-bold ${
                    tab === label ? "bg-teal-600 text-white" : "bg-slate-100 dark:bg-slate-800"
                  }`}
                >
                  <Icon size={16} /> {label}
                </button>
              ))}
            </div>
          </div>
        </header>

        <div className="mx-auto max-w-7xl px-4 py-8">
          {tab === "Dashboard" && <DashboardHome />}
          {tab === "Products" && <ProductsManagement />}
          {tab === "Orders" && <OrdersManagement />}
          {tab === "Users" && <UsersManagement />}
          {tab === "Analytics" && <AnalyticsForecasting />}
          {tab === "Settings" && <AdminSettings />}
        </div>
      </section>
    </main>
  );
}
