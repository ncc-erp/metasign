import { SignatureCreateComponent } from "./signature-management/signature-create/signature-create.component";
import { SignatureRoutes } from "./signature-routing.module";
import { SignatureManagementComponent } from "./signature-management/signature-management.component";
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { MaterialModule } from "@shared/material.module";
import { SharedModule } from "@shared/shared.module";
import { ImageCropperModule } from "ngx-image-cropper";
import { StampCreatorComponent } from "./signature-management/stamp-creator/stamp-creator.component";


@NgModule({
  imports: [
    ImageCropperModule,
    CommonModule,
    SignatureRoutes,
    FormsModule,
    SharedModule
  ],
  declarations: [SignatureManagementComponent, SignatureCreateComponent , StampCreatorComponent],
})
export class SignatureModule { }

