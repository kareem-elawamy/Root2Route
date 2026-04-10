export interface ResponseData<T> {
  succeeded: boolean;
  message: string;
  statusCode: number;
  meta?: {
    totalItems: number;
    totalPages: number;
    currentPage: number;
    pageSize: number;
  };
  errors?: any;
  data: T;
}
