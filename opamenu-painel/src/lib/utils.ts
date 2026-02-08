import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"
import { AxiosError } from "axios"
import type { ErrorDTO } from "@/features/auth/types";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}
export function extractErrorCode(error: ErrorDTO): string {
  if (error.code) {
    return error.code;
  }
  return "";
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
    if (data.errors) {
      // Case 2.1: Array of errors
      if (Array.isArray(data.errors) && data.errors.length > 0) {
        return data.errors.map((e: any) => {
          if (typeof e === 'string') return e;

          // Tratamento para ErrorDTO (code, message, property, details)
          if (e.message)
            return e.message;

          return String(e);
        }).join(", ");
      }

      // Case 2.2: Object of errors (RFC 7807 Validation Errors)
      if (typeof data.errors === 'object' && data.errors !== null) {
        const messages: string[] = [];
        Object.values(data.errors).forEach((err: any) => {
          if (Array.isArray(err)) {
            messages.push(...err.filter(msg => typeof msg === 'string'));
          } else if (typeof err === 'string') {
            messages.push(err);
          }
        });
        if (messages.length > 0) {
          return messages.join(", ");
        }
      }
    }

    // Case 3: Simple title property (RFC 7807) or message property
    if (data?.title && data?.status === 400 && (!data?.errors || (typeof data.errors === 'object' && data.errors !== null && Object.keys(data.errors).length === 0))) {
      return data.title;
    }

    // Case 3: Simple message property (prioridade se não houver lista de erros específica)
    if (data?.message) {
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

export function getErrorDetail(error: unknown): string {
  if (error instanceof AxiosError) {
    const data = error.response?.data;

    // Se não houver dados de resposta, retorna string vazia para detalhe
    if (!data) {
      return "";
    }
    // Case 2: ResponseDTO with errors property
    if (data.errors && Array.isArray(data.errors) && data.errors.length > 0) {
      return data.errors.map((e: any) => {
        if (typeof e === 'string') return e;
        if (e.details && Array.isArray(e.details) && e.details.length > 0)
          return e.details.join(", ");
        else
          return "";
      }).join(", ");
    }
  }

  return "";
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
