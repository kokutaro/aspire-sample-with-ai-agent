import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import App from '../App'

describe('App', () => {
  it('should render the main heading', () => {
    render(<App />)
    expect(screen.getByRole('heading', { level: 1 })).toHaveTextContent('Vite + React')
  })

  it('should increment the count when the button is clicked', async () => {
    render(<App />)
    const button = screen.getByRole('button', { name: /count is/i })
    expect(button).toHaveTextContent('count is a 0')

    await userEvent.click(button)
    expect(button).toHaveTextContent('count is a 1')

    await userEvent.click(button)
    expect(button).toHaveTextContent('count is a 2')
  })

  it('should display the read-the-docs paragraph', () => {
    render(<App />)
    expect(screen.getByText(/Click on the Vite and React logos to learn more/i)).toBeInTheDocument()
  })
})
