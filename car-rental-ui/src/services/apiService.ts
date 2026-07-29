import axios, { AxiosInstance } from 'axios'
import { SearchCriteria, SearchResponse, BookingRequest, BookingResponse, VehicleQuote } from '../types'

interface RawVehicleQuote extends Omit<VehicleQuote, 'category' | 'insuranceType' | 'cancellationPolicy'> {
  category: string | number
  insuranceType: string | number
  cancellationPolicy: string | number
}

interface RawSearchResponse extends Omit<SearchResponse, 'results'> {
  results: RawVehicleQuote[]
}

interface RawBookingDetails {
  make?: string
  model?: string
  year?: number
}

interface RawBookingResponse extends Omit<BookingResponse, 'vehicleCategory' | 'vehicleDetails' | 'insuranceType' | 'cancellationPolicy'> {
  vehicleCategory: string | number
  vehicleDetails: string | RawBookingDetails | null
  insuranceType: string | number
  cancellationPolicy: string | number
}

const vehicleCategoryLabels: Record<number, string> = {
  0: 'Economy',
  1: 'Compact',
  2: 'SUV',
  3: 'Minivan',
}

const insuranceTypeLabels: Record<number, string> = {
  0: 'Basic',
  1: 'Comprehensive',
}

const cancellationPolicyLabels: Record<number, string> = {
  0: 'Free cancellation up to 48 hours before pickup',
  1: 'Non-refundable',
}

/**
 * Centralized API service for all backend communication.
 * Provides methods for search, booking, and confirmation operations.
 */
class ApiService {
  private api: AxiosInstance

  constructor() {
    this.api = axios.create({
      baseURL: 'http://localhost:5000',
      headers: {
        'Content-Type': 'application/json',
      },
      timeout: 10000,
    })
  }

  /**
   * Search for available vehicles based on criteria.
   * @param criteria - Search criteria (location, dates, category)
   * @returns Promise with search results
   */
  async searchVehicles(criteria: SearchCriteria): Promise<SearchResponse> {
    try {
      const response = await this.api.get<RawSearchResponse>('/cars/search', {
        params: {
          pickup: criteria.pickup,
          from: this.formatDate(criteria.from),
          to: this.formatDate(criteria.to),
          category: criteria.category,
        },
      })
      return this.normalizeSearchResponse(response.data)
    } catch (error) {
      throw this.handleError(error)
    }
  }

  /**
   * Create a new booking.
   * @param booking - Booking request data
   * @returns Promise with booking confirmation
   */
  async createBooking(booking: BookingRequest): Promise<BookingResponse> {
    try {
      const response = await this.api.post<RawBookingResponse>('/cars/book', booking)
      return this.normalizeBookingResponse(response.data)
    } catch (error) {
      throw this.handleError(error)
    }
  }

  /**
   * Retrieve booking details by reference number.
   * @param reference - Booking reference number
   * @returns Promise with booking details
   */
  async getBooking(reference: string): Promise<BookingResponse> {
    try {
      const response = await this.api.get<RawBookingResponse>(
        `/cars/booking/${reference}`
      )
      return this.normalizeBookingResponse(response.data)
    } catch (error) {
      throw this.handleError(error)
    }
  }

  /**
   * Normalize search responses so the UI receives display-ready values.
   */
  private normalizeSearchResponse(response: RawSearchResponse): SearchResponse {
    return {
      ...response,
      results: response.results.map((vehicle) => ({
        ...vehicle,
        category: this.getLabel(vehicle.category, vehicleCategoryLabels),
        insuranceType: this.getLabel(vehicle.insuranceType, insuranceTypeLabels),
        cancellationPolicy: this.getLabel(
          vehicle.cancellationPolicy,
          cancellationPolicyLabels
        ),
      })),
    }
  }

  /**
   * Normalize booking responses so the UI receives a consistent shape.
   */
  private normalizeBookingResponse(booking: RawBookingResponse): BookingResponse {
    return {
      ...booking,
      vehicleCategory: this.getLabel(booking.vehicleCategory, vehicleCategoryLabels),
      vehicleDetails: this.formatVehicleDetails(booking.vehicleDetails),
      insuranceType: this.getLabel(booking.insuranceType, insuranceTypeLabels),
      cancellationPolicy: this.getLabel(
        booking.cancellationPolicy,
        cancellationPolicyLabels
      ),
    }
  }

  /**
   * Convert enum-like values to readable labels.
   */
  private getLabel(
    value: string | number,
    labels: Record<number, string>
  ): string {
    if (typeof value === 'string') {
      return value
    }

    return labels[value] ?? String(value)
  }

  /**
   * Convert structured vehicle details to a renderable string.
   */
  private formatVehicleDetails(details: string | RawBookingDetails | null): string {
    if (!details) {
      return 'Vehicle details unavailable'
    }

    if (typeof details === 'string') {
      return details
    }

    const parts = [details.year, details.make, details.model]
      .filter(
        (part): part is string | number =>
          part !== undefined && part !== null && part !== '' && part !== 0
      )
      .map(String)

    return parts.join(' ') || 'Vehicle details unavailable'
  }

  /**
   * Format date to ISO string for API calls.
   * @param date - Date to format
   * @returns ISO date string
   */
  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0]
  }

  /**
   * Handle API errors with user-friendly messages.
   * @param error - Error from API call
   * @returns Error with message
   */
  private handleError(error: unknown): Error {
    if (axios.isAxiosError(error)) {
      if (error.response) {
        // Server responded with error status
        const message =
          error.response.data?.message || `Error: ${error.response.status}`
        return new Error(message)
      } else if (error.request) {
        // Request made but no response
        return new Error('No response from server. Please try again.')
      }
    }
    return new Error('An unexpected error occurred. Please try again.')
  }
}

export default new ApiService()
