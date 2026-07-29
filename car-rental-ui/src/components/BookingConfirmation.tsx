import React from 'react'
import { useNavigate } from 'react-router-dom'
import { BookingResponse } from '../types'
import { formatDate, formatDateTime, formatPrice } from '../utils/dateUtils'
import LoadingSpinner from './LoadingSpinner'
import ErrorMessage from './ErrorMessage'

interface BookingConfirmationProps {
  booking: BookingResponse | null
  loading: boolean
  error: string | null
}

/**
 * Component for displaying booking confirmation details.
 */
const BookingConfirmation: React.FC<BookingConfirmationProps> = ({
  booking,
  loading,
  error,
}) => {
  const navigate = useNavigate()

  if (loading) {
    return <LoadingSpinner message="Loading booking details..." />
  }

  if (error && !booking) {
    return (
      <div style={styles.container}>
        <ErrorMessage message={error} />
        <button
          onClick={() => navigate('/')}
          style={styles.primaryButton}
        >
          Back to Search
        </button>
      </div>
    )
  }

  if (!booking) {
    return (
      <div style={styles.container}>
        <ErrorMessage message="Booking details not available." />
        <button
          onClick={() => navigate('/')}
          style={styles.primaryButton}
        >
          Back to Search
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
          New Search
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

const styles: Record<string, React.CSSProperties> = {
  container: {
    maxWidth: '700px',
    margin: '0 auto',
    padding: '20px',
  },
  successCard: {
    backgroundColor: '#e8f5e9',
    border: '1px solid #4caf50',
    borderRadius: '8px',
    padding: '32px 24px',
    textAlign: 'center',
    marginBottom: '24px',
  },
  successIcon: {
    fontSize: '48px',
    color: '#4caf50',
    marginBottom: '16px',
  },
  successTitle: {
    fontSize: '24px',
    fontWeight: '700',
    color: '#2e7d32',
    marginBottom: '8px',
  },
  successMessage: {
    fontSize: '14px',
    color: '#558b2f',
  },
  referenceCard: {
    backgroundColor: '#fff',
    border: '2px solid #0066cc',
    borderRadius: '8px',
    padding: '24px',
    textAlign: 'center',
    marginBottom: '24px',
  },
  referenceLabel: {
    fontSize: '12px',
    fontWeight: '600',
    color: '#666',
    textTransform: 'uppercase',
    marginBottom: '8px',
  },
  referenceNumber: {
    fontSize: '28px',
    fontWeight: '700',
    color: '#0066cc',
    fontFamily: 'monospace',
    marginBottom: '16px',
    letterSpacing: '2px',
  },
  copyButton: {
    padding: '10px 20px',
    backgroundColor: '#0066cc',
    color: '#fff',
    border: 'none',
    borderRadius: '4px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
  },
  detailsCard: {
    backgroundColor: '#fff',
    border: '1px solid #ddd',
    borderRadius: '8px',
    padding: '24px',
    marginBottom: '24px',
  },
  detailsTitle: {
    fontSize: '18px',
    fontWeight: '600',
    marginBottom: '20px',
    color: '#333',
  },
  detailsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))',
    gap: '24px',
  },
  section: {
    paddingBottom: '16px',
    borderBottom: '1px solid #eee',
  },
  sectionTitle: {
    fontSize: '14px',
    fontWeight: '600',
    color: '#333',
    marginBottom: '12px',
    textTransform: 'uppercase',
  },
  row: {
    display: 'flex',
    justifyContent: 'space-between',
    marginBottom: '8px',
    fontSize: '14px',
  },
  label: {
    color: '#666',
    fontWeight: '500',
  },
  value: {
    color: '#333',
    textAlign: 'right',
  },
  confirmationTime: {
    textAlign: 'center',
    fontSize: '12px',
    color: '#999',
    marginBottom: '24px',
  },
  actions: {
    display: 'grid',
    gridTemplateColumns: '1fr 1fr',
    gap: '12px',
  },
  primaryButton: {
    padding: '12px',
    backgroundColor: '#0066cc',
    color: '#fff',
    border: 'none',
    borderRadius: '4px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
  },
  secondaryButton: {
    padding: '12px',
    backgroundColor: '#eee',
    color: '#333',
    border: 'none',
    borderRadius: '4px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
  },
}

export default BookingConfirmation
