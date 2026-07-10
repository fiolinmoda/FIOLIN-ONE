const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

type ApiErrorBody = {
  message?: string
  title?: string
  errors?: Record<string, string[] | string> | string[]
}

function translateKnownMessage(message: string, status: number): string {
  const normalized = message.trim()

  if (!normalized || normalized === 'Request failed.') {
    return 'İşlem tamamlanamadı. Lütfen bilgileri kontrol edip tekrar deneyiniz.'
  }

  const knownMessages: Record<string, string> = {
    'Failed to fetch': 'Sunucuya ulaşılamadı. Lütfen bağlantıyı ve uygulamanın çalıştığını kontrol ediniz.',
    Unauthorized: 'Oturum bilgisi bulunamadı. Geliştirme ortamında bu işlem yetkisiz olmamalıdır.',
    Forbidden: 'Bu işlem için yetkiniz yok.',
    'Not Found': 'Kayıt bulunamadı.',
  }

  if (knownMessages[normalized]) {
    return knownMessages[normalized]
  }

  if (status === 400 || status === 422) {
    return normalized
  }

  if (status === 401) {
    return knownMessages.Unauthorized
  }

  if (status === 404) {
    return knownMessages['Not Found']
  }

  if (status === 409) {
    return normalized || 'Bu kayıt başka bir kayıtla çakışıyor.'
  }

  if (status >= 500) {
    return 'Sunucuda beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.'
  }

  return normalized
}

function translateNetworkError(exception: unknown): string {
  const message = exception instanceof Error ? exception.message : String(exception)

  if (message === 'Failed to fetch' || message.includes('fetch')) {
    return 'Sunucuya bağlanılamadı. FIOLIN ONE servisinin çalıştığını ve bağlantınızı kontrol edin.'
  }

  return 'Sunucuya bağlanılamadı. Lütfen bağlantınızı kontrol edip tekrar deneyin.'
}

function flattenValidationErrors(errors: ApiErrorBody['errors']): string[] {
  if (!errors) {
    return []
  }

  if (Array.isArray(errors)) {
    return errors.map(String)
  }

  return Object.entries(errors).flatMap(([field, value]) => {
    const messages = Array.isArray(value) ? value : [value]
    return messages.map((message) => `${field}: ${message}`)
  })
}

export async function apiRequest<T>(path: string, init?: RequestInit): Promise<T> {
  const token = window.localStorage.getItem('fiolin-one-token')
  let response: Response

  try {
    response = await fetch(`${apiBaseUrl}${path}`, {
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...init?.headers,
      },
      ...init,
    })
  } catch (exception) {
    throw new Error(translateNetworkError(exception))
  }

  if (!response.ok) {
    const body = await response.json().catch(() => null) as ApiErrorBody | null
    const validationErrors = flattenValidationErrors(body?.errors)
    const fallback = body?.message ?? body?.title ?? 'Request failed.'
    const message = validationErrors.length > 0
      ? validationErrors.join('\n')
      : translateKnownMessage(fallback, response.status)

    throw new Error(message)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json()
}

export async function apiFormRequest<T>(path: string, formData: FormData, init?: RequestInit): Promise<T> {
  const token = window.localStorage.getItem('fiolin-one-token')
  let response: Response

  try {
    response = await fetch(`${apiBaseUrl}${path}`, {
      ...init,
      method: init?.method ?? 'POST',
      headers: {
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...init?.headers,
      },
      body: formData,
    })
  } catch (exception) {
    throw new Error(translateNetworkError(exception))
  }

  if (!response.ok) {
    const body = await response.json().catch(() => null) as ApiErrorBody | null
    const validationErrors = flattenValidationErrors(body?.errors)
    const fallback = body?.message ?? body?.title ?? 'Request failed.'
    const message = validationErrors.length > 0
      ? validationErrors.join('\n')
      : translateKnownMessage(fallback, response.status)

    throw new Error(message)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json()
}

export function toUserMessage(exception: unknown, fallback: string): string {
  return exception instanceof Error && exception.message ? exception.message : fallback
}
