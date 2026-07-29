import React, { useEffect } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { useBooking } from '../hooks/useBooking'
import BookingForm from '../components/BookingForm'
import { BookingRequest } from '../types'

/**
 * Booking page for collecting booking details.
 */
const BookingPage: React.FC = () => {
  const navigate = useNavigate()
  const location = useLocation()
  const { booking, createBooking, loading, error } = useBooking()

  const vehicle = location.state?.vehicle
  const pickupLocation = location.state?.pickupLocation
  const pickupDate = location.state?.pickupDate
  const returnDate = location.state?.returnDate

  // Redirect to confirmation page if booking is already created
  useEffect(() => {
    if (booking?.referenceNumber) {
      navigate(`/confirmation/${booking.referenceNumber}`)
    }
  }, [booking, navigate])

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

const styles: Record<string, React.CSSProperties> = {
  container: {
    maxWidth: '600px',
    margin: '0 auto',
    padding: '40px 20px',
  },
  error: {
    backgroundColor: '#fee',
    border: '1px solid #f66',
    borderRadius: '8px',
    padding: '40px 20px',
    textAlign: 'center',
    color: '#d00',
  },
  button: {
    padding: '10px 20px',
    backgroundColor: '#0066cc',
    color: '#fff',
    border: 'none',
    borderRadius: '4px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
    marginTop: '16px',
  },
}

export default BookingPage
