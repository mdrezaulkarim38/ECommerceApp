import { useState } from "react";
import { Link } from "react-router-dom";
import { X } from "lucide-react";
import toast from "react-hot-toast";
import { Breadcrumbs, Stars } from "../../components/common";
import { useStore } from "../../context/StoreContext";
import { formatCurrency } from "../../utils/pricing";

export function ComparePage() {
  const { state, isAuthenticated, actions } = useStore();
  const [selected, setSelected] = useState(["p-1001", "p-1006"]);
  const products = selected.map((id) => state.products.find((product) => product.id === id)).filter(Boolean);
  const addProduct = (id) => {
    if (selected.includes(id)) return;
    if (selected.length >= 4) {
      toast.error("You can compare up to 4 products");
      return;
    }
    setSelected([...selected, id]);
  };

  return (
    <>
      <Breadcrumbs current="Compare" />
      <main className="mx-auto max-w-7xl px-4 py-8">
        <div className="mb-6 flex flex-wrap items-end justify-between gap-4">
          <div>
            <h1 className="text-3xl font-black text-slate-950 dark:text-white">Compare Products</h1>
            <p className="text-slate-600 dark:text-slate-300">Select up to four products and compare specs side by side.</p>
          </div>
          <select className="input max-w-sm" onChange={(event) => addProduct(event.target.value)} value="">
            <option value="" disabled>Add product</option>
            {state.products.map((product) => <option key={product.id} value={product.id}>{product.name}</option>)}
          </select>
        </div>
        <div className="overflow-auto rounded-2xl border border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900">
          <table className="w-full min-w-[900px] text-left text-sm">
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {["Image", "Price", "Rating", "Category", "Brand", "Availability", "Features", "Action"].map((row) => (
                <tr key={row}>
                  <th className="w-40 bg-slate-50 p-4 dark:bg-slate-800">{row}</th>
                  {products.map((product) => (
                    <td key={product.id} className="p-4 align-top">
                      {row === "Image" && (
                        <div>
                          <button type="button" className="float-right text-rose-600" onClick={() => setSelected(selected.filter((id) => id !== product.id))}><X size={16} /></button>
                          <img src={product.image} alt="" className="h-28 w-36 rounded-xl object-cover" />
                          <p className="mt-2 font-bold text-slate-950 dark:text-white">{product.name}</p>
                        </div>
                      )}
                      {row === "Price" && formatCurrency(product.price)}
                      {row === "Rating" && <Stars value={product.rating} />}
                      {row === "Category" && product.category}
                      {row === "Brand" && product.brand}
                      {row === "Availability" && `${product.stock} in stock`}
                      {row === "Features" && product.features.join(", ")}
                      {row === "Action" && (
                        isAuthenticated ? <button className="btn-mini" type="button" onClick={() => actions.addToCart(product.id)}>Add to Cart</button> : <Link className="btn-mini" to="/auth">Login to Buy</Link>
                      )}
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </main>
    </>
  );
}
