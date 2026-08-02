import type { CSSProperties } from 'react'
import { theme } from '../theme'

export const searchFormStyles: Record<string, CSSProperties> = {
  form: {
    ...theme.components.formContainer,
  },
  title: {
    ...theme.typography.h2,
    marginBottom: theme.spacing.xl,
    color: theme.colors.text,
  },
  grid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
    gap: theme.spacing.lg,
    marginBottom: theme.spacing.xl,
  },
  formGroup: theme.components.formGroup,
  label: theme.components.formLabel,
  input: theme.components.input,
  inputError: theme.components.inputError,
  errorText: theme.components.errorText,
  button: {
    ...theme.components.button,
    ...theme.components.buttonPrimary,
  },
  buttonDisabled: theme.components.buttonDisabled,
}
