
import { Routes, RouterModule } from "@angular/router";
import { SignatureManagementComponent } from "./signature-management/signature-management.component";

const routes: Routes = [
  {
    path: "",
    component: SignatureManagementComponent,
  },
];

export const SignatureRoutes = RouterModule.forChild(routes);
