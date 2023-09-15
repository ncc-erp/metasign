import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponseDto } from '../model/common.dto';
import { BaseApiService } from "./base-api.service";

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService extends BaseApiService {
  changeUrl() {
    return "Configuration"
  }
  constructor(injector: Injector) {
    super(injector)
  }

  public getEmailSetting(): Observable<ApiResponseDto<any>> {
    return this.processGet('GetEmailSetting')
  }

  public setEmailSetting(data): Observable<ApiResponseDto<any>> {
    return this.processPost('SetEmailSetting', data)
  }

  public getGoogleClientId(): Observable<ApiResponseDto<any>> {
    return this.processGet('GetGoogleClientId')
  }

  public setGoogleClientId(data): Observable<ApiResponseDto<any>> {
    return this.processPost('SetGoogleClientId', data)
  }

  public getCurrentPdfSignerName(): Observable<ApiResponseDto<any>> {
    return this.processGet('GetCurrentPdfSignerName')
  }


  public getIsEnableloginByUsername(): Observable<ApiResponseDto<any>> {
    return this.processGet('GetIsEnableloginByUsername')
  }

  public setIsEnableloginByUsername(data): Observable<ApiResponseDto<any>> {
    return this.processPost('SetIsEnableloginByUsername', data)
  }

  public getNotiExprireTime(): Observable<ApiResponseDto<any>> {
    return this.processGet('GetNotiExprireTime')
  }

  public setNotiExprireTime(data): Observable<ApiResponseDto<any>> {
    return this.processPost('SetNotiExprireTime', data)
  }

  public getAWSCredential(): Observable<ApiResponseDto<any>> {
    return this.processGet('GetAWSCredential')
  }

  public setAWSCredential(data): Observable<ApiResponseDto<any>> {
    return this.processPost('SetAWSCredential', data)
  }

  public getSignServerUrlDto(): Observable<ApiResponseDto<any>> {
    return this.processGet('GetSignServerUrlDto')
  }
  public setSignServerUrlDto(data): Observable<ApiResponseDto<any>> {
    return this.processPost('SetSignServerUrlDto', data)
  }

  public getMicrosoftClientId(): Observable<ApiResponseDto<any>> {
    return this.processGet('GetMicrosoftClientId')
  }

  public setMicrosoftClientId(data): Observable<ApiResponseDto<any>> {
    return this.processPost('SetMicrosoftClientId', data)
  }


}
