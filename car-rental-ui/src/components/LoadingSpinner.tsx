import React from 'react'
import { theme } from '../styles/theme'

interface LoadingSpinnerProps {
  message?: string
}

/**
 * Component for displaying loading state.
 */
const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({
  message = 'Loading...',
}) => {
  return (
    <div
      style={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        padding: `${theme.spacing.xxxl} ${theme.spacing.xl}`,
        minHeight: '200px',
      } as React.CSSProperties}
    >
      <div
        style={{
          width: '40px',
          height: '40px',
          border: `4px solid ${theme.colors.border}`,
          borderTop: `4px solid ${theme.colors.primary}`,
          borderRadius: '50%',
          animation: 'spin 1s linear infinite',
          marginBottom: theme.spacing.lg,
        } as React.CSSProperties}
      />
      <style>{`
        @keyframes spin {
          0% { transform: rotate(0deg); }
          100% { transform: rotate(360deg); }
        }
      `}</style>
      <p style={{ color: theme.colors.textLight, fontSize: theme.typography.body.fontSize }}>
        {message}
      </p>
    </div>
  )
}

export default LoadingSpinner
