import { useEffect, useState } from "react";
import { BadgePercent } from "lucide-react";
import toast from "react-hot-toast";
import { Modal } from "./Modal";

export function NewsletterPopup() {
  const [open, setOpen] = useState(false);
  useEffect(() => {
    const seen = sessionStorage.getItem("smartshop_newsletter_seen");
    if (seen) return undefined;
    const timer = setTimeout(() => {
      setOpen(true);
      sessionStorage.setItem("smartshop_newsletter_seen", "1");
    }, 30000);
    return () => clearTimeout(timer);
  }, []);
  return (
    <Modal open={open} onClose={() => setOpen(false)} title="Get smarter deals">
      <div className="space-y-4">
        <div className="rounded-xl bg-teal-50 p-4 text-sm text-teal-900 dark:bg-teal-500/15 dark:text-teal-100">
          <BadgePercent className="mb-2" />
          Weekly AI-picked offers, restock alerts, and seasonal buying guides.
        </div>
        <form
          className="flex gap-2"
          onSubmit={(event) => {
            event.preventDefault();
            toast.success("Subscription saved");
            setOpen(false);
          }}
        >
          <input className="input" placeholder="you@example.com" type="email" required />
          <button className="btn-primary" type="submit">
            Subscribe
          </button>
        </form>
      </div>
    </Modal>
  );
}
