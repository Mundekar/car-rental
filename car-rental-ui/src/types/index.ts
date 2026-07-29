/**
 * Type definitions for search functionality.
 */
export interface SearchCriteria {
  pickup: string
  from: Date
  to: Date
  category?: string
}

/**
 * Type definitions for vehicle quote.
 */
export interface VehicleQuote {
  vehicleId: string
  provider: string
  category: string
  make: string
  model: string
  dailyRate: number
  totalPrice: number
  insuranceType: string
  cancellationPolicy: string
  isAvailable: boolean
}

/**
 * Type definitions for search response.
 */
export interface SearchResponse {
  searchId: string
  pickupLocation: string
  fromDate: Date
  toDate: Date
  daysCount: number
  results: VehicleQuote[]
}

/**
 * Type definitions for booking request.
 */
export interface BookingRequest {
  driverName: string
  documentType: string
  documentNumber: string
  vehicleId: string
  pickupLocation: string
}

/**
 * Type definitions for booking response.
 */
export interface BookingResponse {
  referenceNumber: string
  driverName: string
  vehicleCategory: string
  provider: string
  pickupLocation: string
  fromDate: Date
  toDate: Date
  daysCount: number
  totalPrice: number
  insuranceType: string
  cancellationPolicy: string
  bookingConfirmedAt: Date
}
