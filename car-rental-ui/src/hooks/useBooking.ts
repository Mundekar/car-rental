import { useCallback, useState } from 'react'
import apiService from '../services/apiService'
import { BookingRequest, BookingResponse, VehicleQuote } from '../types'

/**
 * Custom hook for managing booking operations and state.
 */
export const useBooking = () => {
  const [booking, setBooking] = useState<BookingResponse | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [selectedVehicle, setSelectedVehicle] = useState<VehicleQuote | null>(null)

  /**
   * Create a new booking.
   */
  const createBooking = useCallback(async (
    bookingRequest: BookingRequest
  ): Promise<BookingResponse | null> => {
    setLoading(true)
    setError(null)
    try {
      const response = await apiService.createBooking(bookingRequest)
      setBooking(response)
      return response
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Booking failed. Please try again.'
      setError(message)
      return null
    } finally {
      setLoading(false)
    }
  }, [])

  /**
   * Retrieve booking by reference number.
   */
  const getBooking = useCallback(async (reference: string): Promise<BookingResponse | null> => {
    setLoading(true)
    setError(null)
    try {
      const response = await apiService.getBooking(reference)
      setBooking(response)
      return response
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Could not retrieve booking.'
      setError(message)
      return null
    } finally {
      setLoading(false)
    }
  }, [])

  /**
   * Set selected vehicle for booking.
   */
  const selectVehicle = useCallback((vehicle: VehicleQuote): void => {
    setSelectedVehicle(vehicle)
  }, [])

  /**
   * Clear booking state.
   */
  const clearBooking = useCallback((): void => {
    setBooking(null)
    setError(null)
    setSelectedVehicle(null)
  }, [])

  return {
    booking,
    selectedVehicle,
    loading,
    error,
    createBooking,
    getBooking,
    selectVehicle,
    clearBooking,
  }
}
