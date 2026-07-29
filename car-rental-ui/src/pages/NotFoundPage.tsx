import React from 'react'
import { useNavigate } from 'react-router-dom'

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
    padding: '20px',
  },
  content: {
    textAlign: 'center',
  },
  code: {
    fontSize: '120px',
    fontWeight: '700',
    color: '#0066cc',
    margin: '0',
    lineHeight: '1',
  },
  title: {
    fontSize: '32px',
    fontWeight: '600',
    color: '#333',
    marginBottom: '16px',
  },
  message: {
    fontSize: '16px',
    color: '#666',
    marginBottom: '24px',
  },
  button: {
    padding: '12px 24px',
    backgroundColor: '#0066cc',
    color: '#fff',
    border: 'none',
    borderRadius: '4px',
    fontSize: '16px',
    fontWeight: '600',
    cursor: 'pointer',
  },
}

export default NotFoundPage
