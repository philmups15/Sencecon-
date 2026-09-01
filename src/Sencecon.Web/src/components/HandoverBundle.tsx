import { color, font } from '../theme'

export interface HandoverItem {
  name: string
  meta: string
  ext: string
}

const DEFAULT_ITEMS: HandoverItem[] = [
  { name: 'Commissioning record', meta: 'PDF · Signed off 12 Jun 2026', ext: 'PDF' },
  { name: 'As-built drawings', meta: 'PDF · Rev C', ext: 'PDF' },
  { name: 'Equipment warranties', meta: 'ZIP · 4 documents', ext: 'ZIP' },
  { name: 'Spares list', meta: 'XLSX · 18 line items', ext: 'XLS' },
  { name: 'O&M manuals', meta: 'PDF · Inverter + monitoring', ext: 'PDF' },
  { name: 'Initial performance baseline', meta: 'PDF · First 30-day report', ext: 'PDF' },
]

export default function HandoverBundle({
  items = DEFAULT_ITEMS,
  generatedDate = '12 Jun 2026',
}: {
  items?: HandoverItem[]
  generatedDate?: string
}) {
  return (
    <div style={{ border: `1px solid ${color.line}`, borderRadius: 12, background: color.white, overflow: 'hidden' }}>
      <div
        style={{
          padding: '14px 18px',
          borderBottom: `1px solid ${color.line}`,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
        }}
      >
        <div style={{ fontFamily: font.display, fontSize: 14, fontWeight: 700, color: color.ink }}>Handover bundle</div>
        <div style={{ fontSize: 12, color: color.slate }}>Generated {generatedDate}</div>
      </div>
      {items.map((item, i) => (
        <div
          key={i}
          style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            padding: '11px 18px',
            borderBottom: `1px solid ${color.mist2}`,
          }}
        >
          <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
            <div
              style={{
                width: 28,
                height: 28,
                borderRadius: 8,
                background: color.mist,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                fontSize: 12,
                fontWeight: 700,
                color: color.slate,
              }}
            >
              {item.ext}
            </div>
            <div>
              <div style={{ fontSize: 13, fontWeight: 600, color: color.ink }}>{item.name}</div>
              <div style={{ fontSize: 11.5, color: color.slateSoft }}>{item.meta}</div>
            </div>
          </div>
          <button
            style={{
              border: `1px solid ${color.line}`,
              background: color.white,
              borderRadius: 8,
              padding: '5px 12px',
              fontSize: 12,
              fontWeight: 600,
              color: color.slate,
              cursor: 'pointer',
            }}
          >
            Download
          </button>
        </div>
      ))}
    </div>
  )
}
