import { ContractInvalidateDialogComponent } from './contract-invalidate-dialog/contract-invalidate-dialog.component';
import { MatIconRegistry } from "@angular/material/icon";
import { map, concatMap } from "rxjs/operators";
import { FillInputDto, SignInputDto } from "./../../../service/model/design-contract.dto";
import { BreakPoint, ContractInvalidate, ContractSettingType, EContractStatusId } from "./../../../../shared/AppEnums";
import { MatDialog } from "@angular/material/dialog";
import { SignDto, SignMultipleDto } from "./../../../service/model/sign.dto";
import { saveAs } from "file-saver";
import { ContractSigningService } from "./../../../service/api/contract-signing.service";
import { SignerSignatureSettingService } from "./../../../service/api/signer-signature-setting.service";
import {
  Component,
  ElementRef,
  Injector,
  OnInit,
  QueryList,
  ViewChild,
  ViewChildren,
  HostListener
} from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { DomSanitizer } from "@angular/platform-browser";
import * as pdfjsLib from "pdfjs-dist/webpack";
import { ContractRole, ContractStatus } from "@shared/AppEnums";
import { SignatureSettings } from "@app/service/model/design-contract.dto";
import {
  ContractSignatureSettingDto,
  ContractSignerSignatureSettingDto,
} from "@app/service/model/signerSignatureSetting.dto";
import { AppConsts } from "@shared/AppConsts";
import { AppComponentBase } from "@shared/app-component-base";
import { CertDetailDialogComponent } from "./cert-detail-dialog/cert-detail-dialog.component";
import { CertificateDetailDto } from "@app/service/model/certificateDetail.dto";
import { DesktopAppServiceService } from "@app/service/api/desktop-app-service.service";
import { PreviewContractComponent } from "@app/module/home/home/progress-contract/upload-contract/preview-contract/preview-contract.component";
import { ContractDialogConfirmComponent } from './contract-dialog-confirm/contract-dialog-confirm.component';
import { DialogDownloadComponent } from '@app/module/contract/contract-manage/dialog-download/dialog-download.component';
import { ContractFileStoringService } from '@app/service/api/contract-file-storing.service';
@Component({
  selector: "app-un-authen-signing",
  templateUrl: "./un-authen-signing.component.html",
  styleUrls: ["./un-authen-signing.component.css"],
})
export class UnAuthenSigningComponent
  extends AppComponentBase
  implements OnInit {
  private contractSettingId: number = 0;
  private contracId: number = 0;
  private tenantName = "";
  signatureTypeList = AppConsts.signatureTypeList.concat(AppConsts.otherTypeList);
  contractBase64: string = "";
  signatureSetting: SignatureSettings[] = [];
  contractInfo: ContractSignerSignatureSettingDto;
  listSignature: SignDto[] = [];
  isSaving: boolean = false;
  loggedIn: boolean;
  validUser: boolean = false;
  googleLoginOptions = {
    scope: "profile email",
  };
  idPage: number = -1;
  idPdf: number;
  statusSign: string = "Start";
  contractfiles: ContractSignatureSettingDto[] = [];
  contractFilePreview: ContractSignatureSettingDto[] = [];
  scalePreview: number = 1.45
  filledInput: FillInputDto[] = [];
  signatureDefaultElectronic = {} as {
    contractBase64: string,
    signatureType: ContractSettingType
  };
  signatureDefaultStamp = {} as {
    contractBase64: string,
    signatureType: ContractSettingType
  };;
  isInput: boolean = false;
  signatureSettingTemp: SignatureSettings[] = [];
  imageSignatureDigital: string;
  signatureElectronic = [];
  contractInput: any;
  contractLoadding: boolean = true;
  certificateDetail: CertificateDetailDto;
  statusEmitSignatureDigital: boolean = true;
  statusContractSign: boolean = false;
  scaleValue: number;
  isCheckConfirm: boolean = false;
  isCheckShowConfirm: boolean
  isCreator: boolean;
  contractStatus: number;
  ContractRole = ContractRole;
  isComplete: boolean;
  screenWidth: number;

  @ViewChild("contractScrolls") contractScroll: ElementRef;
  @ViewChild("pageContainer") pageContainer: ElementRef;
  @ViewChildren("signature")
  signature: QueryList<ElementRef> = new QueryList(null);
  constructor(
    injector: Injector,
    private signatureSettingService: SignerSignatureSettingService,
    private route: ActivatedRoute,
    private contractSigningService: ContractSigningService,
    public dialog: MatDialog,
    private router: Router,
    private matIconRegistry: MatIconRegistry,
    private domSanitizer: DomSanitizer,
    private desktopAppServiceService: DesktopAppServiceService,
    private contractFileStoringService:ContractFileStoringService
  ) {
    super(injector);
    this.matIconRegistry.addSvgIcon(
      "digital",
      this.domSanitizer.bypassSecurityTrustResourceUrl(
        "../../../../assets/img/signature/Digital.svg"
      )
    );
    this.matIconRegistry.addSvgIcon(
      "electronic",
      this.domSanitizer.bypassSecurityTrustResourceUrl(
        "../../../../assets/img/signature/Electronic.svg"
      )
    );

    this.matIconRegistry.addSvgIcon(
      "text",
      this.domSanitizer.bypassSecurityTrustResourceUrl(
        "../../../../assets/img/signature/Text.svg"
      )
    );

    this.matIconRegistry.addSvgIcon(
      "datePicker",
      this.domSanitizer.bypassSecurityTrustResourceUrl(
        "../../../../assets/img/signature/DatePicker.svg"
      )
    );


    this.matIconRegistry.addSvgIcon(
      "stamp",
      this.domSanitizer.bypassSecurityTrustResourceUrl(
        "../../../../assets/img/signature/stamp.svg"
      )
    );

    this.contractSettingId = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("settingId"))
    )?.settingId;

    this.contracId = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("contractId"))
    )?.contractId;
    this.tenantName = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("contractId"))
    )?.tenancyName;
  }

  ngOnInit(): void {
    this.updateScale();
    this.getSignatureSetting();
    this.screenWidth = window.innerWidth
  }

  @HostListener('window:resize')
  onWindowResize() {
    this.screenWidth = window.innerWidth;
    this.updateScale();
  }

  handleOpenContractInvalidate() {
    if (this.isCreator) {
      this.dialog.open(ContractInvalidateDialogComponent, {
        data: {
          contractSettingId: this.contractSettingId,
          role: ContractInvalidate.contractCreator,
          title: "CancelDocument"
        },
        width: this.screenWidth <= BreakPoint.mobile ? "80%" : "50%",
        height: this.screenWidth <= BreakPoint.Ipad ? "80%" : "60%",
        panelClass: 'contract-dialog',
      });
    }
    else {
      this.dialog.open(ContractDialogConfirmComponent, {
        data: {
          contractSettingId: this.contractSettingId,
          role: ContractInvalidate.contractSigner
        },
        width: this.screenWidth <= BreakPoint.IpadMini ? "70%" : "30%",
        height: this.screenWidth <= BreakPoint.Ipad ? "60%" : "40%",
        panelClass: 'confirm-dialog',
      });
    }
  }

  updateScale() {
    const targetWidth = 820
    const screenWidth = window.innerWidth; // Chiều rộng màn hình hiện tại
    const scaleX = screenWidth / 1224;
    this.scaleValue = screenWidth <= targetWidth ? scaleX : 1;
  }

  handleValueContractText(signature) {
    let existItem = this.filledInput.find(x => x.signerSignatureSettingId == signature.id)
    if (existItem) {
      existItem.content = signature.valueInput
    }
    if (!existItem && !signature.isSigned && signature?.signatureType !== ContractSettingType.Digital && signature?.signatureType !== ContractSettingType.Electronic) {
      this.filledInput.push(
        {
          base64Pdf: this.contractInfo.contractBase64,
          content: signature.valueInput,
          fontSize: signature.fontSize,
          fontFamily: signature.fontFamily,
          pageHeight: signature.heightPage,
          signerSignatureSettingId: signature.id,
          color: signature.fontColor,
          signatureType: signature.signatureType
        } as FillInputDto
      )
    }
  }

  async handleSave() {

    let queryParameters = { id: this.contracId, contracInfo: this.contractInfo.contractName }

    let signatures = this.signatureSetting.filter(x => x.signatureType == ContractSettingType.Digital
      || x.signatureType == ContractSettingType.Electronic || x.signatureType == ContractSettingType.Stamp)

    let setupInputs = this.signatureSetting.filter(x => (x.signatureType == ContractSettingType.Text
      || x.signatureType == ContractSettingType.DatePicker) && x.isAllowSigning)
    this.filledInput = this.filledInput.filter(input => (input.signatureType === ContractSettingType.Text && !!input.content?.trimEnd()) || (input.signatureType === ContractSettingType.DatePicker && input.content !== "Invalid date")
    )

    if (this.filledInput.length < setupInputs.length) {
      abp.message.error(this.ecTransform("PleaseFillOutAllFields"));
      return;
    }

    if (this.listSignature.length !== signatures.length) {
      abp.message.error(this.ecTransform("PleaseFillOutAllFields"));
      return;
    }

    if (this.filledInput.length > 0) {
      let dto = {
        base64Pdf: this.contractInfo.contractBase64,
        listInput: this.filledInput
      } as SignInputDto

      await this.contractSigningService.SignInput(dto).toPromise().then(rs => {
        this.contractInfo.contractBase64 = rs.result

        if (this.filledInput.length == this.signatureSetting.length) {
          let signingResult = {
            contractSettingId: this.contractSettingId,
            signResult: rs.result,
          }
          this.contractSigningService.insertSigningResultAndComplete(signingResult).pipe(
            concatMap(()=> this.contractFileStoringService.clearContractDownloadFiles(this.contracId)))
            .subscribe()
          this.router.navigate(["/app/signging/signing-result"], { queryParams: queryParameters });
          this.statusContractSign = false
          return
        }
      })
    }

    this.contractLoadding = true;
    this.statusContractSign = true
    this.isSaving = true;
    let statusSignatureElectronic: boolean = false;
    let statusSignatureDigital: boolean = false;

    this.listSignature.forEach((signature) => {
      if (signature.signatureType === ContractSettingType.Digital) {
        statusSignatureDigital = true;
      }
      if (signature.signatureType === ContractSettingType.Electronic || signature.signatureType == ContractSettingType.Stamp) {
        statusSignatureElectronic = true;
      }
    });

    if (statusSignatureElectronic && statusSignatureDigital) {

      let signatureDigital = this.listSignature.filter(
        (x) => x.signatureType == ContractSettingType.Digital
      );
      let signatureElectronic = this.listSignature.filter(
        (x) => x.signatureType == ContractSettingType.Electronic || x.signatureType == ContractSettingType.Stamp
      );

      let signatureDigitalPayload = {
        base64Pdf: this.contractInfo.contractBase64,
        signatureBase64: this.imageSignatureDigital,
        signatures: signatureDigital
      }
      this.desktopAppServiceService.SignDigital(signatureDigitalPayload).pipe(concatMap((value) => {
        if (value == null || value == "") {
          this.statusSign = "Start";
          this.idPage = -1;
          abp.message.error(this.ecTransform("SigningFailedPleaseRecheckUSB"))
          return
        }

        let signatureElectronicPayload = {
          contractId: this.contracId,
          signSignatures: signatureElectronic,
          contractBase64: value
        };
        return this.contractSigningService.signMultiple(signatureElectronicPayload)
      }),concatMap((rs) => {

        this.isSaving = false;
        this.contractLoadding = false;
        let signingResult = {
          contractSettingId: this.contractSettingId,
          signResult: rs.result
        }
        return this.contractSigningService.insertSigningResult(signingResult)
      }),concatMap(() => {
        return this.contractFileStoringService.clearContractDownloadFiles(this.contracId)
      })).subscribe(() => {
        this.router.navigate(["/app/signging/signing-result"], { queryParams: queryParameters });
        this.statusContractSign = false
      },
        () => {
          this.isSaving = false
          this.contractLoadding = false;
          this.statusContractSign = false
        }
      )
      return;
    }

    if (statusSignatureDigital) {
      let signatureDigital = this.listSignature.filter(
        (x) => x.signatureType == ContractSettingType.Digital
      );
      let signatureDigitalPayload = {
        base64Pdf: this.contractInfo.contractBase64,
        signatureBase64: this.imageSignatureDigital,
        signatures: signatureDigital,
      };
      this.desktopAppServiceService
        .SignDigital(signatureDigitalPayload).pipe(concatMap((rs) => {
          if (rs == null || rs == "") {
            this.statusSign = "Start";
            this.idPage = -1;
            abp.message.error(this.ecTransform("SigningFailedPleaseRecheckUSB"))
            return
          }

          this.isSaving = false;
          this.contractLoadding = false;
          let signingResult = {
            contractSettingId: this.contractSettingId,
            signResult: rs,
            hasDigital: true
          }
          return this.contractSigningService.insertSigningResultAndComplete(signingResult)
        }),concatMap(() => {
          return this.contractFileStoringService.clearContractDownloadFiles(this.contracId)
        }))
        .subscribe(
          () => {
            this.router.navigate(["/app/signging/signing-result"], { queryParams: queryParameters })
            this.statusContractSign = false

          },
          () => {
            this.isSaving = false;
            this.contractLoadding = false;
            this.statusContractSign = false

          }
        );

      return;
    }

    if (statusSignatureElectronic) {
      let signatureElectronic = this.listSignature.filter(
        (x) => x.signatureType == ContractSettingType.Electronic || x.signatureType == ContractSettingType.Stamp
      );
      let signInput = {
        contractId: this.contracId,
        signSignatures: signatureElectronic,
        contractBase64: this.contractInfo.contractBase64
      } as SignMultipleDto;

      this.contractSigningService.signMultiple(signInput).pipe(concatMap(() => {
        return this.contractFileStoringService.clearContractDownloadFiles(this.contracId)
      })).subscribe(
        () => {
          this.isSaving = false;
          this.contractLoadding = false;
          this.router.navigate(["/app/signging/signing-result"], { queryParams: queryParameters });
          this.statusContractSign = false
        },
        () => {
          this.statusContractSign = false
          this.isSaving = false;
          this.contractLoadding = false;
        }
      );
      return;

    }
  }

  ngAfterViewInit(): void {
    this.signature.changes
      .pipe(
        map((value) => {
          return Array.from(value._results).filter(
            (signature: ElementRef): any => {
              return signature.nativeElement.datas;
            }
          );
        })
      )
      .subscribe((value) => {
        this.contractInput = value;
      });
  }


  private async getSignatureSetting() {
    let jwt = localStorage.getItem("JWT");
    if (!jwt || jwt == "") {
      this.router.navigate([`/app/signging/email-valid`], {
        queryParams: {
          settingId: this.route.snapshot.queryParamMap.get("settingId"),
          contractId: this.route.snapshot.queryParamMap.get("contractId"),
          tenantName: this.route.snapshot.queryParamMap.get("tenantName"),
          status: 1,
        },
      });
    }
    let currentLogin = this.parseJwt(jwt);

    this.contractSigningService.getSignatureDigitalBase64().subscribe(rs => {
      this.imageSignatureDigital =
        "data:image/png;base64," + rs.result;
    })
    this.signatureSettingService.getSignatureSetting(
      this.contractSettingId,
      currentLogin.email
    ).subscribe(async (rs: any) => {
      this.signatureSettingTemp = JSON.parse(JSON.stringify(rs.result.signatureSettings));
      this.isCreator = rs.result.isCreator;
      this.contractStatus = rs.result.role;
      if (rs.result.status === EContractStatusId.Cancelled) {
        this.router.navigate([`/app/signging/email-valid`], {
          queryParams: {
            settingId: this.route.snapshot.queryParamMap.get("settingId"),
            contractId: this.route.snapshot.queryParamMap.get("contractId"),
            tenantName: this.route.snapshot.queryParamMap.get("tenantName"),
            status: ContractRole.Signer,
          },
        });
      }
      let contractfiles = JSON.parse(JSON.stringify({ ...rs.result }));
      this.isComplete = rs.result.isComplete;
      this.isCheckShowConfirm = rs.result.role === ContractRole.Signer
      this.signatureSetting = contractfiles.signatureSettings.filter(
        (value) => {
          return value.isAllowSigning;
        }
      );
      if (rs.result.signatureDefault?.signatureType === ContractSettingType.Stamp) {
        this.signatureDefaultStamp = {
          ...rs.result.signatureDefault
        }
      } else {
        this.signatureDefaultElectronic = {
          ...rs.result.signatureDefault
        }
      }
      this.contractInfo = contractfiles;

      this.contractfiles = await this.generateImageStringsFromBase64PDF(
        contractfiles.contractBase64.split(",")[1]
      );
      this.contractfiles.forEach((file) => {
        file.fileBase64 = "data:image/jpeg;base64," + file.fileBase64;
        this.signatureSettingTemp.forEach((signature) => {
          signature.signatureTypeName = this.checkIconTypeSignature(
            signature.signatureType
          );
          if (signature.page === file.contractPage) {
            signature.heightPage = file.height;
            file.signatureSettings.push(signature);
          }
          if (ContractSettingType.Text === signature.signatureType) {
            this.isInput = true;
          }
        });

      });

      this.contractFilePreview = await this.generateImageStringsFromBase64PDF(
        contractfiles.contractBase64.split(",")[1], this.scalePreview
      );

      this.contractFilePreview.forEach((file, index) => {
        file.fileBase64 = "data:image/jpeg;base64," + file.fileBase64;
        this.signatureSettingTemp.forEach((signature) => {
          signature.signatureTypeName = this.checkIconTypeSignature(
            signature.signatureType
          );
          if (signature.page === file.contractPage) {
            // signature.heightPage = file.height;
            file.signatureSettings.push({ ...signature, positionX: signature.positionX * (this.scalePreview / 2), positionY: signature.positionY * (this.scalePreview / 2), width: signature.width * (this.scalePreview / 2), height: signature.height * (this.scalePreview / 2), fontSize: signature.fontSize * (this.scalePreview / 2) });
            // file.signatureSettings.push(signature)
          }
          if (ContractSettingType.Text === signature.signatureType) {
            this.isInput = true;
          }
        });
      });
      this.contractLoadding = false;
      this.signature.notifyOnChanges();
    },
      () =>
        this.router.navigate([`/app/signging/email-valid`], {
          queryParams: {
            settingId: this.route.snapshot.queryParamMap.get("settingId"),
            contractId: this.route.snapshot.queryParamMap.get("contractId"),
            tenantName: this.route.snapshot.queryParamMap.get("tenantName"),
            status: 1,
          },
        })
    );
  }

  parseJwt(token: string) {
    if (!token) {
      return "";
    }
    var base64Url = token.split(".")[1];
    var base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    var jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map(function (c) {
          return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
        })
        .join("")
    );

    return JSON.parse(jsonPayload);
  }

  checkIconTypeSignature(SignatureType: ContractSettingType) {
    switch (SignatureType) {
      case ContractSettingType.Text:
        return "text";
      case ContractSettingType.DatePicker:
        return "datePicker";
      case ContractSettingType.Electronic:
        return "electronic";
      case ContractSettingType.Digital:
        return "digital";
      case ContractSettingType.Stamp:
        return "stamp";
    }
  }

  async handlePostSignature($event, signature) {
    this.signatureElectronic?.push($event)
    let indexSignature = this.listSignature.findIndex(
      (value) =>
        value.signerSignatureSettingid === $event.signerSignatureSettingid
    );

    switch ($event.signatureType) {
      case ContractSettingType.Electronic:
        this.signatureDefaultElectronic = {
          contractBase64: $event.signartureBase64,
          signatureType: $event.signatureType
        }
        if (indexSignature !== -1) {
          this.listSignature.forEach((x) => {
            if (x.signatureType === ContractSettingType.Electronic) {
              x.signartureBase64 = $event.signartureBase64;
              x.isNewSignature = $event.isNewSignature;
              x.setDefault = $event.setDefault;
            }
          });
        } else {
          signature.isTemporarySigned = true;
          this.listSignature.push($event);
        }
        break;
      case ContractSettingType.Digital:
        if (indexSignature !== -1) {

          let certDetail = {
            ...this.listSignature[indexSignature],
            isDownloadApp: false
          };
          let ref = this.dialog.open(CertDetailDialogComponent, {
            data: certDetail,
            width: "30%",
            height: "35%",
          });

          ref?.afterClosed().subscribe((rs: CertificateDetailDto) => {
            if (rs?.certSerial) {

              signature.ownCA = rs.ownCA
              this.listSignature.forEach((value) => {
                if (
                  value.signerSignatureSettingid ===
                  $event.signerSignatureSettingid
                ) {
                  value.certSerial = rs.certSerial;
                  value.beginDateCA = rs.beginDateCA;
                  value.endDateCA = rs.endDateCA;
                  value.ownCA = rs.ownCA;
                  value.uid = rs.uid;
                  value.organizationCA = rs.organizationCA;
                }
              });
            }
          });
        } else {
          signature.loading = true;
          this.statusEmitSignatureDigital = false;
          this.desktopAppServiceService.getCertificate().subscribe({
            next: (result) => {
              let ref;
              this.statusEmitSignatureDigital = true
              signature.loading = false;
              if (result.Status !== 0) {
                let certDetail = {
                  certSerial: result.CertDetailInfo.CertSerial,
                  organizationCA: result.CertDetailInfo.OrganizationCA,
                  ownCA: result.CertDetailInfo.OwnCA,
                  uid: result.CertDetailInfo.Uid,
                  beginDateCA: result.CertDetailInfo.BeginDateCA,
                  endDateCA: result.CertDetailInfo.EndDateCA,
                  isDownloadApp: false
                };
                ref = this.dialog.open(CertDetailDialogComponent, {
                  data: certDetail,
                  width: "30%",
                  height: "35%",
                });
              }
              ref?.afterClosed().subscribe((value: CertificateDetailDto) => {
                if (value) {
                  signature.ownCA = value.ownCA;
                  signature.isTemporarySigned = true;
                  let signatureToken = $event;
                  signatureToken.certSerial = value.certSerial;
                  signatureToken.beginDateCA = value.beginDateCA;
                  signatureToken.endDateCA = value.endDateCA;
                  signatureToken.ownCA = value.ownCA;
                  signatureToken.uid = value.uid;
                  signatureToken.organizationCA = value.organizationCA;
                  this.listSignature.push(signatureToken);
                  return;
                }
              });
            },
            error: () => {
              this.statusEmitSignatureDigital = true
              signature.loading = false;
              let certDetail = {
                isDownloadApp: true,
              };
              this.dialog.open(CertDetailDialogComponent, {
                data: certDetail,
                width: "40%",
                height: "30%",
              });
            },
          });
        }
        break;
      case ContractSettingType.Stamp:
        this.signatureDefaultStamp =
        {
          contractBase64: $event.signartureBase64,
          signatureType: $event.signatureType
        }
        if (indexSignature !== -1) {
          this.listSignature.forEach((x) => {
            if (x.signatureType === ContractSettingType.Stamp) {
              x.signartureBase64 = $event.signartureBase64;
              x.isNewSignature = $event.isNewSignature;
              x.setDefault = $event.setDefault;
            }
          });
        } else {
          signature.isTemporarySigned = true;
          this.listSignature.push($event);
        }
        break;
    }
  }

  isShowSubmitBtn() {
    return (
      this.contractInfo?.status == ContractStatus.Inprogress &&
      this.contractInfo?.role == ContractRole.Signer
    );
  }

  handleContractScroll(increase: boolean = true) {
    const distance = 500;
    const headerPage = 51;

    if (increase === true) {
      this.idPage++
    }

    if (this.idPage >= this.contractInput.length) {
      this.idPage = 0;
    }

    let { heightHalf, ponsitonMaker } = this.scrollContractEvent(this.idPage);
    this.checkStatusSign(this.contractInput[this.idPage].nativeElement.data);
    this.pageContainer.nativeElement.scrollTo(0, ponsitonMaker - distance);
    this.contractScroll.nativeElement.style.top = ponsitonMaker - headerPage + heightHalf + "px";
  }

  handleScrollMarker(id: number) {
    const distance = 500;
    const headerPage = 51;
    const el = document.getElementById(`${id}`);

    this.idPage = this.contractInput.findIndex(
      (value: { nativeElement: { id: string } }) => value.nativeElement.id === el.id
    );

    let { heightHalf, ponsitonMaker } = this.scrollContractEvent(this.idPage);
    this.checkStatusSign(this.contractInput[this.idPage].nativeElement.data);
    this.pageContainer.nativeElement.scrollTo(0, ponsitonMaker - distance);

    this.contractScroll.nativeElement.style.top = ponsitonMaker - headerPage + heightHalf + "px";
  }

  scrollContractEvent(id) {
    if (!this.isComplete) {
      const currentElementId = this.contractInput[id].nativeElement.id;
      this.contractInput.forEach((element: ElementRef) => {
        const elementId = element.nativeElement.id;
        element.nativeElement.classList.toggle('highlighted', elementId === currentElementId);
      });
    }
    let heightHalf = this.contractInput[id].nativeElement.clientHeight / 2;
    let ponsitonMaker = this.getAbsoluteOffsetFromGivenElement(
      this.contractInput[id].nativeElement,
      this.pageContainer.nativeElement).top;
    return {
      heightHalf,
      ponsitonMaker
    }
  }

  checkStatusSign(value: any) {
    switch (value) {
      case ContractSettingType.Electronic:
        this.statusSign = "SignOrSignature";
        break;
      case ContractSettingType.Text:
        this.statusSign = "FillOrText";
        break;
      case ContractSettingType.DatePicker:
        this.statusSign = "FillOrDate";
        break;
      case ContractSettingType.Digital:
        this.statusSign = "SignOrUSBToken";
        break;
      case ContractSettingType.Stamp:
        this.statusSign = "Stamp";
        break;
    }
  }

  handleScrollPage(idPage: number) {
    this.idPdf = idPage;
    document.getElementById(`${idPage}`).scrollIntoView({ behavior: "smooth" });
  }

  handleClick(index: number) {
    this.idPage = index;
    this.handleContractScroll(false);
  }

  isShowNavigateArrow() {
    return this.contractInfo?.role == ContractRole.Signer;
  }
  getAbsoluteOffsetFromGivenElement(el, relativeEl) {   // finds the offset of el from relativeEl
    var _x = 0;
    var _y = 0;
    while (el && el != relativeEl && !isNaN(el.offsetLeft) && !isNaN(el.offsetTop)) {
      _x += el.offsetLeft - el.scrollLeft + el.clientLeft;
      _y += el.offsetTop - el.scrollTop + el.clientTop;
      el = el.offsetParent;
    }
    return { top: _y, left: _x };
  }

  generateImageStringsFromBase64PDF = async (base64PDF: string, scalePreview?: number) => {
    const arrayBuffer = Uint8Array.from(atob(base64PDF), (c) =>
      c.charCodeAt(0)
    ).buffer;
    const base64Images = [];
    const pdf = await pdfjsLib.getDocument(arrayBuffer).promise;
    const totalPages = pdf.numPages;

    for (let pageNumber = 1; pageNumber <= totalPages; pageNumber++) {
      const page = await pdf.getPage(pageNumber);
      const viewport = page.getViewport({ scale: scalePreview || 2 });
      const canvas = document.createElement("canvas");
      const canvasContext = canvas.getContext("2d");
      canvas.height = viewport.height;
      canvas.width = viewport.width;
      canvasContext.imageSmoothingEnabled = true;
      canvasContext.imageSmoothingQuality = "high";

      await page.render({ canvasContext, viewport }).promise;

      const base64Image = canvas
        .toDataURL("image/jpeg")
        .replace(/^data:image\/(png|jpg|jpeg);base64,/, "");
      base64Images.push({
        contractPage: pageNumber,
        fileBase64: base64Image,
        width: canvas.width,
        height: canvas.height,
        signatureSettings: [],
      });
    }

    return base64Images;
  };

  previewContract() {
    this.contractFilePreview = this.contractFilePreview.map((file, index1) => {
      return {
        ...file, signatureSettings: file.signatureSettings.map((signature, index2) => {
          let valueSignature = this.signatureElectronic?.find(item => item?.signerSignatureSettingid === signature.id)
          return {
            ...signature, valueInput: signature.signatureType === ContractSettingType.DatePicker || signature.signatureType === ContractSettingType.Text ? this.contractfiles[index1]?.signatureSettings[index2].valueInput : valueSignature?.signartureBase64
          }
        }
        )
      }
    })

    const dialogRef = this.dialog.open(PreviewContractComponent, {
      data: {
        contractFilePreviewDesign: this.contractFilePreview,
        isSignedPreview: true,
        contractFileName: this.contractInfo?.contractName
      },
      width: '1000px',
      maxWidth: '1000px',
      panelClass: 'email-dialog',
    })
    dialogRef.afterClosed().subscribe(rs => {
      if (rs) {
      }
    })
  }
  downloadPdf(fileName: string): void {
    this.dialog.open(DialogDownloadComponent, {
      data: {
        fileName: fileName,
        idContract: this.contracId,
      },
      minWidth: '36%',
      minHeight: '250px',
    })
  }

  displayIconSignatureSidebar(signatureType: number) {
    return this.signatureTypeList.find(signatureTypeItem => signatureTypeItem.id === signatureType).icon
  }
}
