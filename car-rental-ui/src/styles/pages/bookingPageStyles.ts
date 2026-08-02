import type { CSSProperties } from 'react'

export const bookingPageStyles: Record<string, CSSProperties> = {
  container: {
    maxWidth: '600px',
    margin: '0 auto',
    padding: '40px 20px',
  },
  error: {
    backgroundColor: '#fee',
    border: '1px solid #f66',
    borderRadius: '8px',
    padding: '40px 20px',
    textAlign: 'center',
    color: '#d00',
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
