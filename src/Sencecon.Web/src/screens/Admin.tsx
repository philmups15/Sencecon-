import { color, font } from '../theme'

const USERS = [
  { name: 'Chanda Kalaba', email: 'chanda.k@kariba-solar.co.zm', role: 'Sales', status: 'Active' },
  { name: 'Bwalya Mutale', email: 'bwalya.m@kariba-solar.co.zm', role: 'Project Manager', status: 'Active' },
  { name: 'Mwansa Banda', email: 'mwansa.b@kariba-solar.co.zm', role: 'Design Engineer', status: 'Active' },
  { name: 'Temba Ngoma', email: 'temba.n@kariba-solar.co.zm', role: 'Field Technician', status: 'Disabled' },
]

const MODULES = ['Opportunities', 'Surveys', 'Projects', 'Plants', 'Reports']
const ROLES = ['Admin', 'Project Manager', 'Sales']

export default function Admin() {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
      <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, overflow: 'hidden' }}>
        <div style={{ padding: '14px 18px', borderBottom: `1px solid ${color.line}`, fontFamily: font.display, fontWeight: 700, fontSize: 14 }}>Users</div>
        <div style={{ display: 'grid', gridTemplateColumns: '1.4fr 1.6fr 1fr 0.8fr', padding: '10px 18px', fontSize: 10.5, fontWeight: 700, letterSpacing: '0.5px', textTransform: 'uppercase', color: color.slateSoft, borderBottom: `1px solid ${color.line}` }}>
          <div>Name</div>
          <div>Email</div>
          <div>Role</div>
          <div>Status</div>
        </div>
        {USERS.map((u) => (
          <div key={u.email} style={{ display: 'grid', gridTemplateColumns: '1.4fr 1.6fr 1fr 0.8fr', padding: '12px 18px', fontSize: 13, borderBottom: `1px solid ${color.mist2}`, alignItems: 'center' }}>
            <div style={{ fontWeight: 600, color: color.ink }}>{u.name}</div>
            <div style={{ color: color.slate }}>{u.email}</div>
            <div style={{ color: color.slate }}>{u.role}</div>
            <div style={{ color: u.status === 'Active' ? color.success : color.slateSoft, fontWeight: 600 }}>{u.status}</div>
          </div>
        ))}
      </div>

      <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, overflow: 'hidden' }}>
        <div style={{ padding: '14px 18px', borderBottom: `1px solid ${color.line}`, fontFamily: font.display, fontWeight: 700, fontSize: 14 }}>Role permissions</div>
        <div style={{ overflowX: 'auto' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 12.5 }}>
            <thead>
              <tr>
                <th style={{ textAlign: 'left', padding: '10px 18px', color: color.slateSoft, fontWeight: 700, textTransform: 'uppercase', fontSize: 10.5, letterSpacing: '0.5px' }}>Module</th>
                {ROLES.map((r) => (
                  <th key={r} style={{ textAlign: 'center', padding: '10px 18px', color: color.slateSoft, fontWeight: 700, textTransform: 'uppercase', fontSize: 10.5, letterSpacing: '0.5px' }}>{r}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {MODULES.map((m) => (
                <tr key={m}>
                  <td style={{ padding: '10px 18px', borderTop: `1px solid ${color.mist2}`, fontWeight: 600, color: color.ink }}>{m}</td>
                  {ROLES.map((r) => (
                    <td key={r} style={{ padding: '10px 18px', borderTop: `1px solid ${color.mist2}`, textAlign: 'center' }}>
                      <span style={{ width: 8, height: 8, borderRadius: 999, display: 'inline-block', background: r === 'Admin' || (r === 'Sales' && m === 'Opportunities') || (r === 'Project Manager' && m !== 'Opportunities') ? color.accent : color.line }} />
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}
