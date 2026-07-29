import { DocumentType, LocationConfig } from '../types'

/**
 * Location configuration mapping.
 */
const LOCATION_CONFIG: LocationConfig = {
  domestic: ['Mumbai', 'Bengaluru'],
  international: ['Dubai', 'Singapore', 'London'],
}

/**
 * Get all available locations.
 * @returns Array of all locations
 */
export const getAllLocations = (): string[] => {
  return [...LOCATION_CONFIG.domestic, ...LOCATION_CONFIG.international]
}

/**
 * Check if location is international.
 * @param location - Location name
 * @returns True if international, false if domestic
 */
export const isInternationalLocation = (location: string): boolean => {
  return LOCATION_CONFIG.international.includes(location)
}

/**
 * Check if location is domestic.
 * @param location - Location name
 * @returns True if domestic, false if international
 */
export const isDomesticLocation = (location: string): boolean => {
  return LOCATION_CONFIG.domestic.includes(location)
}

/**
 * Get allowed document types for a location.
 * @param location - Pickup location
 * @returns Array of allowed document type enums
 */
export const getAllowedDocumentTypes = (location: string): DocumentType[] => {
  if (isInternationalLocation(location)) {
    return [DocumentType.Passport]
  }
  return [DocumentType.NationalId, DocumentType.Passport]
}

/**
 * Validate document for location (client-side).
 * @param documentType - Document type enum value
 * @param location - Pickup location
 * @returns True if document is valid for location, false otherwise
 */
export const isDocumentValidForLocation = (
  documentType: DocumentType,
  location: string
): boolean => {
  const allowedTypes = getAllowedDocumentTypes(location)
  return allowedTypes.includes(documentType)
}

/**
 * Get validation error message for document.
 * @param documentType - Document type selected
 * @param location - Pickup location
 * @returns Error message or empty string if valid
 */
export const getDocumentValidationError = (
  documentType: DocumentType,
  location: string
): string => {
  if (!isDocumentValidForLocation(documentType, location)) {
    if (isInternationalLocation(location)) {
      return `International locations require a Passport. ${location} is an international location.`
    }
    return `Domestic locations accept National ID or Passport.`
  }
  return ''
}

/**
 * Validate driver name.
 * @param name - Driver name
 * @returns Error message or empty string if valid
 */
export const validateDriverName = (name: string): string => {
  if (!name || name.trim().length === 0) {
    return 'Driver name is required.'
  }
  if (name.trim().length < 2) {
    return 'Driver name must be at least 2 characters.'
  }
  return ''
}

/**
 * Validate document number.
 * @param documentNumber - Document number
 * @returns Error message or empty string if valid
 */
export const validateDocumentNumber = (documentNumber: string): string => {
  if (!documentNumber || documentNumber.trim().length === 0) {
    return 'Document number is required.'
  }
  if (documentNumber.trim().length < 5) {
    return 'Document number must be at least 5 characters.'
  }
  return ''
}

/**
 * Validate pickup location.
 * @param location - Pickup location
 * @returns Error message or empty string if valid
 */
export const validatePickupLocation = (location: string): string => {
  if (!location || location.length === 0) {
    return 'Pickup location is required.'
  }
  if (!getAllLocations().includes(location)) {
    return 'Invalid pickup location.'
  }
  return ''
}

/**
 * Validate pickup date.
 * @param date - Pickup date
 * @returns Error message or empty string if valid
 */
export const validatePickupDate = (date: Date): string => {
  if (!date || date.toString() === 'Invalid Date') {
    return 'Pickup date is required.'
  }
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  if (date < today) {
    return 'Pickup date must be today or in the future.'
  }
  return ''
}

/**
 * Validate return date.
 * @param returnDate - Return date
 * @param pickupDate - Pickup date
 * @returns Error message or empty string if valid
 */
export const validateReturnDate = (
  returnDate: Date,
  pickupDate: Date
): string => {
  if (!returnDate || returnDate.toString() === 'Invalid Date') {
    return 'Return date is required.'
  }
  if (returnDate <= pickupDate) {
    return 'Return date must be after pickup date.'
  }
  return ''
}

/**
 * Get minimum date for date picker (today).
 * @returns Today's date as ISO string
 */
export const getMinDate = (): string => {
  const today = new Date()
  return today.toISOString().split('T')[0]
}
