import { ContractSignerSignatureSettingDto } from "./../model/signerSignatureSetting.dto";
import { ApiResponseDto } from "./../model/common.dto";
import { Injectable, Injector } from "@angular/core";
import { Observable } from "rxjs";
import { BaseApiService } from "./base-api.service";

@Injectable({
  providedIn: "root",
})
export class ContractTemplateSettingService extends BaseApiService {
  changeUrl() {
    return "ContractTemplateSetting";
  }

  constructor(injector: Injector) {
    super(injector);
  }

  public getContractTemplateSetting(
    contractId: number
  ): Observable<ApiResponseDto<ContractSignerSignatureSettingDto>> {
    return this.processGet(
      `GetSignatureSettingForContractDesign?contractId=${contractId}`
    );
  }

  public createContractTemplateSetting(
    payload: any
  ): Observable<ApiResponseDto<any>> {
    return this.processPost(`Create`, payload);
  }

  public updateContractTemplateSetting(
    obj: any
  ): Observable<ApiResponseDto<ContractSignerSignatureSettingDto>> {
    return this.processPut(`Update`, obj);
  }

  public deleteContractTemplateSetting(
    id: number
  ): Observable<ApiResponseDto<ContractSignerSignatureSettingDto>> {
    return this.processDelete(`Delete?id=${id}`);
  }

  public getAllContractTemplateSetting(
    contractId: number
  ): Observable<ApiResponseDto<ContractSignerSignatureSettingDto>> {
    return this.processGet(
      `GetAllSignLocation?contractTemplateId=${contractId}`
    );
  }
}
