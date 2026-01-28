import * as React from "react"
import { Input } from "./input"

export const formatPhone = (value: string) => {
    if (!value) return ""
    
    // Remove tudo que não é número
    const numbers = value.replace(/\D/g, "")
    
    // Limita a 11 dígitos
    const limited = numbers.slice(0, 11)

    let formatted = limited
    if (limited.length > 10) {
      // (11) 91234-5678
      formatted = limited.replace(/^(\d{2})(\d{5})(\d{4}).*/, "($1) $2-$3")
    } else if (limited.length > 6) {
      // (11) 1234-5678
      formatted = limited.replace(/^(\d{2})(\d{4})(\d{0,4}).*/, "($1) $2-$3")
    } else if (limited.length > 2) {
      // (11) 1234...
      formatted = limited.replace(/^(\d{2})(\d{0,5})/, "($1) $2")
    } else if (limited.length > 0) {
        formatted = limited.replace(/^(\d*)/, "($1")
    }
    
    return formatted
}

export function PhoneInput({ value, onChange, className, ...props }: React.ComponentProps<typeof Input>) {
    // Garante que o valor exibido esteja sempre formatado
    const formattedValue = React.useMemo(() => {
        return formatPhone(value?.toString() || "")
    }, [value])

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const formatted = formatPhone(e.target.value)
        
        // Atualiza o valor do evento para o formatado
        e.target.value = formatted
        onChange?.(e)
    }

    return (
        <Input 
            {...props}
            value={formattedValue}
            onChange={handleChange}
            maxLength={15}
            className={className}
        />
    )
}
