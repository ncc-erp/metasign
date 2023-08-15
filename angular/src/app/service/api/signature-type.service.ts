import { ApiResponseDto } from "./../model/common.dto";
import { Observable } from "rxjs";
import { Injectable, Injector } from "@angular/core";
import { BaseApiService } from "./base-api.service";
import { SignatureType } from "../model/signature-type.dto";

@Injectable({
  providedIn: "root",
})
export class SignatureTypeService extends BaseApiService {
  changeUrl() {
    return "SignatureType";
  }
  constructor(injector: Injector) {
    super(injector);
  }

  public getAllSignatureTypeService(): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetAll`);
  }
  public createSignatureTypeService(
    signatureType: SignatureType
  ): Observable<ApiResponseDto<any>> {
    return this.processPost("Create", signatureType);
  }

  public getSignatureTypeService(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`Get?id=${id}`);
  }

  public updateSignatureTypeService(
    signatureType: SignatureType
  ): Observable<ApiResponseDto<any>> {
    return this.processPost("Update", signatureType);
  }
}
