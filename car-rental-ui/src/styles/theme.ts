/**
 * Centralized theme constants for the Car Rental application.
 * Defines colors, spacing, typography, and component styles.
 */

export const theme = {
  // Color Palette
  colors: {
    // Primary
    primary: '#0066cc',
    primaryLight: '#e6f0ff',
    primaryDark: '#004999',

    // Neutrals
    white: '#fff',
    black: '#000',
    text: '#333',
    textLight: '#666',
    border: '#ddd',
    background: '#f9f9f9',
    backgroundLight: '#f5f5f5',

    // Status Colors
    success: '#2e7d32',
    successBackground: '#e8f5e9',
    error: '#d00',
    errorBackground: '#fee',
    warning: '#ff9800',
    warningBackground: '#fff3e0',
    info: '#0066cc',
    infoBackground: '#e3f2fd',
  },

  // Spacing Scale (using 4px base unit)
  spacing: {
    xs: '4px',
    sm: '8px',
    md: '12px',
    lg: '16px',
    xl: '20px',
    xxl: '24px',
    xxxl: '32px',
  },

  // Border Radius
  radius: {
    small: '4px',
    medium: '8px',
    large: '12px',
    round: '20px',
  },

  // Typography
  typography: {
    h1: {
      fontSize: '28px',
      fontWeight: '700',
    },
    h2: {
      fontSize: '20px',
      fontWeight: '600',
    },
    h3: {
      fontSize: '18px',
      fontWeight: '600',
    },
    body: {
      fontSize: '14px',
      fontWeight: '400',
    },
    bodySmall: {
      fontSize: '12px',
      fontWeight: '400',
    },
    label: {
      fontSize: '14px',
      fontWeight: '500',
    },
    labelSmall: {
      fontSize: '12px',
      fontWeight: '600',
    },
  },

  // Shadow Effects
  shadows: {
    subtle: '0 1px 3px rgba(0, 0, 0, 0.1)',
    medium: '0 4px 6px rgba(0, 0, 0, 0.1)',
    large: '0 10px 15px rgba(0, 0, 0, 0.1)',
  },

  // Transitions
  transitions: {
    fast: '0.15s',
    normal: '0.2s',
    slow: '0.3s',
  },

  // Common Component Styles
  components: {
    // Input Fields (select, input, textarea)
    input: {
      padding: `${10}px 12px`,
      border: `1px solid #ddd`,
      borderRadius: '4px',
      fontSize: '14px',
      fontFamily: 'inherit',
      transition: 'border-color 0.2s, box-shadow 0.2s',
    } as React.CSSProperties,

    inputError: {
      borderColor: '#d00',
      backgroundColor: '#fee',
    } as React.CSSProperties,

    inputFocus: {
      borderColor: '#0066cc',
      outline: 'none',
      boxShadow: '0 0 0 3px rgba(0, 102, 204, 0.1)',
    } as React.CSSProperties,

    // Buttons
    button: {
      padding: '12px 24px',
      border: 'none',
      borderRadius: '4px',
      fontSize: '16px',
      fontWeight: '600',
      cursor: 'pointer',
      transition: 'background-color 0.2s, opacity 0.2s',
      fontFamily: 'inherit',
    } as React.CSSProperties,

    buttonPrimary: {
      backgroundColor: '#0066cc',
      color: '#fff',
    } as React.CSSProperties,

    buttonPrimaryHover: {
      backgroundColor: '#004999',
    } as React.CSSProperties,

    buttonDisabled: {
      backgroundColor: '#999',
      cursor: 'not-allowed',
      opacity: 0.6,
    } as React.CSSProperties,

    buttonSmall: {
      padding: '8px 16px',
      fontSize: '14px',
    } as React.CSSProperties,

    // Cards
    card: {
      backgroundColor: '#fff',
      border: '1px solid #ddd',
      borderRadius: '8px',
      boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
      transition: 'transform 0.2s, box-shadow 0.2s',
      overflow: 'hidden',
    } as React.CSSProperties,

    cardHover: {
      boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)',
      transform: 'translateY(-2px)',
    } as React.CSSProperties,

    // Forms
    formContainer: {
      backgroundColor: '#fff',
      padding: '24px',
      borderRadius: '8px',
      boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
      marginBottom: '24px',
    } as React.CSSProperties,

    formGroup: {
      display: 'flex',
      flexDirection: 'column',
    } as React.CSSProperties,

    formLabel: {
      fontSize: '14px',
      fontWeight: '500',
      marginBottom: '8px',
      color: '#333',
    } as React.CSSProperties,

    // Badges/Tags
    badge: {
      display: 'inline-block',
      padding: '4px 12px',
      borderRadius: '4px',
      fontSize: '12px',
      fontWeight: '600',
    } as React.CSSProperties,

    badgePrimary: {
      backgroundColor: '#0066cc',
      color: '#fff',
    } as React.CSSProperties,

    badgeSuccess: {
      backgroundColor: '#e8f5e9',
      color: '#2e7d32',
    } as React.CSSProperties,

    badgeError: {
      backgroundColor: '#fee',
      color: '#d00',
    } as React.CSSProperties,

    badgeRound: {
      borderRadius: '20px',
    } as React.CSSProperties,

    // Error Messages
    errorText: {
      fontSize: '12px',
      color: '#d00',
      marginTop: '4px',
    } as React.CSSProperties,

    // Tables/Grids
    grid: {
      display: 'grid',
      gap: '16px',
    } as React.CSSProperties,

    gridResponsive: {
      display: 'grid',
      gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))',
      gap: '20px',
    } as React.CSSProperties,

    // Controls Container
    controls: {
      display: 'flex',
      gap: '20px',
      alignItems: 'center',
      padding: '16px',
      backgroundColor: '#f9f9f9',
      borderRadius: '4px',
      flexWrap: 'wrap',
    } as React.CSSProperties,
  },
} as const

// Type-safe helper for accessing theme values
export type Theme = typeof theme
export type ThemeColors = typeof theme.colors
export type ThemeSpacing = typeof theme.spacing

// Helper function to get spacing value in pixels
export const getSpacing = (key: keyof typeof theme.spacing): string => {
  return theme.spacing[key]
}

// Helper function to get color value
export const getColor = (key: keyof typeof theme.colors): string => {
  return theme.colors[key]
}
