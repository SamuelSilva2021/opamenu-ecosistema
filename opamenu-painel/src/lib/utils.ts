import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"
import { AxiosError } from "axios"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function getErrorMessage(error: unknown): string {
  if (error instanceof AxiosError) {
    const data = error.response?.data;

    // Se não houver dados de resposta, retorna a mensagem do erro HTTP
    if (!data) {
      return error.message || "Erro de conexão com o servidor";
    }
    
    // Case 1: Array of errors (e.g. ["Error 1", "Error 2"] or [{ code: "ERROR", message: "..." }])
    if (Array.isArray(data) && data.length > 0) {
      const first = data[0];
      if (typeof first === 'string') {
          return data.join(", ");
      }
      if (first?.message) {
          return data.map((e: any) => e.message).join(", ");
      }
    }
    
    // Case 2: ResponseDTO with errors property
    if (data.errors && Array.isArray(data.errors) && data.errors.length > 0) {
        return data.errors.map((e: any) => {
            if (typeof e === 'string') return e;
            return e.message || String(e);
        }).join(", ");
    }

    // Case 3: Simple message property (prioridade se não houver lista de erros específica)
    if (data.message) {
      return data.message;
    }

    // Case 4: String body
    if (typeof data === "string") {
      return data;
    }
    
    // Fallback para status code se houver response
    if (error.response?.status) {
        return `Erro ${error.response.status}: ${error.response.statusText || 'Falha na requisição'}`;
    }
  }
  
  if (error instanceof Error) {
    return error.message;
  }
  
  if (typeof error === "string") {
      return error;
  }
  
  return "Ocorreu um erro inesperado";
}

export function formatDocument(value: string) {
  const v = value.replace(/\D/g, "");
  
  if (v.length <= 11) {
    return v
      .replace(/(\d{3})(\d)/, "$1.$2")
      .replace(/(\d{3})(\d)/, "$1.$2")
      .replace(/(\d{3})(\d{1,2})$/, "$1-$2");
  } else {
    return v
      .substring(0, 14)
      .replace(/^(\d{2})(\d)/, "$1.$2")
      .replace(/^(\d{2})\.(\d{3})(\d)/, "$1.$2.$3")
      .replace(/\.(\d{3})(\d)/, ".$1/$2")
      .replace(/(\d{4})(\d)/, "$1-$2");
  }
}
