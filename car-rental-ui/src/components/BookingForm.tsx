import React, { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { DocumentType, VehicleQuote, BookingRequest } from '../types'
import {
  validateDriverName,
  validateDocumentNumber,
  getDocumentValidationError,
  getAllowedDocumentTypes,
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

  const allowedDocTypes = useMemo(
    () => getAllowedDocumentTypes(pickupLocation),
    [pickupLocation]
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
        pickupLocation,
        pickupDate,
        returnDate,
      }

      const success = await onSubmit(bookingRequest)
      if (success) {
        navigate('/confirmation/new')
      }
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
                ...(errors.documentType ? styles.inputError : {}),
              }}
            >
              <option value="">Select document type</option>
              {allowedDocTypes.map((type) => (
                <option key={type} value={type}>
                  {type === DocumentType.NationalId ? 'National ID' : 'Passport'}
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
    padding: '20px',
  },
  formWrapper: {
    backgroundColor: '#fff',
    padding: '24px',
    borderRadius: '8px',
    boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
  },
  title: {
    fontSize: '20px',
    fontWeight: '600',
    marginBottom: '20px',
    color: '#333',
  },
  vehicleSummary: {
    padding: '16px',
    backgroundColor: '#f9f9f9',
    borderRadius: '4px',
    marginBottom: '24px',
    borderLeft: '4px solid #0066cc',
  },
  summaryTitle: {
    fontSize: '14px',
    fontWeight: '600',
    marginBottom: '12px',
    color: '#333',
  },
  summaryGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(2, 1fr)',
    gap: '12px',
    fontSize: '14px',
  },
  summaryLabel: {
    display: 'block',
    color: '#666',
    fontWeight: '500',
    marginBottom: '4px',
  },
  summaryValue: {
    display: 'block',
    color: '#333',
  },
  form: {
    display: 'flex',
    flexDirection: 'column',
    gap: '16px',
  },
  formGroup: {
    display: 'flex',
    flexDirection: 'column',
  },
  label: {
    fontSize: '14px',
    fontWeight: '500',
    marginBottom: '8px',
    color: '#333',
  },
  input: {
    padding: '10px 12px',
    border: '1px solid #ddd',
    borderRadius: '4px',
    fontSize: '14px',
    fontFamily: 'inherit',
  },
  inputError: {
    borderColor: '#f66',
    backgroundColor: '#fff9f9',
  },
  errorText: {
    fontSize: '12px',
    color: '#d00',
    marginTop: '4px',
  },
  validationWarning: {
    fontSize: '12px',
    color: '#d97706',
    marginTop: '8px',
    padding: '8px',
    backgroundColor: '#fef3c7',
    borderRadius: '4px',
    borderLeft: '3px solid #d97706',
  },
  actions: {
    display: 'flex',
    gap: '12px',
    marginTop: '20px',
  },
  cancelButton: {
    flex: 1,
    padding: '12px',
    backgroundColor: '#eee',
    color: '#333',
    border: 'none',
    borderRadius: '4px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
  },
  submitButton: {
    flex: 1,
    padding: '12px',
    backgroundColor: '#0066cc',
    color: '#fff',
    border: 'none',
    borderRadius: '4px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
  },
}

export default BookingForm
