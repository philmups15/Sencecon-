import Chip, { ChipTone } from '../components/Chip'
import HandoverBundle from '../components/HandoverBundle'
import { ScreenProps } from '../types'
import { color, font } from '../theme'

interface Plant {
  id: string
  name: string
  capacity: string
  health: string
  healthTone: ChipTone
  pr: string
}

const PLANTS: Plant[] = [
  { id: 'PLT-014', name: 'Lusaka Ridge C&I', capacity: '1.4 MWp', health: 'Good', healthTone: 'success', pr: '82.4%' },
  { id: 'PLT-011', name: 'Kitwe Industrial Park', capacity: '480 kWp', health: 'Good', healthTone: 'success', pr: '80.1%' },
  { id: 'PLT-009', name: 'Ndola Cold Storage', capacity: '640 kWp', health: 'Critical', healthTone: 'error', pr: '69.8%' },
  { id: 'PLT-006', name: 'Livingstone Lodge', capacity: '95 kWp', healthTone: 'warning', health: 'At risk', pr: '76.2%' },
]

const PLANT_TABS: { key: string; label: string }[] = [
  { key: 'overview', label: 'Overview' },
  { key: 'handover', label: 'Handover' },
  { key: 'work', label: 'Work orders' },
  { key: 'activity', label: 'Activity' },
]

export default function Plants({ state, set }: ScreenProps) {
  const selected = PLANTS.find((p) => p.id === state.plantSelected) || PLANTS[0]

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
      <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, overflow: 'hidden' }}>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1.6fr 1fr 1fr 0.8fr', padding: '10px 16px', fontSize: 10.5, fontWeight: 700, letterSpacing: '0.5px', textTransform: 'uppercase', color: color.slateSoft, borderBottom: `1px solid ${color.line}` }}>
          <div>Plant</div>
          <div>Name</div>
          <div style={{ textAlign: 'right' }}>Capacity</div>
          <div style={{ textAlign: 'right' }}>Performance ratio</div>
          <div>Health</div>
        </div>
        {PLANTS.map((p) => (
          <div
            key={p.id}
            onClick={() => set({ plantSelected: p.id })}
            style={{ display: 'grid', gridTemplateColumns: '1fr 1.6fr 1fr 1fr 0.8fr', padding: '12px 16px', fontSize: 13, borderBottom: `1px solid ${color.mist2}`, cursor: 'pointer', alignItems: 'center', background: p.id === selected.id ? color.mist : 'transparent' }}
          >
            <div style={{ fontFamily: font.mono, fontSize: 12, color: color.slate }}>{p.id}</div>
            <div style={{ fontWeight: 600, color: color.ink }}>{p.name}</div>
            <div style={{ textAlign: 'right', color: color.slate, fontVariantNumeric: 'tabular-nums' }}>{p.capacity}</div>
            <div style={{ textAlign: 'right', color: color.slate, fontVariantNumeric: 'tabular-nums' }}>{p.pr}</div>
            <div><Chip label={p.health} tone={p.healthTone} /></div>
          </div>
        ))}
      </div>

      <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: 18 }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <div>
            <div style={{ fontFamily: font.display, fontSize: 15, fontWeight: 700 }}>{selected.name}</div>
            <div style={{ fontSize: 12, color: color.slateSoft, fontFamily: font.mono, marginTop: 2 }}>{selected.id} · {selected.capacity}</div>
          </div>
          <Chip label={selected.health} tone={selected.healthTone} />
        </div>

        <div style={{ display: 'flex', gap: 20, borderBottom: `1px solid ${color.line}`, marginTop: 16 }}>
          {PLANT_TABS.map((t) => (
            <div
              key={t.key}
              onClick={() => set({ plantTab: t.key as typeof state.plantTab })}
              style={{ padding: '10px 0', fontSize: 13, fontWeight: 700, color: state.plantTab === t.key ? color.ink : color.slate, borderBottom: `2px solid ${state.plantTab === t.key ? color.accent : 'transparent'}`, cursor: 'pointer' }}
            >
              {t.label}
            </div>
          ))}
        </div>

        <div style={{ marginTop: 14 }}>
          {state.plantTab === 'overview' && (
            <div style={{ fontSize: 13, color: color.slate, lineHeight: 1.6 }}>
              Performance ratio {selected.pr}, tracking within 3 points of design baseline. Last inspection 12 days ago, no open non-conformities.
            </div>
          )}
          {state.plantTab === 'handover' && <HandoverBundle />}
          {state.plantTab === 'work' && (
            <div style={{ fontSize: 13, color: color.slate, lineHeight: 1.6 }}>2 open work orders — HVAC filter replacement, quarterly inverter inspection.</div>
          )}
          {state.plantTab === 'activity' && (
            <div style={{ fontSize: 13, color: color.slate, lineHeight: 1.6 }}>Monitoring data synced 6 minutes ago. No alerts in the last 24 hours.</div>
          )}
        </div>
      </div>
    </div>
  )
}
