import { DialogMessTableComponent } from './components/dialog-mess-table/dialog-mess-table.component';
import { ContractTemplateTypePipe } from './pipes/contract-template-type.pipe';
import { ButtomLoginComponent } from './components/buttom-login/buttom-login.component';
import { ContractPreviewComponent } from './components/contract-preview/contract-preview.component';
import { ContractInputPipe } from "./pipes/contract-input.pipe";
import { ContractInputComponent } from "./components/contract-input/contract-input.component";
import { EditorModule, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import { AngularSignaturePadModule } from "@almothafar/angular-signature-pad";
import { CommonModule } from "@angular/common";
import { NgModule, ModuleWithProviders } from "@angular/core";
import { RouterModule } from "@angular/router";
import { NgxPaginationModule } from "ngx-pagination";
import { AppSessionService } from "./session/app-session.service";
import { AppUrlService } from "./nav/app-url.service";
import { AppAuthService } from "./auth/app-auth.service";
import { AppRouteGuard } from "./auth/auth-route-guard";
import { LocalizePipe } from "@shared/pipes/localize.pipe";
import { ListFilterPipe } from "./pipes/listFilter.pipe"
import { EcTranslatePipe } from "./pipes/ecTranslate.pipe"
import {
  FormatDateHourPipe,
  FormatDatePipe,
  FormatDateHour24hPipe,
  FormatDateHourSecondsPipe
} from "@shared/pipes/format-date.pipe";

import { AbpPaginationControlsComponent } from "./components/pagination/abp-pagination-controls.component";
import { AbpValidationSummaryComponent } from "./components/validation/abp-validation.summary.component";
import { AbpModalHeaderComponent } from "./components/modal/abp-modal-header.component";
import { AbpModalFooterComponent } from "./components/modal/abp-modal-footer.component";
import { LayoutStoreService } from "./layout/layout-store.service";

import { BusyDirective } from "./directives/busy.directive";
import { FormatDateHandInputDirective } from "./directives/formatDateHandInput.directive";
import { EqualValidator } from "./directives/equal-validator.directive";
import { MaterialModule } from "./material.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HeaderLanguageMenuComponent } from "@app/layout/header-language-menu.component";
import { BsDropdownModule } from "ngx-bootstrap/dropdown";

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    NgxPaginationModule,
    EditorModule,
    MaterialModule,
    AngularSignaturePadModule,
    FormsModule,
    BsDropdownModule
  ],
  declarations: [
    ContractPreviewComponent,
    ContractInputComponent,
    AbpPaginationControlsComponent,
    AbpValidationSummaryComponent,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
    LocalizePipe,
    BusyDirective,
    FormatDateHandInputDirective,
    EqualValidator,
    FormatDatePipe,
    ContractInputPipe,
    FormatDateHourPipe,
    ListFilterPipe,
    FormatDateHour24hPipe,
    FormatDateHourSecondsPipe,
    EcTranslatePipe,
    HeaderLanguageMenuComponent,
    ButtomLoginComponent,
    ContractTemplateTypePipe,
    DialogMessTableComponent
  ],
  exports: [
    ContractPreviewComponent,
    ContractInputComponent,
    AbpPaginationControlsComponent,
    AbpValidationSummaryComponent,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
    LocalizePipe,
    BusyDirective,
    FormatDateHandInputDirective,
    EqualValidator,
    EditorModule,
    MaterialModule,
    AngularSignaturePadModule,
    FormsModule,
    ReactiveFormsModule,
    FormatDatePipe,
    ContractInputPipe,
    FormatDateHourPipe,
    ListFilterPipe,
    FormatDateHour24hPipe,
    FormatDateHourSecondsPipe,
    EcTranslatePipe,
    HeaderLanguageMenuComponent,
    ButtomLoginComponent,
    BsDropdownModule,
    ContractTemplateTypePipe,
    DialogMessTableComponent
  ],
  providers: [
    { provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" },
  ],
})
export class SharedModule {
  static forRoot(): ModuleWithProviders<SharedModule> {
    return {
      ngModule: SharedModule,
      providers: [
        AppSessionService,
        AppUrlService,
        AppAuthService,
        AppRouteGuard,
        LayoutStoreService,
      ],
    };
  }
}
