import { Injectable, Injector } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { MailPreviewInfoDto } from "../model/admin/emailTemplate.dto";
import { ApiResponseDto } from "../model/common.dto";
import { ContractImages } from "../model/contract.dto";
import { BaseApiService } from "./base-api.service";
import { ContractStatisticDto } from "../model/contractSetting.dto";

@Injectable({
  providedIn: "root",
})
export class ContractService extends BaseApiService {
  _currentStep = new BehaviorSubject<number>(0);
  _currentStep$ = this._currentStep.asObservable();

  _currentQuickFilter = new BehaviorSubject<number>(0);
  _currentQuickFilter$ = this._currentQuickFilter.asObservable();

  _currentStatus = new BehaviorSubject<number>(-1);
  _currentStatus$ = this._currentStatus.asObservable();

  changeUrl() {
    return "contract";
  }

  constructor(injector: Injector) {
    super(injector);
  }

  public GetAllPagingContract(data): Observable<ApiResponseDto<any>> {
    return this.processPost("GetAllPaging", data);
  }

  public GetContractByFilterPaging(data): Observable<ApiResponseDto<any>> {
    return this.processPost("GetContractByFilterPaging", data);
  }


  public createContractFromTemplate(payload): Observable<ApiResponseDto<any>> {
    return this.processPost(`CreateContractFromTemplate`, payload);
  }

  public GetContractDetail(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetContractDetail?contractId=${id}`);
  }

  public GetContractDesignInfo(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetContractDesginInfo?contractId=${id}`);
  }

  public GetContractFileImage(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetContractFileImage?contractId=${id}`);
  }

  public GetContractFile(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetContractDesginInfo?contractId=${id}`);
  }

  public GetSendMailInfo(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetSendMailInfo?contractId=${id}`);
  }

  public SendMail(data): Observable<ApiResponseDto<any>> {
    return this.processPost(`SendMail`, data);
  }

  public SendMailToViewer(data): Observable<ApiResponseDto<any>> {
    return this.processPost(`SendMailToViewer`, data);
  }

  public GetSignUrl(settingId: number, contractId: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetSignUrl?settingId=${settingId}&contractId=${contractId}`);
  }

  public GetContractMailContent(
    id: number
  ): Observable<ApiResponseDto<MailPreviewInfoDto>> {
    return this.processGet(`GetContractMailContent?contractId=${id}`);
  }

  public GetContractStatistic(): Observable<
    ApiResponseDto<ContractStatisticDto>
  > {
    return this.processGet("GetContractStatistic");
  }

  public SaveDraft(contractId: number): Observable<ApiResponseDto<number>> {
    return this.processPut(`SaveDraft?contractId=${contractId}`, {});
  }


  public ResendMailOne(data): Observable<ApiResponseDto<any>> {
    return this.processPost('ResendMailOne', data);
  }

  public ResendMailAll(contractId: number): Observable<ApiResponseDto<number>> {
    return this.processPost(`ResendMailAll?contractId=${contractId}`, {});
  }

  public CancelContract(
    input: any
  ): Observable<ApiResponseDto<number>> {
    return this.processPost(`CancelContract`, input);
  }

  public GetAllSigners(): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetAllSigners`);
  }
  public ConvertFile(input): Observable<ApiResponseDto<any>> {
    const formData = new FormData()
    formData.append("File", input)
    return this.processPost(`ConvertFile`, formData)
  }

  public downloadContact(id, input): Observable<ApiResponseDto<any>> {
    return this.processGet(`DownloadContractAndCertificate?ContractId=${id}&DownloadType=${input}`)
  }
  public checkSignerDownload(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`CheckContractHasSigned?contractId=${id}`)
  }

  public updateProcessOrder(id: number): Observable<ApiResponseDto<any>> {
    return this.processPut(`UpdateProcessOrder?contractId=${id}`, {})
  }
  public checkHasInput(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`CheckHasInput?contractId=${id}`)
  }
  public removeAllSignature(id: number): Observable<ApiResponseDto<any>> {
    return this.processDelete(`RemoveAllSignature?contractId=${id}`);
  }
  public setNotiExpiredContract(id): Observable<ApiResponseDto<any>>{
    return this.processPost(`SetNotiExpiredContract?contractId=${id}`,{})
  }
}
