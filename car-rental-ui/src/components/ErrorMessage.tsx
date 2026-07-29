import React from 'react'

interface ErrorMessageProps {
  message: string | null
}

/**
 * Component for displaying error messages.
 */
const ErrorMessage: React.FC<ErrorMessageProps> = ({ message }) => {
  if (!message) return null

  return (
    <div
      style={{
        padding: '12px 16px',
        backgroundColor: '#fee',
        border: '1px solid #f66',
        borderRadius: '4px',
        color: '#d00',
        fontSize: '14px',
        marginBottom: '16px',
      }}
      role="alert"
    >
      ⚠ {message}
    </div>
  )
}

export default ErrorMessage
