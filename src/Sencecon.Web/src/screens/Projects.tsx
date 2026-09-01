import LifecycleTimeline from '../components/LifecycleTimeline'
import { ScreenProps } from '../types'
import { color, font } from '../theme'

interface Project {
  id: string
  name: string
  customer: string
  pm: string
  budget: string
  actual: string
  stage: 'survey' | 'design' | 'deployment' | 'commissioning' | 'operating'
}

const PROJECTS: Project[] = [
  { id: 'PRJ-0032', name: 'Mukuba Steel Rooftop', customer: 'Mukuba Steel', pm: 'Bwalya M.', budget: '$612,000', actual: '$401,200', stage: 'deployment' },
  { id: 'PRJ-0029', name: 'Copperbelt Cold Chain', customer: 'Copperbelt Cold Chain', pm: 'Chanda K.', budget: '$798,000', actual: '$210,000', stage: 'design' },
  { id: 'PRJ-0026', name: 'Livingstone Lodge Collective', customer: 'Livingstone Lodge Collective', pm: 'Bwalya M.', budget: '$121,000', actual: '$118,400', stage: 'commissioning' },
]

const PROJECT_TABS: { key: string; label: string }[] = [
  { key: 'tasks', label: 'Tasks' },
  { key: 'subs', label: 'Subcontractors' },
  { key: 'budget', label: 'Budget' },
  { key: 'risk', label: 'Risk' },
]

const TAB_BODY: Record<string, string> = {
  tasks: '18 of 26 tasks complete. Next: mount racking on Block C, due Thursday.',
  subs: 'Roofing Co. Zambia (structural) · PowerLine Electrical (AC/DC install) · both on schedule.',
  budget: 'Committed $401,200 of $612,000 budget. Variance driven by a 6% steel racking price increase.',
  risk: 'One open risk: roof penetration count exceeds original structural assessment — awaiting engineer sign-off.',
}

export default function Projects({ state, set }: ScreenProps) {
  const selected = PROJECTS.find((p) => p.id === state.projectSelected) || PROJECTS[0]

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3,1fr)', gap: 14 }}>
        {PROJECTS.map((p) => (
          <div
            key={p.id}
            onClick={() => set({ projectSelected: p.id })}
            style={{ background: color.white, border: `1px solid ${p.id === selected.id ? color.primary : color.line}`, borderRadius: 12, padding: 16, cursor: 'pointer' }}
          >
            <div style={{ fontFamily: font.mono, fontSize: 11.5, color: color.slateSoft }}>{p.id}</div>
            <div style={{ fontFamily: font.display, fontSize: 14.5, fontWeight: 700, color: color.ink, marginTop: 4 }}>{p.name}</div>
            <div style={{ fontSize: 12, color: color.slate, marginTop: 8 }}>{p.budget} budget · {p.actual} spent</div>
          </div>
        ))}
      </div>

      <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: 18 }}>
        <div style={{ fontFamily: font.display, fontSize: 15, fontWeight: 700 }}>{selected.name}</div>
        <div style={{ fontSize: 12, color: color.slateSoft, marginTop: 2 }}>{selected.customer} · PM {selected.pm}</div>
        <LifecycleTimeline stage={selected.stage} />

        <div style={{ display: 'flex', gap: 20, borderBottom: `1px solid ${color.line}` }}>
          {PROJECT_TABS.map((t) => (
            <div
              key={t.key}
              onClick={() => set({ projectTab: t.key as typeof state.projectTab })}
              style={{ padding: '10px 0', fontSize: 13, fontWeight: 700, color: state.projectTab === t.key ? color.ink : color.slate, borderBottom: `2px solid ${state.projectTab === t.key ? color.accent : 'transparent'}`, cursor: 'pointer' }}
            >
              {t.label}
            </div>
          ))}
        </div>
        <div style={{ fontSize: 13, color: color.slate, marginTop: 14, lineHeight: 1.6 }}>{TAB_BODY[state.projectTab]}</div>
      </div>
    </div>
  )
}
