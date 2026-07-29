import React from 'react'
import { useNavigate } from 'react-router-dom'
import { useSearch } from '../hooks/useSearch'
import SearchForm from '../components/SearchForm'
import ErrorMessage from '../components/ErrorMessage'
import { theme } from '../styles/theme'
import { SearchCriteria } from '../types'

/**
 * Home/Search page for the car rental application.
 */
const HomePage: React.FC = () => {
  const navigate = useNavigate()
  const { loading, error, search, clearSearch } = useSearch()

  const handleSearch = async (criteria: SearchCriteria): Promise<void> => {
    clearSearch()
    await search(criteria)
    
    // Navigate to results page
    navigate('/results', {
      state: {
        criteria,
      },
    })
  }

  return (
    <div style={styles.container}>
      <div style={styles.header}>
        <h1 style={styles.title}>Car Rental Availability System</h1>
        <p style={styles.subtitle}>Find and book your perfect vehicle</p>
      </div>

      {error && <ErrorMessage message={error} />}

      <SearchForm onSearch={handleSearch} loading={loading} />

      <div style={styles.info}>
        <h2>How it works</h2>
        <div style={styles.steps}>
          <div style={styles.step}>
            <div style={styles.stepNumber}>1</div>
            <h3>Search</h3>
            <p>Enter your pickup location and travel dates</p>
          </div>
          <div style={styles.step}>
            <div style={styles.stepNumber}>2</div>
            <h3>Compare</h3>
            <p>Browse available vehicles and compare prices</p>
          </div>
          <div style={styles.step}>
            <div style={styles.stepNumber}>3</div>
            <h3>Book</h3>
            <p>Select a vehicle and complete your booking</p>
          </div>
          <div style={styles.step}>
            <div style={styles.stepNumber}>4</div>
            <h3>Confirm</h3>
            <p>Get your booking confirmation instantly</p>
          </div>
        </div>
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  container: {
    maxWidth: '1200px',
    margin: '0 auto',
    padding: `${theme.spacing.xxxl} ${theme.spacing.xl}`,
  },
  header: {
    textAlign: 'center',
    marginBottom: theme.spacing.xxxl,
  } as React.CSSProperties,
  title: {
    fontSize: '36px',
    fontWeight: '700',
    color: theme.colors.text,
    marginBottom: theme.spacing.sm,
  },
  subtitle: {
    fontSize: '18px',
    color: theme.colors.textLight,
    marginBottom: '0',
  },
  info: {
    marginTop: '60px',
    paddingTop: theme.spacing.xxxl,
    borderTop: `1px solid ${theme.colors.border}`,
  } as React.CSSProperties,
  steps: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
    gap: theme.spacing.xl,
    marginTop: '30px',
  } as React.CSSProperties,
  step: {
    textAlign: 'center',
    padding: theme.spacing.xl,
  } as React.CSSProperties,
  stepNumber: {
    width: '40px',
    height: '40px',
    lineHeight: '40px',
    backgroundColor: theme.colors.primary,
    color: theme.colors.white,
    borderRadius: '50%',
    fontSize: '20px',
    fontWeight: '700',
    margin: `0 auto ${theme.spacing.md}`,
  } as React.CSSProperties,
}

export default HomePage
