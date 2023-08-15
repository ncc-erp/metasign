import { Injectable, Injector } from "@angular/core";
import { BaseApiService } from "./base-api.service";
import { ApiResponseDto } from "../model/common.dto";
import { Observable } from "rxjs";
import { ContractSignerSignatureSettingDto } from "../model/signerSignatureSetting.dto";
import { ContractSigners } from "../model/contractSetting.dto";
@Injectable({
  providedIn: "root",
})
export class ContractTemplateSignerService extends BaseApiService {
  changeUrl() {
    return "ContractTemplateSigner";
  }

  constructor(injector: Injector) {
    super(injector);
  }

  public createContractTemplateSigner(
    payload: any
  ): Observable<ApiResponseDto<any>> {
    return this.processPost(`Create`, payload);
  }

  public getAllByContractTemplateId(id): Observable<ApiResponseDto<ContractSigners>>
  {
    return this.processGet(
      `GetAllByContractTemplateId?id=${id}`
    );
  }
  
  public updateContractTemplateSigner(
    obj: any
  ): Observable<ApiResponseDto<any>> {
    return this.processPut(`Update`, obj);
  }

  public deteleContractTemplateSigner(
    id: number
  ): Observable<ApiResponseDto<any>> {
    return this.processDelete(`Delete?id=${id}`);
  }



}
