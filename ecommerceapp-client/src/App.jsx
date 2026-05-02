import { Navigate, Route, Routes, useLocation } from "react-router-dom";
import { BackToTop, Footer, Header, NewsletterPopup } from "./components/common";
import { useStore } from "./context/StoreContext";
import AdminDashboard from "./pages/admin/AdminDashboard";
import {
  AuthPage,
  BrandDetailPage,
  BrandsPage,
  CartPage,
  CheckoutPage,
  ComparePage,
  DealsPage,
  HomePage,
  NotFoundPage,
  ProductDetailPage,
  ProfilePage,
  RecommendationsPage,
  SupportPage,
  TrackOrderPage,
} from "./pages";

function ProtectedRoute({ children, admin = false }) {
  const { isAuthenticated, isAdmin } = useStore();
  const location = useLocation();

  if (admin) {
    return isAdmin ? children : <Navigate to="/" replace />;
  }

  if (!isAuthenticated) {
    return <Navigate to="/auth" replace state={{ from: location }} />;
  }

  return children;
}

export default function App() {
  const location = useLocation();
  const isAdminRoute = location.pathname.startsWith("/admin");

  return (
    <div className="min-h-screen bg-slate-50 text-slate-700 transition-colors dark:bg-slate-950 dark:text-slate-200">
      {!isAdminRoute && <Header />}
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/auth" element={<AuthPage />} />
        <Route path="/products/:productId" element={<ProductDetailPage />} />
        <Route path="/deals" element={<DealsPage />} />
        <Route path="/brands" element={<BrandsPage />} />
        <Route path="/sellers" element={<BrandsPage />} />
        <Route path="/brands/:brandId" element={<BrandDetailPage />} />
        <Route path="/compare" element={<ComparePage />} />
        <Route path="/support" element={<SupportPage />} />
        <Route
          path="/cart"
          element={
            <ProtectedRoute>
              <CartPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/checkout"
          element={
            <ProtectedRoute>
              <CheckoutPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/profile"
          element={
            <ProtectedRoute>
              <ProfilePage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/wishlist"
          element={
            <ProtectedRoute>
              <ProfilePage forcedTab="Wishlist" />
            </ProtectedRoute>
          }
        />
        <Route
          path="/orders"
          element={
            <ProtectedRoute>
              <ProfilePage forcedTab="Order History" />
            </ProtectedRoute>
          }
        />
        <Route
          path="/recommendations"
          element={
            <ProtectedRoute>
              <RecommendationsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/track-order/:orderId"
          element={
            <ProtectedRoute>
              <TrackOrderPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/admin/*"
          element={
            <ProtectedRoute admin>
              <AdminDashboard />
            </ProtectedRoute>
          }
        />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
      {!isAdminRoute && <Footer />}
      {!isAdminRoute && <NewsletterPopup />}
      <BackToTop />
    </div>
  );
}
