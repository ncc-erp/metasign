import { Routes, RouterModule } from "@angular/router";
import { ContractTemplatesProgressComponent } from "./contract-templates-progress/contract-templates-progress.component";
import { ContractTemplatesUploadComponent } from "./contract-templates-progress/contract-templates-upload/contract-templates-upload.component";
import { ContractTemplatesDesignComponent } from "./contract-templates-progress/contract-templates-design/contract-templates-design.component";
import { ContractTemplatesSettingComponent } from "./contract-templates-progress/contract-templates-setting/contract-templates-setting.component";

const routes: Routes = [
  {
    path: "",
    component: ContractTemplatesProgressComponent,
    children: [
      { path: "", redirectTo: "upload", pathMatch: "full" },
      {
        path: "upload",
        component: ContractTemplatesUploadComponent,
      },
      {
        path: "setting",
        component: ContractTemplatesSettingComponent,
      },
      {
        path: "design",
        component: ContractTemplatesDesignComponent,
      }
    ],
  },
];

export const ContractTemplatesCreateRoutes = RouterModule.forChild(routes);
