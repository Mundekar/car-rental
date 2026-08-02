import type { CSSProperties } from 'react'
import { theme } from '../theme'

export const notFoundPageStyles: Record<string, CSSProperties> = {
  container: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    minHeight: 'calc(100vh - 100px)',
    padding: theme.spacing.xl,
  } as CSSProperties,
  content: {
    textAlign: 'center',
  } as CSSProperties,
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
