import Chip, { ChipTone } from '../components/Chip'
import { ScreenProps } from '../types'
import { color, font } from '../theme'

interface Opp {
  id: string
  customer: string
  capacity: string
  stage: string
  location: string
  next: string
  owner: string
  value: string
}

const STAGES = ['Qualifying', 'Site visit', 'Proposal', 'Negotiation', 'Won']
const STAGE_TONE: Record<string, ChipTone> = {
  Qualifying: 'slate',
  'Site visit': 'primary',
  Proposal: 'accent',
  Negotiation: 'warning',
  Won: 'success',
}

const OPPS: Opp[] = [
  { id: 'OPP-1042', customer: 'Mukuba Steel', capacity: '480 kWp', stage: 'Proposal', location: 'Kitwe', next: 'Send proposal v2', owner: 'Chanda K.', value: '$612,000' },
  { id: 'OPP-1039', customer: 'Zamsun Retail Group', capacity: '220 kWp', stage: 'Site visit', location: 'Lusaka', next: 'Schedule survey', owner: 'Chanda K.', value: '$289,000' },
  { id: 'OPP-1035', customer: 'Copperbelt Cold Chain', capacity: '640 kWp', stage: 'Negotiation', location: 'Ndola', next: 'Contract redlines', owner: 'Bwalya M.', value: '$798,000' },
  { id: 'OPP-1028', customer: 'Livingstone Lodge Collective', capacity: '95 kWp', stage: 'Won', location: 'Livingstone', next: 'Kick off survey', owner: 'Chanda K.', value: '$121,000' },
  { id: 'OPP-1021', customer: 'Mazabuka Sugar Co.', capacity: '1.2 MWp', stage: 'Qualifying', location: 'Mazabuka', next: 'Qualify budget', owner: 'Bwalya M.', value: '$1,480,000' },
]

export default function Opportunities({ state, set }: ScreenProps) {
  const byStage = STAGES.map((stage) => ({ stage, tone: STAGE_TONE[stage], cards: OPPS.filter((o) => o.stage === stage) }))
  const detail = OPPS.find((o) => o.id === state.oppSelected)

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
      <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
        <button
          onClick={() => set({ oppView: 'list' })}
          style={{ padding: '7px 14px', borderRadius: 8, border: `1px solid ${color.line}`, background: state.oppView === 'list' ? color.accentTint : color.white, color: color.slate, fontSize: 12.5, fontWeight: 600, cursor: 'pointer' }}
        >
          List
        </button>
        <button
          onClick={() => set({ oppView: 'kanban' })}
          style={{ padding: '7px 14px', borderRadius: 8, border: `1px solid ${color.line}`, background: state.oppView === 'kanban' ? color.accentTint : color.white, color: color.slate, fontSize: 12.5, fontWeight: 600, cursor: 'pointer' }}
        >
          Kanban
        </button>
      </div>

      {state.oppView === 'list' && (
        <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, overflow: 'hidden' }}>
          <div style={{ display: 'grid', gridTemplateColumns: '1.6fr 0.9fr 1fr 1.3fr 1fr 0.9fr', padding: '10px 16px', fontSize: 10.5, fontWeight: 700, letterSpacing: '0.5px', textTransform: 'uppercase', color: color.slateSoft, borderBottom: `1px solid ${color.line}` }}>
            <div>Customer</div>
            <div>Capacity</div>
            <div>Stage</div>
            <div>Next action</div>
            <div>Owner</div>
            <div>Value</div>
          </div>
          {OPPS.map((o) => (
            <div
              key={o.id}
              onClick={() => set({ oppSelected: o.id })}
              style={{ display: 'grid', gridTemplateColumns: '1.6fr 0.9fr 1fr 1.3fr 1fr 0.9fr', padding: '12px 16px', fontSize: 13, borderBottom: `1px solid ${color.mist2}`, cursor: 'pointer', alignItems: 'center' }}
            >
              <div style={{ fontWeight: 600, color: color.ink }}>
                {o.customer}
                <div style={{ fontSize: 11, color: color.slateSoft, fontWeight: 500 }}>{o.location}</div>
              </div>
              <div style={{ color: color.slate }}>{o.capacity}</div>
              <div>
                <Chip label={o.stage} tone={STAGE_TONE[o.stage]} />
              </div>
              <div style={{ color: color.slate }}>{o.next}</div>
              <div style={{ color: color.slate }}>{o.owner}</div>
              <div style={{ color: color.ink, fontWeight: 600 }}>{o.value}</div>
            </div>
          ))}
        </div>
      )}

      {state.oppView === 'kanban' && (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(5,1fr)', gap: 12 }}>
          {byStage.map((col) => (
            <div key={col.stage}>
              <div style={{ fontSize: 11.5, fontWeight: 700, color: color.slate, marginBottom: 8 }}>{col.stage}</div>
              <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
                {col.cards.map((c) => (
                  <div
                    key={c.id}
                    onClick={() => set({ oppSelected: c.id })}
                    style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 10, padding: 12, boxShadow: '0 1px 2px rgba(18,32,31,0.05)', cursor: 'pointer' }}
                  >
                    <div style={{ fontSize: 12.5, fontWeight: 700, color: color.ink }}>{c.customer}</div>
                    <div style={{ fontSize: 11, color: color.slate, marginTop: 3 }}>
                      {c.capacity} · {c.location}
                    </div>
                    <div style={{ fontSize: 11, color: color.slateSoft, marginTop: 6 }}>{c.next}</div>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}

      {detail && (
        <div style={{ position: 'fixed', top: 0, right: 0, bottom: 0, width: 380, background: color.white, borderLeft: `1px solid ${color.line}`, boxShadow: '-8px 0 24px rgba(18,32,31,0.12)', zIndex: 30, padding: 20, overflow: 'auto' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
            <div style={{ fontFamily: font.display, fontSize: 16, fontWeight: 700 }}>{detail.customer}</div>
            <span onClick={() => set({ oppSelected: null })} style={{ cursor: 'pointer', color: color.slateSoft, fontSize: 18 }}>
              ×
            </span>
          </div>
          <div style={{ fontSize: 12, color: color.slateSoft, fontFamily: font.mono, marginTop: 2 }}>{detail.id}</div>
          <div style={{ marginTop: 14, display: 'flex', flexDirection: 'column', gap: 10, fontSize: 13 }}>
            <div>
              <span style={{ color: color.slate }}>Capacity</span>
              <div style={{ fontWeight: 600 }}>{detail.capacity}</div>
            </div>
            <div>
              <span style={{ color: color.slate }}>Location</span>
              <div style={{ fontWeight: 600 }}>{detail.location}</div>
            </div>
            <div>
              <span style={{ color: color.slate }}>Indicative value</span>
              <div style={{ fontWeight: 600 }}>{detail.value}</div>
            </div>
            <div>
              <span style={{ color: color.slate }}>Next action</span>
              <div style={{ fontWeight: 600 }}>{detail.next}</div>
            </div>
            <div>
              <span style={{ color: color.slate }}>Owner</span>
              <div style={{ fontWeight: 600 }}>{detail.owner}</div>
            </div>
          </div>
          <button style={{ width: '100%', marginTop: 20, padding: 10, background: color.accent, color: color.ink, border: 'none', borderRadius: 8, fontWeight: 700, fontSize: 13, cursor: 'pointer' }}>
            Convert to project
          </button>
        </div>
      )}
    </div>
  )
}
