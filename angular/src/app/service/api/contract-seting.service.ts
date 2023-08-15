import { ContractSettingDto, ContractSigners } from './../model/contractSetting.dto';
import { Observable } from 'rxjs';
import { Injectable, Injector } from "@angular/core";
import { BaseApiService } from './base-api.service';
import { ApiResponseDto } from '../model/common.dto';

@Injectable({
  providedIn: 'root'
})
export class ContractSettingService extends BaseApiService {
  changeUrl() {
    return "ContractSetting";
  }



  constructor(injector: Injector) {
    super(injector);
  }

  public GetSettingByContractId(contractId: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetSettingByContractId?contractId=${contractId}`)
  }

  public voidOrDeclineToSignDto(payload): Observable<ApiResponseDto<any>> 
  {
    return this.processPost(`VoidOrDeclineToSignDto`,payload)

  }

}
