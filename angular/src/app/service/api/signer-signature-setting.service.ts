import { Injectable, Injector } from "@angular/core";
import { Observable } from "rxjs";
import { ApiResponseDto } from "../model/common.dto";
import { ContractSignerSignatureSettingDto } from "../model/signerSignatureSetting.dto";
import { BaseApiService } from "./base-api.service";

@Injectable({
  providedIn: "root",
})
export class SignerSignatureSettingService extends BaseApiService {
  changeUrl() {
    return "SignerSignatureSetting";
  }

  constructor(injector: Injector) {
    super(injector);
  }
  public getSignatureSettingForContractDesign(
    contractId: number
  ): Observable<ApiResponseDto<ContractSignerSignatureSettingDto>> {
    return this.processGet(
      `GetSignatureSettingForContractDesign?contractId=${contractId}`
    );
  }

  public getSignatureSetting(
    id: number,
    email: string
  ): Observable<ApiResponseDto<ContractSignerSignatureSettingDto>> {
    return this.processGet(
      `GetSignatureSetting?contractSettingId=${id}&email=${email}`
    );
  }

  public createSignerSignatureSetting(
    obj: any
  ): Observable<ApiResponseDto<any>> {
    return this.processPost(`Create`, obj);
  }

  public updateSignerSignatureSetting(
    obj: any
  ): Observable<ApiResponseDto<ContractSignerSignatureSettingDto>> {
    return this.processPut(`Update`, obj);
  }

  public deleteSignerSignatureSetting(
    id: number
  ): Observable<ApiResponseDto<ContractSignerSignatureSettingDto>> {
    return this.processDelete(`Delete?id=${id}`);
  }

  public getMassContractNotSign(
    guid: string
  ): Observable<ApiResponseDto<any>> {
    return this.processPost(`GetMassContractNotSign?massGuid=${guid}`);
  }
 
}
