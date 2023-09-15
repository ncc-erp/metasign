import { ApiResponseDto } from "./../model/common.dto";
import { Observable } from "rxjs";
import { Injectable, Injector } from "@angular/core";
import { BaseApiService } from "./base-api.service";

@Injectable({
  providedIn: "root",
})
export class SignatureUserService extends BaseApiService {
  changeUrl() {
    return "SignatureUser";
  }
  constructor(injector: Injector) {
    super(injector);
  }

  public createSignatureUserService(
    payload: any
  ): Observable<ApiResponseDto<any>> {
    return this.processPost(`Create`, payload);
  }

  public getSignatureUserAllService(): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetAll`);
  }

  public getSignatureUserService(id): Observable<ApiResponseDto<any>> {
    return this.processGet(`Get?id=${id}`);
  }

  public updateSignatureUserService(
    payload: any
  ): Observable<ApiResponseDto<any>> {
    return this.processPut(`Update`, payload);
  }

  public deleteSignerSignatureSetting(
    id: number
  ): Observable<ApiResponseDto<any>> {
    return this.processDelete(`Delete?id=${id}`);
  }

  public setDefaultSignature(payload: any): Observable<ApiResponseDto<any>> {
    return this.processPost(`SetDefaultSignature`, payload)
  }

  public GetAllByEmail(id:number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetAllByEmail?settingId=${id}`);
  }
}
