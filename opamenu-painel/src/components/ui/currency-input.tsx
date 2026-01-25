import * as React from "react"
import { Input } from "@/components/ui/input"
import { cn } from "@/lib/utils"

interface CurrencyInputProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, "onChange" | "value"> {
  value?: number | string
  onChange: (value: number) => void
}

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value)
}

export const CurrencyInput = React.forwardRef<HTMLInputElement, CurrencyInputProps>(
  ({ className, value, onChange, ...props }, ref) => {
    const [displayValue, setDisplayValue] = React.useState("")

    React.useEffect(() => {
      const numericValue = typeof value === "string" ? parseFloat(value) : value
      if (numericValue !== undefined && !isNaN(numericValue)) {
        setDisplayValue(formatCurrency(numericValue))
      } else {
        setDisplayValue("")
      }
    }, [value])

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      const rawValue = e.target.value.replace(/\D/g, "")
      const numericValue = Number(rawValue) / 100
      
      onChange(numericValue)
      // Display value update is handled by useEffect when parent updates prop
      // But for immediate feedback/cursor management, we might need more handling.
      // For simple cases, relying on prop update is safer for "controlled" behavior.
    }

    return (
      <Input
        ref={ref}
        type="text"
        inputMode="numeric"
        className={cn("text-left", className)}
        value={displayValue}
        onChange={handleChange}
        {...props}
      />
    )
  }
)
CurrencyInput.displayName = "CurrencyInput"
