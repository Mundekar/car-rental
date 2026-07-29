import React from 'react'

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
        padding: '40px 20px',
        minHeight: '200px',
      }}
    >
      <div
        style={{
          width: '40px',
          height: '40px',
          border: '4px solid #ddd',
          borderTop: '4px solid #0066cc',
          borderRadius: '50%',
          animation: 'spin 1s linear infinite',
          marginBottom: '16px',
        }}
      />
      <style>{`
        @keyframes spin {
          0% { transform: rotate(0deg); }
          100% { transform: rotate(360deg); }
        }
      `}</style>
      <p style={{ color: '#666', fontSize: '14px' }}>{message}</p>
    </div>
  )
}

export default LoadingSpinner
