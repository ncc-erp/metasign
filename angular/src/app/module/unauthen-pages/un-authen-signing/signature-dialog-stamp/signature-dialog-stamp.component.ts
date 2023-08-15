import { Inject } from "@angular/core";
import { Component, OnInit } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import { SignatureUserService } from "@app/service/api/signature-user.service";
import { AppConsts } from "@shared/AppConsts";
import { ElementRef, ViewChild, Injector } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { ContractSettingType } from "@shared/AppEnums";

export enum signatureTab {
  signatureUser = 0,
  signatureUpload = 1,
}

@Component({
  selector: "app-signature-dialog-stamp",
  templateUrl: "./signature-dialog-stamp.component.html",
  styleUrls: ["./signature-dialog-stamp.component.css"],
})
export class SignatureDialogStampComponent
  extends AppComponentBase
  implements OnInit
{
  constructor(
    private signatureUserService: SignatureUserService,
    public route: ActivatedRoute,
    public dialogRef: MatDialogRef<any>,
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
    this.contractSettingId = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("settingId"))
    )?.settingId;
  }
  sizeBoxSignature: { width: number; height: number } = {
    width: AppConsts.DEFAULT_SIGNATURE_WIDTH,
    height: AppConsts.DEFAULT_SIGNATURE_HEIGHT,
  };
  @ViewChild("inputFile") public inputFile: ElementRef;

  public indexTab: number = signatureTab.signatureUser;
  public isLoggedIn: boolean;
  public cropImg: string;
  contractSettingId: number;
  setDefaultSignature: boolean = false;
  signatureUpload: string;
  signatureTab = signatureTab;
  signatureUser;
  signatureUserValue;
  signaturePayload;
  maximumSizeofSignatureImage: number = AppConsts.maximumSizeofSignatureImage
  ngOnInit() {
    this.isLoggedIn = this.data?.signatureSetting.isLoggedIn;
    if (!this.isLoggedIn) {
      this.indexTab = signatureTab.signatureUpload;
    }
    this.signatureUserService
      .GetAllByEmail(this.contractSettingId)
      .subscribe((value) => {
        this.signatureUser = value.result.filter(signature => signature.signatureType === ContractSettingType.Stamp);;
        this.signatureUserValue = this.signatureUser.find(
          (signature) => signature.isDefault === true
        )?.id;
      });
  }

  clickSignature(id: number) {
    this.signatureUserValue = id;
  }

  onTabChanged($event) {
    this.indexTab = $event.index;
  }

  handleSave() {
    switch (this.indexTab) {
      case signatureTab.signatureUser:
        let signatureStamp = this.signatureUser.find(
          (signature) => signature.id === this.signatureUserValue
        );
        this.signaturePayload = {
          base64: signatureStamp?.fileBase64,
          isNewSignature: false,
          setDefault: this.setDefaultSignature,
        };
        break;

      case signatureTab.signatureUpload:
        this.signaturePayload = {
          base64: this.signatureUpload,
          isNewSignature: true,
          setDefault: this.setDefaultSignature,
        };
        break;
    }

    if (!this.signaturePayload.base64) {
      abp.message.error("", this.ecTransform("EmptyOrInvalidSignature"));
      return;
    }

    this.dialogRef.close(this.signaturePayload);
  }

  handleRemove() {
    this.signatureUpload = null;
    this.cropImg = null;
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

  imageCropped($event) {
    this.signatureUpload = $event.base64;
  }
}
