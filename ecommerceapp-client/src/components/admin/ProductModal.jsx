import { useRef, useState } from "react";
import { Loader, Save, Upload, X } from "lucide-react";
import toast from "react-hot-toast";
import { Modal } from "../common";
import { useStore } from "../../context/StoreContext";
import { emptyProduct } from "../../data/adminDashboardData";
import apiClient from "../../services/apiClient";

export function ProductModal({ open, product, onClose }) {
  const { state, actions } = useStore();
  const [form, setForm] = useState(product || emptyProduct);
  const [uploading, setUploading] = useState(false);
  const fileRef = useRef(null);

  if (!form) return null;

  const handleUpload = async (event) => {
    const file = event.target.files?.[0];
    if (!file) return;

    if (file.size > 10 * 1024 * 1024) {
      toast.error("File too large. Max 10MB");
      return;
    }

    const allowed = ["image/jpeg", "image/png", "image/webp", "image/gif"];
    if (!allowed.includes(file.type)) {
      toast.error("Invalid file type. Allowed: jpg, png, webp, gif");
      return;
    }

    setUploading(true);
    try {
      const formData = new FormData();
      formData.append("file", file);
      const { data } = await apiClient.post("/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });
      setForm({ ...form, image: data.data });
      toast.success(data.message || "Image uploaded");
    } catch (err) {
      toast.error(err.response?.data?.message || "Upload failed");
    }
    setUploading(false);
    event.target.value = "";
  };

  const submit = (event) => {
    event.preventDefault();
    const catId = state.categories?.find((c) => c.name === form.category)?.id || null;
    const payload = {
      ...form,
      categoryId: catId,
      brandId: null,
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
          <select className="input" value={form.category || ""} onChange={(event) => setForm({ ...form, category: event.target.value })}>
            <option value="">Select category</option>
            {(state.categories || []).map((item) => <option key={item.id || item.name} value={item.name || item}>{item.name || item}</option>)}
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
          <label className="label">Product Image</label>
          <input ref={fileRef} type="file" accept="image/jpeg,image/png,image/webp,image/gif" className="hidden" onChange={handleUpload} />
          <div className="flex flex-wrap items-start gap-4">
            {form.image && (
              <div className="relative h-32 w-32 overflow-hidden rounded-xl border border-slate-200 dark:border-slate-700">
                <img src={form.image} alt="Preview" className="h-full w-full object-cover" />
                <button type="button" className="absolute right-1 top-1 grid h-6 w-6 place-items-center rounded-full bg-rose-600 text-white text-xs" onClick={() => setForm({ ...form, image: "" })}>
                  <X size={14} />
                </button>
              </div>
            )}
            <button type="button" className="btn-secondary" disabled={uploading} onClick={() => fileRef.current?.click()}>
              {uploading ? <Loader size={18} className="animate-spin" /> : <Upload size={18} />}
              {uploading ? "Uploading..." : "Upload Image"}
            </button>
          </div>
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
