import { BarChart3, CheckCircle2, Shield, Sparkles } from "lucide-react";
import { Area, AreaChart, Bar, BarChart, CartesianGrid, Legend, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import { Metric, Panel } from "../../../components/admin";
import { demandData, forecastData, seasonalData } from "../../../data/adminDashboardData";

export function AnalyticsForecasting() {
  return (
    <div className="space-y-6">
      <div className="grid gap-6 xl:grid-cols-[1.2fr_0.8fr]">
        <Panel title="Sales Forecast: Next 3 Months">
          <div className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart data={forecastData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Area type="monotone" dataKey="upper" stroke="#99f6e4" fill="#ccfbf1" />
                <Area type="monotone" dataKey="lower" stroke="#e2e8f0" fill="#f1f5f9" />
                <Line type="monotone" dataKey="predicted" stroke="#0f766e" strokeWidth={3} />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </Panel>
        <Panel title="AI Model Performance Metrics">
          <div className="grid gap-4">
            <Metric label="Recommendation Accuracy" value="87.4%" icon={Sparkles} />
            <Metric label="Click-through Rate" value="12.5%" icon={CheckCircle2} />
            <Metric label="Conversion Rate" value="8.2%" icon={BarChart3} />
            <Metric label="Forecast Accuracy MAPE" value="92.3%" icon={Shield} />
          </div>
        </Panel>
      </div>
      <div className="grid gap-6 xl:grid-cols-2">
        <Panel title="Demand Forecasting by Category">
          <div className="h-72">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={demandData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="category" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="demand" fill="#0f766e" radius={[8, 8, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </Panel>
        <Panel title="Seasonal Trend Analysis">
          <div className="h-72">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={seasonalData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="week" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line type="monotone" dataKey="Electronics" stroke="#0f766e" strokeWidth={3} />
                <Line type="monotone" dataKey="Clothing" stroke="#e11d48" strokeWidth={3} />
                <Line type="monotone" dataKey="Sports" stroke="#2563eb" strokeWidth={3} />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </Panel>
      </div>
    </div>
  );
}
