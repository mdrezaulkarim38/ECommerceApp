import { useState } from "react";
import { Download, Edit3, Plus, Search, Trash2, Upload } from "lucide-react";
import toast from "react-hot-toast";
import { ImportModal, ProductModal, StatusPill } from "../../../components/admin";
import { useStore } from "../../../context/StoreContext";
import { emptyProduct } from "../../../data/adminDashboardData";
import { adminService } from "../../../services/api";
import { formatCurrency } from "../../../utils/pricing";

export function ProductsManagement() {
  const { state, actions } = useStore();
  const [query, setQuery] = useState("");
  const [category, setCategory] = useState("All");
  const [editing, setEditing] = useState(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalKey, setModalKey] = useState(0);
  const [importOpen, setImportOpen] = useState(false);
  const products = state.products.filter((product) =>
    (category === "All" || product.category === category) &&
    `${product.name} ${product.sku}`.toLowerCase().includes(query.toLowerCase()),
  );

  const openNew = () => {
    setEditing({ ...emptyProduct });
    setModalKey((value) => value + 1);
    setModalOpen(true);
  };

  const openEdit = (product) => {
    setEditing({ ...product, featuresText: product.features?.join(", ") || "" });
    setModalKey((value) => value + 1);
    setModalOpen(true);
  };

  const handleExport = async () => {
    try {
      await adminService.exportProducts();
      toast.success("Products exported");
    } catch {
      toast.error("Export failed");
    }
  };

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div className="grid flex-1 gap-3 md:grid-cols-[1fr_220px]">
          <label className="relative">
            <Search className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
            <input className="input pl-11" value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search products" />
          </label>
            <select className="input" value={category} onChange={(event) => setCategory(event.target.value)}>
              <option>All</option>
              {(state.categories || []).map((item) => <option key={item.slug || item.id}>{item.name || item}</option>)}
            </select>
        </div>
        <div className="flex flex-wrap gap-2">
          <button className="btn-secondary" type="button" onClick={() => setImportOpen(true)}><Upload size={18} /> Import</button>
          <button className="btn-secondary" type="button" onClick={handleExport}><Download size={18} /> Export</button>
          <button className="btn-primary" type="button" onClick={openNew}><Plus size={18} /> Add New Product</button>
        </div>
      </div>
      <div className="overflow-auto rounded-2xl border border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900">
        <table className="w-full min-w-[980px] text-left text-sm">
          <thead className="bg-slate-50 text-slate-500 dark:bg-slate-800">
            <tr>
              <th className="p-4">ID</th>
              <th className="p-4">Image</th>
              <th className="p-4">Name</th>
              <th className="p-4">Price</th>
              <th className="p-4">Stock</th>
              <th className="p-4">Status</th>
              <th className="p-4">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
            {products.map((product) => (
              <tr key={product.id}>
                <td className="p-4 font-mono text-xs">{product.id}</td>
                <td className="p-4"><img src={product.image} alt="" className="h-12 w-12 rounded-lg object-cover" /></td>
                <td className="p-4 font-bold text-slate-950 dark:text-white">{product.name}</td>
                <td className="p-4">{formatCurrency(product.price)}</td>
                <td className="p-4">{product.stock}</td>
                <td className="p-4">{product.stock <= 8 ? <StatusPill tone="warn" label="Low Stock" /> : <StatusPill tone="ok" label="Active" />}</td>
                <td className="p-4">
                  <div className="flex gap-2">
                    <button className="btn-icon" type="button" onClick={() => openEdit(product)}><Edit3 size={16} /></button>
                    <button
                      className="btn-icon text-rose-600"
                      type="button"
                      onClick={() => {
                        if (window.confirm(`Delete ${product.name}?`)) actions.adminDeleteProduct(product.id);
                      }}
                    >
                      <Trash2 size={16} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <ProductModal key={modalKey} open={modalOpen} product={editing} onClose={() => setModalOpen(false)} />
      <ImportModal open={importOpen} onClose={() => setImportOpen(false)} onImported={() => actions.loadProducts()} />
    </div>
  );
}
