export function Field({ label, value, onChange }) {
  return (
    <label>
      <span className="label">{label}</span>
      <input className="input" value={value} onChange={(event) => onChange(event.target.value)} />
    </label>
  );
}
