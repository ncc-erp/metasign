import { DialogTemplatePreviewComponent } from './home/progress-contract/upload-contract/dialog-template-preview/dialog-template-preview.component';
import { DialogContractEditorComponent } from './home/progress-contract/upload-contract/dialog-contract-editor/dialog-contract-editor.component';
import { DialogContractTemplateComponent } from './home/progress-contract/upload-contract/dialog-contract-template/dialog-contract-template.component';
import { SharedModule } from "@shared/shared.module";
import { ContractEmailSettingComponent } from "./home/progress-contract/contract-email-setting/contract-email-setting.component";
import { SettingContractComponent } from "./home/progress-contract/setting-contract/setting-contract.component";
import { UploadContractComponent } from "./home/progress-contract/upload-contract/upload-contract.component";
import { ProgressContractComponent } from "./home/progress-contract/progress-contract.component";
import { NgModule } from "@angular/core";
import { HomeRoutingModule } from "./home-routing.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { HomeComponent } from "./home/home.component";
import { SignatureTypeComponent } from "./home/progress-contract/design-contract/signature-type/signature-type.component";
import { DesignContractComponent } from "./home/progress-contract/design-contract/design-contract.component";
import { SendMailResultComponent } from './home/progress-contract/contract-email-setting/send-mail-result/send-mail-result.component';
import { PreviewContractComponent } from './home/progress-contract/upload-contract/preview-contract/preview-contract.component';
@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    SharedModule,
    HomeRoutingModule,
  ],
  declarations: [
    DialogTemplatePreviewComponent,
    DialogContractEditorComponent,
    UploadContractComponent,
    SettingContractComponent,
    ProgressContractComponent,
    ContractEmailSettingComponent,
    DesignContractComponent,
    SignatureTypeComponent,
    HomeComponent,
    SendMailResultComponent,
    DialogContractTemplateComponent,
    PreviewContractComponent
  ],
  exports: [UploadContractComponent, DesignContractComponent,SettingContractComponent],

  providers: [],
})
export class HomeModule { }
