import React, { useState } from 'react'
import { SearchCriteria, VehicleCategory } from '../types'
import { theme } from '../styles/theme'
import {
  validatePickupDate,
  validateReturnDate,
  getAllLocations,
  getMinDate,
} from '../utils/validation'

interface SearchFormProps {
  onSearch: (criteria: SearchCriteria) => void
  loading: boolean
}

/**
 * Component for vehicle search form.
 * Collects pickup location, dates, and optional category filter.
 */
const SearchForm: React.FC<SearchFormProps> = ({ onSearch, loading }) => {
  const [pickup, setPickup] = useState('')
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [category, setCategory] = useState('')
  const [errors, setErrors] = useState<Record<string, string>>({})

  const handleSubmit = (e: React.FormEvent): void => {
    e.preventDefault()
    const newErrors: Record<string, string> = {}

    // Validate pickup location
    if (!pickup) {
      newErrors.pickup = 'Pickup location is required.'
    }

    // Validate pickup date
    if (!from) {
      newErrors.from = 'Pickup date is required.'
    } else {
      const pickupDate = new Date(from)
      const pickupError = validatePickupDate(pickupDate)
      if (pickupError) {
        newErrors.from = pickupError
      }
    }

    // Validate return date
    if (!to) {
      newErrors.to = 'Return date is required.'
    } else if (from) {
      const pickupDate = new Date(from)
      const returnDate = new Date(to)
      const returnError = validateReturnDate(returnDate, pickupDate)
      if (returnError) {
        newErrors.to = returnError
      }
    }

    setErrors(newErrors)

    if (Object.keys(newErrors).length === 0) {
      onSearch({
        pickup,
        from: new Date(from),
        to: new Date(to),
        category: category || undefined,
      })
    }
  }

  const locations = getAllLocations()
  const categories = Object.values(VehicleCategory)
  const minDate = getMinDate()

  return (
    <form onSubmit={handleSubmit} style={styles.form}>
      <h2 style={styles.title}>Search Vehicles</h2>

      <div style={styles.grid}>
        {/* Pickup Location */}
        <div style={styles.formGroup}>
          <label htmlFor="pickup" style={styles.label}>
            Pickup Location *
          </label>
          <select
            id="pickup"
            value={pickup}
            onChange={(e) => {
              setPickup(e.target.value)
              setErrors({ ...errors, pickup: '' })
            }}
            style={{
              ...styles.input,
              ...(errors.pickup ? styles.inputError : {}),
            }}
          >
            <option value="">Select a location</option>
            {locations.map((loc) => (
              <option key={loc} value={loc}>
                {loc}
              </option>
            ))}
          </select>
          {errors.pickup && (
            <span style={styles.errorText}>{errors.pickup}</span>
          )}
        </div>

        {/* Pickup Date */}
        <div style={styles.formGroup}>
          <label htmlFor="from" style={styles.label}>
            Pickup Date *
          </label>
          <input
            id="from"
            type="date"
            value={from}
            min={minDate}
            onChange={(e) => {
              setFrom(e.target.value)
              setErrors({ ...errors, from: '' })
            }}
            style={{
              ...styles.input,
              ...(errors.from ? styles.inputError : {}),
            }}
          />
          {errors.from && <span style={styles.errorText}>{errors.from}</span>}
        </div>

        {/* Return Date */}
        <div style={styles.formGroup}>
          <label htmlFor="to" style={styles.label}>
            Return Date *
          </label>
          <input
            id="to"
            type="date"
            value={to}
            min={from || minDate}
            onChange={(e) => {
              setTo(e.target.value)
              setErrors({ ...errors, to: '' })
            }}
            style={{
              ...styles.input,
              ...(errors.to ? styles.inputError : {}),
            }}
          />
          {errors.to && <span style={styles.errorText}>{errors.to}</span>}
        </div>

        {/* Category Filter (Optional) */}
        <div style={styles.formGroup}>
          <label htmlFor="category" style={styles.label}>
            Vehicle Category (Optional)
          </label>
          <select
            id="category"
            value={category}
            onChange={(e) => setCategory(e.target.value)}
            style={styles.input}
          >
            <option value="">All Categories</option>
            {categories.map((cat) => (
              <option key={cat} value={cat}>
                {cat}
              </option>
            ))}
          </select>
        </div>
      </div>

      <button
        type="submit"
        disabled={loading}
        style={{
          ...styles.button,
          ...(loading ? styles.buttonDisabled : {}),
        }}
      >
        {loading ? 'Searching...' : 'Search'}
      </button>
    </form>
  )
}

const styles: Record<string, React.CSSProperties> = {
  form: {
    ...theme.components.formContainer,
  },
  title: {
    ...theme.typography.h2,
    marginBottom: theme.spacing.xl,
    color: theme.colors.text,
  },
  grid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
    gap: theme.spacing.lg,
    marginBottom: theme.spacing.xl,
  },
  formGroup: theme.components.formGroup,
  label: theme.components.formLabel,
  input: theme.components.input,
  inputError: theme.components.inputError,
  errorText: theme.components.errorText,
  button: {
    ...theme.components.button,
    ...theme.components.buttonPrimary,
  },
  buttonDisabled: theme.components.buttonDisabled,
}

export default SearchForm
