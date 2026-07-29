import React, { useEffect } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { useSearch } from '../hooks/useSearch'
import ResultsTable from '../components/ResultsTable'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import { SearchCriteria } from '../types'

/**
 * Results page for displaying search results with sorting and filtering.
 */
const ResultsPage: React.FC = () => {
  const navigate = useNavigate()
  const location = useLocation()
  const { results, loading, error, hasSearched, searchData, search } = useSearch()

  const criteria = location.state?.criteria as SearchCriteria | undefined

  useEffect(() => {
    if (criteria && !hasSearched) {
      search(criteria)
    }
  }, [criteria, hasSearched, search])

  if (!criteria) {
    return (
      <div style={styles.container}>
        <ErrorMessage message="No search criteria provided. Please search again." />
        <button onClick={() => navigate('/')} style={styles.button}>
          Back to Search
        </button>
      </div>
    )
  }

  if (loading) {
    return <LoadingSpinner message="Searching for vehicles..." />
  }

  if (error) {
    return (
      <div style={styles.container}>
        <ErrorMessage message={error} />
        <button onClick={() => navigate('/')} style={styles.button}>
          Back to Search
        </button>
      </div>
    )
  }

  return (
    <div style={styles.container}>
      <div style={styles.header}>
        <h1 style={styles.title}>Search Results</h1>
        <button onClick={() => navigate('/')} style={styles.newSearchButton}>
          New Search
        </button>
      </div>

      {results.length === 0 ? (
        <div style={styles.emptyState}>
          <h3>No vehicles found</h3>
          <p>Try adjusting your search criteria.</p>
          <button onClick={() => navigate('/')} style={styles.button}>
            Modify Search
          </button>
        </div>
      ) : (
        <ResultsTable
          results={results}
          pickupLocation={searchData?.pickupLocation || criteria.pickup}
          pickupDate={searchData?.pickupDate || criteria.from.toISOString()}
          returnDate={searchData?.returnDate || criteria.to.toISOString()}
        />
      )}
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  container: {
    maxWidth: '1200px',
    margin: '0 auto',
    padding: '40px 20px',
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '30px',
  },
  title: {
    fontSize: '28px',
    fontWeight: '700',
    color: '#333',
    margin: '0',
  },
  newSearchButton: {
    padding: '10px 20px',
    backgroundColor: '#666',
    color: '#fff',
    border: 'none',
    borderRadius: '4px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
  },
  emptyState: {
    backgroundColor: '#fff',
    padding: '60px 20px',
    borderRadius: '8px',
    textAlign: 'center',
    color: '#666',
  },
  button: {
    padding: '10px 20px',
    backgroundColor: '#0066cc',
    color: '#fff',
    border: 'none',
    borderRadius: '4px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
    marginTop: '16px',
  },
}

export default ResultsPage
