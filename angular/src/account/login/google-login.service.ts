import { AppConsts } from './../../shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from "../../app/service/api/base-api.service";
@Injectable({
  providedIn: 'root'
})
export class GoogleLoginService extends BaseApiService {

  changeUrl() {
    return 'TokenAuth';
  }

  constructor(
    injector:Injector
  ) {
    super(injector);
  }

  name() {
    return 'TokenAuth';
  }
  googleAuthenticate(googleToken: string): Observable<any> {
    return this.httpClient.post(AppConsts.remoteServiceBaseUrl +
      '/api/TokenAuth/GoogleAuthenticate', {googleToken: googleToken});
  }
}
