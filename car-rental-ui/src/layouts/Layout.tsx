import React from 'react'
import { Link } from 'react-router-dom'

interface LayoutProps {
  children: React.ReactNode
}

/**
 * Main layout component for the application.
 */
const Layout: React.FC<LayoutProps> = ({ children }) => {
  return (
    <div style={styles.root}>
      <header style={styles.header}>
        <div style={styles.headerContent}>
          <Link to="/" style={styles.logo}>
            <span style={styles.logoText}>🚗 CarRental</span>
          </Link>
          <nav style={styles.nav}>
            <Link to="/" style={styles.navLink}>
              Search
            </Link>
          </nav>
        </div>
      </header>

      <main style={styles.main}>{children}</main>

      <footer style={styles.footer}>
        <div style={styles.footerContent}>
          <p>&copy; 2026 Car Rental Availability System. All rights reserved.</p>
        </div>
      </footer>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
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

export default Layout
