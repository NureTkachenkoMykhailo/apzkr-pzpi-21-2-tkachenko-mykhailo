export interface ResponseObject<T> {
  statusCode?: number;
  isSuccessfulResult: boolean;
  errors: any[];
  payload: T;
}
