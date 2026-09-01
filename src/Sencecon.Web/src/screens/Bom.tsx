import Chip, { ChipTone } from '../components/Chip'
import { ScreenProps } from '../types'
import { color } from '../theme'

interface BomItem {
  component: string
  qty: number
  unitCost: string
  supplier: string
  status: string
}

const STATUS_TONE: Record<string, ChipTone> = {
  Confirmed: 'success',
  Pending: 'warning',
  Ordered: 'primary',
  Backordered: 'error',
}

const ITEMS: BomItem[] = [
  { component: '550W Monocrystalline module', qty: 872, unitCost: '$142.00', supplier: 'Longi Solar', status: 'Confirmed' },
  { component: '150kW string inverter', qty: 3, unitCost: '$9,400.00', supplier: 'Huawei', status: 'Ordered' },
  { component: 'Roof mount racking kit', qty: 44, unitCost: '$310.00', supplier: 'K2 Systems', status: 'Pending' },
  { component: 'AC combiner panel', qty: 1, unitCost: '$4,120.00', supplier: 'Schneider Electric', status: 'Backordered' },
  { component: 'DC cable, 6mm² (100m roll)', qty: 18, unitCost: '$86.00', supplier: 'Prysmian', status: 'Confirmed' },
]

export default function Bom({}: ScreenProps) {
  return (
    <div style={{ background: color.white, border: `1px solid ${color.line}`, borderRadius: 12, overflow: 'hidden' }}>
      <div style={{ display: 'grid', gridTemplateColumns: '1.8fr 0.7fr 1fr 1.1fr 1fr', padding: '10px 16px', fontSize: 10.5, fontWeight: 700, letterSpacing: '0.5px', textTransform: 'uppercase', color: color.slateSoft, borderBottom: `1px solid ${color.line}` }}>
        <div>Component</div>
        <div style={{ textAlign: 'right' }}>Qty</div>
        <div style={{ textAlign: 'right' }}>Unit cost</div>
        <div>Supplier</div>
        <div>Status</div>
      </div>
      {ITEMS.map((item, i) => (
        <div key={i} style={{ display: 'grid', gridTemplateColumns: '1.8fr 0.7fr 1fr 1.1fr 1fr', padding: '12px 16px', fontSize: 13, borderBottom: `1px solid ${color.mist2}`, alignItems: 'center' }}>
          <div style={{ fontWeight: 600, color: color.ink }}>{item.component}</div>
          <div style={{ textAlign: 'right', color: color.slate, fontVariantNumeric: 'tabular-nums' }}>{item.qty}</div>
          <div style={{ textAlign: 'right', color: color.slate, fontVariantNumeric: 'tabular-nums' }}>{item.unitCost}</div>
          <div style={{ color: color.slate }}>{item.supplier}</div>
          <div><Chip label={item.status} tone={STATUS_TONE[item.status]} /></div>
        </div>
      ))}
    </div>
  )
}
