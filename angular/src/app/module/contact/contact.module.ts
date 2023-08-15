import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContactComponent } from './contact-management/contact.component';
import { FormsModule } from "@angular/forms";
import { NgxPaginationModule } from "ngx-pagination";
import { ContactRoutes } from './contact-routing.module';
import { SharedModule } from '@shared/shared.module';
import { CreateEditContactComponent } from './contact-management/create-edit-contact/create-edit-contact.component';

@NgModule({
  declarations: [ContactComponent, CreateEditContactComponent],
  imports: [
    CommonModule,
    FormsModule,
    ContactRoutes,
    SharedModule,
    NgxPaginationModule,
  ]
})
export class ContactModule { }
