import HandoverBundle from '../components/HandoverBundle'
import LifecycleTimeline from '../components/LifecycleTimeline'
import { ScreenProps } from '../types'
import { color, font } from '../theme'

const TYPES = [
  { key: 'rooftop', label: 'Rooftop' },
  { key: 'ground', label: 'Ground-mount' },
  { key: 'carport', label: 'Carport' },
]

export default function Commissioning({ state, set }: ScreenProps) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
      <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: 18 }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <div style={{ fontFamily: font.display, fontSize: 15, fontWeight: 700 }}>Mukuba Steel Rooftop — commissioning</div>
          <div style={{ display: 'flex', gap: 6 }}>
            {TYPES.map((t) => (
              <button
                key={t.key}
                onClick={() => set({ commType: t.key })}
                style={{ padding: '6px 12px', borderRadius: 8, border: `1px solid ${color.line}`, background: state.commType === t.key ? color.accentTint : color.white, color: state.commType === t.key ? color.deep : color.slate, fontSize: 12, fontWeight: 600, cursor: 'pointer' }}
              >
                {t.label}
              </button>
            ))}
          </div>
        </div>
        <LifecycleTimeline stage="commissioning" />
        <div style={{ fontSize: 13, color: color.slate, lineHeight: 1.6 }}>
          AC and DC commissioning tests complete. Final performance verification scheduled for Thursday before handover to operations.
        </div>
      </div>

      <HandoverBundle />
    </div>
  )
}
