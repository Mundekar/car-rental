import React from 'react'
import { useNavigate } from 'react-router-dom'
import { BookingResponse } from '../types'
import { theme } from '../styles/theme'
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
    padding: theme.spacing.xl,
  },
  successCard: {
    backgroundColor: theme.colors.successBackground,
    border: `1px solid ${theme.colors.success}`,
    borderRadius: theme.radius.medium,
    padding: `${theme.spacing.xxxl} ${theme.spacing.xxl}`,
    textAlign: 'center',
    marginBottom: theme.spacing.xxl,
  } as React.CSSProperties,
  successIcon: {
    fontSize: '48px',
    color: theme.colors.success,
    marginBottom: theme.spacing.lg,
  },
  successTitle: {
    fontSize: '24px',
    fontWeight: '700',
    color: theme.colors.success,
    marginBottom: theme.spacing.sm,
  },
  successMessage: {
    fontSize: theme.typography.body.fontSize,
    color: theme.colors.success,
  },
  referenceCard: {
    backgroundColor: theme.colors.white,
    border: `2px solid ${theme.colors.primary}`,
    borderRadius: theme.radius.medium,
    padding: theme.spacing.xxl,
    textAlign: 'center',
    marginBottom: theme.spacing.xxl,
  } as React.CSSProperties,
  referenceLabel: {
    fontSize: theme.typography.labelSmall.fontSize,
    fontWeight: '600',
    color: theme.colors.textLight,
    textTransform: 'uppercase',
    marginBottom: theme.spacing.sm,
  },
  referenceNumber: {
    fontSize: '28px',
    fontWeight: '700',
    color: theme.colors.primary,
    fontFamily: 'monospace',
    marginBottom: theme.spacing.lg,
    letterSpacing: '2px',
  },
  copyButton: {
    padding: `${theme.spacing.sm} ${theme.spacing.xl}`,
    backgroundColor: theme.colors.primary,
    color: theme.colors.white,
    border: 'none',
    borderRadius: theme.radius.small,
    fontSize: theme.typography.body.fontSize,
    fontWeight: '600',
    cursor: 'pointer',
    transition: `background-color ${theme.transitions.normal}`,
  } as React.CSSProperties,
  detailsCard: {
    backgroundColor: theme.colors.white,
    border: `1px solid ${theme.colors.border}`,
    borderRadius: theme.radius.medium,
    padding: theme.spacing.xxl,
    marginBottom: theme.spacing.xxl,
  } as React.CSSProperties,
  detailsTitle: {
    fontSize: theme.typography.h3.fontSize,
    fontWeight: '600',
    marginBottom: theme.spacing.xl,
    color: theme.colors.text,
  },
  detailsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))',
    gap: theme.spacing.xxl,
  } as React.CSSProperties,
  section: {
    paddingBottom: theme.spacing.lg,
    borderBottom: `1px solid ${theme.colors.border}`,
  } as React.CSSProperties,
  sectionTitle: {
    fontSize: theme.typography.label.fontSize,
    fontWeight: '600',
    color: theme.colors.text,
    marginBottom: theme.spacing.md,
    textTransform: 'uppercase',
  },
  row: {
    display: 'flex',
    justifyContent: 'space-between',
    marginBottom: theme.spacing.sm,
    fontSize: theme.typography.body.fontSize,
  } as React.CSSProperties,
  label: {
    color: theme.colors.textLight,
    fontWeight: '500',
  },
  value: {
    color: theme.colors.text,
    textAlign: 'right',
  } as React.CSSProperties,
  confirmationTime: {
    textAlign: 'center',
    fontSize: theme.typography.bodySmall.fontSize,
    color: '#999',
    marginBottom: theme.spacing.xxl,
  } as React.CSSProperties,
  actions: {
    display: 'grid',
    gridTemplateColumns: '1fr 1fr',
    gap: theme.spacing.md,
  } as React.CSSProperties,
  primaryButton: {
    padding: theme.spacing.lg,
    backgroundColor: theme.colors.primary,
    color: theme.colors.white,
    border: 'none',
    borderRadius: theme.radius.small,
    fontSize: theme.typography.body.fontSize,
    fontWeight: '600',
    cursor: 'pointer',
    transition: `background-color ${theme.transitions.normal}`,
  } as React.CSSProperties,
  secondaryButton: {
    padding: theme.spacing.lg,
    backgroundColor: theme.colors.border,
    color: theme.colors.text,
    border: 'none',
    borderRadius: theme.radius.small,
    fontSize: theme.typography.body.fontSize,
    fontWeight: '600',
    cursor: 'pointer',
    transition: `background-color ${theme.transitions.normal}`,
  } as React.CSSProperties,
}

export default BookingConfirmation
