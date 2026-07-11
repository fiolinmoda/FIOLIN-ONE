import QRCode from 'qrcode'

export type BarcodePrintItem = {
  id: string
  modelCode: string
  productName: string
  color: string
  size: string
  barcode: string
  quantity: number
}

export type BarcodePrintLabel = BarcodePrintItem & {
  qrDataUrl: string
}

export interface BarcodePrintService {
  prepareLabels(items: BarcodePrintItem[]): Promise<BarcodePrintLabel[]>
  print(): void
}

export class BrowserBarcodePrintService implements BarcodePrintService {
  async prepareLabels(items: BarcodePrintItem[]): Promise<BarcodePrintLabel[]> {
    const expanded = items.flatMap((item) =>
      Array.from({ length: item.quantity }, (_, index) => ({
        ...item,
        id: `${item.id}-${index}`,
        quantity: 1,
      })),
    )

    return Promise.all(
      expanded.map(async (item) => ({
        ...item,
        qrDataUrl: await QRCode.toDataURL(item.barcode, {
          errorCorrectionLevel: 'M',
          margin: 0,
          width: 150,
        }),
      })),
    )
  }

  print() {
    window.print()
  }
}
