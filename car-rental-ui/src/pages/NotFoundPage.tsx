import React from 'react'
import { useNavigate } from 'react-router-dom'
import { notFoundPageStyles as styles } from '../styles/pages/notFoundPageStyles'

/**
 * 404 Not Found page.
 */
const NotFoundPage: React.FC = () => {
  const navigate = useNavigate()

  return (
    <div style={styles.container}>
      <div style={styles.content}>
        <h1 style={styles.code}>404</h1>
        <h2 style={styles.title}>Page Not Found</h2>
        <p style={styles.message}>
          The page you're looking for doesn't exist or has been moved.
        </p>
        <button onClick={() => navigate('/')} style={styles.button}>
          Back to Home
        </button>
      </div>
    </div>
  )
}

export default NotFoundPage
