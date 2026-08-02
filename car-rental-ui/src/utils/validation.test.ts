import { describe, expect, it } from 'vitest'
import {
  getDocumentValidationError,
  validateDriverName,
  validateDocumentNumber,
} from './validation'
import { DocumentType } from '../types'

describe('validation utilities', () => {
  it('requires passport for international locations', () => {
    const error = getDocumentValidationError(DocumentType.NationalId, 'Dubai')

    expect(error).toContain('International locations require a Passport')
  })

  it('accepts valid driver name', () => {
    expect(validateDriverName('Alice Doe')).toBe('')
  })

  it('rejects short document number', () => {
    expect(validateDocumentNumber('1234')).toBe('Document number must be at least 5 characters.')
  })
})
