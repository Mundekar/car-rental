import type { CSSProperties } from 'react'

export const layoutStyles: Record<string, CSSProperties> = {
  root: {
    display: 'flex',
    flexDirection: 'column',
    minHeight: '100vh',
  },
  header: {
    backgroundColor: '#fff',
    borderBottom: '1px solid #ddd',
    padding: '16px 0',
    boxShadow: '0 1px 3px rgba(0, 0, 0, 0.05)',
  },
  headerContent: {
    maxWidth: '1200px',
    margin: '0 auto',
    padding: '0 20px',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  logo: {
    textDecoration: 'none',
  },
  logoText: {
    fontSize: '20px',
    fontWeight: '700',
    color: '#0066cc',
  },
  nav: {
    display: 'flex',
    gap: '20px',
  },
  navLink: {
    textDecoration: 'none',
    color: '#333',
    fontSize: '14px',
    fontWeight: '500',
    transition: 'color 0.2s',
  },
  main: {
    flex: 1,
    backgroundColor: '#f9f9f9',
  },
  footer: {
    backgroundColor: '#333',
    color: '#fff',
    padding: '24px 0',
    marginTop: '60px',
  },
  footerContent: {
    maxWidth: '1200px',
    margin: '0 auto',
    padding: '0 20px',
    textAlign: 'center',
    fontSize: '14px',
  },
}
