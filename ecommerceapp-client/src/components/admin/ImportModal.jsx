import { useState, useRef } from "react";
import { Download, Loader, Upload, X } from "lucide-react";
import toast from "react-hot-toast";
import { Modal } from "../common";
import { adminService } from "../../services/api";

export function ImportModal({ open, onClose, onImported }) {
  const [file, setFile] = useState(null);
  const [uploading, setUploading] = useState(false);
  const [result, setResult] = useState(null);
  const fileRef = useRef(null);

  const handleDownloadTemplate = async () => {
    try {
      await adminService.downloadTemplate();
      toast.success("Template downloaded");
    } catch {
      toast.error("Failed to download template");
    }
  };

  const handleFileChange = (event) => {
    const selected = event.target.files?.[0];
    if (selected) {
      if (!selected.name.endsWith('.xlsx')) {
        toast.error("Please select an .xlsx file");
        return;
      }
      setFile(selected);
      setResult(null);
    }
  };

  const handleUpload = async () => {
    if (!file) {
      toast.error("Please select a file first");
      return;
    }

    setUploading(true);
    setResult(null);
    try {
      const data = await adminService.importProducts(file);
      setResult(data);
      if (data.imported > 0) {
        toast.success(`${data.imported} product(s) imported`);
        onImported?.();
      }
      if (data.errors?.length > 0) {
        toast.error(`${data.errors.length} error(s) found`);
      }
    } catch (err) {
      toast.error(err.response?.data?.message || "Import failed");
    }
    setUploading(false);
  };

  const handleClose = () => {
    setFile(null);
    setResult(null);
    onClose();
  };

  return (
    <Modal open={open} onClose={handleClose} title="Import Products">
      <div className="space-y-5">
        <div className="rounded-xl border border-slate-200 bg-slate-50 p-4 dark:border-slate-700 dark:bg-slate-800">
          <h3 className="mb-2 text-sm font-bold">How to import</h3>
          <ol className="ml-5 list-decimal space-y-1 text-sm text-slate-600 dark:text-slate-300">
            <li>Download the Excel template below</li>
            <li>Fill in your product data (Name is required, see column headers for format)</li>
            <li>Upload the completed .xlsx file here</li>
          </ol>
        </div>

        <div className="flex justify-center">
          <button className="btn-secondary" type="button" onClick={handleDownloadTemplate}>
            <Download size={18} /> Download Template
          </button>
        </div>

        <div className="border-t border-slate-200 pt-4 dark:border-slate-700">
          <div
            className="flex cursor-pointer flex-col items-center gap-2 rounded-xl border-2 border-dashed border-slate-300 p-8 text-center transition hover:border-teal-400 dark:border-slate-600"
            onClick={() => fileRef.current?.click()}
          >
            <Upload size={32} className="text-slate-400" />
            <p className="text-sm font-bold text-slate-600 dark:text-slate-300">
              {file ? file.name : "Click to select .xlsx file"}
            </p>
            <p className="text-xs text-slate-400">{file ? `${(file.size / 1024).toFixed(1)} KB` : "Max file size: 10MB"}</p>
          </div>
          <input ref={fileRef} type="file" accept=".xlsx" className="hidden" onChange={handleFileChange} />
        </div>

        {result && (
          <div className="rounded-xl border p-4 text-sm dark:border-slate-700">
            <p className="font-bold text-teal-600">Imported: {result.imported}</p>
            {result.errors?.length > 0 && (
              <details className="mt-2">
                <summary className="cursor-pointer font-bold text-rose-600">{result.errors.length} error(s)</summary>
                <ul className="ml-4 mt-1 list-disc text-xs text-slate-500">
                  {result.errors.map((e, i) => <li key={i}>{e}</li>)}
                </ul>
              </details>
            )}
          </div>
        )}

        <div className="flex justify-end gap-3">
          <button className="btn-secondary" type="button" onClick={handleClose}>
            <X size={18} /> Cancel
          </button>
          <button className="btn-primary" type="button" disabled={!file || uploading} onClick={handleUpload}>
            {uploading ? <Loader size={18} className="animate-spin" /> : <Upload size={18} />}
            {uploading ? "Importing..." : "Import"}
          </button>
        </div>
      </div>
    </Modal>
  );
}
