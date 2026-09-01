import HandoverBundle from '../components/HandoverBundle'
import { color, font } from '../theme'

export default function Portal() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
      <div style={{ background: color.deep, borderRadius: 12, padding: 26, color: color.white }}>
        <div style={{ fontSize: 12, color: '#B9D3D0', fontWeight: 700, textTransform: 'uppercase', letterSpacing: '0.06em' }}>Kariba Solar Services</div>
        <div style={{ fontFamily: font.display, fontSize: 22, fontWeight: 800, marginTop: 8 }}>Livingstone Lodge Collective is producing well.</div>
        <div style={{ fontSize: 13, color: '#B9D3D0', marginTop: 8, maxWidth: 480 }}>
          Your 95 kWp system generated 11.4 MWh this month — 4% above forecast. Next scheduled maintenance visit is 14 Sep 2026.
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3,1fr)', gap: 14 }}>
        {[
          { label: 'This month', value: '11.4 MWh' },
          { label: 'Performance ratio', value: '81.6%' },
          { label: 'CO₂ avoided', value: '8.9 t' },
        ].map((s) => (
          <div key={s.label} style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: '16px 18px' }}>
            <div style={{ fontSize: 12, color: color.slate }}>{s.label}</div>
            <div style={{ fontFamily: font.display, fontSize: 22, fontWeight: 800, color: color.ink, marginTop: 6 }}>{s.value}</div>
          </div>
        ))}
      </div>

      <HandoverBundle />
    </div>
  )
}
