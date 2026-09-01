import Chip, { ChipTone } from '../components/Chip'
import { ScreenProps } from '../types'
import { color, font } from '../theme'

const TABS = [
  { key: 'array', label: 'Array layout' },
  { key: 'electrical', label: 'Electrical' },
  { key: 'structural', label: 'Structural' },
]

interface Design {
  id: string
  project: string
  status: string
  revision: string
}

const STATUS_TONE: Record<string, ChipTone> = {
  Draft: 'slate',
  'In review': 'warning',
  Approved: 'success',
}

const DESIGNS: Design[] = [
  { id: 'DSN-0091', project: 'Mukuba Steel — Kitwe', status: 'In review', revision: 'Rev B' },
  { id: 'DSN-0088', project: 'Copperbelt Cold Chain — Ndola', status: 'Approved', revision: 'Rev C' },
  { id: 'DSN-0086', project: 'Livingstone Lodge Collective', status: 'Draft', revision: 'Rev A' },
]

const TAB_CONTENT: Record<string, string> = {
  array: 'Roof-mount array, 3 sub-arrays, 480 kWp DC, azimuth 0°, 12° tilt. String layout finalised, awaiting shading study sign-off.',
  electrical: 'Single-line diagram drafted: 3 × 150kW string inverters, AC combiner at roof level, 400A main tie-in.',
  structural: 'Roof loading assessment pending from structural engineer — required before Rev C can be issued.',
}

export default function Design({ state, set }: ScreenProps) {
  const selected = DESIGNS.find((d) => d.id === state.designSelected) || DESIGNS[0]

  return (
    <div style={{ display: 'grid', gridTemplateColumns: '1.4fr 1fr', gap: 14, alignItems: 'start' }}>
      <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, overflow: 'hidden' }}>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1.6fr 1fr 0.8fr', padding: '10px 16px', fontSize: 10.5, fontWeight: 700, letterSpacing: '0.5px', textTransform: 'uppercase', color: color.slateSoft, borderBottom: `1px solid ${color.line}` }}>
          <div>Design</div>
          <div>Project</div>
          <div>Status</div>
          <div>Revision</div>
        </div>
        {DESIGNS.map((d) => (
          <div
            key={d.id}
            onClick={() => set({ designSelected: d.id })}
            style={{ display: 'grid', gridTemplateColumns: '1fr 1.6fr 1fr 0.8fr', padding: '12px 16px', fontSize: 13, borderBottom: `1px solid ${color.mist2}`, cursor: 'pointer', alignItems: 'center', background: d.id === selected.id ? color.mist : 'transparent' }}
          >
            <div style={{ fontFamily: font.mono, fontSize: 12, color: color.slate }}>{d.id}</div>
            <div style={{ fontWeight: 600, color: color.ink }}>{d.project}</div>
            <div><Chip label={d.status} tone={STATUS_TONE[d.status]} /></div>
            <div style={{ color: color.slate }}>{d.revision}</div>
          </div>
        ))}
      </div>

      <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: 18 }}>
        <div style={{ fontFamily: font.display, fontSize: 15, fontWeight: 700 }}>{selected.project}</div>
        <div style={{ fontSize: 12, color: color.slateSoft, fontFamily: font.mono, marginTop: 2 }}>{selected.id} · {selected.revision}</div>

        <div style={{ display: 'flex', gap: 20, borderBottom: `1px solid ${color.line}`, marginTop: 16 }}>
          {TABS.map((t) => (
            <div
              key={t.key}
              onClick={() => set({ designTab: t.key })}
              style={{ padding: '10px 0', fontSize: 13, fontWeight: 700, color: state.designTab === t.key ? color.ink : color.slate, borderBottom: `2px solid ${state.designTab === t.key ? color.accent : 'transparent'}`, cursor: 'pointer' }}
            >
              {t.label}
            </div>
          ))}
        </div>
        <div style={{ fontSize: 13, color: color.slate, marginTop: 14, lineHeight: 1.6 }}>{TAB_CONTENT[state.designTab] || TAB_CONTENT.array}</div>
      </div>
    </div>
  )
}
