import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalService } from 'ngx-bootstrap/modal';
import { TenantChangeDialogComponent } from '../tenant-change-dialog/tenant-change-dialog.component';


@Component({
  selector: 'app-tenant-change',
  templateUrl: './tenant-change.component.html',
  styleUrls: ['./tenant-change.component.css']
})
export class TenantChangeComponent extends AppComponentBase implements OnInit {

  tenancyName;
  name;

  constructor(injector: Injector, private _modalService: BsModalService) {
    super(injector);
  }

  get isMultiTenancyEnabled(): boolean {
    return abp.multiTenancy.isEnabled;
  }

  ngOnInit() {
    if (this.appSession.tenant) {
      this.tenancyName = this.appSession.tenant.tenancyName;
      this.name = this.appSession.tenant.name;
    }
  }

  showChangeModal(): void {
    const modal = this._modalService.show(TenantChangeDialogComponent);
    if (this.appSession.tenant) {
      modal.content.tenancyName = this.appSession.tenant.tenancyName;
    }
  }

}
