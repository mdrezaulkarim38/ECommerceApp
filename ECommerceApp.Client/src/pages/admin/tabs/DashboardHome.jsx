import { AlertTriangle, BarChart3, Package, Users } from "lucide-react";
import { Bar, CartesianGrid, ComposedChart, Legend, Line, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { Panel } from "../../../components/admin";
import { Stars, StatusBadge } from "../../../components/common";
import { useStore } from "../../../context/StoreContext";
import { salesData } from "../../../data/adminDashboardData";
import { formatCurrency } from "../../../utils/pricing";

export function DashboardHome() {
  const { analytics } = useStore();
  const kpis = [
    ["Total Revenue", formatCurrency(analytics.revenue), "+18.4%", BarChart3],
    ["Total Orders", analytics.totalOrders, "+9.2%", Package],
    ["Total Users", analytics.totalUsers, "+12.1%", Users],
    ["Low Stock Items", analytics.lowStock.length, "Needs review", AlertTriangle],
  ];
  return (
    <div className="space-y-6">
      <div className="grid gap-5 md:grid-cols-2 xl:grid-cols-4">
        {kpis.map(([label, value, trend, Icon]) => (
          <div key={label} className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
            <div className="flex items-center justify-between">
              <Icon className="text-teal-600 dark:text-teal-300" />
              <span className="rounded-full bg-emerald-50 px-3 py-1 text-xs font-bold text-emerald-700 dark:bg-emerald-500/15 dark:text-emerald-200">
                {trend}
              </span>
            </div>
            <p className="mt-6 text-sm font-bold text-slate-500">{label}</p>
            <p className="mt-1 text-3xl font-black text-slate-950 dark:text-white">{value}</p>
          </div>
        ))}
      </div>

      <div className="grid gap-6 xl:grid-cols-[1.4fr_0.8fr]">
        <Panel title="Sales Chart: Last 7 Days vs Last 30 Days">
          <div className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <ComposedChart data={salesData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="day" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="last30" fill="#94a3b8" radius={[8, 8, 0, 0]} />
                <Line type="monotone" dataKey="last7" stroke="#0f766e" strokeWidth={3} />
              </ComposedChart>
            </ResponsiveContainer>
          </div>
        </Panel>
        <Panel title="Low Stock Alerts">
          <div className="grid gap-3">
            {analytics.lowStock.map((product) => (
              <div key={product.id} className="flex items-center justify-between rounded-xl bg-amber-50 p-3 text-amber-900 dark:bg-amber-500/15 dark:text-amber-100">
                <span className="font-bold">{product.name}</span>
                <span>{product.stock} left</span>
              </div>
            ))}
          </div>
        </Panel>
      </div>

      <div className="grid gap-6 xl:grid-cols-2">
        <Panel title="Top Selling Products">
          <div className="overflow-auto">
            <table className="w-full min-w-[520px] text-left text-sm">
              <thead className="text-slate-500">
                <tr><th className="p-3">Product</th><th className="p-3">Rating</th><th className="p-3">Sales</th></tr>
              </thead>
              <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
                {analytics.topProducts.map((product) => (
                  <tr key={product.id}>
                    <td className="p-3 font-bold text-slate-950 dark:text-white">{product.name}</td>
                    <td className="p-3"><Stars value={product.rating} /></td>
                    <td className="p-3">{product.sales}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </Panel>
        <Panel title="Recent Orders">
          <div className="grid gap-3">
            {analytics.recentOrders.map((order) => (
              <div key={order.id} className="flex items-center justify-between gap-3 rounded-xl border border-slate-200 p-3 dark:border-slate-800">
                <div>
                  <p className="font-bold text-slate-950 dark:text-white">{order.id}</p>
                  <p className="text-sm text-slate-500">{order.customerName}</p>
                </div>
                <StatusBadge status={order.status} />
              </div>
            ))}
          </div>
        </Panel>
      </div>
    </div>
  );
}
