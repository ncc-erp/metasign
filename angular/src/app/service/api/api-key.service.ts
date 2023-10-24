import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from "./base-api.service";
import { Observable } from 'rxjs';
import { ApiResponseDto } from '../model/common.dto';

@Injectable({
  providedIn: 'root'
})
export class ApiKeyService extends BaseApiService{
  changeUrl() {
    return "ApiKey"
  }
  constructor(injector: Injector) {
    super(injector)
  }

  public GenerateApiKey(): Observable<ApiResponseDto<any>> {
    return this.processPost('GenerateApiKey',{})
  }

  public GetApiKey(): Observable<ApiResponseDto<any>> {
    return this.processGet('GetApiKey')
  }
}
