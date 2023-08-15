import { ContractSigningService } from '@app/service/api/contract-signing.service';
import { CertificateDetailDto } from './../../../../service/model/certificateDetail.dto';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Component, OnInit, Inject } from '@angular/core';
import { ContractPublicService } from '@app/service/api/contract-public.service';
import * as FileSaver from 'file-saver';
import { DesktopAppServiceService } from '@app/service/api/desktop-app-service.service';

@Component({
  selector: 'app-cert-detail-dialog',
  templateUrl: './cert-detail-dialog.component.html',
  styleUrls: ['./cert-detail-dialog.component.css']
})
export class CertDetailDialogComponent implements OnInit {
  certificate = {} as CertificateDetailDto
  constructor( @Inject(MAT_DIALOG_DATA) public data: any,
  private dialogRef: MatDialogRef<CertDetailDialogComponent>,
  private contractPublicService:ContractPublicService,
  private desktopAppServiceService: DesktopAppServiceService
  ) { }

  ngOnInit(): void {
    this.certificate = this.data;
  }
  
  converFile(s) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
    return buf;
  }
   downloadFileExe(fileByte, fileName:string) {
    const file = new Blob([this.converFile(atob(fileByte))], {
      type: ""
    });
    FileSaver.saveAs(file, fileName);
    this.data.isDownloadApp = false;
  }

  handleDownloadFileExe()
  {
    this.dialogRef.close();
    this.contractPublicService.downloadApp().subscribe((rs)=>{
        this.downloadFileExe(rs.result,'metasign.exe');
    })
  }

  getCertificate(){
    this.desktopAppServiceService.getCertificate().subscribe(result=>{
      if(result.Status !== 0)
      {
        let certDetailInfo =
          {
            certSerial: result.CertDetailInfo.CertSerial,
            organizationCA: result.CertDetailInfo.OrganizationCA,
            ownCA: result.CertDetailInfo.OwnCA,
            uid: result.CertDetailInfo.Uid,
            beginDateCA: result.CertDetailInfo.BeginDateCA,
            endDateCA: result.CertDetailInfo.EndDateCA,
          };
          this.certificate = certDetailInfo;
      }

    })
  }
  onConfirm(){
    this.dialogRef.close(this.certificate)
  }
  handleClose()
  {
    this.dialogRef.close();
  }
}
