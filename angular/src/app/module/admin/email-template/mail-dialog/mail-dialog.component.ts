import { Injector } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { EmailTemplateService } from '@app/service/api/email-template.service';
import { EditEmailDialogData, MailPreviewInfo } from '@app/service/model/admin/emailTemplate.dto';
import { EmailFunc } from '@shared/AppEnums';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { EditMailDialogComponent } from '../edit-mail-dialog/edit-mail-dialog.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

@Component({
  selector: 'app-mail-dialog',
  templateUrl: './mail-dialog.component.html',
  styleUrls: ['./mail-dialog.component.css']
})
export class MailDialogComponent extends DialogComponentBase<any> implements OnInit {
  public mailInfo = new MailPreviewInfo();
  public saving: boolean = false;
  public content: SafeHtml = '';
  public cancelDisabled: boolean = false;
  public saveDisabled: boolean = false;
  public showEditButton: boolean = false;
  public showDialogHeader: boolean = true;
  public showSendMailButton: boolean = true;
  public showSendMailHeader: boolean = true;
  public templateId: number;
  public EmailTypes = EmailFunc;
  constructor(injector: Injector, public sanitizer: DomSanitizer,
    private dialog: MatDialog,
    private emailTemplateService: EmailTemplateService) {
    super(injector)
  }

  ngOnInit(): void {
    this.showSendMailButton = this.dialogData.showSendMailButton;
    this.showSendMailHeader = this.dialogData.showSendMailHeader;
    Object.assign(this, this.dialogData)
    this.templateId = this.dialogData.templateId
    if (this.templateId) {
      this.getFakeData()
    }
  }


  editTemplate() {
    const dialogData: EditEmailDialogData = {
      mailInfo: { ...this.mailInfo },
      showDialogHeader: false,
      temporarySave: true,
    }
    const editDialog = this.dialog.open(EditMailDialogComponent, {
      data: dialogData,
      width: '1600px',
      panelClass: 'email-dialog'
    })

    editDialog.afterClosed().subscribe(rs => {
      if (rs) {
        this.mailInfo = { ...rs }
      }
    })
  }

  getFakeData() {
      this.emailTemplateService.previewTemplate(this.templateId).subscribe(rs => {
        this.mailInfo = rs.result
      })
  }

  sendMail() {
    if (!this.mailInfo.sendToEmail) {
      abp.message.error("Invalid email address!")
      return;
    }
    this.dialogRef.close(this.mailInfo)
  }

  isShowPreviewBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_EmailTemplate_PreviewTemplate_SendMail)
  }


}
