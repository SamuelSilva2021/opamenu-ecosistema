import { AlertCircle } from "lucide-react";
import { getErrorDetail, getErrorMessage } from "@/lib/utils";
import type { ErrorDTO } from "../types";

interface AuthErrorAlertProps {
  error: ErrorDTO;
}

export function AuthErrorAlert({ error }: AuthErrorAlertProps) {
  if (!error) return null;

  const message = getErrorMessage(error);
  const detail = getErrorDetail(error);
  
  return (
    <div className="rounded-md bg-red-50 p-4 border border-red-200 dark:bg-red-900/20 dark:border-red-900/50 animate-in fade-in slide-in-from-top-1">
        <div className="flex">
          <div className="shrink-0">
            <AlertCircle className="h-5 w-5 text-red-400" aria-hidden="true" />
          </div>
          <div className="ml-3">
            <h3 className="text-sm font-medium text-red-800 dark:text-red-200">{message}</h3>
            <div className="mt-2 text-sm text-red-700 dark:text-red-300">
              {detail && <p className="mt-1">{detail}</p>}
            </div>
          </div>
        </div>
      </div>
  );
}
