import { act, renderHook } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { useCounter } from '../useCounter'

describe('useCounter', () => {
  it('should return initial count', () => {
    const { result } = renderHook(() => useCounter())
    expect(result.current.count).toBe(0)
  })

  it('should return custom initial count', () => {
    const { result } = renderHook(() => useCounter(10))
    expect(result.current.count).toBe(10)
  })

  it('should increment the count', () => {
    const { result } = renderHook(() => useCounter())
    act(() => {
      result.current.increment()
    })
    expect(result.current.count).toBe(1)
  })

  it('should decrement the count', () => {
    const { result } = renderHook(() => useCounter(5))
    act(() => {
      result.current.decrement()
    })
    expect(result.current.count).toBe(4)
  })

  it('should reset the count to initial value', () => {
    const { result } = renderHook(() => useCounter(5))
    act(() => {
      result.current.increment()
      result.current.increment()
      result.current.reset()
    })
    expect(result.current.count).toBe(5)
  })
})
