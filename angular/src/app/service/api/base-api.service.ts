import { AppConsts } from '../../../shared/AppConsts';

import { catchError } from 'rxjs/operators';
import { AbpSessionService } from 'abp-ng2-module';
import { PagedRequestDto, PagedResultDto } from '../../../shared/paged-listing-component-base';

import { Injectable, Injector } from '@angular/core';
import { throwError, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment';
import { ApiResponseDto } from '../model/common.dto';
import { EcTranslatePipe } from '@shared/pipes/ecTranslate.pipe';


@Injectable({
  providedIn: 'root'
})
export abstract class BaseApiService {

  protected baseUrl = AppConsts.remoteServiceBaseUrl;
  protected httpClient: HttpClient
  public abpsession: AbpSessionService
  protected currentLoginUserId
  private ecTranslate : EcTranslatePipe

  protected get rootUrl() {
    return this.baseUrl + '/api/services/app/' + this.changeUrl() + "/";
  }



  constructor(injector: Injector) {
    this.httpClient = injector.get(HttpClient)
    this.abpsession = injector.get(AbpSessionService)
    this.ecTranslate =  injector.get(EcTranslatePipe)
    this.currentLoginUserId = this.abpsession.userId;
  }   

  abstract changeUrl();


  public handleError(error: any, api: string, payload?: any) {
    if (error?.error?.error?.message) {
      abp.message.error(`${this.ecTranslate.transform(error.error.error.message)}`)
    }
    else {
      abp.message.error(error)
    }
    let errMessage = {
      userId: this.currentLoginUserId,
      message: error,
      api: api,
      payload: payload ? JSON.stringify(payload) : 'no payload'
    } as ErrorMessageDto
    if (typeof window['postSentryLog'] == 'function') {
      window['postSentryLog'](moment(new Date()).format("YYYY/MM/DD") + `call api error`, errMessage)
    }
    return throwError(error);
  }

  public processGet(apiString: string): Observable<ApiResponseDto<any>> {
    return this.httpClient.get<any>(this.rootUrl + apiString)
      .pipe(catchError(err => {
        this.handleError(err, apiString);
        throw err
      }));
  }

  protected processGetAllPaging(apiString: string, payload: PagedRequestDto): Observable<ApiResponseDto<PagedResultDto>> {
    return this.httpClient.post<any>(this.rootUrl + apiString, payload)
      .pipe(catchError(err => {
        this.handleError(err, apiString, payload);
        throw err
      }));
  }

  public processPost(apiString: string, payload?: any): Observable<ApiResponseDto<any>> {
    return this.httpClient.post<any>(this.rootUrl + apiString, payload)
      .pipe(catchError(err => {
        this.handleError(err, apiString, payload);
        throw err
      }));
  }

  public processPut(apiString: string, payload: any): Observable<ApiResponseDto<any>> {
    return this.httpClient.put<any>(this.rootUrl + apiString, payload)
      .pipe(catchError(err => {
        this.handleError(err, apiString, payload);
        throw err
      }));
  }

  public processDelete(apiString: string,): Observable<ApiResponseDto<any>> {
    return this.httpClient.delete<any>(this.rootUrl + apiString)
      .pipe(catchError(err => {
        this.handleError(err, apiString);
        throw err
      }));
  }

  public getAll(): Observable<ApiResponseDto<any>> {
    return this.processGet("GetAll")
  }

  public get(id: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`Get?id=${id}`)
  }

  public getAllPagging(payload: PagedRequestDto): Observable<ApiResponseDto<PagedResultDto>> {
    return this.processGetAllPaging("GetAllPaging", payload)
  }

  public getByEmployeeId(employeeId: number, payload: PagedRequestDto): Observable<ApiResponseDto<PagedResultDto>> {
    return this.processGetAllPaging(`GetByEmployeeId?employeeId=${employeeId}`, payload)
  }

  public create(data: any): Observable<ApiResponseDto<any>> {
    return this.processPost(`Create`, data);
  }

  public update(data: any): Observable<ApiResponseDto<any>> {
    return this.processPut(`Update`, data);
  }

  public delete(id: number): Observable<ApiResponseDto<any>> {

    return this.processDelete(`Delete?id=${id}`);
  }
}
export interface ErrorMessageDto {
  userId: any;
  api: string;
  payload: any;
  message: string;
}
