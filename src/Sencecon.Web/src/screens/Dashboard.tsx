import Chip, { ChipTone } from '../components/Chip'
import { ScreenProps } from '../types'
import { color, font } from '../theme'

const KPIS = [
  { label: 'Operating plants', value: '27', trend: '+2 this quarter', trendColor: color.success },
  { label: 'In delivery', value: '9', trend: '3 in commissioning', trendColor: color.warning },
  { label: 'Open work orders', value: '46', trend: '6 breaching SLA', trendColor: color.error },
  { label: 'Pipeline value', value: '$4.2M', trend: '14 open opportunities', trendColor: color.slate },
]

const MAP_PINS = [
  { name: 'Lusaka Ridge C&I', x: '48%', y: '62%', color: color.primary },
  { name: 'Kitwe Industrial Park', x: '58%', y: '22%', color: color.primary },
  { name: 'Ndola Cold Storage', x: '66%', y: '26%', color: color.warning },
  { name: 'Livingstone Lodge', x: '40%', y: '88%', color: color.success },
]

const STAGE_DIST = [
  { label: 'Operating', count: 27, color: color.primary },
  { label: 'Commissioning', count: 5, color: color.mid },
  { label: 'Deployment', count: 4, color: color.warning },
  { label: 'Design & survey', count: 9, color: color.deep },
]

const ACTIVITY = [
  { who: 'Mwansa B.', what: 'signed off survey SRV-0148', time: '42 min ago', color: color.success },
  { who: 'Field team 3', what: 'closed WO-2288 at Kitwe Industrial Park', time: '2h ago', color: color.primary },
  { who: 'Chanda K.', what: 'moved Mukuba Steel to Proposal', time: '3h ago', color: color.mid },
  { who: 'System', what: 'flagged inverter fault at Ndola Cold Storage', time: '5h ago', color: color.error },
  { who: 'Temba N.', what: 'submitted commissioning test results — AC side', time: 'Yesterday', color: color.warning },
]

const ATTENTION_PLANTS: { name: string; issue: string; healthLabel: string; healthTone: ChipTone }[] = [
  { name: 'Ndola Cold Storage', issue: 'Performance ratio down 11% week-on-week', healthLabel: 'Critical', healthTone: 'error' },
  { name: 'Chisamba Farms', issue: '2 SLA-breaching work orders open', healthLabel: 'At risk', healthTone: 'warning' },
  { name: 'Livingstone Lodge', issue: 'Comms offline 6 hours', healthLabel: 'At risk', healthTone: 'warning' },
  { name: 'Mazabuka Sugar Co.', issue: 'Monitoring data stale 2 days', healthLabel: 'Watch', healthTone: 'slate' },
]

export default function Dashboard({}: ScreenProps) {
  const conicStops = STAGE_DIST.reduce<{ str: string[]; acc: number }>(
    (state, s) => {
      const total = STAGE_DIST.reduce((sum, x) => sum + x.count, 0)
      const start = state.acc
      const end = state.acc + (s.count / total) * 100
      state.str.push(`${s.color} ${start}% ${end}%`)
      state.acc = end
      return state
    },
    { str: [], acc: 0 },
  ).str.join(', ')

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4,1fr)', gap: 14 }}>
        {KPIS.map((k) => (
          <div key={k.label} style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: '16px 18px' }}>
            <div style={{ fontSize: 12, color: color.slate, fontWeight: 600 }}>{k.label}</div>
            <div style={{ fontFamily: font.display, fontSize: 26, fontWeight: 800, letterSpacing: '-0.5px', marginTop: 6, color: color.ink }}>{k.value}</div>
            <div style={{ fontSize: 11.5, marginTop: 6, color: k.trendColor, fontWeight: 600 }}>{k.trend}</div>
          </div>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1.3fr 1fr', gap: 14 }}>
        <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: 18 }}>
          <div style={{ fontFamily: font.display, fontSize: 13.5, fontWeight: 700, marginBottom: 12 }}>Plants — Zambia</div>
          <div style={{ position: 'relative', height: 260, background: color.mist, borderRadius: 10, border: `1px dashed ${color.line}` }}>
            {MAP_PINS.map((p) => (
              <div key={p.name} style={{ position: 'absolute', left: p.x, top: p.y, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                <div style={{ width: 11, height: 11, borderRadius: 999, background: p.color, border: `2px solid ${color.white}`, boxShadow: '0 1px 4px rgba(18,32,31,0.25)' }} />
                <div style={{ fontSize: 10.5, fontWeight: 600, color: color.slate, marginTop: 3, background: color.white, padding: '1px 5px', borderRadius: 4 }}>{p.name}</div>
              </div>
            ))}
          </div>
        </div>

        <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: 18 }}>
          <div style={{ fontFamily: font.display, fontSize: 13.5, fontWeight: 700, marginBottom: 12 }}>Lifecycle stage distribution</div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 20 }}>
            <div style={{ width: 120, height: 120, borderRadius: 999, flex: 'none', background: `conic-gradient(${conicStops})` }} />
            <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
              {STAGE_DIST.map((s) => (
                <div key={s.label} style={{ display: 'flex', alignItems: 'center', gap: 8, fontSize: 12.5, color: color.slate }}>
                  <span style={{ width: 8, height: 8, borderRadius: 2, background: s.color }} />
                  {s.label} <span style={{ color: color.slateSoft }}>{s.count}</span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 14 }}>
        <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: 18 }}>
          <div style={{ fontFamily: font.display, fontSize: 13.5, fontWeight: 700, marginBottom: 10 }}>This week</div>
          {ACTIVITY.map((a, i) => (
            <div key={i} style={{ display: 'flex', gap: 10, padding: '9px 0', borderBottom: `1px solid ${color.mist2}` }}>
              <div style={{ width: 6, height: 6, borderRadius: 999, background: a.color, marginTop: 6, flex: 'none' }} />
              <div>
                <div style={{ fontSize: 12.5, color: color.ink }}>
                  <b>{a.who}</b> {a.what}
                </div>
                <div style={{ fontSize: 11, color: color.slateSoft, marginTop: 2 }}>{a.time}</div>
              </div>
            </div>
          ))}
        </div>

        <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: 18 }}>
          <div style={{ fontFamily: font.display, fontSize: 13.5, fontWeight: 700, marginBottom: 10 }}>Plants needing attention</div>
          {ATTENTION_PLANTS.map((p) => (
            <div key={p.name} style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '9px 0', borderBottom: `1px solid ${color.mist2}` }}>
              <div>
                <div style={{ fontSize: 12.5, fontWeight: 600, color: color.ink }}>{p.name}</div>
                <div style={{ fontSize: 11, color: color.slateSoft }}>{p.issue}</div>
              </div>
              <Chip label={p.healthLabel} tone={p.healthTone} />
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}
