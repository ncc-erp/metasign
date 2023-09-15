import { ContractDialogMailComponent } from './contract-templates-management/contract-dialog-mail/contract-dialog-mail.component';
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ContractTemplatesRoutes } from "./contract-templates-routing.module";
import { SharedModule } from "@shared/shared.module";
import { MaterialModule } from "@shared/material.module";
import { TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import { Routes, RouterModule } from "@angular/router";
import { ContractTemplateDialogComponent } from "./contract-templates-management/contract-template-dialog/contract-template-dialog.component";
import { ContractTemplatesComponent } from './contract-templates-management/contract-templates.component';
import { NgxPaginationModule } from "ngx-pagination";
import { ContractTemplateListComponent } from "./contract-templates-management/contract-template-list/contract-template-list.component";
import { HomeModule } from '../home/home.module';



@NgModule({
  declarations: [
    ContractTemplateDialogComponent,
    ContractTemplatesComponent,
    ContractTemplateListComponent,
    ContractDialogMailComponent
  ],
  imports: [ContractTemplatesRoutes, CommonModule, SharedModule , NgxPaginationModule, HomeModule],
})
export class ContractTemplatesModule {}
