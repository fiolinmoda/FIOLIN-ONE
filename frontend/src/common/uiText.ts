export const commonText = {
  add: 'Yeni Kayıt',
  back: 'Geri',
  cancel: 'Vazgeç',
  delete: 'Sil',
  edit: 'Düzenle',
  export: 'Dışa Aktar',
  no: 'Hayır',
  none: 'Seçiniz',
  save: 'Kaydet',
  search: 'Ara',
  yes: 'Evet',
}

const statusLabels: Record<string, string> = {
  Active: 'Aktif',
  Passive: 'Pasif',
  Draft: 'Taslak',
  Approved: 'Onaylandı',
  'Partially Received': 'Kısmi Kabul',
  'Fully Received': 'Tam Kabul',
  'Waiting Invoice': 'Fatura Bekliyor',
  'Waiting Payment': 'Ödeme Bekliyor',
  Completed: 'Tamamlandı',
  Cancelled: 'İptal',
  Accepted: 'Kabul Edildi',
  Difference: 'Fark Var',
  Open: 'Açık',
  Received: 'Teslim Alındı',
  PLANNED: 'Planlandı',
  FABRIC_ALLOCATED: 'Kumaş Ayrıldı',
  CUTTING: 'Kesimde',
  AT_WORKSHOP: 'Atölyede',
  AT_IRONING_PACKAGING: 'Ütü/Pakette',
  READY_FOR_WAREHOUSE: 'Depoya Hazır',
  CANCELLED: 'İptal',
  OUT_OF_STOCK: 'Stokta Yok',
  'OUT OF STOCK': 'Stokta Yok',
  Stock: 'Stok',
  Dealer: 'Bayi',
  Trendyol: 'Trendyol',
  Custom: 'Özel',
  Purchase: 'Satın Alma',
  'Production Consumption': 'Üretim Tüketimi',
  'Manual Adjustment': 'Manuel Düzeltme',
  'Inventory Count': 'Sayım',
  Return: 'İade',
  SENT: 'Gönderildi',
  PARTIAL_RETURN: 'Kısmi Döndü',
  RETURNED: 'Döndü',
}

export function trStatus(value: string | null | undefined): string {
  if (!value) {
    return ''
  }

  return statusLabels[value] ?? value
}

export function requiredMessage(label: string): string {
  return `${label} zorunludur.`
}

export function confirmDelete(label: string): boolean {
  return window.confirm(`${label} kaydını silmek istediğinizden emin misiniz?`)
}

export function focusFirstInvalid(form: HTMLFormElement): void {
  window.setTimeout(() => {
    const invalid = form.querySelector<HTMLElement>(':invalid, [aria-invalid="true"]')
    invalid?.focus()
  })
}

export const dialogPaperSx = {
  maxHeight: 'calc(100vh - 32px)',
}

export const dialogContentSx = {
  pt: 1,
  pb: 2,
  maxHeight: { xs: 'calc(100vh - 180px)', md: '70vh' },
  overflowY: 'auto',
}
