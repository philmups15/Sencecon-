import Chip, { ChipTone } from '../components/Chip'
import { ScreenProps } from '../types'
import { color } from '../theme'

interface WorkOrder {
  id: string
  plant: string
  type: string
  priority: string
  priorityTone: ChipTone
  assignee: string
  status: string
  statusTone: ChipTone
}

const ORDERS: WorkOrder[] = [
  { id: 'WO-2291', plant: 'Lusaka Ridge C&I', type: 'HVAC filter replacement', priority: 'High', priorityTone: 'error', assignee: 'Field team 2', status: 'Overdue', statusTone: 'error' },
  { id: 'WO-2288', plant: 'Kitwe Industrial Park', type: 'Quarterly inspection', priority: 'Medium', priorityTone: 'warning', assignee: 'Field team 3', status: 'Completed', statusTone: 'success' },
  { id: 'WO-2285', plant: 'Ndola Cold Storage', type: 'Inverter fault', priority: 'High', priorityTone: 'error', assignee: 'Temba N.', status: 'In progress', statusTone: 'primary' },
  { id: 'WO-2280', plant: 'Livingstone Lodge', type: 'Comms module swap', priority: 'Medium', priorityTone: 'warning', assignee: 'Field team 1', status: 'Scheduled', statusTone: 'slate' },
]

export default function WorkOrders({}: ScreenProps) {
  return (
    <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, overflow: 'hidden' }}>
      <div style={{ display: 'grid', gridTemplateColumns: '0.9fr 1.4fr 1.4fr 0.8fr 1fr 1fr', padding: '10px 16px', fontSize: 10.5, fontWeight: 700, letterSpacing: '0.5px', textTransform: 'uppercase', color: color.slateSoft, borderBottom: `1px solid ${color.line}` }}>
        <div>Work order</div>
        <div>Plant</div>
        <div>Type</div>
        <div>Priority</div>
        <div>Assignee</div>
        <div>Status</div>
      </div>
      {ORDERS.map((wo) => (
        <div key={wo.id} style={{ display: 'grid', gridTemplateColumns: '0.9fr 1.4fr 1.4fr 0.8fr 1fr 1fr', padding: '12px 16px', fontSize: 13, borderBottom: `1px solid ${color.mist2}`, alignItems: 'center' }}>
          <div style={{ fontWeight: 600, color: color.ink }}>{wo.id}</div>
          <div style={{ color: color.slate }}>{wo.plant}</div>
          <div style={{ color: color.slate }}>{wo.type}</div>
          <div><Chip label={wo.priority} tone={wo.priorityTone} /></div>
          <div style={{ color: color.slate }}>{wo.assignee}</div>
          <div><Chip label={wo.status} tone={wo.statusTone} /></div>
        </div>
      ))}
    </div>
  )
}
