import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContractLookupPageComponent } from './contract-lookup-page/contract-lookup-page.component';
import { ContractLookupSiteRoutes } from './contract-lookup-routing.module';
import { SharedModule } from '@shared/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { FormsModule } from '@angular/forms';
import { EmailLoginValidatorComponent } from './email-login-validator/email-login-validator.component';

@NgModule({
  imports: [
    FormsModule,
    CommonModule,
    ContractLookupSiteRoutes,
    SharedModule,
    NgxPaginationModule
  ],
  declarations: [ContractLookupPageComponent,EmailLoginValidatorComponent]
})
export class ContractLookupSiteModule { }
