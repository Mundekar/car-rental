import type { CSSProperties } from 'react'

export const resultsPageStyles: Record<string, CSSProperties> = {
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
