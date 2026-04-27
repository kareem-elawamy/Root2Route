export interface ResponseData<T> {
  succeeded: boolean;
  message: string;
  statusCode: number;
  roles?: string[];
  meta?: {
    totalItems: number;
    totalPages: number;
    currentPage: number;
    pageSize: number;
  };
  errors?: any;
  data: T;
}
