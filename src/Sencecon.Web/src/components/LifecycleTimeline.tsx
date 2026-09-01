import { color, font } from '../theme'

export type LifecycleStage = 'survey' | 'design' | 'deployment' | 'commissioning' | 'operating'

const STAGES: LifecycleStage[] = ['survey', 'design', 'deployment', 'commissioning', 'operating']
const LABELS: Record<LifecycleStage, string> = {
  survey: 'Survey',
  design: 'Design',
  deployment: 'Deployment',
  commissioning: 'Commissioning',
  operating: 'Operating',
}

interface Node {
  label: string
  mark: string | number
  circleBg: string
  circleFg: string
  circleBorder: string
  labelColor: string
  date: string
  hasLine: boolean
  lineColor: string
  isLast: 0 | 1
}

function buildNodes(stage: LifecycleStage, dates: Partial<Record<LifecycleStage, string>>): Node[] {
  const curIdx = STAGES.indexOf(stage)
  return STAGES.map((s, i) => {
    const state = i < curIdx ? 'past' : i === curIdx ? 'current' : 'upcoming'
    let circleBg = color.white
    let circleFg = color.slateSoft
    let circleBorder = color.line
    let labelColor = color.slateSoft
    let mark: string | number = i + 1
    if (state === 'past') {
      circleBg = color.accent
      circleFg = color.ink
      circleBorder = color.accent
      labelColor = color.ink
      mark = '✓'
    }
    if (state === 'current') {
      circleBg = color.primaryTint
      circleFg = color.deep
      circleBorder = color.primary
      labelColor = color.ink
    }
    return {
      label: LABELS[s],
      mark,
      circleBg,
      circleFg,
      circleBorder,
      labelColor,
      date: dates[s] || (state === 'upcoming' ? '—' : ''),
      hasLine: i < STAGES.length - 1,
      lineColor: i < curIdx ? color.accent : color.line,
      isLast: i === STAGES.length - 1 ? 0 : 1,
    }
  })
}

export default function LifecycleTimeline({
  stage = 'operating',
  variant = 'full',
  dates = {},
}: {
  stage?: LifecycleStage
  variant?: 'full' | 'compact'
  dates?: Partial<Record<LifecycleStage, string>>
}) {
  const nodes = buildNodes(stage, dates)

  if (variant === 'compact') {
    return (
      <div style={{ display: 'flex', alignItems: 'center', gap: 2, width: '100%' }}>
        {nodes.map((node, i) => (
          <div key={i} style={{ display: 'flex', alignItems: 'center', flex: 1 }}>
            <div
              title={node.label}
              style={{
                width: 10,
                height: 10,
                borderRadius: 999,
                flex: 'none',
                background: node.circleBg,
                border: `2px solid ${node.circleBorder}`,
              }}
            />
            {node.hasLine && (
              <div style={{ flex: 1, height: 2, background: node.lineColor, minWidth: 6 }} />
            )}
          </div>
        ))}
      </div>
    )
  }

  return (
    <div style={{ display: 'flex', alignItems: 'flex-start', width: '100%', padding: '20px 8px' }}>
      {nodes.map((node, i) => (
        <div key={i} style={{ display: 'flex', alignItems: 'center', flex: node.isLast }}>
          <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', minWidth: 96 }}>
            <div
              style={{
                width: 32,
                height: 32,
                borderRadius: 999,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                fontSize: 13,
                fontWeight: 700,
                background: node.circleBg,
                color: node.circleFg,
                border: `2px solid ${node.circleBorder}`,
              }}
            >
              {node.mark}
            </div>
            <div style={{ marginTop: 8, fontSize: 13, fontWeight: 600, color: node.labelColor, fontFamily: font.display }}>{node.label}</div>
            <div style={{ marginTop: 2, fontSize: 11, color: color.slate, fontFamily: font.mono }}>
              {node.date}
            </div>
          </div>
          {node.hasLine && (
            <div style={{ flex: 1, height: 2, background: node.lineColor, margin: '0 4px 34px', minWidth: 24 }} />
          )}
        </div>
      ))}
    </div>
  )
}
