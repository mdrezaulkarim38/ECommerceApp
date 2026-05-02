export function AddressForm({ address, setAddress }) {
  const fields = [
    ["fullName", "Full name"],
    ["line1", "Address line 1"],
    ["line2", "Address line 2"],
    ["city", "City"],
    ["state", "State"],
    ["zip", "Zip code"],
    ["country", "Country"],
  ];
  return (
    <div className="grid gap-4 sm:grid-cols-2">
      {fields.map(([key, label]) => (
        <div key={key} className={key === "line1" || key === "line2" ? "sm:col-span-2" : ""}>
          <label className="label">{label}</label>
          <input className="input" value={address[key] || ""} onChange={(event) => setAddress({ ...address, [key]: event.target.value })} />
        </div>
      ))}
    </div>
  );
}
