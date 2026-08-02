import React from 'react'
import { useNavigate } from 'react-router-dom'
import { useSearch } from '../hooks/useSearch'
import SearchForm from '../components/SearchForm'
import ErrorMessage from '../components/ErrorMessage'
import { SearchCriteria } from '../types'
import { homePageStyles as styles } from '../styles/pages/homePageStyles'

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

export default HomePage
