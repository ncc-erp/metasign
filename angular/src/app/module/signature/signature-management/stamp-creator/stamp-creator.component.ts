import { Component, Inject, Injector, OnInit } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { AppConsts } from "@shared/AppConsts";
import { ContractSettingType } from "@shared/AppEnums";
import { AppComponentBase } from "@shared/app-component-base";
import { ActionType } from "../signature-management.component";
import { SignatureUserService } from "@app/service/api/signature-user.service";
import { actionEditSignature } from "@app/service/model/signature-user.dto";
import { ElementRef, ViewChild } from "@angular/core";

@Component({
  selector: "app-stamp-creator",
  templateUrl: "./stamp-creator.component.html",
  styleUrls: ["./stamp-creator.component.css"],
})
export class StampCreatorComponent extends AppComponentBase implements OnInit {
  public lableDialog: boolean = false;
  public cropImg: string = null;
  public signaturePayload: string;
  public isNeedUpload: boolean = false;
  public signatureUpload: string;
  public isDefault: boolean = false;
  maximumSizeofSignatureImage: number = AppConsts.maximumSizeofSignatureImage
  @ViewChild("inputFile") public inputFile: ElementRef;

  constructor(
    public dialogRef: MatDialogRef<StampCreatorComponent>,
    private signatureUserService: SignatureUserService,
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) public data: actionEditSignature
  ) {
    super(injector);
  }
  sizeBoxSignature: { width: number; height: number } = {
    width: AppConsts.DEFAULT_SIGNATURE_WIDTH,
    height: AppConsts.DEFAULT_SIGNATURE_HEIGHT,
  };

  ngOnInit() {
    if (this.data.type === ActionType.edit) {
      this.isNeedUpload = true;
      this.signatureUserService
        .getSignatureUserService(this.data.id)
        .subscribe((value) => {
          this.lableDialog = true;
          this.signaturePayload = value.result.fileBase64;
          this.isDefault = value.result.isDefault;
        });
    }
  }

  imageCropped($event) {
    this.signatureUpload = $event.base64;
  }

  handleRemove() {
    this.signatureUpload = null;
    this.cropImg = null;
    this.isNeedUpload = false;
    this.inputFile.nativeElement.value = null;
  }

  handleValueFile($event) {
    if ($event.target.files[0]) {
      if (
        !AppConsts.signatureFormatAllowed.includes($event.target.files[0].type)
      ) {
        abp.message.error(
          "Định dạng tệp không được hỗ trợ. Vui lòng chọn ảnh có định dạng JPG, JPEG, PNG."
        );
        this.inputFile.nativeElement.value = null;
        return;
      }

      if ($event.target.files[0].size > AppConsts.maximumSizeofSignatureImage * 1024) {
        abp.message.error(
          "Kích thước tệp vượt quá giới hạn cho phép. Vui lòng chọn một ảnh nhỏ hơn 300 kb."
        );
        this.inputFile.nativeElement.value = null;
        return;
      }
      this.cropImg = $event;
    }
  }

  handleSave() {
    this.signaturePayload = this.signatureUpload;
    const obj = {
      id: this.data.id,
      signatureType: ContractSettingType.Stamp,
      userId: this.appSession.userId,
      file: "demo",
      fileBase64: this.signaturePayload,
      type: this.data,
      isDefault: this.isDefault,
    };

    if (!this.signaturePayload) {
      abp.message.error("", this.ecTransform("EmptyOrInvalidSignature"));
      return;
    }

    this.dialogRef.close(obj);
  }
}
