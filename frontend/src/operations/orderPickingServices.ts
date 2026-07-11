export type PickingOrderItem = {
  id: string
  imageUrl: string
  modelCode: string
  productName: string
  color: string
  size: string
  barcode: string
  requestedQuantity: number
  availableStock: number
  shelf: string
  box: string
}

export type PickingOrder = {
  id: string
  orderNumber: string
  customerName: string
  orderDate: string
  items: PickingOrderItem[]
}

export interface IOrderSource {
  getPendingOrders(): Promise<PickingOrder[]>
}

export interface IStockService {
  reserve(order: PickingOrder): Promise<void>
}

export interface IShipmentService {
  complete(order: PickingOrder): Promise<void>
}

export interface IBarcodeScanner {
  normalize(value: string): string
}

export interface IAudioFeedback {
  success(): void
  error(): void
}

export class KeyboardBarcodeScanner implements IBarcodeScanner {
  normalize(value: string) {
    return value.trim()
  }
}

export class BrowserBeepFeedback implements IAudioFeedback {
  success() {
    this.beep(880, 90)
  }

  error() {
    this.beep(220, 180)
  }

  private beep(frequency: number, durationMs: number) {
    const AudioContextClass = window.AudioContext || window.webkitAudioContext
    const context = new AudioContextClass()
    const oscillator = context.createOscillator()
    const gain = context.createGain()

    oscillator.type = 'sine'
    oscillator.frequency.value = frequency
    gain.gain.value = 0.05
    oscillator.connect(gain)
    gain.connect(context.destination)
    oscillator.start()
    window.setTimeout(() => {
      oscillator.stop()
      void context.close()
    }, durationMs)
  }
}

export class MockOrderSource implements IOrderSource {
  async getPendingOrders() {
    return mockOrders
  }
}

export class NoopStockService implements IStockService {
  async reserve(_order: PickingOrder) {
    return
  }
}

export class NoopShipmentService implements IShipmentService {
  async complete(_order: PickingOrder) {
    return
  }
}

declare global {
  interface Window {
    webkitAudioContext?: typeof AudioContext
  }
}

const mockOrders: PickingOrder[] = [
  {
    id: 'ty-10001',
    orderNumber: 'TY-2026-10001',
    customerName: 'Ayşe Yılmaz',
    orderDate: '2026-07-11',
    items: [
      {
        id: 'ty-10001-1',
        imageUrl: 'https://cdn.dsmcdn.com/ty1891/prod/QC_PREP/20260626/10/4c618592-89d1-3179-bd33-293fe8bff7ba/1_org_zoom.jpg',
        modelCode: 'FL1454',
        productName: 'Bağlama Detaylı Bürümcük İkili Takım',
        color: 'Bordo',
        size: 'L',
        barcode: 'fl1454brl',
        requestedQuantity: 2,
        availableStock: 18,
        shelf: 'A-03',
        box: 'K-12',
      },
      {
        id: 'ty-10001-2',
        imageUrl: 'https://cdn.dsmcdn.com/ty1891/prod/QC_PREP/20260626/10/4c618592-89d1-3179-bd33-293fe8bff7ba/1_org_zoom.jpg',
        modelCode: 'FL1454',
        productName: 'Bağlama Detaylı Bürümcük İkili Takım',
        color: 'Bordo',
        size: 'XL',
        barcode: 'fl1454brx',
        requestedQuantity: 1,
        availableStock: 7,
        shelf: 'A-03',
        box: 'K-12',
      },
    ],
  },
  {
    id: 'web-24017',
    orderNumber: 'WEB-2026-24017',
    customerName: 'Zeynep Kaya',
    orderDate: '2026-07-11',
    items: [
      {
        id: 'web-24017-1',
        imageUrl: 'https://cdn.dsmcdn.com/ty1891/prod/QC_PREP/20260626/10/4c618592-89d1-3179-bd33-293fe8bff7ba/1_org_zoom.jpg',
        modelCode: '0922',
        productName: 'Sandy Madonna Yaka Etkili Elbise',
        color: 'Siyah',
        size: 'M',
        barcode: '0922fsim',
        requestedQuantity: 3,
        availableStock: 11,
        shelf: 'B-01',
        box: 'K-04',
      },
      {
        id: 'web-24017-2',
        imageUrl: 'https://cdn.dsmcdn.com/ty1891/prod/QC_PREP/20260626/10/4c618592-89d1-3179-bd33-293fe8bff7ba/1_org_zoom.jpg',
        modelCode: '0922',
        productName: 'Sandy Madonna Yaka Etkili Elbise',
        color: 'Mürdüm',
        size: 'S',
        barcode: '0922fms',
        requestedQuantity: 1,
        availableStock: 9,
        shelf: 'B-02',
        box: 'K-05',
      },
    ],
  },
  {
    id: 'ty-10002',
    orderNumber: 'TY-2026-10002',
    customerName: 'Elif Demir',
    orderDate: '2026-07-10',
    items: [
      {
        id: 'ty-10002-1',
        imageUrl: 'https://cdn.dsmcdn.com/ty1891/prod/QC_PREP/20260626/10/4c618592-89d1-3179-bd33-293fe8bff7ba/1_org_zoom.jpg',
        modelCode: 'F0778',
        productName: 'Kadın Bej Keten Cepli Etek',
        color: 'Bej',
        size: 'L',
        barcode: 'TYBIVWJA34807SOE77',
        requestedQuantity: 1,
        availableStock: 6,
        shelf: 'C-07',
        box: 'K-21',
      },
    ],
  },
]
