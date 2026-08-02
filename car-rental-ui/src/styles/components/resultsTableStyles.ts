import type { CSSProperties } from 'react'
import { theme } from '../theme'

export const resultsTableStyles: Record<string, CSSProperties> = {
  container: {
    marginTop: theme.spacing.xxl,
  },
  controls: theme.components.controls,
  controlGroup: {
    display: 'flex',
    gap: theme.spacing.sm,
    alignItems: 'center',
  },
  controlLabel: theme.components.formLabel,
  select: theme.components.input,
  resultCount: {
    fontSize: theme.typography.body.fontSize,
    color: theme.colors.textLight,
    marginLeft: 'auto',
  },
  grid: theme.components.gridResponsive,
  card: theme.components.card,
  cardHeader: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: `${theme.spacing.md} ${theme.spacing.lg}`,
    backgroundColor: theme.colors.backgroundLight,
    borderBottom: `1px solid ${theme.colors.border}`,
  } as CSSProperties,
  providerBadge: {
    ...theme.components.badge,
    ...theme.components.badgePrimary,
    ...theme.components.badgeRound,
  },
  availability: {
    fontSize: theme.typography.labelSmall.fontSize,
  },
  availableTag: {
    ...theme.components.badge,
    ...theme.components.badgeSuccess,
  },
  unavailableTag: {
    ...theme.components.badge,
    ...theme.components.badgeError,
  },
  cardContent: {
    padding: theme.spacing.lg,
  },
  vehicleName: {
    ...theme.typography.h3,
    marginBottom: theme.spacing.md,
    color: theme.colors.text,
  },
  details: {
    marginBottom: theme.spacing.lg,
  },
  detailRow: {
    display: 'flex',
    justifyContent: 'space-between',
    fontSize: theme.typography.body.fontSize,
    marginBottom: theme.spacing.sm,
  } as CSSProperties,
  detailLabel: {
    color: theme.colors.textLight,
    fontWeight: '500',
  },
  detailValue: {
    color: theme.colors.text,
  },
  bookButton: {
    ...theme.components.button,
    ...theme.components.buttonPrimary,
    width: '100%',
    padding: theme.spacing.lg,
    fontSize: theme.typography.label.fontSize,
  },
  bookButtonDisabled: theme.components.buttonDisabled,
  emptyState: {
    backgroundColor: theme.colors.white,
    padding: `${theme.spacing.xxxl} ${theme.spacing.xl}`,
    borderRadius: theme.radius.medium,
    textAlign: 'center' as const,
    color: theme.colors.textLight,
  },
}
