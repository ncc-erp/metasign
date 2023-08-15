import { HttpErrorResponse } from "@angular/common/http";

export class ApiResponseDto<T>{
    result?: T;
    targetUrl?: string;
    success: boolean;
    error: HttpErrorResponse;
    unAuthorizedRequest?: boolean;
    loading: boolean;


}
