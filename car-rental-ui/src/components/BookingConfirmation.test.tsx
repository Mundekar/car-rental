import { describe, expect, it } from 'vitest'
import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import BookingConfirmation from './BookingConfirmation'
import { BookingResponse } from '../types'

const sampleBooking: BookingResponse = {
  referenceNumber: 'CR-20260802-123456',
  driverName: 'John Doe',
  vehicleCategory: 'SUV',
  vehicleDetails: '2026 Toyota RAV4',
  provider: 'PremiumDrive',
  pickupLocation: 'Mumbai',
  fromDate: '2026-08-10T00:00:00Z',
  toDate: '2026-08-12T00:00:00Z',
  daysCount: 2,
  dailyRate: 5000,
  totalPrice: 10000,
  insuranceType: 'Comprehensive',
  cancellationPolicy: 'Free cancellation up to 48 hours before pickup',
  bookingConfirmedAt: '2026-08-02T10:00:00Z',
}

describe('BookingConfirmation', () => {
  it('shows loading message when loading is true', () => {
    render(
      <MemoryRouter>
        <BookingConfirmation booking={null} loading={true} error={null} />
      </MemoryRouter>
    )

    expect(screen.getByText('Loading booking details...')).toBeInTheDocument()
  })

  it('shows empty booking fallback when booking is missing', () => {
    render(
      <MemoryRouter>
        <BookingConfirmation booking={null} loading={false} error={null} />
      </MemoryRouter>
    )

    expect(screen.getByRole('alert')).toHaveTextContent('Booking details not available.')
    expect(screen.getByRole('button', { name: 'Back to Search' })).toBeInTheDocument()
  })

  it('renders booking details when booking data is available', () => {
    render(
      <MemoryRouter>
        <BookingConfirmation booking={sampleBooking} loading={false} error={null} />
      </MemoryRouter>
    )

    expect(screen.getByText('Booking Confirmed!')).toBeInTheDocument()
    expect(screen.getByText('CR-20260802-123456')).toBeInTheDocument()
    expect(screen.getByText('John Doe')).toBeInTheDocument()
    expect(screen.getByText('PremiumDrive')).toBeInTheDocument()
  })
})
