import axios, { AxiosInstance } from 'axios'
import { SearchCriteria, SearchResponse, BookingRequest, BookingResponse } from '../types'

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
      const response = await this.api.get<SearchResponse>('/cars/search', {
        params: {
          pickup: criteria.pickup,
          from: this.formatDate(criteria.from),
          to: this.formatDate(criteria.to),
          category: criteria.category,
        },
      })
      return response.data
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
      const response = await this.api.post<BookingResponse>('/cars/book', booking)
      return response.data
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
      const response = await this.api.get<BookingResponse>(
        `/cars/booking/${reference}`
      )
      return response.data
    } catch (error) {
      throw this.handleError(error)
    }
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
