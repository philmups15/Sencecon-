import { color, font } from '../theme'

export default function Login({ onSignIn }: { onSignIn: () => void }) {
  return (
    <div style={{ minHeight: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <div style={{ width: 340, background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, padding: 28 }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 9, marginBottom: 22 }}>
          <span style={{ width: 20, height: 20, borderRadius: 5, background: color.accent, flex: 'none' }} />
          <div style={{ fontFamily: font.display, fontSize: 16, fontWeight: 800, color: color.deep }}>Sencecon</div>
        </div>

        <div style={{ fontFamily: font.display, fontSize: 18, fontWeight: 700, color: color.ink }}>Sign in</div>
        <div style={{ fontSize: 13, color: color.slate, marginTop: 4 }}>Access your solar delivery and operations workspace.</div>

        <div style={{ marginTop: 20, display: 'flex', flexDirection: 'column', gap: 14 }}>
          <div>
            <label style={{ display: 'block', fontSize: 12.5, fontWeight: 700, marginBottom: 6, color: color.ink }}>Email</label>
            <input
              type="email"
              defaultValue="chanda.k@kariba-solar.co.zm"
              style={{ width: '100%', boxSizing: 'border-box', padding: '10px 12px', borderRadius: 8, border: `1px solid ${color.line}`, fontSize: 13.5, fontFamily: font.body, color: color.ink }}
            />
          </div>
          <div>
            <label style={{ display: 'block', fontSize: 12.5, fontWeight: 700, marginBottom: 6, color: color.ink }}>Password</label>
            <input
              type="password"
              defaultValue="••••••••••"
              style={{ width: '100%', boxSizing: 'border-box', padding: '10px 12px', borderRadius: 8, border: `1px solid ${color.line}`, fontSize: 13.5, fontFamily: font.body, color: color.ink }}
            />
          </div>
        </div>

        <button
          onClick={onSignIn}
          style={{ width: '100%', marginTop: 22, padding: 11, background: color.accent, color: color.ink, border: 'none', borderRadius: 8, fontWeight: 700, fontSize: 13.5, cursor: 'pointer' }}
        >
          Sign in
        </button>
      </div>
    </div>
  )
}
