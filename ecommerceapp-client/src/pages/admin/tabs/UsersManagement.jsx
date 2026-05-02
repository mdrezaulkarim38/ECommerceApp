import { useState } from "react";
import { Search } from "lucide-react";
import { StatusPill } from "../../../components/admin";
import { useStore } from "../../../context/StoreContext";

export function UsersManagement() {
  const { state, actions } = useStore();
  const [query, setQuery] = useState("");
  const users = state.users.filter((user) => `${user.name} ${user.email}`.toLowerCase().includes(query.toLowerCase()));
  const orderCount = (userId) => state.orders.filter((order) => order.userId === userId).length;
  return (
    <div className="space-y-5">
      <label className="relative block max-w-xl">
        <Search className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
        <input className="input pl-11" value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search users" />
      </label>
      <div className="overflow-auto rounded-2xl border border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900">
        <table className="w-full min-w-[960px] text-left text-sm">
          <thead className="bg-slate-50 text-slate-500 dark:bg-slate-800">
            <tr><th className="p-4">Name</th><th className="p-4">Email</th><th className="p-4">Role</th><th className="p-4">Join Date</th><th className="p-4">Orders</th><th className="p-4">Actions</th></tr>
          </thead>
          <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
            {users.map((user) => (
              <tr key={user.id}>
                <td className="p-4 font-bold text-slate-950 dark:text-white">{user.name}</td>
                <td className="p-4">{user.email}</td>
                <td className="p-4"><StatusPill tone={user.role === "admin" ? "info" : "ok"} label={user.role} /></td>
                <td className="p-4">{user.joinedAt}</td>
                <td className="p-4">{orderCount(user.id)}</td>
                <td className="p-4">
                  <div className="flex flex-wrap gap-2">
                    <button className="btn-mini" type="button" onClick={() => actions.adminToggleBlock(user.id)}>{user.blocked ? "Unblock" : "Block"}</button>
                    <button className="btn-mini" type="button" onClick={() => actions.adminToggleRole(user.id)}>{user.role === "admin" ? "Demote" : "Promote"}</button>
                    <button className="btn-mini text-rose-600" type="button" onClick={() => window.confirm(`Delete ${user.email}?`) && actions.adminDeleteUser(user.id)}>Delete</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
