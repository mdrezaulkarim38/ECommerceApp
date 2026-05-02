import { useState } from "react";
import { useSearchParams } from "react-router-dom";
import { Heart, MapPin, Package, Plus, ShieldCheck } from "lucide-react";
import { AddressForm } from "../../components/account/AddressForm";
import { OrderDetailsModal } from "../../components/account/OrderDetailsModal";
import { Breadcrumbs, EmptyState, Modal, ProductCard, StatusBadge } from "../../components/common";
import { useStore } from "../../context/StoreContext";
import { formatCurrency } from "../../utils/pricing";

export function ProfilePage({ forcedTab }) {
  const { currentUser, currentUserOrders, wishlistProducts, actions } = useStore();
  const [searchParams] = useSearchParams();
  const initial = forcedTab || (searchParams.get("tab") === "orders" ? "Order History" : "Overview");
  const [tab, setTab] = useState(initial);
  const [details, setDetails] = useState(null);
  const [profile, setProfile] = useState({ name: currentUser.name, email: currentUser.email, phone: currentUser.phone, password: "" });
  const [addressOpen, setAddressOpen] = useState(false);
  const [address, setAddress] = useState({ label: "", fullName: currentUser.name, line1: "", line2: "", city: "", state: "", zip: "", country: "Bangladesh" });
  const tabs = ["Overview", "Order History", "Wishlist", "Addresses", "Settings"];

  return (
    <>
      <Breadcrumbs current="Profile" />
      <main className="mx-auto max-w-7xl px-4 py-8">
        <div className="mb-6 flex flex-wrap items-end justify-between gap-4">
          <div>
            <h1 className="text-3xl font-black text-slate-950 dark:text-white">My Account</h1>
            <p className="text-slate-600 dark:text-slate-300">Welcome back, {currentUser.name}</p>
          </div>
          <div className="flex flex-wrap gap-2">
            {tabs.map((item) => (
              <button
                key={item}
                type="button"
                onClick={() => setTab(item)}
                className={`rounded-full px-4 py-2 text-sm font-bold ${tab === item ? "bg-teal-600 text-white" : "bg-white text-slate-600 ring-1 ring-slate-200 dark:bg-slate-900 dark:text-slate-200 dark:ring-slate-700"}`}
              >
                {item}
              </button>
            ))}
          </div>
        </div>

        {tab === "Overview" && (
          <div className="grid gap-5 lg:grid-cols-4">
            {[
              ["Orders", currentUserOrders.length, Package],
              ["Wishlist", wishlistProducts.length, Heart],
              ["Addresses", currentUser.addresses?.length || 0, MapPin],
              ["Member Since", currentUser.joinedAt, ShieldCheck],
            ].map(([label, value, Icon]) => (
              <div key={label} className="rounded-2xl border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
                <Icon className="mb-4 text-teal-600 dark:text-teal-300" />
                <p className="text-sm font-bold text-slate-500">{label}</p>
                <p className="mt-1 text-2xl font-black text-slate-950 dark:text-white">{value}</p>
              </div>
            ))}
          </div>
        )}

        {tab === "Order History" && (
          <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900">
            <table className="w-full min-w-[760px] text-left text-sm">
              <thead className="bg-slate-50 text-slate-500 dark:bg-slate-800">
                <tr>
                  <th className="p-4">Order ID</th>
                  <th className="p-4">Date</th>
                  <th className="p-4">Total</th>
                  <th className="p-4">Status</th>
                  <th className="p-4">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
                {currentUserOrders.map((order) => (
                  <tr key={order.id}>
                    <td className="p-4 font-bold text-slate-950 dark:text-white">{order.id}</td>
                    <td className="p-4">{order.date}</td>
                    <td className="p-4">{formatCurrency(order.total)}</td>
                    <td className="p-4"><StatusBadge status={order.status} /></td>
                    <td className="p-4">
                      <div className="flex gap-2">
                        <button className="btn-mini" type="button" onClick={() => setDetails(order)}>View Details</button>
                        <button
                          className="btn-mini"
                          type="button"
                          onClick={() => order.items.forEach((item) => actions.addToCart(item.productId, item.quantity))}
                        >
                          Reorder
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {tab === "Wishlist" && (
          <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
            {wishlistProducts.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
            {!wishlistProducts.length && <div className="lg:col-span-4"><EmptyState icon={Heart} title="No wishlist products" message="Save products you like and manage them here." /></div>}
          </div>
        )}

        {tab === "Addresses" && (
          <div className="space-y-5">
            <button className="btn-primary" type="button" onClick={() => setAddressOpen(true)}><Plus size={18} /> Add Address</button>
            <div className="grid gap-5 md:grid-cols-2">
              {currentUser.addresses?.map((item) => (
                <div key={item.id} className="rounded-2xl border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
                  <p className="font-black text-slate-950 dark:text-white">{item.label}</p>
                  <p className="mt-2 text-slate-600 dark:text-slate-300">{item.fullName}</p>
                  <p className="text-sm text-slate-500">{item.line1}, {item.city}, {item.country}</p>
                  <button className="mt-4 text-sm font-bold text-rose-600" type="button" onClick={() => actions.deleteAddress(item.id)}>Delete</button>
                </div>
              ))}
            </div>
          </div>
        )}

        {tab === "Settings" && (
          <form
            className="max-w-2xl space-y-4 rounded-2xl border border-slate-200 bg-white p-6 dark:border-slate-800 dark:bg-slate-900"
            onSubmit={(event) => {
              event.preventDefault();
              actions.updateProfile(profile);
            }}
          >
            <div>
              <label className="label">Name</label>
              <input className="input" value={profile.name} onChange={(event) => setProfile({ ...profile, name: event.target.value })} />
            </div>
            <div>
              <label className="label">Email</label>
              <input className="input" value={profile.email} onChange={(event) => setProfile({ ...profile, email: event.target.value })} />
            </div>
            <div>
              <label className="label">Phone</label>
              <input className="input" value={profile.phone} onChange={(event) => setProfile({ ...profile, phone: event.target.value })} />
            </div>
            <div>
              <label className="label">New Password</label>
              <input className="input" type="password" value={profile.password} onChange={(event) => setProfile({ ...profile, password: event.target.value })} />
            </div>
            <button className="btn-primary" type="submit">Save Settings</button>
          </form>
        )}
      </main>
      <OrderDetailsModal order={details} onClose={() => setDetails(null)} />
      <Modal open={addressOpen} onClose={() => setAddressOpen(false)} title="Add Address">
        <form
          className="space-y-4"
          onSubmit={(event) => {
            event.preventDefault();
            actions.addAddress(address);
            setAddressOpen(false);
          }}
        >
          <input className="input" placeholder="Label" value={address.label} onChange={(event) => setAddress({ ...address, label: event.target.value })} />
          <AddressForm address={address} setAddress={setAddress} />
          <button className="btn-primary" type="submit">Save Address</button>
        </form>
      </Modal>
    </>
  );
}
