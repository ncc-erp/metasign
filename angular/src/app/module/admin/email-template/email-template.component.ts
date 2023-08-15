import { EditMailDialogComponent } from './edit-mail-dialog/edit-mail-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { EmailDto, MailDialogData, MailPreviewInfoDto } from './../../../service/model/admin/emailTemplate.dto';
import { EmailTemplateService } from './../../../service/api/email-template.service';
import { Component, Injector, OnInit } from '@angular/core';
import { MailDialogComponent } from './mail-dialog/mail-dialog.component';
import { AppComponentBase } from '@shared/app-component-base';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

@Component({
  selector: 'app-email-template',
  templateUrl: './email-template.component.html',
  styleUrls: ['./email-template.component.css']
})
export class EmailTemplateComponent extends AppComponentBase implements OnInit {
  public templates: EmailDto[] = []
  public showSendMailHeader: boolean = true;

  constructor(injector: Injector,
    private mailTemplateService: EmailTemplateService,
    private dialog: MatDialog) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAll()
  }

  getAll() {
    this.mailTemplateService.getAll().subscribe(rs => {
      this.templates = rs.result
    })
  }

  preview(mailData: EmailDto) {
    const previewDialogData: MailDialogData = {
      templateId: mailData.id,
      title: `${this.ecTransform('Preview')} ${mailData.name}`,
    }
    const dialogRef = this.dialog.open(MailDialogComponent, {
      data: previewDialogData,
      width: '1600px',
      panelClass: 'email-dialog',
    })
    dialogRef.afterClosed().subscribe((mailInfo: MailPreviewInfoDto) => {
      if (!mailInfo) return;

      this.mailTemplateService.sendMail(mailInfo).subscribe(res => {
        if (res?.success) {
          abp.message.success(this.ecTransform('SendMailSuccessful'))
        }
      });
    });
  }

  edit(mailData: EmailDto) {
    const dialogRef = this.dialog.open(EditMailDialogComponent, {
      data: {
        templateId: mailData.id,
        title: `${this.ecTransform('Edit')} ${mailData.name}`,
        showSendMailHeader: this.showSendMailHeader,
        isEditTemplate: true
      },
      width: '85%',
      maxWidth: '85%',
      panelClass: 'email-dialog',
    })
    dialogRef.afterClosed().subscribe(rs => {
      this.getAll();
    })
  }

  isShowEditbtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_EmailTemplate_Edit)
  }

  isShowPreviewBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_EmailTemplate_PreviewTemplate)
  }
}
