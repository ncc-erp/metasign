import { EmailValidComponent } from './email-valid/email-valid.component';
import { UnAuthenSigningComponent } from './un-authen-signing/un-authen-signing.component';
import { RouterModule, Routes } from '@angular/router';
import { SigningResultComponent } from './signing-result/signing-result.component';
import { SigningRejectComponent } from './signing-reject/signing-reject.component';



const routes: Routes = [
  {
    path: "unAuthen-signing",
    component: UnAuthenSigningComponent
  },
  {
    path: "signing-result",
    component: SigningResultComponent
  },
  {
    path: "signing-reject",
    component: SigningRejectComponent
  },
  {
    path: "email-valid",
    component: EmailValidComponent
  },

];

export const UnAuthenPageRoutingModule = RouterModule.forChild(routes);
