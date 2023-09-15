import { Component, Inject, Injector, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, Validators, FormArray } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SignServerService } from '@app/service/api/sign-server.service';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { MatDialog } from '@angular/material/dialog';
import { CreatePropertiesComponent } from './create-properties/create-properties.component';
import { values } from 'pdf-lib';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { ETabDetailWorker } from '@shared/AppEnums';
import { EventEmitter } from 'stream';
import { SignServerDataService } from '../services/sign-server.service';

@Component({
  selector: 'app-properties-worker',
  templateUrl: './properties-worker.component.html',
  styleUrls: ['./properties-worker.component.css']
})
export class PropertiesWorkerComponent extends DialogComponentBase<any> implements OnInit {
  public workerId: number;
  public workerName: string;
  propertiesWorker = {}
  isDisabledForm: boolean = true
  dataForm: { label: string, formControlName: string, editable?: boolean }[] = []
  dataFormCertificateInfo: { label: string, formControlName: string }[] = []
  formPropertiesWorker = this.fb.group({})
  formCertificateInfo = this.fb.group({})
  propertiesToRemove = []
  propertiesCanBeAdded: { editable: boolean, key: string }[];
  tempPropertiesCanBeAdded: { editable: boolean, key: string }[];
  isLoadingProperties: boolean = true
  statusWorker: string
  typeWorker
  allProperties: { editable: boolean, key: string }[] = []
  listCertificate: { key: string, value: string }
  isTabCertificate: boolean = false
  isAction: boolean = false
  listCertificateInfo = {}
  currentTabIndex: number = 0
  constructor(injector: Injector, private fb: FormBuilder, private dialog: MatDialog, private signServerService: SignServerService, public dialogRef: MatDialogRef<PropertiesWorkerComponent>, @Inject(MAT_DIALOG_DATA) public data: any, private signServerDataService: SignServerDataService) {
    super(injector)
  }

  ngOnInit(): void {
    this.workerName = this.data.workerName
    this.statusWorker = this.data.status
    this.typeWorker = this.data.type
    this.signServerService.GetPropertiesPermissionList(this.typeWorker).subscribe(res => {
      this.allProperties = res.result?.payload

      this.signServerService.GetWorkerProperties(this.data.workerId).subscribe(res => {
        this.propertiesWorker = res.result
        this.isTabCertificate = this.propertiesWorker['IMPLEMENTATION_CLASS'].split('.').pop().toLowerCase().includes('signer')
        this.propertiesWorker['TYPE'] === 'CRYPTO_WORKER' ? this.isAction = false : this.isAction = true
        let keyProperties = Object.keys(this.propertiesWorker);
        if (keyProperties?.length) {
          this.isLoadingProperties = false
        }
        this.handleDataForm()
        this.formPropertiesWorker.patchValue({ ...this.propertiesWorker })
        this.propertiesCanBeAdded = this.allProperties?.filter(property => !keyProperties.includes(property.key))
        this.tempPropertiesCanBeAdded = [...this.propertiesCanBeAdded]
        this.signServerDataService._propertiesCanBeAdded.next(this.tempPropertiesCanBeAdded)
      })
    })

    this.signServerService.GetSignerCertificateInfo(this.data.workerId).subscribe(res => {
      this.listCertificateInfo = { ...res.result?.payload }
      let keyCertificateInfo = Object.keys(this.listCertificateInfo)
      if (keyCertificateInfo?.length) {
        this.isLoadingProperties = false
      }
      keyCertificateInfo.map(key => {
        this.formCertificateInfo.addControl(key, this.fb.control({ value: '', disabled: true }))
        this.dataFormCertificateInfo?.push({ label: key, formControlName: key })
      })
      this.formCertificateInfo.patchValue({ ...this.listCertificateInfo })
    })
  }

  handleDataForm() {
    Object.keys(this.propertiesWorker).map(key => {
      this.formPropertiesWorker.addControl(key, this.fb.control({ value: '', disabled: true }))
      this.dataForm?.push({ label: key, formControlName: key })

      if (this.allProperties.length) {
        this.dataForm?.map(property => {
          let prop = this.allProperties?.find(item => item?.key === property?.label)
          if (prop) {
            return property.editable = prop?.editable
          }
        })
      }
    })
  }

  editFormPropertiesWorker() {
    this.isDisabledForm = !this.isDisabledForm
    if (this.isDisabledForm) {
      this.allProperties?.map(property => {
        property.editable && this.formPropertiesWorker.controls[property.key].disable()
      })
    } else {
      this.allProperties?.map(property => {
        property.editable && this.formPropertiesWorker.controls[property.key]?.enable()
      })
    }
  }

  addProperties() {
    const dialogRef = this.dialog.open(CreatePropertiesComponent, {
      data: {
        propertiesWorker: this.propertiesWorker,
        allProperties: this.allProperties,
        propertiesCanBeAdded: this.tempPropertiesCanBeAdded
      },
      width: '30%',
      maxWidth: '60%',

      panelClass: 'email-dialog',
    })

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        let propertiesAdded = this.dataForm.find(data => data.label === result.name)
        !propertiesAdded && this.dataForm.push({ label: result.name, formControlName: result.name, editable: true })
        this.formPropertiesWorker.addControl(result.name, this.fb.control({ value: result.value, disabled: this.isDisabledForm }))
        this.formPropertiesWorker.value[result.name] = result.value;
        this.formPropertiesWorker.controls[result.name].patchValue(result.value)
        this.propertiesWorker[result.name] = result.value
        this.tempPropertiesCanBeAdded = this.tempPropertiesCanBeAdded.filter(property => property.key !== result.name)
        this.signServerDataService._propertiesCanBeAdded.next(this.tempPropertiesCanBeAdded)

        let obj = {
          workerId: this.data.workerId,
          propertiesAndValues: {
            ...this.formPropertiesWorker.value
          },
          propertiesToRemove: [...this.propertiesToRemove]
        }
        this.signServerService.ConfigWorker(obj).subscribe(res => {
          abp.notify.success(this.ecTransform("WorkerAttributeAddedSuccessfully"))
        })
      }
    })
  }

  cancelChangeForm() {
    this.handleDataForm()
    this.formPropertiesWorker.patchValue({ ...this.propertiesWorker })
    this.isDisabledForm = true
    this.formPropertiesWorker.disable()
    this.tempPropertiesCanBeAdded = [...this.propertiesCanBeAdded]
    this.signServerDataService._propertiesCanBeAdded.next(this.tempPropertiesCanBeAdded)
  }

  removeProperty(key: string) {
    this.dataForm = this.dataForm.filter(item => item.label !== key)
    this.formPropertiesWorker.removeControl(key)
    this.propertiesToRemove.push(key)
  }

  saveChangeForm(data) {
    this.isDisabledForm = true
    this.formPropertiesWorker.disable()
    this.tempPropertiesCanBeAdded = this.allProperties.filter(property => !Object.keys(this.formPropertiesWorker.value).includes(property.key))
    this.signServerDataService._propertiesCanBeAdded.next(this.tempPropertiesCanBeAdded)
    let obj = {
      workerId: this.data.workerId,
      propertiesAndValues: {
        ...data
      },
      propertiesToRemove: [...this.propertiesToRemove]
    }
    this.signServerService.ConfigWorker(obj).subscribe(res => {
      abp.notify.success(this.ecTransform("UpdatePropertiesSuccessfully"))
    })
  }

  handleTabChange(event) {
    this.currentTabIndex = event.index
  }
}
