import { Routes, RouterModule } from "@angular/router";
import { ContractManageComponent } from "./contract-manage/contract-manage.component";
import { DetailContractComponent } from "./detail-contract/detail-contract.component";

const routes: Routes = [
  {
    path: "",
    component: ContractManageComponent,
  },
  {
    path: 'details/:id',
    component: DetailContractComponent
  }
];

export const ContractRoutes = RouterModule.forChild(routes);
