import { EmailLoginValidatorComponent } from './email-login-validator/email-login-validator.component';
import { Routes, RouterModule } from "@angular/router";
import { ContractLookupPageComponent } from "./contract-lookup-page/contract-lookup-page.component";

const routes: Routes = [
  {
    path: "contract-lookup",
    component: ContractLookupPageComponent,
  },
  {
    path: "",
    component: EmailLoginValidatorComponent,
  },
];

export const ContractLookupSiteRoutes = RouterModule.forChild(routes);
