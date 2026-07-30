import React, { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { DocumentType, VehicleQuote, BookingRequest } from '../types'
import { theme } from '../styles/theme'
import {
  validateDriverName,
  validateDocumentNumber,
  getDocumentValidationError,
} from '../utils/validation'
import { formatPrice } from '../utils/dateUtils'
import ErrorMessage from './ErrorMessage'
import LoadingSpinner from './LoadingSpinner'

interface BookingFormProps {
  vehicle: VehicleQuote
  pickupLocation: string
  pickupDate: string
  returnDate: string
  onSubmit: (booking: BookingRequest) => Promise<boolean>
  loading: boolean
  apiError: string | null
}

/**
 * Component for collecting booking details with client-side validation.
 */
const BookingForm: React.FC<BookingFormProps> = ({
  vehicle,
  pickupLocation,
  pickupDate,
  returnDate,
  onSubmit,
  loading,
  apiError,
}) => {
  const navigate = useNavigate()
  const [driverName, setDriverName] = useState('')
  const [documentType, setDocumentType] = useState<DocumentType | null>(null)
  const [documentNumber, setDocumentNumber] = useState('')
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [validationError, setValidationError] = useState<string>('')

  const ALL_DOC_TYPES = useMemo(
    () => [
      { value: DocumentType.NationalId, label: 'National ID' },
      { value: DocumentType.Passport, label: 'Passport' },
    ],
    []
  )

  const handleDocumentTypeChange = (
    e: React.ChangeEvent<HTMLSelectElement>
  ): void => {
    const type = parseInt(e.target.value, 10) as DocumentType
    setDocumentType(type)
    setValidationError('')

    const error = getDocumentValidationError(type, pickupLocation)
    if (error) {
      setValidationError(error)
    }
  }

  const handleSubmit = async (e: React.FormEvent): Promise<void> => {
    e.preventDefault()
    const newErrors: Record<string, string> = {}

    const nameError = validateDriverName(driverName)
    if (nameError) {
      newErrors.driverName = nameError
    }

    if (documentType === null) {
      newErrors.documentType = 'Document type is required.'
    }

    const docError = validateDocumentNumber(documentNumber)
    if (docError) {
      newErrors.documentNumber = docError
    }

    if (documentType !== null) {
      const validationErr = getDocumentValidationError(
        documentType,
        pickupLocation
      )
      if (validationErr) {
        newErrors.documentValidation = validationErr
      }
    }

    setErrors(newErrors)

    if (Object.keys(newErrors).length === 0 && documentType !== null) {
      const bookingRequest: BookingRequest = {
        driverName: driverName.trim(),
        documentType: documentType as number,
        documentNumber: documentNumber.trim(),
        vehicleId: vehicle.vehicleId,
        provider: vehicle.provider,
        vehicleCategory: vehicle.category,
        vehicleMake: vehicle.make,
        vehicleModel: vehicle.model,
        dailyRate: vehicle.dailyRate,
        totalPrice: vehicle.totalPrice,
        insuranceType: vehicle.insuranceType,
        cancellationPolicy: vehicle.cancellationPolicy,
        pickupLocation,
        pickupDate,
        returnDate,
      }

      await onSubmit(bookingRequest)
    }
  }

  if (loading) {
    return <LoadingSpinner message="Processing your booking..." />
  }

  return (
    <div style={styles.container}>
      <div style={styles.formWrapper}>
        <h2 style={styles.title}>Complete Your Booking</h2>

        {apiError && <ErrorMessage message={apiError} />}

        <div style={styles.vehicleSummary}>
          <h3 style={styles.summaryTitle}>Selected Vehicle</h3>
          <div style={styles.summaryGrid}>
            <div>
              <span style={styles.summaryLabel}>Vehicle:</span>
              <span style={styles.summaryValue}>
                {vehicle.make} {vehicle.model}
              </span>
            </div>
            <div>
              <span style={styles.summaryLabel}>Category:</span>
              <span style={styles.summaryValue}>{vehicle.category}</span>
            </div>
            <div>
              <span style={styles.summaryLabel}>Provider:</span>
              <span style={styles.summaryValue}>{vehicle.provider}</span>
            </div>
            <div>
              <span style={styles.summaryLabel}>Total Price:</span>
              <span style={{ ...styles.summaryValue, fontWeight: '600', color: '#0066cc' }}>
                {formatPrice(vehicle.totalPrice)}
              </span>
            </div>
          </div>
        </div>

        <form onSubmit={handleSubmit} style={styles.form}>
          <div style={styles.formGroup}>
            <label htmlFor="driverName" style={styles.label}>
              Driver Name *
            </label>
            <input
              id="driverName"
              type="text"
              placeholder="Enter your full name"
              value={driverName}
              onChange={(e) => {
                setDriverName(e.target.value)
                setErrors({ ...errors, driverName: '' })
              }}
              style={{
                ...styles.input,
                ...(errors.driverName ? styles.inputError : {}),
              }}
            />
            {errors.driverName && (
              <span style={styles.errorText}>{errors.driverName}</span>
            )}
          </div>

          <div style={styles.formGroup}>
            <label htmlFor="documentType" style={styles.label}>
              Document Type *
            </label>
            <select
              id="documentType"
              value={documentType === null ? '' : documentType}
              onChange={handleDocumentTypeChange}
              style={{
                ...styles.input,
                ...((errors.documentType || validationError) ? styles.inputError : {}),
              }}
            >
              <option value="">Select document type</option>
              {ALL_DOC_TYPES.map(({ value, label }) => (
                <option key={value} value={value}>
                  {label}
                </option>
              ))}
            </select>
            {errors.documentType && (
              <span style={styles.errorText}>{errors.documentType}</span>
            )}

            {validationError && (
              <div style={styles.validationWarning}>{validationError}</div>
            )}
          </div>

          <div style={styles.formGroup}>
            <label htmlFor="documentNumber" style={styles.label}>
              Document Number *
            </label>
            <input
              id="documentNumber"
              type="text"
              placeholder="Enter document number"
              value={documentNumber}
              onChange={(e) => {
                setDocumentNumber(e.target.value)
                setErrors({ ...errors, documentNumber: '' })
              }}
              style={{
                ...styles.input,
                ...(errors.documentNumber ? styles.inputError : {}),
              }}
            />
            {errors.documentNumber && (
              <span style={styles.errorText}>{errors.documentNumber}</span>
            )}
          </div>

          <div style={styles.formGroup}>
            <label htmlFor="pickupLocation" style={styles.label}>
              Pickup Location
            </label>
            <input
              id="pickupLocation"
              type="text"
              value={pickupLocation}
              disabled
              style={{ ...styles.input, backgroundColor: '#f5f5f5' }}
            />
          </div>

          {errors.documentValidation && (
            <ErrorMessage message={errors.documentValidation} />
          )}

          <div style={styles.actions}>
            <button
              type="button"
              onClick={() => navigate('/results')}
              style={styles.cancelButton}
            >
              Back to Results
            </button>
            <button type="submit" style={styles.submitButton}>
              Confirm Booking
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  container: {
    maxWidth: '500px',
    margin: '0 auto',
    padding: theme.spacing.xl,
  },
  formWrapper: theme.components.formContainer,
  title: {
    ...theme.typography.h2,
    marginBottom: theme.spacing.xl,
    color: theme.colors.text,
  },
  vehicleSummary: {
    padding: theme.spacing.lg,
    backgroundColor: theme.colors.background,
    borderRadius: theme.radius.small,
    marginBottom: theme.spacing.xxl,
    borderLeft: `4px solid ${theme.colors.primary}`,
  } as React.CSSProperties,
  summaryTitle: {
    fontSize: theme.typography.label.fontSize,
    fontWeight: '600',
    marginBottom: theme.spacing.md,
    color: theme.colors.text,
  },
  summaryGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(2, 1fr)',
    gap: theme.spacing.md,
    fontSize: theme.typography.body.fontSize,
  } as React.CSSProperties,
  summaryLabel: {
    display: 'block',
    color: theme.colors.textLight,
    fontWeight: '500',
    marginBottom: theme.spacing.xs,
  } as React.CSSProperties,
  summaryValue: {
    display: 'block',
    color: theme.colors.text,
  } as React.CSSProperties,
  form: {
    display: 'flex',
    flexDirection: 'column',
    gap: theme.spacing.lg,
  } as React.CSSProperties,
  formGroup: theme.components.formGroup,
  label: theme.components.formLabel,
  input: theme.components.input,
  inputError: theme.components.inputError,
  errorText: theme.components.errorText,
  validationWarning: {
    fontSize: theme.typography.bodySmall.fontSize,
    color: theme.colors.error,
    marginTop: theme.spacing.lg,
    padding: theme.spacing.sm,
    backgroundColor: theme.colors.errorBackground,
    borderRadius: theme.radius.small,
    borderLeft: `3px solid ${theme.colors.error}`,
  } as React.CSSProperties,
  actions: {
    display: 'flex',
    gap: theme.spacing.md,
    marginTop: theme.spacing.xl,
  } as React.CSSProperties,
  cancelButton: {
    flex: 1,
    padding: theme.spacing.lg,
    backgroundColor: theme.colors.border,
    color: theme.colors.text,
    border: 'none',
    borderRadius: theme.radius.small,
    fontSize: theme.typography.label.fontSize,
    fontWeight: '600',
    cursor: 'pointer',
    transition: `background-color ${theme.transitions.normal}`,
  } as React.CSSProperties,
  submitButton: {
    flex: 1,
    padding: theme.spacing.lg,
    backgroundColor: theme.colors.primary,
    color: theme.colors.white,
    border: 'none',
    borderRadius: theme.radius.small,
    fontSize: theme.typography.label.fontSize,
    fontWeight: '600',
    cursor: 'pointer',
    transition: `background-color ${theme.transitions.normal}`,
  } as React.CSSProperties,
}

export default BookingForm
