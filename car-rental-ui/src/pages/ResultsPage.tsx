import React, { useEffect } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { useSearch } from '../hooks/useSearch'
import ResultsTable from '../components/ResultsTable'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import { SearchCriteria } from '../types'
import { resultsPageStyles as styles } from '../styles/pages/resultsPageStyles'

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

export default ResultsPage
