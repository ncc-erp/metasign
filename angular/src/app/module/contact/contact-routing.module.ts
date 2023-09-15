import { ContactComponent } from './contact-management/contact.component';
import { Routes, RouterModule } from "@angular/router";

const routes: Routes = [
  {
    path: "",
    component: ContactComponent,
  },
];

export const ContactRoutes = RouterModule.forChild(routes)
