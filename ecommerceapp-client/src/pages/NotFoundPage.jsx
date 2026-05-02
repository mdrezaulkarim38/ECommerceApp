import { Link } from "react-router-dom";
import { AlertTriangle } from "lucide-react";
import { EmptyState } from "../components/common";

export function NotFoundPage() {
  return (
    <main className="mx-auto max-w-7xl px-4 py-12">
      <EmptyState icon={AlertTriangle} title="Page not found" message="The route does not exist in this frontend demo." action={<Link className="btn-primary" to="/">Go Home</Link>} />
    </main>
  );
}
