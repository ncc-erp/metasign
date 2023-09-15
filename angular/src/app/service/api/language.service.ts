import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from "./base-api.service";
import { Observable } from 'rxjs';
import { ApiResponseDto } from '../model/common.dto';

@Injectable({
    providedIn: "root",
  })

export class LangService extends BaseApiService {
  changeUrl() {
    return "Language"
  }
  constructor(injector: Injector) {
    super(injector)
  }

  public GetCurrentUserLanguage(baseUrl:string, payload?: string): Observable<ApiResponseDto<any>> {
    if(!payload) {
      return this.httpClient.get<any>(baseUrl + `/api/services/app/Language/GetCurrentUserLanguage?currentUserLanguage=`)
    }
    return this.httpClient.get<any>(baseUrl + `/api/services/app/Language/GetCurrentUserLanguage?currentUserLanguage=${payload}`)
  }
}
