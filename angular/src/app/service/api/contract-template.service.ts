import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Injector } from "@angular/core";
import { BaseApiService } from "./base-api.service";
import { ApiResponseDto } from "../model/common.dto";

@Injectable({
  providedIn: "root",
})
export class ContractTemplateService extends BaseApiService {
  changeUrl() {
    return "ContractTemplate";
  }

  constructor(injector: Injector) {
    super(injector);
  }

  public createFileTemplate(payload: any): Observable<ApiResponseDto<any>> {
    return this.processPost(`Create`, payload);
  }

  public updateFileTemplate(payload: any): Observable<ApiResponseDto<any>> {
    return this.processPut(`Update`, payload);
  }

  public getAllContractTemplate(): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetAll`);
  }

  public getAllPagingContractTemplate(payload): Observable<ApiResponseDto<any>> {
    return this.processGetAllPaging(`GetAllPaging`, payload);
  }

  public getContractTemplate(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`Get?id=${id}`);
  }

  public deleteFileTemplate(id: number): Observable<ApiResponseDto<any>> {
    return this.processDelete(`Delete?id=${id}`);
  }

  public updateProcessOrder(id: number): Observable<ApiResponseDto<any>> {
    return this.processPut(`UpdateProcessOrder?contractTemplateId=${id}`, {})
  }
  public checkHasInput(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`CheckHasInput?contractTemplateId=${id}`)
  }
  public removeAllSignature(id: number): Observable<ApiResponseDto<any>> {
    return this.processDelete(`RemoveAllSignature?contractTemplateId=${id}`);
  }

  public downloadMassTemplate(templateId: number): Observable<ApiResponseDto<any>>{
    return this.processPost(`DownloadMassTemplate?templateId=${templateId}`, null);
  }

  public validImportMassTemplate(input): Observable<ApiResponseDto<any>>
  {
    return this.processPost(`ValidImportMassTemplate`, input);
  }
}
