import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'

/**
 * Root application component.
 */
const App = () => {
  return (
    <div>
      <h1>Car Rental Availability System</h1>
    </div>
  )
}

const rootElement = document.getElementById('root')
if (rootElement) {
  createRoot(rootElement).render(
    <StrictMode>
      <App />
    </StrictMode>
  )
}
