import { useState } from 'react'
import { AppState, Screen, initialAppState } from './types'
import { color, font } from './theme'
import Dashboard from './screens/Dashboard'
import Opportunities from './screens/Opportunities'
import Surveys from './screens/Surveys'
import Design from './screens/Design'
import Bom from './screens/Bom'
import Projects from './screens/Projects'
import Plants from './screens/Plants'
import WorkOrders from './screens/WorkOrders'
import Commissioning from './screens/Commissioning'
import Reports from './screens/Reports'
import Portal from './screens/Portal'
import Admin from './screens/Admin'
import Empty from './screens/Empty'
import Login from './screens/Login'

interface NavItemDef {
  label: string
  key: Screen
}

interface NavGroupDef {
  label: string
  items: NavItemDef[]
}

const NAV_GROUPS: NavGroupDef[] = [
  { label: 'Overview', items: [{ label: 'Dashboard', key: 'dashboard' }] },
  {
    label: 'Pre-operations',
    items: [
      { label: 'Opportunities', key: 'opportunities' },
      { label: 'Site surveys', key: 'surveys' },
      { label: 'Design', key: 'design' },
      { label: 'BOM & procurement', key: 'bom' },
      { label: 'Projects', key: 'projects' },
    ],
  },
  {
    label: 'Operations',
    items: [
      { label: 'Plants', key: 'plants' },
      { label: 'Work orders', key: 'workorders' },
      { label: 'Commissioning & handover', key: 'commissioning' },
    ],
  },
  {
    label: 'Insight',
    items: [
      { label: 'Reports', key: 'reports' },
      { label: 'Customer portal', key: 'portal' },
    ],
  },
  { label: 'System', items: [{ label: 'Administration', key: 'admin' }] },
  { label: 'Cross-cutting', items: [{ label: 'Empty & error states', key: 'empty' }] },
]

const TITLES: Record<Screen, string> = {
  dashboard: 'Dashboard',
  opportunities: 'Opportunity pipeline',
  surveys: 'Site surveys',
  design: 'Design workspace',
  bom: 'Bill of materials',
  projects: 'Projects',
  plants: 'Plants',
  workorders: 'Work orders',
  commissioning: 'Commissioning & handover',
  reports: 'Reports',
  portal: 'Customer portal',
  admin: 'Administration',
  empty: 'Empty & error states',
  login: '',
}

const NOTIFICATIONS = [
  { title: 'Work order WO-2291 overdue', body: 'HVAC filter replacement — Lusaka Ridge C&I', channel: 'Email', time: '8m ago' },
  { title: 'Survey signed off', body: 'SRV-0148 for Kitwe Industrial Park', channel: 'WhatsApp', time: '1h ago' },
  { title: 'New opportunity', body: 'Mukuba Steel — 480kWp rooftop enquiry', channel: 'SMS', time: '3h ago' },
  { title: 'Non-conformity closed', body: 'NC-0032 on Ndola Cold Storage', channel: 'Email', time: 'Yesterday' },
]

const TENANTS = ['Kariba Solar Services', 'Zamsun Power Ltd', 'Copperbelt Energy Co-op']

export default function App() {
  const [state, setState] = useState<AppState>(initialAppState)

  const set = (patch: Partial<AppState> | ((prev: AppState) => Partial<AppState>)) => {
    setState((prev) => ({ ...prev, ...(typeof patch === 'function' ? patch(prev) : patch) }))
  }

  const go = (screen: Screen) => set({ screen, tenantOpen: false, notifOpen: false })

  const screenProps = { state, set }

  return (
    <div style={{ display: 'flex', width: '100%', height: '100vh', background: color.mist, overflow: 'hidden', position: 'relative', fontSize: 14 }}>
      <div style={{ width: 236, flex: 'none', background: color.white, borderRight: `1px solid ${color.line}`, display: 'flex', flexDirection: 'column', padding: '18px 12px' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 9, padding: '4px 8px 20px' }}>
          <span style={{ width: 20, height: 20, borderRadius: 5, background: color.accent, flex: 'none' }} />
          <div style={{ fontFamily: font.display, fontSize: 16, fontWeight: 800, letterSpacing: '-0.3px', color: color.deep }}>Sencecon</div>
        </div>

        {NAV_GROUPS.map((group) => (
          <div key={group.label} style={{ marginBottom: 14 }}>
            <div style={{ fontSize: 10.5, fontWeight: 700, letterSpacing: '0.6px', color: color.slateSoft, textTransform: 'uppercase', padding: '6px 10px 6px' }}>
              {group.label}
            </div>
            {group.items.map((item) => {
              const active = state.screen === item.key
              return (
                <div
                  key={item.key}
                  onClick={() => go(item.key)}
                  style={{
                    display: 'flex',
                    alignItems: 'center',
                    gap: 10,
                    padding: '8px 10px',
                    borderRadius: 8,
                    cursor: 'pointer',
                    marginBottom: 1,
                    background: active ? color.accentTint : 'transparent',
                    color: active ? color.deep : color.slate,
                    fontWeight: active ? 700 : 500,
                    fontSize: 13.5,
                  }}
                >
                  <span style={{ width: 6, height: 6, borderRadius: 2, background: active ? color.accent : color.line, flex: 'none' }} />
                  {item.label}
                </div>
              )
            })}
          </div>
        ))}

        <div style={{ marginTop: 'auto', padding: 10, borderTop: `1px solid ${color.line}` }}>
          <div onClick={() => go('login')} style={{ fontSize: 12.5, color: color.slateSoft, cursor: 'pointer', padding: '6px 10px' }}>
            Log out
          </div>
        </div>
      </div>

      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>
        <div style={{ height: 56, flex: 'none', borderBottom: `1px solid ${color.line}`, background: color.white, display: 'flex', alignItems: 'center', gap: 16, padding: '0 20px', position: 'relative' }}>
          <div style={{ fontFamily: font.display, fontSize: 15, fontWeight: 700, color: color.ink }}>{TITLES[state.screen]}</div>
          <div style={{ flex: 1, maxWidth: 360, display: 'flex', alignItems: 'center', gap: 8, background: color.mist, border: `1px solid ${color.line}`, borderRadius: 8, padding: '7px 10px' }}>
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke={color.slateSoft} strokeWidth={2}>
              <circle cx="11" cy="11" r="7" />
              <path d="M21 21l-4.3-4.3" />
            </svg>
            <span style={{ fontSize: 13, color: color.slateSoft }}>Search plants, work orders, opportunities…</span>
          </div>
          <div style={{ flex: 1 }} />

          <div
            onClick={() => set((st) => ({ tenantOpen: !st.tenantOpen, notifOpen: false }))}
            style={{ display: 'flex', alignItems: 'center', gap: 6, padding: '6px 10px', border: `1px solid ${color.line}`, borderRadius: 8, cursor: 'pointer', fontSize: 12.5, fontWeight: 600, color: color.slate }}
          >
            Kariba Solar Services
            <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke={color.slate} strokeWidth={2.2}>
              <path d="M6 9l6 6 6-6" />
            </svg>
          </div>
          {state.tenantOpen && (
            <div style={{ position: 'absolute', top: 52, right: 150, width: 220, background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, boxShadow: '0 8px 24px rgba(18,32,31,0.12)', padding: 6, zIndex: 20 }}>
              <div style={{ fontSize: 10.5, fontWeight: 700, color: color.slateSoft, letterSpacing: '0.5px', textTransform: 'uppercase', padding: '6px 10px' }}>Switch tenant</div>
              {TENANTS.map((t, i) => (
                <div
                  key={t}
                  style={{
                    padding: '8px 10px',
                    borderRadius: 8,
                    background: i === 0 ? color.accentTint : undefined,
                    color: i === 0 ? color.deep : color.slate,
                    fontWeight: i === 0 ? 600 : 400,
                    fontSize: 13,
                    cursor: i === 0 ? 'default' : 'pointer',
                  }}
                >
                  {t}
                </div>
              ))}
            </div>
          )}

          <div
            onClick={() => set((st) => ({ notifOpen: !st.notifOpen, tenantOpen: false }))}
            style={{ width: 32, height: 32, borderRadius: 8, border: `1px solid ${color.line}`, display: 'flex', alignItems: 'center', justifyContent: 'center', cursor: 'pointer', position: 'relative' }}
          >
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke={color.slate} strokeWidth={2}>
              <path d="M18 8a6 6 0 10-12 0c0 7-3 9-3 9h18s-3-2-3-9" />
              <path d="M13.7 21a2 2 0 01-3.4 0" />
            </svg>
            <span style={{ position: 'absolute', top: 5, right: 6, width: 7, height: 7, borderRadius: 999, background: color.error }} />
          </div>
          {state.notifOpen && (
            <div style={{ position: 'absolute', top: 52, right: 76, width: 320, maxHeight: 360, overflow: 'auto', background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, boxShadow: '0 8px 24px rgba(18,32,31,0.12)', zIndex: 20 }}>
              <div style={{ padding: '12px 14px', borderBottom: `1px solid ${color.line}`, fontWeight: 700, fontSize: 13 }}>Notifications</div>
              {NOTIFICATIONS.map((n, i) => (
                <div key={i} style={{ padding: '11px 14px', borderBottom: `1px solid ${color.mist2}`, fontSize: 12.5 }}>
                  <div style={{ fontWeight: 600, color: color.ink }}>{n.title}</div>
                  <div style={{ color: color.slate, marginTop: 2 }}>{n.body}</div>
                  <div style={{ color: color.slateSoft, marginTop: 3, fontSize: 11 }}>
                    {n.channel} · {n.time}
                  </div>
                </div>
              ))}
            </div>
          )}

          <div style={{ width: 32, height: 32, borderRadius: 999, background: color.accentTint, color: color.deep, display: 'flex', alignItems: 'center', justifyContent: 'center', fontWeight: 700, fontSize: 12.5 }}>
            NM
          </div>
        </div>

        <div style={{ flex: 1, overflow: 'auto', padding: '22px 26px', background: color.mist }}>
          {state.screen === 'dashboard' && <Dashboard {...screenProps} />}
          {state.screen === 'opportunities' && <Opportunities {...screenProps} />}
          {state.screen === 'surveys' && <Surveys {...screenProps} />}
          {state.screen === 'design' && <Design {...screenProps} />}
          {state.screen === 'bom' && <Bom {...screenProps} />}
          {state.screen === 'projects' && <Projects {...screenProps} />}
          {state.screen === 'plants' && <Plants {...screenProps} />}
          {state.screen === 'workorders' && <WorkOrders {...screenProps} />}
          {state.screen === 'commissioning' && <Commissioning {...screenProps} />}
          {state.screen === 'reports' && <Reports />}
          {state.screen === 'portal' && <Portal />}
          {state.screen === 'admin' && <Admin />}
          {state.screen === 'empty' && <Empty />}
          {state.screen === 'login' && <Login onSignIn={() => go('dashboard')} />}
        </div>
      </div>
    </div>
  )
}
