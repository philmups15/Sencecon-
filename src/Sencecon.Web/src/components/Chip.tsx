import { ChipTone, toneMap } from '../theme'

export type { ChipTone }

export default function Chip({ label, tone = 'slate' }: { label: string; tone?: ChipTone }) {
  const t = toneMap[tone] || toneMap.slate
  return (
    <span
      style={{
        display: 'inline-flex',
        alignItems: 'center',
        gap: 6,
        padding: '3px 10px 3px 8px',
        borderRadius: 999,
        fontSize: 12,
        fontWeight: 600,
        lineHeight: 1.6,
        whiteSpace: 'nowrap',
        background: t.bg,
        color: t.fg,
      }}
    >
      <span style={{ width: 6, height: 6, borderRadius: 999, background: t.dot, flex: 'none' }} />
      {label}
    </span>
  )
}
