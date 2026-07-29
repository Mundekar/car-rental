import React, { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { VehicleQuote } from '../types'
import { formatPrice } from '../utils/dateUtils'

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
  const [sortBy, setSortBy] = useState<'asc' | 'desc'>('asc')
  const [filterCategory, setFilterCategory] = useState('')

  // Get unique categories from results
  const categories = useMemo(() => {
    const cats = new Set(results.map((r) => r.category))
    return Array.from(cats).sort()
  }, [results])

  // Filter and sort results
  const filteredAndSorted = useMemo(() => {
    let filtered = results

    if (filterCategory) {
      filtered = filtered.filter((r) => r.category === filterCategory)
    }

    return filtered.sort((a, b) => {
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
            onChange={(e) => setSortBy(e.target.value as 'asc' | 'desc')}
            style={styles.select}
          >
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

const styles: Record<string, React.CSSProperties> = {
  container: {
    marginTop: '24px',
  },
  controls: {
    display: 'flex',
    gap: '20px',
    alignItems: 'center',
    marginBottom: '24px',
    padding: '16px',
    backgroundColor: '#f9f9f9',
    borderRadius: '4px',
    flexWrap: 'wrap',
  },
  controlGroup: {
    display: 'flex',
    gap: '8px',
    alignItems: 'center',
  },
  controlLabel: {
    fontSize: '14px',
    fontWeight: '500',
    color: '#333',
  },
  select: {
    padding: '8px 12px',
    border: '1px solid #ddd',
    borderRadius: '4px',
    fontSize: '14px',
    fontFamily: 'inherit',
  },
  resultCount: {
    fontSize: '14px',
    color: '#666',
    marginLeft: 'auto',
  },
  grid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))',
    gap: '20px',
  },
  card: {
    backgroundColor: '#fff',
    border: '1px solid #ddd',
    borderRadius: '8px',
    overflow: 'hidden',
    boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
    transition: 'transform 0.2s, box-shadow 0.2s',
    cursor: 'pointer',
  },
  cardHeader: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: '12px 16px',
    backgroundColor: '#f5f5f5',
    borderBottom: '1px solid #eee',
  },
  providerBadge: {
    display: 'inline-block',
    backgroundColor: '#0066cc',
    color: '#fff',
    padding: '4px 12px',
    borderRadius: '20px',
    fontSize: '12px',
    fontWeight: '600',
  },
  availability: {
    fontSize: '12px',
  },
  availableTag: {
    display: 'inline-block',
    backgroundColor: '#e8f5e9',
    color: '#2e7d32',
    padding: '4px 12px',
    borderRadius: '4px',
    fontSize: '12px',
    fontWeight: '600',
  },
  unavailableTag: {
    display: 'inline-block',
    backgroundColor: '#fee',
    color: '#d00',
    padding: '4px 12px',
    borderRadius: '4px',
    fontSize: '12px',
    fontWeight: '600',
  },
  cardContent: {
    padding: '16px',
  },
  vehicleName: {
    fontSize: '16px',
    fontWeight: '600',
    marginBottom: '12px',
    color: '#333',
  },
  details: {
    marginBottom: '16px',
  },
  detailRow: {
    display: 'flex',
    justifyContent: 'space-between',
    fontSize: '14px',
    marginBottom: '8px',
  },
  detailLabel: {
    color: '#666',
    fontWeight: '500',
  },
  detailValue: {
    color: '#333',
  },
  bookButton: {
    width: '100%',
    padding: '12px',
    backgroundColor: '#0066cc',
    color: '#fff',
    border: 'none',
    borderRadius: '4px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
    transition: 'background-color 0.2s',
  },
  bookButtonDisabled: {
    backgroundColor: '#999',
    cursor: 'not-allowed',
  },
  emptyState: {
    backgroundColor: '#fff',
    padding: '40px 20px',
    borderRadius: '8px',
    textAlign: 'center',
    color: '#666',
  },
}

export default ResultsTable
