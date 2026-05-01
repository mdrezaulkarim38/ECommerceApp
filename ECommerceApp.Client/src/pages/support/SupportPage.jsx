import { useState } from "react";
import { MessageCircle, Plus, ShieldCheck, Truck } from "lucide-react";
import toast from "react-hot-toast";
import { Breadcrumbs } from "../../components/common";

export function SupportPage() {
  const [openFaq, setOpenFaq] = useState("Orders");
  const [chat, setChat] = useState(false);
  const faqs = {
    Orders: "You can place, track, and reorder mock orders from the account dashboard.",
    Payments: "Credit card, PayPal, and Cash on Delivery are simulated for presentation purposes.",
    Shipping: "Shipping costs are calculated in checkout and free above $200.",
    Returns: "Returns and refunds are mocked through the support form.",
    Account: "Profile, addresses, password, and wishlist are stored in localStorage.",
  };
  return (
    <>
      <Breadcrumbs current="Support" />
      <main className="mx-auto grid max-w-7xl gap-8 px-4 py-8 lg:grid-cols-[1fr_0.8fr]">
        <section>
          <h1 className="text-3xl font-black text-slate-950 dark:text-white">Help & Support Center</h1>
          <div className="mt-6 divide-y divide-slate-100 rounded-2xl border border-slate-200 bg-white dark:divide-slate-800 dark:border-slate-800 dark:bg-slate-900">
            {Object.entries(faqs).map(([title, body]) => (
              <div key={title}>
                <button className="flex w-full items-center justify-between p-5 text-left font-bold" type="button" onClick={() => setOpenFaq(openFaq === title ? "" : title)}>
                  {title}<Plus size={18} />
                </button>
                {openFaq === title && <p className="px-5 pb-5 text-slate-600 dark:text-slate-300">{body}</p>}
              </div>
            ))}
          </div>
          <div className="mt-6 grid gap-5 md:grid-cols-2">
            <div className="rounded-2xl bg-slate-50 p-5 dark:bg-slate-900"><Truck className="mb-2 text-teal-600" /><h2 className="font-black">Shipping Information</h2><p className="mt-2 text-sm text-slate-600 dark:text-slate-300">Standard delivery is simulated at 3 to 5 business days with free shipping above $200.</p></div>
            <div className="rounded-2xl bg-slate-50 p-5 dark:bg-slate-900"><ShieldCheck className="mb-2 text-teal-600" /><h2 className="font-black">Returns & Refunds</h2><p className="mt-2 text-sm text-slate-600 dark:text-slate-300">Refund requests can be submitted through the mock contact form for supervisor demo flow.</p></div>
          </div>
        </section>
        <section className="rounded-2xl border border-slate-200 bg-white p-6 dark:border-slate-800 dark:bg-slate-900">
          <h2 className="text-xl font-black text-slate-950 dark:text-white">Contact Us</h2>
          <form
            className="mt-4 space-y-4"
            onSubmit={(event) => {
              event.preventDefault();
              toast.success("Support request submitted");
              event.currentTarget.reset();
            }}
          >
            <input className="input" placeholder="Name" required />
            <input className="input" placeholder="Email" type="email" required />
            <input className="input" placeholder="Order ID (optional)" />
            <textarea className="input min-h-32" placeholder="Message" required />
            <button className="btn-primary w-full" type="submit">Submit Request</button>
          </form>
        </section>
      </main>
      <button className="fixed bottom-20 right-5 z-40 grid h-14 w-14 place-items-center rounded-full bg-teal-600 text-white shadow-xl" type="button" onClick={() => setChat((value) => !value)}>
        <MessageCircle />
      </button>
      {chat && (
        <div className="fixed bottom-36 right-5 z-40 w-80 rounded-2xl border border-slate-200 bg-white p-4 shadow-2xl dark:border-slate-800 dark:bg-slate-900">
          <p className="font-bold text-slate-950 dark:text-white">SmartShop Chat</p>
          <p className="mt-2 rounded-xl bg-slate-100 p-3 text-sm dark:bg-slate-800">Hi, this is a mock live chat. How can we help?</p>
          <input className="input mt-3" placeholder="Type a message..." />
        </div>
      )}
    </>
  );
}
