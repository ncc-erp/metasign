import { UnAuthenSigningComponent } from "./module/unauthen-pages/un-authen-signing/un-authen-signing.component";
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { AppComponent } from "./app.component";
import { AppRouteGuard } from "@shared/auth/auth-route-guard";
import { AboutComponent } from "./about/about.component";
import { UsersComponent } from "./users/users.component";
import { TenantsComponent } from "./tenants/tenants.component";
import { RolesComponent } from "app/roles/roles.component";
import { ChangePasswordComponent } from "./users/change-password/change-password.component";
import { HomeComponent } from "./module/home/home/home.component";
import { SendMailResultComponent } from "./module/home/home/progress-contract/contract-email-setting/send-mail-result/send-mail-result.component";

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: "",
        component: AppComponent,
        children: [
          {
            path: "home",
            component: HomeComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "users",
            component: UsersComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "roles",
            component: RolesComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "tenants",
            component: TenantsComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "about",
            component: AboutComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "send-mail-result",
            component: SendMailResultComponent,
          },
          {
            path: "update-password",
            component: ChangePasswordComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "home",
            loadChildren: () =>
              import("./module/home/home.module").then((m) => m.HomeModule),
            data: { preload: true },
            canActivate: [AppRouteGuard],
          },
          {
            path: "admin",
            loadChildren: () =>
              import("./module/admin/admin.module").then((m) => m.AdminModule),
            data: { preload: true },
            canActivate: [AppRouteGuard],
          },
          {
            path: "signature",
            loadChildren: () =>
              import("./module/signature/signature.module").then(
                (m) => m.SignatureModule
              ),
            data: { preload: true },
            canActivate: [AppRouteGuard],
          },
          {
            path: "contracts",
            loadChildren: () =>
              import("./module/contract/contract.module").then(
                (m) => m.ContractModule
              ),
            data: { preload: true },
            canActivate: [AppRouteGuard],
          },
          {
            path: "contacts",
            loadChildren: () =>
              import("./module/contact/contact.module").then(
                (m) => m.ContactModule
              ),
            data: { preload: true },
            canActivate: [AppRouteGuard],
          },
          {
            path: "templates",
            loadChildren: () =>
              import("./module/contract-templates/contract-templates.module").then(
                (m) => m.ContractTemplatesModule
              ),
            data: { preload: true },
            canActivate: [AppRouteGuard],
          },
        ],
      },
      {
        path: "signging",
        loadChildren: () =>
          import("./module/unauthen-pages/unauthen-pages.module").then(
            (m) => m.UnauthenPagesModule
          ),
        data: { preload: true },
      },
      {
        path: "email-login",
        loadChildren: () =>
          import("./module/contract-lookup/contract-lookup.module").then(
            (m) => m.ContractLookupSiteModule
          ),
        data: { preload: true },
      }
    ]),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule { }
