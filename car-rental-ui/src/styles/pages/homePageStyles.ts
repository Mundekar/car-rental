import type { CSSProperties } from 'react'
import { theme } from '../theme'

export const homePageStyles: Record<string, CSSProperties> = {
  container: {
    maxWidth: '1200px',
    margin: '0 auto',
    padding: `${theme.spacing.xxxl} ${theme.spacing.xl}`,
  },
  header: {
    textAlign: 'center',
    marginBottom: theme.spacing.xxxl,
  } as CSSProperties,
  title: {
    fontSize: '36px',
    fontWeight: '700',
    color: theme.colors.text,
    marginBottom: theme.spacing.sm,
  },
  subtitle: {
    fontSize: '18px',
    color: theme.colors.textLight,
    marginBottom: '0',
  },
  info: {
    marginTop: '60px',
    paddingTop: theme.spacing.xxxl,
    borderTop: `1px solid ${theme.colors.border}`,
  } as CSSProperties,
  steps: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
    gap: theme.spacing.xl,
    marginTop: '30px',
  } as CSSProperties,
  step: {
    textAlign: 'center',
    padding: theme.spacing.xl,
  } as CSSProperties,
  stepNumber: {
    width: '40px',
    height: '40px',
    lineHeight: '40px',
    backgroundColor: theme.colors.primary,
    color: theme.colors.white,
    borderRadius: '50%',
    fontSize: '20px',
    fontWeight: '700',
    margin: `0 auto ${theme.spacing.md}`,
  } as CSSProperties,
}
