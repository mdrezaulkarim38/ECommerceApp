import { useEffect, useState } from "react";
import { Save } from "lucide-react";
import toast from "react-hot-toast";
import { Field, Panel, Toggle } from "../../../components/admin";
import { useStore } from "../../../context/StoreContext";
import { adminService } from "../../../services/api";

export function AdminSettings() {
  const { state, actions } = useStore();
  const [form, setForm] = useState(state.settings);

  useEffect(() => {
    (async () => {
      try {
        const settings = await adminService.getSettings();
        setForm(settings);
      } catch { /* ignore */ }
    })();
  }, []);
  const update = (key, value) => setForm((current) => ({ ...current, [key]: value }));
  return (
    <form
      className="grid gap-6 xl:grid-cols-2"
      onSubmit={(event) => {
        event.preventDefault();
        actions.updateSettings(form);
      }}
    >
      <Panel title="General Store Settings">
        <div className="grid gap-4">
          <Field label="Store Name" value={form.storeName} onChange={(value) => update("storeName", value)} />
          <Field label="Store Email" value={form.email} onChange={(value) => update("email", value)} />
          <Field label="Currency" value={form.currency} onChange={(value) => update("currency", value)} />
          <Field label="Tax Rate" value={form.taxRate} onChange={(value) => update("taxRate", value)} />
        </div>
      </Panel>
      <Panel title="AI Model Settings">
        <div className="space-y-4">
          <Field label="Retrain Schedule" value={form.retrainSchedule} onChange={(value) => update("retrainSchedule", value)} />
          <Toggle label="Recommendation Engine" checked={form.recommendationEnabled} onChange={(value) => update("recommendationEnabled", value)} />
          <Toggle label="Demand Forecasting" checked={form.forecastingEnabled} onChange={(value) => update("forecastingEnabled", value)} />
          <div className="flex flex-wrap gap-3">
            <button className="btn-secondary" type="button" onClick={() => toast.success("Backup created")}>Backup</button>
            <button className="btn-secondary" type="button" onClick={() => toast.success("Restore simulated")}>Restore</button>
            <button className="btn-secondary" type="button" onClick={actions.resetDemoData}>Reset Demo Data</button>
          </div>
          <button className="btn-primary" type="submit"><Save size={18} /> Save Settings</button>
        </div>
      </Panel>
    </form>
  );
}
