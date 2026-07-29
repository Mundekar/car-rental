import React from 'react'
import { theme } from '../styles/theme'

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
        padding: `${theme.spacing.md} ${theme.spacing.lg}`,
        backgroundColor: theme.colors.errorBackground,
        border: `1px solid ${theme.colors.error}`,
        borderRadius: theme.radius.small,
        color: theme.colors.error,
        fontSize: theme.typography.body.fontSize,
        marginBottom: theme.spacing.lg,
      } as React.CSSProperties}
      role="alert"
    >
      ⚠ {message}
    </div>
  )
}

export default ErrorMessage
