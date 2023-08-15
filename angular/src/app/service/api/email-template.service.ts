
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { MailPreviewInfo, MailPreviewInfoDto, UpdateEmailTemplate } from '../model/admin/emailTemplate.dto';
import { ApiResponseDto } from '../model/common.dto';
import { BaseApiService } from "./base-api.service";

@Injectable({
  providedIn: 'root'
})
export class EmailTemplateService extends BaseApiService {

  changeUrl() {
    return "EmailTemplate"
  }

  constructor(injector: Injector) {
    super(injector);
  }

  public previewTemplate(templateId: number): Observable<ApiResponseDto<MailPreviewInfo>> {
    return this.processGet(`PreviewTemplate?id=${templateId}`);
  }

  public getTemplateById(id: number): Observable<ApiResponseDto<MailPreviewInfo>> {
    return this.processGet(`GetTemplateById?id=${id}`);
  }

  public updateTemplate(input: UpdateEmailTemplate): Observable<ApiResponseDto<MailPreviewInfoDto>> {
    return this.processPut(`UpdateTemplate`, input);
  }

  public sendMail(input: MailPreviewInfoDto): Observable<ApiResponseDto<MailPreviewInfoDto>> {
    return this.processPost(`SendMail`, input);
  }

}
