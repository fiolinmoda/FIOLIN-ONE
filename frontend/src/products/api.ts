import type { Product, ProductInput, ProductVariant, ProductVariantInput } from './types'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...init?.headers,
    },
    ...init,
  })

  if (!response.ok) {
    const body = await response.json().catch(() => null)
    const message = body?.message ?? 'Request failed.'

    throw new Error(message)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json()
}

export async function getProducts(search: string): Promise<Product[]> {
  const query = search.trim()
  const path = query ? `/api/products?search=${encodeURIComponent(query)}` : '/api/products'

  return request<Product[]>(path)
}

export async function getProduct(id: string): Promise<Product> {
  return request<Product>(`/api/products/${id}`)
}

export async function createProduct(product: ProductInput): Promise<Product> {
  return request<Product>('/api/products', {
    method: 'POST',
    body: JSON.stringify(product),
  })
}

export async function updateProduct(id: string, product: ProductInput): Promise<Product> {
  return request<Product>(`/api/products/${id}`, {
    method: 'PUT',
    body: JSON.stringify(product),
  })
}

export async function deleteProduct(id: string): Promise<void> {
  await request<void>(`/api/products/${id}`, {
    method: 'DELETE',
  })
}

export async function getProductVariants(productId: string): Promise<ProductVariant[]> {
  return request<ProductVariant[]>(`/api/products/${productId}/variants`)
}

export async function createProductVariant(
  productId: string,
  variant: ProductVariantInput,
): Promise<ProductVariant> {
  return request<ProductVariant>(`/api/products/${productId}/variants`, {
    method: 'POST',
    body: JSON.stringify(variant),
  })
}

export async function updateProductVariant(
  productId: string,
  variantId: string,
  variant: ProductVariantInput,
): Promise<ProductVariant> {
  return request<ProductVariant>(`/api/products/${productId}/variants/${variantId}`, {
    method: 'PUT',
    body: JSON.stringify(variant),
  })
}

export async function deleteProductVariant(productId: string, variantId: string): Promise<void> {
  await request<void>(`/api/products/${productId}/variants/${variantId}`, {
    method: 'DELETE',
  })
}
