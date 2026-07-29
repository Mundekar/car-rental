import React from 'react'
import { useNavigate } from 'react-router-dom'
import { theme } from '../styles/theme'

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

const styles: Record<string, React.CSSProperties> = {
  container: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    minHeight: 'calc(100vh - 100px)',
    padding: theme.spacing.xl,
  } as React.CSSProperties,
  content: {
    textAlign: 'center',
  } as React.CSSProperties,
  code: {
    fontSize: '120px',
    fontWeight: '700',
    color: theme.colors.primary,
    margin: '0',
    lineHeight: '1',
  },
  title: {
    fontSize: '32px',
    fontWeight: '600',
    color: theme.colors.text,
    marginBottom: theme.spacing.lg,
  },
  message: {
    fontSize: '16px',
    color: theme.colors.textLight,
    marginBottom: theme.spacing.xxl,
  },
  button: {
    ...theme.components.button,
    ...theme.components.buttonPrimary,
  },
}

export default NotFoundPage
