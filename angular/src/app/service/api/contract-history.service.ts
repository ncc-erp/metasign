import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from "./base-api.service";
import { Observable } from 'rxjs';
import { ApiResponseDto } from '../model/common.dto';


@Injectable({
  providedIn: 'root'
})
export class ContractHistoryService  extends BaseApiService {

  changeUrl() {
    return "ContractHistory";
  }

  constructor(injector: Injector) { super(injector); }

  public GetHistoryContractById(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetHistoriesByContractId?contractId=${id}`);
  }

}
