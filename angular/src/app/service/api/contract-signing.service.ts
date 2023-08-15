import { Observable } from 'rxjs';
import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { ApiResponseDto } from '../model/common.dto';
import { SignDto, SignMultipleDto } from '../model/sign.dto';
import { SignInputDto } from '../model/design-contract.dto';

@Injectable({
  providedIn: 'root'
})
export class ContractSigningService extends BaseApiService {

  changeUrl() {
    return "ContractSigning";
  }



  constructor(injector: Injector) {
    super(injector);
  }


  public SignInput(payload:SignInputDto): Observable<ApiResponseDto<any>> {
    return this.processPost(`SignInput`,payload)
  }

  public SignProcess(data:SignDto): Observable<ApiResponseDto<any>>{
    return this.processPost(`SignProcess`, data);
  }

  public signMultiple(data:SignMultipleDto): Observable<ApiResponseDto<string>>{
    return this.processPost(`SignMultiple`, data);
  }

  public ValidEmail(data): Observable<ApiResponseDto<boolean>>{
    return this.processPost(`ValidEmail`, data);
  }

  public ValidContract(contractId:number): Observable<ApiResponseDto<any>>{
    return this.processGet(`ValidContract?contractId=${contractId}`);
  }

  public getSignatureDigitalBase64(): Observable<ApiResponseDto<any>>{
    return this.processGet(`GetSignatureBase64`);
  }

  public insertSigningResult(data):Observable<ApiResponseDto<any>>
  {
    return this.processPost(`InsertSigningResult`,data);
  }

  public insertSigningResultAndComplete(data):Observable<ApiResponseDto<any>>
  {
    return this.processPost(`InsertSigningResultAndComplete`,data);
  }

}
