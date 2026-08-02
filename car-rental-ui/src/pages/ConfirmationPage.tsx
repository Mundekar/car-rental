import React, { useEffect } from 'react'
import { useLocation, useParams } from 'react-router-dom'
import { useBooking } from '../hooks/useBooking'
import BookingConfirmation from '../components/BookingConfirmation'
import { BookingResponse } from '../types'

interface ConfirmationLocationState {
  reference?: string
  booking?: BookingResponse
}

/**
 * Confirmation page for displaying booking confirmation and lookup.
 */
const ConfirmationPage: React.FC = () => {
  const { reference } = useParams<{ reference: string }>()
  const location = useLocation()
  const { booking, getBooking, loading, error } = useBooking()

  const locationState = (location.state as ConfirmationLocationState | null) ?? null
  const stateBooking = locationState?.booking ?? null
  const resolvedReference =
    (reference && reference !== 'new' ? reference : undefined) ||
    locationState?.reference ||
    stateBooking?.referenceNumber

  useEffect(() => {
    if (resolvedReference) {
      // Retrieve booking details when reference comes from URL or navigation state.
      getBooking(resolvedReference)
    }
  }, [resolvedReference, getBooking])

  return (
    <div style={styles.container}>
      <BookingConfirmation
        booking={booking ?? stateBooking}
        loading={loading}
        error={error}
      />
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
