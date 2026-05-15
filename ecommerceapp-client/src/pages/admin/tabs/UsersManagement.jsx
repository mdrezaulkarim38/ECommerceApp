import { useEffect, useState } from "react";
import { ChevronLeft, ChevronRight, Search } from "lucide-react";
import { StatusPill } from "../../../components/admin";
import { adminService } from "../../../services/api";
import { useStore } from "../../../context/StoreContext";

export function UsersManagement() {
  const { state, actions } = useStore();
  const [query, setQuery] = useState("");
  const [users, setUsers] = useState([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalUsers, setTotalUsers] = useState(0);
  const pageSize = 10;

  useEffect(() => {
    (async () => {
      try {
        const result = await adminService.getUsers(page, pageSize, query);
        setUsers(result.users);
        setTotalPages(Math.max(1, Math.ceil(result.totalUsers / pageSize)));
        setTotalUsers(result.totalUsers);
      } catch { /* ignore */ }
    })();
  }, [page, query]);

  const orderCount = (userId) => state.orders.filter((order) => order.userId === userId).length;

  return (
    <div className="space-y-5">
      <label className="relative block max-w-xl">
        <Search className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
        <input className="input pl-11" value={query} onChange={(event) => { setQuery(event.target.value); setPage(1); }} placeholder="Search users" />
      </label>
      <div className="overflow-auto rounded-2xl border border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900">
        <table className="w-full min-w-[960px] text-left text-sm">
          <thead className="bg-slate-50 text-slate-500 dark:bg-slate-800">
            <tr><th className="p-4">Name</th><th className="p-4">Email</th><th className="p-4">Role</th><th className="p-4">Orders</th><th className="p-4">Actions</th></tr>
          </thead>
          <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
            {users.length === 0 ? (
              <tr><td colSpan={5} className="p-8 text-center text-slate-400">No users found</td></tr>
            ) : users.map((user) => (
              <tr key={user.id}>
                <td className="p-4 font-bold text-slate-950 dark:text-white">{user.name}</td>
                <td className="p-4">{user.email}</td>
                <td className="p-4"><StatusPill tone={user.role === "admin" ? "info" : "ok"} label={user.role} /></td>
                <td className="p-4">{orderCount(user.id)}</td>
                <td className="p-4">
                  <div className="flex flex-wrap gap-2">
                    <button className="btn-mini" type="button" onClick={() => actions.adminToggleBlock(user.id)}>Block</button>
                    <button className="btn-mini" type="button" onClick={() => actions.adminToggleRole(user.id)}>{user.role === "admin" ? "Demote" : "Promote"}</button>
                    <button className="btn-mini text-rose-600" type="button" onClick={() => window.confirm(`Delete ${user.email}?`) && actions.adminDeleteUser(user.id)}>Delete</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-slate-500">Showing {users.length} of {totalUsers} users</p>
          <div className="flex items-center gap-2">
            <button className="btn-mini" type="button" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>
              <ChevronLeft size={16} />
            </button>
            {Array.from({ length: totalPages }).map((_, i) => (
              <button
                key={i}
                type="button"
                onClick={() => setPage(i + 1)}
                className={`grid h-8 w-8 place-items-center rounded-full text-sm font-bold ${
                  page === i + 1
                    ? "bg-teal-600 text-white"
                    : "bg-white text-slate-600 ring-1 ring-slate-200 dark:bg-slate-900 dark:text-slate-200 dark:ring-slate-700"
                }`}
              >
                {i + 1}
              </button>
            ))}
            <button className="btn-mini" type="button" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}>
              <ChevronRight size={16} />
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
