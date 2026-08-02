import React from 'react'
import { Link } from 'react-router-dom'
import { layoutStyles as styles } from '../styles/layouts/layoutStyles'

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

export default Layout
