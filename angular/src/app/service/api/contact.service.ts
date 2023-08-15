import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from "./base-api.service";
import { Observable } from 'rxjs';
import { ApiResponseDto } from '../model/common.dto';


@Injectable({
  providedIn: 'root'
})
export class ContactService extends BaseApiService {
  changeUrl() {
    return "Contact"
  }
  constructor(injector: Injector) {
    super(injector)
  }

  public getAll(): Observable<ApiResponseDto<any>> {
    return this.processGet('GetAll')
  }

  public getContactAllPaging(data): Observable<ApiResponseDto<any>> {
    return this.processPost('GetAllPaging', data)
  }

  public createContact(data): Observable<ApiResponseDto<any>> {
    return this.processPost('CreateContact', data)
  }

  public updateContact(data): Observable<ApiResponseDto<any>> {
    return this.processPost('Update', data)
  }

  public deleteContact(id): Observable<ApiResponseDto<any>> {
    return this.processDelete(`Delete?id=${id}`)
  }
}


