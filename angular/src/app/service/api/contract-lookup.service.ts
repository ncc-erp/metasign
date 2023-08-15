import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from "./base-api.service";
import { Observable } from 'rxjs';
import { ApiResponseDto } from '../model/common.dto';

@Injectable({
  providedIn: 'root'
})
export class ContractLookupService extends BaseApiService {

  constructor(injector: Injector) {
    super(injector);
  }

    changeUrl() {
        return "LookupPage";
    }

  public getContractDetailByGuid(payload): Observable<ApiResponseDto<any>> {
    return this.processPost(`GetContractDetailByGuid`,payload);
  }


}
