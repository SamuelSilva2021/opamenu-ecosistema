export interface ApiResponse<T> {
  succeeded: boolean;
  message: string | null;
  errors: any[] | null;
  data: T;
}
