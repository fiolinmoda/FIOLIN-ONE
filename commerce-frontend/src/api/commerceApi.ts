export type CommerceSeo = {
  metaTitle: string
  metaDescription: string
  canonical: string
  openGraphTitle: string
  openGraphDescription: string
  twitterCard: string
  schemaJson?: string | null
}

export type CommerceCategory = {
  id: string
  name: string
  code: string
  slug: string
  sortOrder: number
}

export type CommerceProduct = {
  id: string
  modelCode: string
  slug: string
  productName: string
  category?: string | null
  imageUrl?: string | null
  price: number
  stock: number
}

export type CommerceVariant = {
  color: string
  size: string
  barcode: string
  stock: number
  price: number
}

export type CommerceProductDetail = CommerceProduct & {
  variants: CommerceVariant[]
  seo: CommerceSeo
}

export type CommerceBanner = {
  title: string
  subtitle?: string | null
  imageUrl: string
  linkUrl?: string | null
  placement: string
}

export type CommerceSlider = {
  title: string
  subtitle?: string | null
  imageUrl: string
  linkUrl?: string | null
}

export type CommerceHome = {
  heroSlider: CommerceSlider[]
  categories: CommerceCategory[]
  newSeason: CommerceProduct[]
  bestSellers: CommerceProduct[]
  newArrivals: CommerceProduct[]
  campaigns: CommerceBanner[]
  seo: CommerceSeo
}

const apiBaseUrl = import.meta.env.VITE_COMMERCE_API_BASE_URL ?? 'http://localhost:5000'

async function getJson<T>(path: string, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, { signal })

  if (!response.ok) {
    throw new Error('Vitrin verisi alınamadı.')
  }

  return response.json() as Promise<T>
}

export const commerceApi = {
  getHome: (signal?: AbortSignal) => getJson<CommerceHome>('/commerce/home', signal),
  getCategories: (signal?: AbortSignal) => getJson<CommerceCategory[]>('/commerce/categories', signal),
  getProducts: (search?: string, signal?: AbortSignal) => {
    const query = search ? `?search=${encodeURIComponent(search)}` : ''
    return getJson<CommerceProduct[]>(`/commerce/products${query}`, signal)
  },
  getProduct: (slug: string, signal?: AbortSignal) => getJson<CommerceProductDetail>(`/commerce/product/${slug}`, signal),
}
