import { useMemo, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { Sparkles } from "lucide-react";
import toast from "react-hot-toast";
import { Breadcrumbs, Modal } from "../../components/common";
import { useStore } from "../../context/StoreContext";

export function AuthPage() {
  const { actions } = useStore();
  const navigate = useNavigate();
  const location = useLocation();
  const [mode, setMode] = useState("login");
  const [forgotOpen, setForgotOpen] = useState(false);
  const [loginForm, setLoginForm] = useState({ email: "", password: "", remember: true });
  const [registerForm, setRegisterForm] = useState({
    name: "",
    email: "",
    phone: "",
    address: "",
    password: "",
    confirmPassword: "",
  });

  const strength = useMemo(() => {
    const password = registerForm.password;
    let score = 0;
    if (password.length >= 8) score += 1;
    if (/[A-Z]/.test(password)) score += 1;
    if (/[0-9]/.test(password)) score += 1;
    if (/[^A-Za-z0-9]/.test(password)) score += 1;
    return score;
  }, [registerForm.password]);

  const submitLogin = (event) => {
    event.preventDefault();
    const user = actions.login(loginForm.email, loginForm.password);
    if (user) navigate(user.role === "admin" ? "/admin" : location.state?.from?.pathname || "/");
  };

  const submitRegister = (event) => {
    event.preventDefault();
    if (registerForm.password !== registerForm.confirmPassword) {
      toast.error("Passwords do not match");
      return;
    }
    if (strength < 2) {
      toast.error("Please use a stronger password");
      return;
    }
    const user = actions.register(registerForm);
    if (user) navigate("/");
  };

  return (
    <>
      <Breadcrumbs current="Login" />
      <main className="mx-auto grid min-h-[calc(100vh-10rem)] max-w-7xl gap-8 px-4 py-10 lg:grid-cols-[0.95fr_1.05fr]">
        <section className="relative overflow-hidden rounded-3xl bg-slate-950 p-8 text-white">
          <img
            src="https://images.unsplash.com/photo-1556742111-a301076d9d18?auto=format&fit=crop&w=1100&q=80"
            alt=""
            className="absolute inset-0 h-full w-full object-cover opacity-35"
          />
          <div className="relative z-10 flex h-full min-h-[520px] flex-col justify-between">
            <div>
              <span className="inline-flex items-center gap-2 rounded-full bg-white/15 px-4 py-2 text-sm font-bold backdrop-blur">
                <Sparkles size={16} /> Smart commerce demo
              </span>
              <h1 className="mt-6 max-w-xl text-4xl font-black leading-tight md:text-5xl">
                Personalized shopping meets business intelligence.
              </h1>
              <p className="mt-4 max-w-lg text-slate-200">
                Sign in as a customer, or use the admin demo account to explore analytics, products, orders, users, and AI metrics.
              </p>
            </div>
            <div className="grid gap-3 rounded-2xl bg-white/10 p-4 text-sm backdrop-blur">
              <p className="font-bold">Demo credentials</p>
              <p>Admin: admin@shop.com / admin123</p>
              <p>User: ayesha@example.com / user123</p>
            </div>
          </div>
        </section>

        <section className="rounded-3xl border border-slate-200 bg-white p-6 shadow-xl dark:border-slate-800 dark:bg-slate-900">
          <div className="mb-6 grid grid-cols-2 rounded-full bg-slate-100 p-1 dark:bg-slate-800">
            {["login", "register"].map((item) => (
              <button
                key={item}
                type="button"
                onClick={() => setMode(item)}
                className={`rounded-full px-4 py-3 text-sm font-bold capitalize transition ${
                  mode === item ? "bg-white text-teal-700 shadow dark:bg-slate-950 dark:text-teal-300" : "text-slate-500"
                }`}
              >
                {item}
              </button>
            ))}
          </div>

          {mode === "login" ? (
            <form className="space-y-4" onSubmit={submitLogin}>
              <div>
                <label className="label">Email</label>
                <input
                  className="input"
                  type="email"
                  required
                  value={loginForm.email}
                  onChange={(event) => setLoginForm({ ...loginForm, email: event.target.value })}
                />
              </div>
              <div>
                <label className="label">Password</label>
                <input
                  className="input"
                  type="password"
                  required
                  value={loginForm.password}
                  onChange={(event) => setLoginForm({ ...loginForm, password: event.target.value })}
                />
              </div>
              <div className="flex items-center justify-between gap-3 text-sm">
                <label className="flex items-center gap-2 text-slate-600 dark:text-slate-300">
                  <input
                    type="checkbox"
                    checked={loginForm.remember}
                    onChange={(event) => setLoginForm({ ...loginForm, remember: event.target.checked })}
                  />
                  Remember me
                </label>
                <button type="button" onClick={() => setForgotOpen(true)} className="font-bold text-teal-700 dark:text-teal-300">
                  Forgot password?
                </button>
              </div>
              <button className="btn-primary w-full" type="submit">
                Login
              </button>
            </form>
          ) : (
            <form className="space-y-4" onSubmit={submitRegister}>
              <div className="grid gap-4 sm:grid-cols-2">
                <div>
                  <label className="label">Full Name</label>
                  <input
                    className="input"
                    required
                    value={registerForm.name}
                    onChange={(event) => setRegisterForm({ ...registerForm, name: event.target.value })}
                  />
                </div>
                <div>
                  <label className="label">Phone</label>
                  <input
                    className="input"
                    required
                    value={registerForm.phone}
                    onChange={(event) => setRegisterForm({ ...registerForm, phone: event.target.value })}
                  />
                </div>
              </div>
              <div>
                <label className="label">Email</label>
                <input
                  className="input"
                  type="email"
                  required
                  value={registerForm.email}
                  onChange={(event) => setRegisterForm({ ...registerForm, email: event.target.value })}
                />
              </div>
              <div>
                <label className="label">Address</label>
                <input
                  className="input"
                  required
                  value={registerForm.address}
                  onChange={(event) => setRegisterForm({ ...registerForm, address: event.target.value })}
                />
              </div>
              <div className="grid gap-4 sm:grid-cols-2">
                <div>
                  <label className="label">Password</label>
                  <input
                    className="input"
                    type="password"
                    required
                    value={registerForm.password}
                    onChange={(event) => setRegisterForm({ ...registerForm, password: event.target.value })}
                  />
                  <div className="mt-2 grid grid-cols-4 gap-1">
                    {Array.from({ length: 4 }).map((_, index) => (
                      <span
                        key={index}
                        className={`h-1.5 rounded-full ${index < strength ? "bg-teal-500" : "bg-slate-200 dark:bg-slate-700"}`}
                      />
                    ))}
                  </div>
                </div>
                <div>
                  <label className="label">Confirm Password</label>
                  <input
                    className="input"
                    type="password"
                    required
                    value={registerForm.confirmPassword}
                    onChange={(event) => setRegisterForm({ ...registerForm, confirmPassword: event.target.value })}
                  />
                </div>
              </div>
              <button className="btn-primary w-full" type="submit">
                Create Account
              </button>
            </form>
          )}
        </section>
      </main>
      <Modal open={forgotOpen} onClose={() => setForgotOpen(false)} title="Reset password">
        <form
          className="space-y-4"
          onSubmit={(event) => {
            event.preventDefault();
            toast.success("Mock reset link sent");
            setForgotOpen(false);
          }}
        >
          <p className="text-slate-600 dark:text-slate-300">Enter your email and SmartShop will simulate a reset link.</p>
          <input className="input" type="email" placeholder="you@example.com" required />
          <button className="btn-primary" type="submit">
            Send Reset Link
          </button>
        </form>
      </Modal>
    </>
  );
}
