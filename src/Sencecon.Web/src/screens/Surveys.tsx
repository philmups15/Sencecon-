import Chip, { ChipTone } from '../components/Chip'
import LifecycleTimeline from '../components/LifecycleTimeline'
import { ScreenProps } from '../types'
import { color, font } from '../theme'

interface Survey {
  id: string
  site: string
  status: string
  progress: number
  surveyor: string
  date: string
}

const STATUS_TONE: Record<string, ChipTone> = {
  Scheduled: 'slate',
  'In progress': 'primary',
  Completed: 'accent',
  'Signed off': 'success',
}

const SURVEYS: Survey[] = [
  { id: 'SRV-0148', site: 'Kitwe Industrial Park', status: 'Signed off', progress: 100, surveyor: 'Mwansa B.', date: '18 Aug 2026' },
  { id: 'SRV-0151', site: 'Zamsun Retail Group — Lusaka', status: 'In progress', progress: 60, surveyor: 'Temba N.', date: '27 Aug 2026' },
  { id: 'SRV-0152', site: 'Copperbelt Cold Chain — Ndola', status: 'Completed', progress: 100, surveyor: 'Mwansa B.', date: '24 Aug 2026' },
  { id: 'SRV-0153', site: 'Mazabuka Sugar Co.', status: 'Scheduled', progress: 0, surveyor: 'Chanda K.', date: '05 Sep 2026' },
]

export default function Surveys({ state, set }: ScreenProps) {
  const selected = SURVEYS.find((s) => s.id === state.surveySelected) || SURVEYS[0]

  return (
    <div style={{ display: 'grid', gridTemplateColumns: '1.5fr 1fr', gap: 14, alignItems: 'start' }}>
      <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, overflow: 'hidden' }}>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1.6fr 1fr 1fr 1fr', padding: '10px 16px', fontSize: 10.5, fontWeight: 700, letterSpacing: '0.5px', textTransform: 'uppercase', color: color.slateSoft, borderBottom: `1px solid ${color.line}` }}>
          <div>Survey</div>
          <div>Site</div>
          <div>Status</div>
          <div>Surveyor</div>
          <div>Date</div>
        </div>
        {SURVEYS.map((s) => (
          <div
            key={s.id}
            onClick={() => set({ surveySelected: s.id })}
            style={{
              display: 'grid',
              gridTemplateColumns: '1fr 1.6fr 1fr 1fr 1fr',
              padding: '12px 16px',
              fontSize: 13,
              borderBottom: `1px solid ${color.mist2}`,
              cursor: 'pointer',
              alignItems: 'center',
              background: s.id === selected.id ? color.mist : 'transparent',
            }}
          >
            <div style={{ fontFamily: font.mono, fontSize: 12, color: color.slate }}>{s.id}</div>
            <div style={{ fontWeight: 600, color: color.ink }}>{s.site}</div>
            <div><Chip label={s.status} tone={STATUS_TONE[s.status]} /></div>
            <div style={{ color: color.slate }}>{s.surveyor}</div>
            <div style={{ color: color.slate }}>{s.date}</div>
          </div>
        ))}
      </div>

      <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: 18 }}>
        <div style={{ fontFamily: font.display, fontSize: 15, fontWeight: 700, color: color.ink }}>{selected.site}</div>
        <div style={{ fontSize: 12, color: color.slateSoft, fontFamily: font.mono, marginTop: 2 }}>{selected.id}</div>
        <div style={{ marginTop: 14 }}>
          <LifecycleTimeline stage="survey" variant="compact" />
        </div>
        <div style={{ marginTop: 16, display: 'flex', flexDirection: 'column', gap: 8, fontSize: 13 }}>
          <div><span style={{ color: color.slate }}>Progress</span><div style={{ fontWeight: 600 }}>{selected.progress}%</div></div>
          <div><span style={{ color: color.slate }}>Surveyor</span><div style={{ fontWeight: 600 }}>{selected.surveyor}</div></div>
          <div><span style={{ color: color.slate }}>Date</span><div style={{ fontWeight: 600 }}>{selected.date}</div></div>
        </div>
      </div>
    </div>
  )
}
