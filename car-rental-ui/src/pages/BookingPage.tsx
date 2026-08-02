import React from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { useBooking } from '../hooks/useBooking'
import BookingForm from '../components/BookingForm'
import { BookingRequest } from '../types'
import { bookingPageStyles as styles } from '../styles/pages/bookingPageStyles'

/**
 * Booking page for collecting booking details.
 */
const BookingPage: React.FC = () => {
  const navigate = useNavigate()
  const location = useLocation()
  const { createBooking, loading, error } = useBooking()

  const vehicle = location.state?.vehicle
  const pickupLocation = location.state?.pickupLocation
  const pickupDate = location.state?.pickupDate
  const returnDate = location.state?.returnDate

  if (!vehicle) {
    return (
      <div style={styles.container}>
        <div style={styles.error}>
          <h2>Vehicle not found</h2>
          <p>Please select a vehicle from the search results.</p>
          <button onClick={() => navigate('/results')} style={styles.button}>
            Back to Results
          </button>
        </div>
      </div>
    )
  }

  const handleBookingSubmit = async (
    bookingRequest: BookingRequest
  ): Promise<boolean> => {
    const result = await createBooking(bookingRequest)

    if (result?.referenceNumber) {
      navigate(`/confirmation/${result.referenceNumber}`)
      return true
    }

    return !!result
  }

  return (
    <div style={styles.container}>
      <BookingForm
        vehicle={vehicle}
        pickupLocation={pickupLocation}
        pickupDate={pickupDate}
        returnDate={returnDate}
        onSubmit={handleBookingSubmit}
        loading={loading}
        apiError={error}
      />
    </div>
  )
}

export default BookingPage
