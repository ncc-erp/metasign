import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from "./base-api.service";
import { Observable } from 'rxjs';
import { ApiResponseDto } from '../model/common.dto';


@Injectable({
  providedIn: 'root'
})
export class SignServerService extends BaseApiService {

  changeUrl() {
    return "SignServer";
  }

  constructor(injector: Injector) {
    super(injector)
  }

  public GetAllPdfSigners(): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetAllWorkers`)
  }

  public AddPdfSigners(type: string): Observable<ApiResponseDto<any>> {
    return this.processPost(`AddWorker?implementationClass=${type}`)
  }

  public GetWorkerProperties(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetWorkerPropertiesById?workerId=${id}`)
  }

  public ConfigWorker(data: any): Observable<ApiResponseDto<any>> {
    return this.processPost(`ConfigWorker`, data)
  }

  public GetPropertiesPermissionList(type: string): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetPropertiesPermissionList?implementationClass=${type}`)
  }

  public GetSignerCertificateInfo(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetSignerCertificateInfo?workerId=${id}`)
  }
}
