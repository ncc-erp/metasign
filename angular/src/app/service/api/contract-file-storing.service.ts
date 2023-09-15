import { Injectable, Injector } from "@angular/core";
import { BaseApiService } from "./base-api.service";
import { Observable } from "rxjs";
import { ApiResponseDto } from "../model/common.dto";
@Injectable({
  providedIn: "root",
})
export class ContractFileStoringService extends BaseApiService {
  changeUrl() {
    return "FileStoring";
  }
  constructor(injector: Injector) {
    super(injector);
  }
  public getPresignedDownloadUrl(
    contractId,
    type
  ): Observable<ApiResponseDto<any>> {
    return this.processGet(
      `GetPresignedDownloadUrl?ContractId=${contractId}&DownloadType=${type}`
    );
  }

  public clearContractDownloadFiles(
    contractId
  ): Observable<ApiResponseDto<any>> {
    return this.processDelete(
      `ClearContractDownloadFiles?ContractId=${contractId}`
    );
  }
}
