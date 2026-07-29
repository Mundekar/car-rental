import React, { useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { useBooking } from '../hooks/useBooking'
import BookingConfirmation from '../components/BookingConfirmation'

/**
 * Confirmation page for displaying booking confirmation and lookup.
 */
const ConfirmationPage: React.FC = () => {
  const { reference } = useParams<{ reference: string }>()
  const { booking, getBooking, loading, error } = useBooking()

  useEffect(() => {
    if (reference && reference !== 'new') {
      // Retrieve booking by reference number
      getBooking(reference)
    }
  }, [reference, getBooking])

  return (
    <div style={styles.container}>
      <BookingConfirmation booking={booking} loading={loading} error={error} />
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  container: {
    maxWidth: '800px',
    margin: '0 auto',
    padding: '40px 20px',
  },
}

export default ConfirmationPage
