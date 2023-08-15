import { SharedModule } from './../../../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminRoutingModule } from './admin-routing.module';
import { EmailTemplateComponent } from './email-template/email-template.component';
import { MailDialogComponent } from './email-template/mail-dialog/mail-dialog.component';
import { EditMailDialogComponent } from './email-template/edit-mail-dialog/edit-mail-dialog.component';
import { ConfigurationComponent } from './configuration/configuration.component';
import { SignServerComponent } from './sign-server/sign-server.component';
import { MaterialModule } from '@shared/material.module';
import { NgxPaginationModule } from 'ngx-pagination';


import { ReactiveFormsModule } from "@angular/forms";
import { PropertiesWorkerComponent } from './sign-server/properties-worker/properties-worker.component';
import { CreatePropertiesComponent } from './sign-server/properties-worker/create-properties/create-properties.component';
import { CreateWorkerComponent } from './sign-server/create-worker/create-worker.component';
import { SignServerDataService } from './sign-server/services/sign-server.service';

@NgModule({
  declarations: [
    EmailTemplateComponent,
    MailDialogComponent,
    EditMailDialogComponent,
    ConfigurationComponent,
    SignServerComponent,
    PropertiesWorkerComponent,
    CreatePropertiesComponent,
    CreateWorkerComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AdminRoutingModule,
    SharedModule,
    MaterialModule,
    NgxPaginationModule
  ],
  providers: [SignServerDataService]
})
export class AdminModule { }
