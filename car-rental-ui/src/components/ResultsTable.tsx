import React, { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { VehicleQuote } from '../types'
import { formatPrice } from '../utils/dateUtils'
import { resultsTableStyles as styles } from '../styles/components/resultsTableStyles'

interface ResultsTableProps {
  results: VehicleQuote[]
  pickupLocation: string
  pickupDate: string
  returnDate: string
}

/**
 * Component for displaying search results with sorting and filtering.
 */
const ResultsTable: React.FC<ResultsTableProps> = ({
  results,
  pickupLocation,
  pickupDate,
  returnDate,
}) => {
  const navigate = useNavigate()
  const [sortBy, setSortBy] = useState<'asc' | 'desc' | ''>('')
  const [filterCategory, setFilterCategory] = useState('')

  // Get unique categories from results
  const categories = useMemo(() => {
    const cats = new Set(results.map((r) => r.category))
    return Array.from(cats).sort()
  }, [results])

  // Filter results; only sort client-side when the user has explicitly chosen an order.
  // When sortBy is empty the server-side order (ascending by price) is preserved.
  const filteredAndSorted = useMemo(() => {
    let filtered = results

    if (filterCategory) {
      filtered = filtered.filter((r) => r.category === filterCategory)
    }

    if (!sortBy) {
      return filtered
    }

    return filtered.slice().sort((a, b) => {
      return sortBy === 'asc'
        ? a.totalPrice - b.totalPrice
        : b.totalPrice - a.totalPrice
    })
  }, [results, filterCategory, sortBy])

  const handleBookClick = (vehicle: VehicleQuote): void => {
    navigate('/booking', {
      state: {
        vehicle,
        pickupLocation,
        pickupDate,
        returnDate,
      },
    })
  }

  if (results.length === 0) {
    return (
      <div style={styles.emptyState}>
        <h3>No vehicles found</h3>
        <p>Try adjusting your search criteria.</p>
      </div>
    )
  }

  return (
    <div style={styles.container}>
      {/* Sorting and Filtering Controls */}
      <div style={styles.controls}>
        <div style={styles.controlGroup}>
          <label htmlFor="sort" style={styles.controlLabel}>
            Sort by Price:
          </label>
          <select
            id="sort"
            value={sortBy}
            onChange={(e) => setSortBy(e.target.value as 'asc' | 'desc' | '')}
            style={styles.select}
          >
            <option value="">Default (Lowest to Highest)</option>
            <option value="asc">Lowest to Highest</option>
            <option value="desc">Highest to Lowest</option>
          </select>
        </div>

        <div style={styles.controlGroup}>
          <label htmlFor="category" style={styles.controlLabel}>
            Category:
          </label>
          <select
            id="category"
            value={filterCategory}
            onChange={(e) => setFilterCategory(e.target.value)}
            style={styles.select}
          >
            <option value="">All Categories</option>
            {categories.map((cat) => (
              <option key={cat} value={cat}>
                {cat}
              </option>
            ))}
          </select>
        </div>

        <div style={styles.resultCount}>
          Showing {filteredAndSorted.length} vehicle
          {filteredAndSorted.length !== 1 ? 's' : ''}
        </div>
      </div>

      {/* Results Grid */}
      <div style={styles.grid}>
        {filteredAndSorted.map((vehicle) => (
          <div key={vehicle.vehicleId} style={styles.card}>
            <div style={styles.cardHeader}>
              <div style={styles.providerBadge}>{vehicle.provider}</div>
              <div style={styles.availability}>
                {vehicle.isAvailable ? (
                  <span style={styles.availableTag}>Available</span>
                ) : (
                  <span style={styles.unavailableTag}>Unavailable</span>
                )}
              </div>
            </div>

            <div style={styles.cardContent}>
              <h3 style={styles.vehicleName}>
                {vehicle.make} {vehicle.model}
              </h3>

              <div style={styles.details}>
                <div style={styles.detailRow}>
                  <span style={styles.detailLabel}>Category:</span>
                  <span style={styles.detailValue}>{vehicle.category}</span>
                </div>
                <div style={styles.detailRow}>
                  <span style={styles.detailLabel}>Daily Rate:</span>
                  <span style={styles.detailValue}>
                    {formatPrice(vehicle.dailyRate)}
                  </span>
                </div>
                <div style={styles.detailRow}>
                  <span style={styles.detailLabel}>Total Price:</span>
                  <span style={{ ...styles.detailValue, fontWeight: '600', color: '#0066cc' }}>
                    {formatPrice(vehicle.totalPrice)}
                  </span>
                </div>
                <div style={styles.detailRow}>
                  <span style={styles.detailLabel}>Insurance:</span>
                  <span style={styles.detailValue}>{vehicle.insuranceType}</span>
                </div>
                <div style={styles.detailRow}>
                  <span style={styles.detailLabel}>Cancellation:</span>
                  <span style={styles.detailValue}>{vehicle.cancellationPolicy}</span>
                </div>
              </div>

              <button
                onClick={() => handleBookClick(vehicle)}
                disabled={!vehicle.isAvailable}
                style={{
                  ...styles.bookButton,
                  ...(vehicle.isAvailable ? {} : styles.bookButtonDisabled),
                }}
              >
                {vehicle.isAvailable ? 'Book Now' : 'Unavailable'}
              </button>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default ResultsTable
