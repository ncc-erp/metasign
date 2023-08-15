import { SignatureDialogStampComponent } from './un-authen-signing/signature-dialog-stamp/signature-dialog-stamp.component';
import { SigningRejectComponent } from './signing-reject/signing-reject.component';
import { ContractDialogConfirmComponent } from './un-authen-signing/contract-dialog-confirm/contract-dialog-confirm.component';
import { DesktopAppServiceService } from './../../service/api/desktop-app-service.service';
import { ContractPublicService } from './../../service/api/contract-public.service';
import { SignatureResultDialogComponent } from './un-authen-signing/signature-result-dialog/signature-result-dialog.component';
import { SignatureDialogComponent } from './un-authen-signing/signature-dialog/signature-dialog.component';
import { SharedModule } from '../../../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UnAuthenSigningComponent } from './un-authen-signing/un-authen-signing.component';
import { MaterialModule } from "../../../shared/material.module";
import { UnAuthenPageRoutingModule } from './unauthen-page-routing.module';
import { SignatureComponent } from './un-authen-signing/signature/signature.component';
import { FormsModule } from '@angular/forms';
import { SigningResultComponent } from './signing-result/signing-result.component';
import { ImageCropperModule } from 'ngx-image-cropper';
import { EmailValidComponent } from './email-valid/email-valid.component';
import { CertDetailDialogComponent } from './un-authen-signing/cert-detail-dialog/cert-detail-dialog.component';
import { ContractInvalidateDialogComponent } from './un-authen-signing/contract-invalidate-dialog/contract-invalidate-dialog.component';



@NgModule({
  declarations: [
    UnAuthenSigningComponent,
    SignatureComponent,
    SignatureDialogComponent,
    SignatureResultDialogComponent,
    EmailValidComponent,
    SigningResultComponent,
    CertDetailDialogComponent,
    ContractInvalidateDialogComponent,
    ContractDialogConfirmComponent,
    SigningRejectComponent,
    SignatureDialogStampComponent

  ],
  imports: [
    ImageCropperModule,
    FormsModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    UnAuthenPageRoutingModule,
  ],
  providers: [
    ContractPublicService,
    DesktopAppServiceService
  ],
})
export class UnauthenPagesModule { }
