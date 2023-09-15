import { Component, Injector, OnInit } from '@angular/core';
import { SignServerService } from '../../../service/api/sign-server.service'
import { MatDialog } from '@angular/material/dialog';
import { PropertiesWorkerComponent } from './properties-worker/properties-worker.component';
import { EColorStatusPdfSigner, ENameStatusPdfSigner, EResStatusPdfSigner, ETypeWorker } from '@shared/AppEnums';
import { ConfigurationService } from '@app/service/api/configuration.service';
import { CreateWorkerComponent } from './create-worker/create-worker.component';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';

@Component({
  selector: 'app-sign-server',
  templateUrl: './sign-server.component.html',
  styleUrls: ['./sign-server.component.css']
})
export class SignServerComponent extends PagedListingComponentBase<any> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    throw new Error('Method not implemented.');
  }
  protected delete(entity: any): void {
    throw new Error('Method not implemented.');
  }
  currentPdfSignerName: string
  listActivePdfSigners
  isLoading: boolean = false
  constructor(injector: Injector, private signServerService: SignServerService, private dialog: MatDialog, private configurationService: ConfigurationService) {
    super(injector)
  }

  ngOnInit(): void {
    this.getPdfSigners()
    this.configurationService.getCurrentPdfSignerName().subscribe(res => this.currentPdfSignerName = res.result.currentPdfSigner)
  }

  getPdfSigners() {
    this.signServerService.GetAllPdfSigners().subscribe(res => {
      this.listActivePdfSigners = res.result;
    })
  }

  addPdfWorker() {
    const dialogRef = this.dialog.open(CreateWorkerComponent, {
      data: {},
      width: '50%',
      maxWidth: '50%',
      panelClass: 'email-dialog',
    })
    dialogRef.afterClosed().subscribe(rs => {
      if (rs) {
        this.isLoading = true
        this.signServerService.AddPdfSigners(rs).subscribe(res => {
          this.getPdfSigners()
          abp.notify.success(this.ecTransform('SuccessfullyAddNewWorker'));
          this.isLoading = false;
        })
      }
    })
  }

  handleColorStatus(status: string) {
    switch (status) {
      case EResStatusPdfSigner.Active: return EColorStatusPdfSigner.Active
      case EResStatusPdfSigner.Offline: return EColorStatusPdfSigner.Offline
      case EResStatusPdfSigner.Disable: return EColorStatusPdfSigner.Disable
    }
  }

  handleNameStatus(status: string) {
    switch (status) {
      case EResStatusPdfSigner.Active: return ENameStatusPdfSigner.Active
      case EResStatusPdfSigner.Offline: return ENameStatusPdfSigner.Offline
      case EResStatusPdfSigner.Disable: return ENameStatusPdfSigner.Disable
    }
  }

  viewPropertiesWorker(id: number, nameWorker: string, status: string): void {
    let typeWorker
    if (nameWorker.toLocaleLowerCase().includes('pdfsigner')) {
      typeWorker = ETypeWorker.Pdfsigner;
    } else {
      typeWorker = ETypeWorker.Crypto
    }
    const dialogRef = this.dialog.open(PropertiesWorkerComponent, {
      data: {
        workerId: id,
        workerName: nameWorker,
        status: status,
        type: typeWorker
      },
      width: '70%',
      maxWidth: '70%',
      panelClass: 'email-dialog',
    })
    dialogRef.afterClosed().subscribe(rs => {
    })
  }

  handleChangeStatus(status: string, id: number) {
    let obj = {
      workerId: id,
      propertiesAndValues: {
        DISABLED: 'TRUE'
      },
      propertiesToRemove: []
    }
    status === EResStatusPdfSigner.Active ? obj.propertiesAndValues.DISABLED = 'TRUE' : obj.propertiesAndValues.DISABLED = 'FALSE'

    abp.message.confirm("", this.ecTransform('AreYouSureYouWantToChangeTheStatusOfPdfWorker'), (rs) => {
      if (rs) {
        this.signServerService.ConfigWorker(obj).subscribe(res => {
          abp.notify.success(this.ecTransform('UpdatePropertiesSuccessfully'))
          this.getPdfSigners()
        })
      }
    })
  }
}



