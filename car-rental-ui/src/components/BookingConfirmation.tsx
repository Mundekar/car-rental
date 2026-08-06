import React from 'react'
import { useNavigate } from 'react-router-dom'
import { BookingResponse, SearchCriteria } from '../types'
import { formatDate, formatDateTime, formatPrice } from '../utils/dateUtils'
import LoadingSpinner from './LoadingSpinner'
import ErrorMessage from './ErrorMessage'
import { bookingConfirmationStyles as styles } from '../styles/components/bookingConfirmationStyles'

interface BookingConfirmationProps {
  booking: BookingResponse | null
  loading: boolean
  error: string | null
  criteria?: SearchCriteria
}

/**
 * Component for displaying booking confirmation details.
 */
const BookingConfirmation: React.FC<BookingConfirmationProps> = ({
  booking,
  loading,
  error,
  criteria,
}) => {
  const navigate = useNavigate()

  const navigateBack = () => {
    if (criteria) {
      navigate('/results', { state: { criteria } })
      return
    }

    navigate('/')
  }

  if (loading) {
    return <LoadingSpinner message="Loading booking details..." />
  }

  if (error && !booking) {
    return (
      <div style={styles.container}>
        <ErrorMessage message={error} />
        <button
          onClick={navigateBack}
          style={styles.primaryButton}
        >
          {criteria ? 'Back to Results' : 'Back to Search'}
        </button>
      </div>
    )
  }

  if (!booking) {
    return (
      <div style={styles.container}>
        <ErrorMessage message="Booking details not available." />
        <button
          onClick={navigateBack}
          style={styles.primaryButton}
        >
          {criteria ? 'Back to Results' : 'Back to Search'}
        </button>
      </div>
    )
  }

  return (
    <div style={styles.container}>
      {/* Success Message */}
      <div style={styles.successCard}>
        <div style={styles.successIcon}>✓</div>
        <h1 style={styles.successTitle}>Booking Confirmed!</h1>
        <p style={styles.successMessage}>
          Your booking has been confirmed. Check your email for details.
        </p>
      </div>

      {/* Booking Reference */}
      <div style={styles.referenceCard}>
        <div style={styles.referenceLabel}>Booking Reference</div>
        <div style={styles.referenceNumber}>{booking.referenceNumber}</div>
        <button
          onClick={() => {
            navigator.clipboard.writeText(booking.referenceNumber)
            alert('Reference number copied!')
          }}
          style={styles.copyButton}
        >
          Copy Reference
        </button>
      </div>

      {/* Booking Details */}
      <div style={styles.detailsCard}>
        <h2 style={styles.detailsTitle}>Booking Details</h2>

        <div style={styles.detailsGrid}>
          {/* Passenger Information */}
          <section style={styles.section}>
            <h3 style={styles.sectionTitle}>Passenger Information</h3>
            <div style={styles.row}>
              <span style={styles.label}>Driver Name:</span>
              <span style={styles.value}>{booking.driverName}</span>
            </div>
          </section>

          {/* Vehicle Information */}
          <section style={styles.section}>
            <h3 style={styles.sectionTitle}>Vehicle Information</h3>
            <div style={styles.row}>
              <span style={styles.label}>Vehicle:</span>
              <span style={styles.value}>{booking.vehicleDetails}</span>
            </div>
            <div style={styles.row}>
              <span style={styles.label}>Category:</span>
              <span style={styles.value}>{booking.vehicleCategory}</span>
            </div>
            <div style={styles.row}>
              <span style={styles.label}>Provider:</span>
              <span style={styles.value}>{booking.provider}</span>
            </div>
          </section>

          {/* Trip Details */}
          <section style={styles.section}>
            <h3 style={styles.sectionTitle}>Trip Details</h3>
            <div style={styles.row}>
              <span style={styles.label}>Pickup Location:</span>
              <span style={styles.value}>{booking.pickupLocation}</span>
            </div>
            <div style={styles.row}>
              <span style={styles.label}>Pickup Date:</span>
              <span style={styles.value}>{formatDate(booking.fromDate)}</span>
            </div>
            <div style={styles.row}>
              <span style={styles.label}>Return Date:</span>
              <span style={styles.value}>{formatDate(booking.toDate)}</span>
            </div>
            <div style={styles.row}>
              <span style={styles.label}>Duration:</span>
              <span style={styles.value}>{booking.daysCount} day{booking.daysCount !== 1 ? 's' : ''}</span>
            </div>
          </section>

          {/* Pricing Details */}
          <section style={styles.section}>
            <h3 style={styles.sectionTitle}>Pricing Details</h3>
            <div style={styles.row}>
              <span style={styles.label}>Daily Rate:</span>
              <span style={styles.value}>{formatPrice(booking.dailyRate)}</span>
            </div>
            <div style={styles.row}>
              <span style={styles.label}>Insurance:</span>
              <span style={styles.value}>{booking.insuranceType}</span>
            </div>
            <div style={{ ...styles.row, borderTop: '1px solid #eee', paddingTop: '12px', marginTop: '12px' }}>
              <span style={{ ...styles.label, fontWeight: '700' }}>Total Price:</span>
              <span style={{ ...styles.value, fontWeight: '700', color: '#0066cc', fontSize: '18px' }}>
                {formatPrice(booking.totalPrice)}
              </span>
            </div>
          </section>

          {/* Policy Information */}
          <section style={styles.section}>
            <h3 style={styles.sectionTitle}>Policy Information</h3>
            <div style={styles.row}>
              <span style={styles.label}>Cancellation Policy:</span>
              <span style={styles.value}>{booking.cancellationPolicy}</span>
            </div>
          </section>
        </div>
      </div>

      {/* Booking Confirmation Time */}
      <div style={styles.confirmationTime}>
        Confirmed on {formatDateTime(booking.bookingConfirmedAt)}
      </div>

      {/* Action Buttons */}
      <div style={styles.actions}>
        <button
          onClick={() => navigate('/')}
          style={styles.primaryButton}
        >
          Back to Search
        </button>
        <button
          onClick={() => window.print()}
          style={styles.secondaryButton}
        >
          Print Confirmation
        </button>
      </div>
    </div>
  )
}

export default BookingConfirmation
