import { EmailTemplateComponent } from "./email-template/email-template.component";
import { Routes, RouterModule } from "@angular/router";
import { ConfigurationComponent } from './configuration/configuration.component';
import { SignServerComponent } from "./sign-server/sign-server.component";
import { ContractTemplateComponent } from "./contract-template/contract-template.component";

const routes: Routes = [
  {
    path: "email-templates",
    component: EmailTemplateComponent,
  },
  {
    path: "configurations",
    component: ConfigurationComponent,
  },
  {
    path: "sign-server",
    component: SignServerComponent,
  },
  {
    path: "contract-template",
    component: ContractTemplateComponent,
  }

];

export const AdminRoutingModule = RouterModule.forChild(routes);
