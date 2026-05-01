import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const root = path.resolve(__dirname, "..");
const markdownPath = path.join(root, "docs", "API_ENDPOINT_ARCHITECTURE.md");
const outputPath = path.join(root, "docs", "API_ENDPOINT_ARCHITECTURE.pdf");

const markdown = fs.readFileSync(markdownPath, "utf8");

const pageWidth = 595.28;
const pageHeight = 841.89;
const marginX = 48;
const marginTop = 54;
const marginBottom = 46;
const usableWidth = pageWidth - marginX * 2;

const escapePdf = (value) =>
  value
    .replace(/\\/g, "\\\\")
    .replace(/\(/g, "\\(")
    .replace(/\)/g, "\\)")
    .replace(/[^\x09\x0A\x0D\x20-\x7E]/g, "");

const cleanInline = (value) =>
  value
    .replace(/\*\*/g, "")
    .replace(/`([^`]+)`/g, "$1")
    .replace(/\[(.*?)\]\(.*?\)/g, "$1")
    .replace(/^\s*-\s+/, "- ")
    .replace(/^\s*\d+\.\s+/, (match) => match.trim() + " ");

const widthOf = (text, size, font) => {
  const factor = font === "F3" ? 0.6 : 0.52;
  return text.length * size * factor;
};

const wrap = (text, size, font, indent = "") => {
  const words = text.split(/\s+/).filter(Boolean);
  const lines = [];
  let current = indent;
  for (const word of words) {
    const next = current.trim() ? `${current} ${word}` : `${indent}${word}`;
    if (widthOf(next, size, font) <= usableWidth || current.trim() === "") {
      current = next;
    } else {
      lines.push(current);
      current = `${indent}${word}`;
    }
  }
  if (current.trim()) lines.push(current);
  return lines;
};

const drawLines = [];
let inCode = false;

for (const rawLine of markdown.split(/\r?\n/)) {
  const line = rawLine.trimEnd();

  if (line.startsWith("```")) {
    inCode = !inCode;
    drawLines.push({ text: "", size: 4, font: "F1", leading: 5 });
    continue;
  }

  if (inCode) {
    const codeLine = rawLine.replace(/\t/g, "  ");
    drawLines.push(...wrap(codeLine || " ", 8.5, "F3").map((text) => ({ text, size: 8.5, font: "F3", leading: 11 })));
    continue;
  }

  if (!line.trim()) {
    drawLines.push({ text: "", size: 6, font: "F1", leading: 8 });
    continue;
  }

  if (line.startsWith("# ")) {
    drawLines.push(...wrap(cleanInline(line.slice(2)), 20, "F2").map((text) => ({ text, size: 20, font: "F2", leading: 25 })));
    drawLines.push({ text: "", size: 5, font: "F1", leading: 7 });
    continue;
  }

  if (line.startsWith("## ")) {
    drawLines.push({ text: "", size: 4, font: "F1", leading: 6 });
    drawLines.push(...wrap(cleanInline(line.slice(3)), 14, "F2").map((text) => ({ text, size: 14, font: "F2", leading: 18 })));
    continue;
  }

  if (line.startsWith("### ")) {
    drawLines.push({ text: "", size: 3, font: "F1", leading: 5 });
    drawLines.push(...wrap(cleanInline(line.slice(4)), 11.5, "F2").map((text) => ({ text, size: 11.5, font: "F2", leading: 15 })));
    continue;
  }

  const isEndpoint = /^-\s+`(GET|POST|PUT|PATCH|DELETE)\s/.test(line);
  const isBullet = line.trimStart().startsWith("-");
  const font = isEndpoint ? "F3" : isBullet ? "F1" : "F1";
  const size = isEndpoint ? 8.6 : 9.5;
  const indent = isBullet ? "  " : "";
  drawLines.push(
    ...wrap(cleanInline(line), size, font, indent).map((text) => ({
      text,
      size,
      font,
      leading: isEndpoint ? 11.5 : 13,
    })),
  );
}

const pages = [];
let currentPage = [];
let y = pageHeight - marginTop;

for (const line of drawLines) {
  if (y - line.leading < marginBottom) {
    pages.push(currentPage);
    currentPage = [];
    y = pageHeight - marginTop;
  }
  currentPage.push({ ...line, y });
  y -= line.leading;
}
if (currentPage.length) pages.push(currentPage);

const objects = [];
const addObject = (content) => {
  objects.push(content);
  return objects.length;
};

const catalogId = addObject("<< /Type /Catalog /Pages 2 0 R >>");
const pagesId = addObject("");
const helveticaId = addObject("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
const boldId = addObject("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>");
const courierId = addObject("<< /Type /Font /Subtype /Type1 /BaseFont /Courier >>");

const pageIds = [];

for (let pageIndex = 0; pageIndex < pages.length; pageIndex += 1) {
  const page = pages[pageIndex];
  const commands = [
    "BT",
    `/F2 9 Tf`,
    `1 0 0 1 ${marginX} ${pageHeight - 28} Tm`,
    `(SmartShop AI API Endpoint Architecture) Tj`,
    `/F1 8 Tf`,
    `1 0 0 1 ${pageWidth - 96} 28 Tm`,
    `(Page ${pageIndex + 1}) Tj`,
    "ET",
  ];

  for (const line of page) {
    if (!line.text.trim()) continue;
    commands.push("BT");
    commands.push(`/${line.font} ${line.size} Tf`);
    commands.push(`1 0 0 1 ${marginX} ${line.y.toFixed(2)} Tm`);
    commands.push(`(${escapePdf(line.text)}) Tj`);
    commands.push("ET");
  }

  const stream = commands.join("\n");
  const contentId = addObject(`<< /Length ${Buffer.byteLength(stream, "utf8")} >>\nstream\n${stream}\nendstream`);
  const pageId = addObject(
    `<< /Type /Page /Parent ${pagesId} 0 R /MediaBox [0 0 ${pageWidth} ${pageHeight}] /Resources << /Font << /F1 ${helveticaId} 0 R /F2 ${boldId} 0 R /F3 ${courierId} 0 R >> >> /Contents ${contentId} 0 R >>`,
  );
  pageIds.push(pageId);
}

objects[pagesId - 1] = `<< /Type /Pages /Kids [${pageIds.map((id) => `${id} 0 R`).join(" ")}] /Count ${pageIds.length} >>`;

let pdf = "%PDF-1.4\n";
const offsets = [0];

for (let index = 0; index < objects.length; index += 1) {
  offsets.push(Buffer.byteLength(pdf, "utf8"));
  pdf += `${index + 1} 0 obj\n${objects[index]}\nendobj\n`;
}

const xrefOffset = Buffer.byteLength(pdf, "utf8");
pdf += `xref\n0 ${objects.length + 1}\n`;
pdf += "0000000000 65535 f \n";
for (let index = 1; index < offsets.length; index += 1) {
  pdf += `${String(offsets[index]).padStart(10, "0")} 00000 n \n`;
}
pdf += `trailer\n<< /Size ${objects.length + 1} /Root ${catalogId} 0 R >>\nstartxref\n${xrefOffset}\n%%EOF\n`;

fs.writeFileSync(outputPath, pdf, "binary");
console.log(outputPath);
