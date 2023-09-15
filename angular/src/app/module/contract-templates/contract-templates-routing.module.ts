import { ContractTemplateListComponent } from './contract-templates-management/contract-template-list/contract-template-list.component';
import { Routes, RouterModule } from "@angular/router";
import { ContractTemplatesComponent } from "./contract-templates-management/contract-templates.component";

const routes: Routes = [
  {
    path: "",
    component: ContractTemplatesComponent,
    children: [
      {
        path: "",
        component: ContractTemplateListComponent,
      },
    ],
  },

  {
    path: "templates-create",
    loadChildren: () =>
      import("./contract-templates-create/contract-templates-create.module").then((m) => m.ContractTemplatesCreateModule),
  
  },

 
];

export const ContractTemplatesRoutes = RouterModule.forChild(routes);
