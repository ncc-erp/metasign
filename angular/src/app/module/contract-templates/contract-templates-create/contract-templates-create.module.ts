import { ContractTemplatesSettingComponent } from './contract-templates-progress/contract-templates-setting/contract-templates-setting.component';
import { ContractTemplatesDesignComponent } from "./contract-templates-progress/contract-templates-design/contract-templates-design.component";
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ContractTemplatesCreateRoutes } from "./contract-templates-create-routing.module";
import { ContractTemplatesUploadComponent } from "./contract-templates-progress/contract-templates-upload/contract-templates-upload.component";
import { ContractTemplatesProgressComponent } from "./contract-templates-progress/contract-templates-progress.component";
import { SharedModule } from "@shared/shared.module";
import { UploadContractComponent } from "@app/module/home/home/progress-contract/upload-contract/upload-contract.component";
import { HomeModule } from "@app/module/home/home.module";

@NgModule({
  imports: [CommonModule, ContractTemplatesCreateRoutes, SharedModule, HomeModule],
  declarations: [
    ContractTemplatesUploadComponent,
    ContractTemplatesProgressComponent,
    ContractTemplatesDesignComponent,
    ContractTemplatesSettingComponent
      
  ],
})
export class ContractTemplatesCreateModule {}
