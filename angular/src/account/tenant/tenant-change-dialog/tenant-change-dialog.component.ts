import { Component, Injector } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "@shared/app-component-base";
import { AccountServiceProxy } from "@shared/service-proxies/service-proxies";
import { AppTenantAvailabilityState } from "@shared/AppEnums";
import {
  IsTenantAvailableInput,
  IsTenantAvailableOutput,
} from "@shared/service-proxies/service-proxies";
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from "@angular/forms";

@Component({
  selector: "app-tenant-change-dialog",
  templateUrl: "./tenant-change-dialog.component.html",
  styleUrls: ["./tenant-change-dialog.component.css"],
})
export class TenantChangeDialogComponent extends AppComponentBase {
  saving = false;
  tenancyName;
  changeTenantForm = this.fb.group({
    tenancyName: ["", [Validators.required, Validators.maxLength(30)]],
  });

  ngOnInit(): void {

    this.tenancyName = this.appSession.tenant?.tenancyName;
    if(this.tenancyName)
    {
      this.changeTenantForm.patchValue({tenancyName: this.tenancyName})
    }

  }

  constructor(
    injector: Injector,
    private _accountService: AccountServiceProxy,
    public bsModalRef: BsModalRef,
    private fb: FormBuilder
  ) {
    super(injector);
  }

  save(): void {



    if (!this.changeTenantForm.value.tenancyName) {
      abp.multiTenancy.setTenantIdCookie(undefined);
      this.bsModalRef.hide();
      location.reload();
      return;
    }

    const input = new IsTenantAvailableInput();
    input.tenancyName = this.changeTenantForm.value.tenancyName.trim();

    this.saving = true;
    this._accountService.isTenantAvailable(input).subscribe(
      (result: IsTenantAvailableOutput) => {
        switch (result.state) {
          case AppTenantAvailabilityState.Available:
            abp.multiTenancy.setTenantIdCookie(result.tenantId);
            location.reload();
            return;
          case AppTenantAvailabilityState.InActive:
            this.message.warn(
              "Tenant is not active"
            );
            break;
          case AppTenantAvailabilityState.NotFound:
            this.message.warn(
              "Can not find tenant"
            );
            break;
        }
      },
      () => {
        this.saving = false;
      }
    );
  }
}
