/**
 * Enumeration of document types for booking.
 */
export enum DocumentType {
  NationalId = 0,
  Passport = 1,
}

/**
 * Enumeration of vehicle categories.
 */
export enum VehicleCategory {
  Economy = 'Economy',
  Comfort = 'Comfort',
  Premium = 'Premium',
}

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
  fromDate: string
  toDate: string
  daysCount: number
  results: VehicleQuote[]
}

/**
 * Type definitions for booking request.
 */
export interface BookingRequest {
  driverName: string
  documentType: number
  documentNumber: string
  vehicleId: string
  provider: string
  vehicleCategory: string
  vehicleMake: string
  vehicleModel: string
  dailyRate: number
  totalPrice: number
  insuranceType: string
  cancellationPolicy: string
  pickupLocation: string
  pickupDate: string
  returnDate: string
}

/**
 * Type definitions for booking response.
 */
export interface BookingResponse {
  referenceNumber: string
  driverName: string
  vehicleCategory: string
  vehicleDetails: string
  provider: string
  pickupLocation: string
  fromDate: string
  toDate: string
  daysCount: number
  dailyRate: number
  totalPrice: number
  insuranceType: string
  cancellationPolicy: string
  bookingConfirmedAt: string
}

/**
 * Location configuration for document validation.
 */
export interface LocationConfig {
  domestic: string[]
  international: string[]
}

/**
 * Error response from API.
 */
export interface ApiError {
  message: string
  statusCode: number
}
