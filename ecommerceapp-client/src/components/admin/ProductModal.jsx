import { useState } from "react";
import { Save, X } from "lucide-react";
import { Modal } from "../common";
import { useStore } from "../../context/StoreContext";
import { emptyProduct } from "../../data/adminDashboardData";

export function ProductModal({ open, product, onClose }) {
  const { state, actions } = useStore();
  const [form, setForm] = useState(product || emptyProduct);

  if (!form) return null;

  const submit = (event) => {
    event.preventDefault();
    const payload = {
      ...form,
      features: (form.featuresText || "").split(",").map((item) => item.trim()).filter(Boolean),
      images: [form.image],
      specs: form.specs || { Warranty: "1 year", Source: "Admin demo" },
    };
    if (product?.id) actions.adminUpdateProduct(payload);
    else actions.adminAddProduct(payload);
    onClose();
  };

  return (
    <Modal open={open} onClose={onClose} title={product?.id ? "Edit Product" : "Add Product"} wide>
      <form className="grid gap-4 md:grid-cols-2" onSubmit={submit}>
        <div className="md:col-span-2">
          <label className="label">Name</label>
          <input className="input" value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} required />
        </div>
        <div className="md:col-span-2">
          <label className="label">Description</label>
          <textarea className="input min-h-24" value={form.description} onChange={(event) => setForm({ ...form, description: event.target.value })} />
        </div>
        <div>
          <label className="label">Price</label>
          <input className="input" type="number" value={form.price} onChange={(event) => setForm({ ...form, price: event.target.value })} />
        </div>
        <div>
          <label className="label">Original Price</label>
          <input className="input" type="number" value={form.originalPrice} onChange={(event) => setForm({ ...form, originalPrice: event.target.value })} />
        </div>
        <div>
          <label className="label">Stock</label>
          <input className="input" type="number" value={form.stock} onChange={(event) => setForm({ ...form, stock: event.target.value })} />
        </div>
        <div>
          <label className="label">Category</label>
          <select className="input" value={form.category} onChange={(event) => setForm({ ...form, category: event.target.value })}>
            {(state.categories || []).map((item) => <option key={item.id || item.name}>{item.name || item}</option>)}
          </select>
        </div>
        <div>
          <label className="label">Brand</label>
          <input className="input" value={form.brand} onChange={(event) => setForm({ ...form, brand: event.target.value })} />
        </div>
        <div>
          <label className="label">SKU</label>
          <input className="input" value={form.sku} onChange={(event) => setForm({ ...form, sku: event.target.value })} />
        </div>
        <div className="md:col-span-2">
          <label className="label">Image URL</label>
          <input className="input" value={form.image} onChange={(event) => setForm({ ...form, image: event.target.value })} />
        </div>
        <div className="md:col-span-2">
          <label className="label">Features</label>
          <input className="input" value={form.featuresText || ""} onChange={(event) => setForm({ ...form, featuresText: event.target.value })} placeholder="Comma separated" />
        </div>
        <div className="md:col-span-2 flex justify-end gap-3">
          <button className="btn-secondary" type="button" onClick={onClose}><X size={18} /> Cancel</button>
          <button className="btn-primary" type="submit"><Save size={18} /> Save Product</button>
        </div>
      </form>
    </Modal>
  );
}
