import { useState } from 'react'
import apiService from '../services/apiService'
import { SearchCriteria, SearchResponse, VehicleQuote } from '../types'

/**
 * Custom hook for managing search operations and state.
 */
export const useSearch = () => {
  const [results, setResults] = useState<VehicleQuote[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [hasSearched, setHasSearched] = useState(false)
  const [searchData, setSearchData] = useState<{
    pickupLocation: string
    pickupDate: string
    returnDate: string
  } | null>(null)

  /**
   * Perform search with given criteria.
   */
  const search = async (criteria: SearchCriteria): Promise<void> => {
    setLoading(true)
    setError(null)
    try {
      const response: SearchResponse = await apiService.searchVehicles(criteria)
      setResults(response.results)
      setSearchData({
        pickupLocation: response.pickupLocation,
        pickupDate: response.fromDate.toString(),
        returnDate: response.toDate.toString(),
      })
      setHasSearched(true)
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Search failed. Please try again.'
      setError(message)
      setResults([])
    } finally {
      setLoading(false)
    }
  }

  /**
   * Clear search results and reset state.
   */
  const clearSearch = (): void => {
    setResults([])
    setError(null)
    setHasSearched(false)
    setSearchData(null)
  }

  return {
    results,
    loading,
    error,
    hasSearched,
    searchData,
    search,
    clearSearch,
  }
}
