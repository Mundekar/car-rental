import React, { useEffect } from 'react'
import { useLocation, useParams } from 'react-router-dom'
import { useBooking } from '../hooks/useBooking'
import BookingConfirmation from '../components/BookingConfirmation'
import { BookingResponse, SearchCriteria } from '../types'
import { confirmationPageStyles as styles } from '../styles/pages/confirmationPageStyles'

interface ConfirmationLocationState {
  reference?: string
  booking?: BookingResponse
  criteria?: SearchCriteria
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
        criteria={locationState?.criteria}
      />
    </div>
  )
}

export default ConfirmationPage
