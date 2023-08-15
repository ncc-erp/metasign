import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContractLookupPageComponent } from './contract-lookup-page/contract-lookup-page.component';
import { ContractLookupSiteRoutes } from './contract-lookup-routing.module';
import { SharedModule } from '@shared/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { FormsModule } from '@angular/forms';

@NgModule({
  imports: [
    FormsModule,
    CommonModule,
    ContractLookupSiteRoutes,
    SharedModule,
    NgxPaginationModule
  ],
  declarations: [ContractLookupPageComponent]
})
export class ContractLookupSiteModule { }
