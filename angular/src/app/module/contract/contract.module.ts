import { NgModule } from "@angular/core";
import { ContractManageComponent } from "./contract-manage/contract-manage.component";
import { ContractRoutes } from "./contract.routing";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { SharedModule } from "@shared/shared.module";
import { NgxPaginationModule } from "ngx-pagination";
import { DetailContractComponent } from './detail-contract/detail-contract.component';
import { HistoryContractComponent } from './contract-manage/history-contract/history-contract.component';
import { DialogDownloadComponent } from './contract-manage/dialog-download/dialog-download.component';

@NgModule({
  imports: [CommonModule, ContractRoutes, FormsModule, SharedModule,  ReactiveFormsModule , NgxPaginationModule ],
  declarations: [ContractManageComponent, DetailContractComponent, HistoryContractComponent ,DialogDownloadComponent],
})
export class ContractModule { }
