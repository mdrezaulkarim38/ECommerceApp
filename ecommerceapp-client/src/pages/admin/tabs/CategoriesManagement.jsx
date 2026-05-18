import { useState } from "react";
import { Edit3, Plus, Trash2 } from "lucide-react";
import toast from "react-hot-toast";
import { Modal } from "../../../components/common";
import { useStore } from "../../../context/StoreContext";

const emptyCategory = { name: "", description: "", displayOrder: 0, parentCategoryId: null, imageUrl: "" };

export function CategoriesManagement() {
  const { state, actions } = useStore();
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState(null);

  const openNew = () => {
    setEditing({ ...emptyCategory });
    setModalOpen(true);
  };

  const openEdit = (cat) => {
    setEditing({ id: cat.id, name: cat.name, description: cat.description || "", displayOrder: cat.displayOrder, parentCategoryId: cat.parentCategoryId, imageUrl: cat.imageUrl || "" });
    setModalOpen(true);
  };

  const handleDelete = (cat) => {
    if (window.confirm(`Delete category "${cat.name}"?`)) {
      actions.adminDeleteCategory(cat.id);
    }
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    const form = new FormData(event.target);
    const payload = {
      name: form.get("name"),
      description: form.get("description"),
      displayOrder: Number(form.get("displayOrder")) || 0,
      parentCategoryId: form.get("parentCategoryId") ? Number(form.get("parentCategoryId")) : null,
      imageUrl: form.get("imageUrl") || "",
    };
    if (editing?.id) {
      actions.adminUpdateCategory({ ...payload, id: editing.id });
    } else {
      actions.adminAddCategory(payload);
    }
    setModalOpen(false);
  };

  const categories = state.categories || [];

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h2 className="text-lg font-bold">All Categories</h2>
        <button className="btn-primary" type="button" onClick={openNew}><Plus size={18} /> Add Category</button>
      </div>
      <div className="overflow-auto rounded-2xl border border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900">
        <table className="w-full min-w-[780px] text-left text-sm">
          <thead className="bg-slate-50 text-slate-500 dark:bg-slate-800">
            <tr>
              <th className="p-4">ID</th>
              <th className="p-4">Name</th>
              <th className="p-4">Slug</th>
              <th className="p-4">Display Order</th>
              <th className="p-4">Products</th>
              <th className="p-4">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
            {categories.length === 0 && (
              <tr><td colSpan={6} className="p-8 text-center text-slate-400">No categories yet</td></tr>
            )}
            {categories.map((cat) => (
              <tr key={cat.id}>
                <td className="p-4 font-mono text-xs">{cat.id}</td>
                <td className="p-4 font-bold text-slate-950 dark:text-white">{cat.name}</td>
                <td className="p-4 text-slate-500">{cat.slug}</td>
                <td className="p-4">{cat.displayOrder}</td>
                <td className="p-4">{cat.productCount || 0}</td>
                <td className="p-4">
                  <div className="flex gap-2">
                    <button className="btn-icon" type="button" onClick={() => openEdit(cat)}><Edit3 size={16} /></button>
                    <button className="btn-icon text-rose-600" type="button" onClick={() => handleDelete(cat)}><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing?.id ? "Edit Category" : "Add Category"}>
        <form className="grid gap-4" onSubmit={handleSubmit}>
          <div>
            <label className="label">Name</label>
            <input className="input" name="name" defaultValue={editing?.name || ""} required />
          </div>
          <div>
            <label className="label">Description</label>
            <textarea className="input min-h-20" name="description" defaultValue={editing?.description || ""} />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">Display Order</label>
              <input className="input" type="number" name="displayOrder" defaultValue={editing?.displayOrder ?? 0} />
            </div>
            <div>
              <label className="label">Parent Category</label>
              <select className="input" name="parentCategoryId" defaultValue={editing?.parentCategoryId || ""}>
                <option value="">None (Top level)</option>
                {categories.filter((c) => c.id !== editing?.id).map((c) => (
                  <option key={c.id} value={c.id}>{c.name}</option>
                ))}
              </select>
            </div>
          </div>
          <div>
            <label className="label">Image URL</label>
            <input className="input" name="imageUrl" defaultValue={editing?.imageUrl || ""} placeholder="https://..." />
          </div>
          <div className="flex justify-end gap-3">
            <button className="btn-secondary" type="button" onClick={() => setModalOpen(false)}>Cancel</button>
            <button className="btn-primary" type="submit">{editing?.id ? "Update" : "Create"} Category</button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
