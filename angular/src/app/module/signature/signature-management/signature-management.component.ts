import { StampCreatorComponent } from './stamp-creator/stamp-creator.component';
import { switchMap } from "rxjs/operators";
import { Component, OnInit, Injector, } from "@angular/core";
import {
  MatDialog,
} from "@angular/material/dialog";
import { SignatureCreateComponent } from "./signature-create/signature-create.component";
import { SignatureUserService } from "@app/service/api/signature-user.service";
import { NgSignaturePadOptions } from "@almothafar/angular-signature-pad";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";
import * as moment from 'moment';
import { ContractSettingType, EDisplayedColumnSignature, EKeyColumnSignature, ETypeSort } from "@shared/AppEnums";
import { types } from "util";
import { actionEditSignature } from '@app/service/model/signature-user.dto';

export enum ActionType {
  edit,
  add,
}

@Component({
  selector: "app-signature-management",
  templateUrl: "./signature-management.component.html",
  styleUrls: ["./signature-management.component.css"],
})
export class SignatureManagementComponent
  extends PagedListingComponentBase<any>
  implements OnInit {
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    throw new Error("Method not implemented.");
  }
  protected delete(entity: any): void {
    throw new Error("Method not implemented.");
  }
  listSignature;
  isTableLoading: boolean = false
  DisplayedColumnSignature = { No: { name: EDisplayedColumnSignature.NumericalOrder }, Signature: { name: EDisplayedColumnSignature.Signature }, CreationTime: { name: EDisplayedColumnSignature.CreationTime, statusSort: ETypeSort.default, key: EKeyColumnSignature.CreationTime }, UpdateTime: { name: EDisplayedColumnSignature.UpdatedTime, statusSort: ETypeSort.default, key: EKeyColumnSignature.UpdatedTime }, DefaultSignature: { name: EDisplayedColumnSignature.DefaultSignature }, Actions: { name: EDisplayedColumnSignature.Actions } }
  typeSort = { Default: ETypeSort.default, Up: ETypeSort.up, Down: ETypeSort.down }
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    private signatureUserService: SignatureUserService,
  ) {
    super(injector);
  }
  signatureType = ContractSettingType
  statusAction: ActionType;
  signaturePadOptions: NgSignaturePadOptions = {
    minWidth: 5,
    canvasWidth: 500,
    canvasHeight: 300,
  };

  ngOnInit() {
    this.getDataSignature();
  }

  openDialog(type: ActionType | actionEditSignature ,signaturetype: number): void {

    let dialogRef
    if(signaturetype === ContractSettingType.Stamp)
    {
      dialogRef = this.dialog.open(StampCreatorComponent, {
        data: type,
        height: "85%",
        width: "45%",
        panelClass: 'signature_dialog',
      });
    }
    else{
      dialogRef = this.dialog.open(SignatureCreateComponent, {
        data: type,
        height: "85%",
        width: "45%",
        panelClass: 'signature_dialog',
      });
    }

    dialogRef
      .afterClosed()
      .pipe(
        switchMap((value:any) => {
          this.statusAction = value.type;
          if (!value) {
            return;
          }
          if (value.type == ActionType.add) {
            delete value.type;
            delete value.id;
            return this.signatureUserService.createSignatureUserService(value);
          } else {
            delete value.type;
            return this.signatureUserService.updateSignatureUserService(value);
          }
        }),
        switchMap(() => this.signatureUserService.getSignatureUserAllService())
      )
      .subscribe({
        next: (data) => {
          this.listSignature = data.result;
          if (this.statusAction == ActionType.add) {
            abp.notify.success(this.ecTransform("SignatureCreatedSuccessfully"));
          } else {
            abp.notify.success(this.ecTransform('SignatureUpdatedSuccessfully'));
          }
        },
        error: (error) => { },
      });
  }

  handleAddSignature() {
    this.openDialog(ActionType.add, ContractSettingType.Electronic);
  }
  handleEditSignature(signature) {
    const editSignature : actionEditSignature = {
      type: ActionType.edit,
      id: signature.id,
      isDefault: signature.isDefault,
    };
    this.openDialog(editSignature,signature.signatureType);
  }

  handleAddSignatureStamp()
  {
    this.openDialog(ActionType.add, ContractSettingType.Stamp);
  }


  handleDeleteSignature(id) {
    abp.message.confirm("", this.ecTransform('AreYouSureYouWantToDeleteThisSignature'), (rs) => {
      if (rs) {
        this.signatureUserService
          .deleteSignerSignatureSetting(id)
          .pipe(
            switchMap(() => {
              return this.signatureUserService.getSignatureUserAllService();
            })
          )
          .subscribe((data) => {
            this.listSignature = data.result;
            abp.notify.success(this.ecTransform('SignatureDeletedSuccessfully'));
          });
      }
    });
  }

  getDataSignature() {
    this.isTableLoading = true
    this.signatureUserService.getSignatureUserAllService().subscribe((data) => {
      this.listSignature = data.result
      // .sort(function (a, b) {
      //   return moment(b.creationTime).diff(moment(a.creationTime), 'milliseconds');
      // });
      // this.listSignature.map((signature, index) => {
      //   if (signature.isDefault) {
      //     let element = this.listSignature.splice(index, 1)[0];
      //     this.listSignature.unshift(element);
      //   }
      // })
      this.isTableLoading = false
    });
  }

  sort(columnSort: string, typeSort: number) {
    this.listSignature = this.listSignature.sort(function (a, b) {
      if (typeSort === ETypeSort.up) {
        return moment(a[columnSort]).diff(moment(b[columnSort]), 'milliseconds');
      }
      else if (typeSort === ETypeSort.down || typeSort === ETypeSort.default) {
        return moment(b[columnSort]).diff(moment(a[columnSort]), 'milliseconds');
      }
    })

  }

  handleChangeSortSignature(columnSort: string, typeSort: number) {
    switch (columnSort) {
      case this.DisplayedColumnSignature.CreationTime.name: {
        this.DisplayedColumnSignature.CreationTime.statusSort = typeSort
        this.DisplayedColumnSignature.UpdateTime.statusSort = ETypeSort.default
        this.sort(this.DisplayedColumnSignature.CreationTime.key, typeSort)
        break
      }
      case this.DisplayedColumnSignature.UpdateTime.name: {
        this.DisplayedColumnSignature.UpdateTime.statusSort = typeSort
        this.DisplayedColumnSignature.CreationTime.statusSort = ETypeSort.default
        this.sort(this.DisplayedColumnSignature.UpdateTime.key, typeSort)
        break
      }
    }

  }

  setDefaultSignature(signature, isDefault) {
    isDefault ? abp.message.confirm("", this.ecTransform('RemoveDefaultSignature'), (rs) => {
      if (rs) {
        this.signatureUserService
          .setDefaultSignature({ id: signature.id, isDefault: false })
          .subscribe((res) => {
            this.getDataSignature();
            if (res?.success) {
              abp.notify.success(this.ecTransform('RemoveDefaultSignatureSuccessfully'));
            }
          });
      }
    }) :
      abp.message.confirm("", this.ecTransform('SetAsDefaultSignature') + "?", (rs) => {
        if (rs) {
          this.signatureUserService
            .setDefaultSignature({ id: signature.id, isDefault: true })
            .subscribe((res) => {
              this.getDataSignature();
              if (res?.success) {
                abp.notify.success(this.ecTransform('SetupDefaultSignatureSuccessfully'));
              }
            });
        }
      });
  }
  public isShowCreateBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Signature_Create);
  }

  public isShowEditBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Signature_Edit);
  }

  public isShowDeleteBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Signature_Delete);
  }

  public isAllowSetDefault() {
    return this.isGranted(PERMISSIONS_CONSTANT.Signature_SetDefult);
  }
}
