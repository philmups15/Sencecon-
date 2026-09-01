import { color, font } from '../theme'

export default function Empty() {
  return (
    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 14 }}>
      <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: 40, display: 'flex', flexDirection: 'column', alignItems: 'center', textAlign: 'center' }}>
        <div style={{ width: 44, height: 44, borderRadius: 12, background: color.mist2, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke={color.slateSoft} strokeWidth={2}>
            <circle cx="11" cy="11" r="7" />
            <path d="M21 21l-4.3-4.3" />
          </svg>
        </div>
        <div style={{ fontFamily: font.display, fontSize: 15, fontWeight: 700, color: color.ink, marginTop: 14 }}>No results for "Mansa Hill"</div>
        <div style={{ fontSize: 13, color: color.slate, marginTop: 6, maxWidth: 260 }}>Check the spelling, or search by opportunity, plant, or work order ID instead.</div>
        <button style={{ marginTop: 18, padding: '9px 18px', borderRadius: 8, border: `1px solid ${color.line}`, background: color.white, color: color.slate, fontSize: 12.5, fontWeight: 600, cursor: 'pointer' }}>Clear search</button>
      </div>

      <div style={{ background: color.errorBg, border: '1px solid #F0C6C2', borderRadius: 12, padding: 40, display: 'flex', flexDirection: 'column', alignItems: 'center', textAlign: 'center' }}>
        <div style={{ width: 44, height: 44, borderRadius: 12, background: color.white, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke={color.error} strokeWidth={2}>
            <path d="M12 8v5M12 16h.01" />
            <circle cx="12" cy="12" r="9" />
          </svg>
        </div>
        <div style={{ fontFamily: font.display, fontSize: 15, fontWeight: 700, color: '#7C2620', marginTop: 14 }}>Couldn't load plant telemetry</div>
        <div style={{ fontSize: 13, color: '#7C2620', marginTop: 6, maxWidth: 260 }}>The monitoring provider didn't respond. Retry, or check status at status.sencecon.com.</div>
        <button style={{ marginTop: 18, padding: '9px 18px', borderRadius: 8, border: 'none', background: color.error, color: color.white, fontSize: 12.5, fontWeight: 600, cursor: 'pointer' }}>Retry</button>
      </div>
    </div>
  )
}
