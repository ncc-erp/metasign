import { Observable } from "rxjs";
import { BaseApiService } from "./base-api.service";
import { Injectable, Injector } from '@angular/core';
import { ApiResponseDto } from "../model/common.dto";

@Injectable()

export class ContractPublicService extends BaseApiService {
  changeUrl() {
    return "Public";
  }
constructor(injector: Injector) { super(injector);  }

public downloadApp(): Observable<ApiResponseDto<any>> {
  return this.processGet(`DownloadApp`);
}
}
