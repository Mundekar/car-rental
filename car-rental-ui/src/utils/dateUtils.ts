/**
 * Format date to display format (e.g., "Aug 01, 2026").
 * @param date - Date to format
 * @returns Formatted date string
 */
export const formatDate = (date: Date | string): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date
  if (dateObj.toString() === 'Invalid Date') return ''

  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
  }
  return dateObj.toLocaleDateString('en-US', options)
}

/**
 * Format date to ISO string for API (e.g., "2026-08-01").
 * @param date - Date to format
 * @returns ISO date string
 */
export const formatDateForApi = (date: Date): string => {
  return date.toISOString().split('T')[0]
}

/**
 * Format date and time for display (e.g., "Aug 01, 2026 3:45 PM").
 * @param date - Date to format
 * @returns Formatted date and time string
 */
export const formatDateTime = (date: Date | string): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date
  if (dateObj.toString() === 'Invalid Date') return ''

  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: true,
  }
  return dateObj.toLocaleDateString('en-US', options)
}

/**
 * Calculate days between two dates.
 * @param startDate - Start date
 * @param endDate - End date
 * @returns Number of days
 */
export const calculateDays = (startDate: Date, endDate: Date): number => {
  const msPerDay = 24 * 60 * 60 * 1000
  return Math.ceil((endDate.getTime() - startDate.getTime()) / msPerDay)
}

/**
 * Get minimum date for date picker (today).
 * @returns Today's date
 */
export const getMinDate = (): string => {
  const today = new Date()
  return formatDateForApi(today)
}

/**
 * Format price with currency symbol.
 * @param price - Price amount
 * @param currency - Currency code (default: USD)
 * @returns Formatted price string
 */
export const formatPrice = (price: number, currency: string = 'USD'): string => {
  const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency,
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })
  return formatter.format(price)
}
