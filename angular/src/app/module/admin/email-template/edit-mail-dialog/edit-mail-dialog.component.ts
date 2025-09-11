import { Component, OnInit, Injector } from "@angular/core";
import { IEmailTemplate } from "@app/module/home/home/progress-contract/contract-email-setting/interfaces/email-template-interface";
import { ContractService } from "@app/service/api/contract.service";
import { EmailTemplateService } from "@app/service/api/email-template.service";
import {
  EditEmailDialogData,
  MailPreviewInfo,
  UpdateEmailTemplate,
} from "@app/service/model/admin/emailTemplate.dto";
import { MatSelectChange } from "@node_modules/@angular/material/select";
import { EmailFunc, TemplateType } from "@shared/AppEnums";
import { DialogComponentBase } from "@shared/dialog-component-base";

@Component({
  selector: "app-edit-mail-dialog",
  templateUrl: "./edit-mail-dialog.component.html",
  styleUrls: ["./edit-mail-dialog.component.css"],
})
export class EditMailDialogComponent
  extends DialogComponentBase<EditEmailDialogData>
  implements OnInit
{
  public templateId: number;
  public mailInfo: MailPreviewInfo = {} as MailPreviewInfo;
  public saving: boolean = false;
  public ccS = [];
  public showDialogHeader: boolean = true;
  public cancelDisabled: boolean = false;
  public saveDisabled: boolean = false;
  public temporarySave: boolean;
  public EmailTypes = EmailFunc;
  public showSendMailHeader: boolean = true;
  public isEditTemplate: boolean = true;
  public emailTemplateList: IEmailTemplate[] = [];
  private contractId: number;

  constructor(
    injector: Injector,
    private emailTemplateService: EmailTemplateService,
    private contractService: ContractService
  ) {
    super(injector);
    this.contractId = this.dialogData.contractId;
  }

  ngOnInit(): void {
    Object.assign(this, this.dialogData);
    if (this.dialogData.isEditTemplate) {
      this.getTemplateById();
    }
 }

  getTemplateById() {
    this.emailTemplateService
      .getTemplateById(this.templateId)
      .subscribe((rs) => {
        this.mailInfo = rs.result;
      });
  }

  getContractMailContent() {
    this.contractService
      .GetContractMailContent(this.contractId, this.templateId)
      .subscribe((rs) => {
        if (rs && rs.success) {
          this.mailInfo = rs.result;
        }
      });
  }

  onSelectEmailTemplate(event: MatSelectChange) {
    this.templateId = event.value;
    this.getContractMailContent();
  }

  save() {
    if (this.templateId) {
      this.handleSaveTemplate();
      return;
    } else this.dialogRef.close(this.mailInfo);
  }

  handleSaveTemplate() {
    if (this.templateId) {
      this.mailInfo.bodyMessage = this.mailInfo.bodyMessage.replace(
        /(\r\n|\n|\r)/gm,
        ""
      );

      delete this.mailInfo.arrCCs;
      const updateDto: UpdateEmailTemplate = {
        bodyMessage: this.mailInfo.bodyMessage,
        description: this.mailInfo.description,
        id: this.templateId,
        listCC: this.mailInfo.cCs,
        name: this.mailInfo.name,
        subject: this.mailInfo.subject,
        type: this.mailInfo.type,
        sendToEmail: this.mailInfo.sendToEmail,
      };

      if (this.isEditTemplate) {
        this.emailTemplateService.updateTemplate(updateDto).subscribe((rs) => {
          abp.notify.success("Update successful");
        });
      }
    }
    this.dialogRef.close({
      ...this.mailInfo,
      selectedEmailTemplate: this.templateId,
    });
  }

  isShowHeaderSendMail() {
    return this.mailInfo.templateType == TemplateType.Mail;
  }
}
