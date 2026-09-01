export type Screen =
  | 'dashboard'
  | 'opportunities'
  | 'surveys'
  | 'design'
  | 'bom'
  | 'projects'
  | 'plants'
  | 'workorders'
  | 'commissioning'
  | 'reports'
  | 'portal'
  | 'admin'
  | 'empty'
  | 'login'

export interface AppState {
  screen: Screen
  tenantOpen: boolean
  notifOpen: boolean
  oppView: 'list' | 'kanban'
  oppSelected: string | null
  surveySelected: string
  designSelected: string
  designTab: string
  projectSelected: string
  projectTab: 'tasks' | 'subs' | 'budget' | 'risk'
  plantSelected: string
  plantTab: 'overview' | 'handover' | 'work' | 'activity'
  historyOpen: boolean
  woSelected: string
  commType: string
}

export const initialAppState: AppState = {
  screen: 'dashboard',
  tenantOpen: false,
  notifOpen: false,
  oppView: 'list',
  oppSelected: null,
  surveySelected: 'SRV-0148',
  designSelected: 'DSN-0091',
  designTab: 'array',
  projectSelected: 'PRJ-0032',
  projectTab: 'tasks',
  plantSelected: 'PLT-014',
  plantTab: 'overview',
  historyOpen: false,
  woSelected: 'WO-2291',
  commType: 'rooftop',
}

export type SetState = (patch: Partial<AppState> | ((prev: AppState) => Partial<AppState>)) => void

export interface ScreenProps {
  state: AppState
  set: SetState
}
