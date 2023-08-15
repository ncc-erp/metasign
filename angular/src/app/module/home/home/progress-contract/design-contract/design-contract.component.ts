import { ContractTemplateSettingService } from "./../../../../../service/api/contract-template-setting.service";
import { ContractSettingType } from "@shared/AppEnums";
import * as pdfjsLib from "pdfjs-dist/webpack";
import {
  SignatureSettings,
  SignerContract,
  contractInput,
} from "./../../../../../service/model/design-contract.dto";
import { contractStep } from "./../../../../../../shared/AppEnums";
import { debounceTime, switchMap, catchError } from "rxjs/operators";
import {
  Component,
  ElementRef,
  Injector,
  OnInit,
  ViewChild,
  ViewChildren,
} from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { ContractSettingService } from "@app/service/api/contract-seting.service";
import { ContractService } from "@app/service/api/contract.service";
import { DomSanitizer } from "@angular/platform-browser";
import { AppConsts } from "@shared/AppConsts";
import { SignerSignatureSettingService } from "@app/service/api/signer-signature-setting.service";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import {
  trigger,
  state,
  style,
  animate,
  transition,
} from "@angular/animations";
import { SetLocalStorageContract } from "@shared/helpers/FunctionsHelper";
import { Subject } from "rxjs";

import { AppComponentBase } from "@shared/app-component-base";
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";
import { ContractTemplateService } from "@app/service/api/contract-template.service";
import { ContractTemplateSignerService } from "@app/service/api/contract-template-signer.service";
import { MatDialog } from "@angular/material/dialog";
import { PreviewContractComponent } from "../upload-contract/preview-contract/preview-contract.component";
@Component({
  selector: "app-design-contract",
  templateUrl: "./design-contract.component.html",
  styleUrls: ["./design-contract.component.css"],
  animations: [
    trigger("contractSelect", [
      state(
        "open",
        style({
          left: 0,
        })
      ),
      state(
        "closed",
        style({
          left: "100%",
        })
      ),
      transition("open => closed", [animate("0.2s")]),
      transition("closed => open", [animate("0.2s")]),
    ]),
  ],
})
export class DesignContractComponent
  extends AppComponentBase
  implements OnInit {
  contractId: number;
  step: number;
  signerContract: SignerContract[];
  valueSignerContract: number;
  fieldsColor: string;
  contractFile: any;
  contractFilePreview: any;
  scalePreview: number = 1.45
  signerActionForm: FormGroup;
  focusSignatureId: number;
  valueSignerContractEdit: number;
  oldValueSignerContractEdit: number;
  currentSigner: any;
  contractName: string;
  indexPdf: number;
  onDrag: boolean = false;
  contractInput: contractInput;
  fontList = AppConsts.fontFamily;
  fontSizeList = AppConsts.fontSize;
  fontColorlist = AppConsts.fontColor;
  fontFamily: string = this.fontList[0];
  fontSize: number = this.fontSizeList[AppConsts.defaultFontSize];
  fontColor: string = this.fontColorlist[0].value;
  contractInputUpdate = new Subject<string>();
  signatureType = ContractSettingType;
  contractLoadding: boolean = true;
  templateContractId: number;
  templateContract: boolean;
  useTemplateContract: boolean;
  signerChange: any
  sizeTextArea: { width: number, height: number, fontSize: number, fontFamily: string } = { width: AppConsts.DEFAULT_INPUT_WIDTH, height: AppConsts.DEFAULT_INPUT_HEIGHT, fontSize: this.fontSizeList[AppConsts.defaultFontSize], fontFamily: this.fontList[0] }
  initSizeTextArea: { width: number, height: number } = { width: AppConsts.DEFAULT_INPUT_WIDTH, height: AppConsts.DEFAULT_INPUT_HEIGHT }
  sizeDatePicker: { width: number, height: number, fontSize: number, fontFamily: string } = { width: AppConsts.DEFAULT_INPUT_WIDTH, height: AppConsts.DEFAULT_INPUT_HEIGHT, fontSize: this.fontSizeList[AppConsts.defaultFontSize], fontFamily: this.fontList[0] }
  templateContractEdit: boolean;
  private isCheckSignerType: boolean;
  private isCheckInputType: boolean;
  messageCheck: string;
  private currentSiger: SignatureSettings[] = [];
  private isCheckSigner: boolean;
  private isPreOrder: boolean;
  isCheckLengthSigner: number[] = []
  isCheckLengthInput: number[] = []
  signatureTypeList = AppConsts.signatureTypeList;
  otherTypeList = AppConsts.otherTypeList;

  @ViewChild("dropZone", { read: ElementRef }) dropZone: ElementRef;
  @ViewChildren("page") elements: any;

  constructor(
    injector: Injector,
    private contractService: ContractService,
    private signerSignatureSettingService: SignerSignatureSettingService,
    private router: Router,
    private route: ActivatedRoute,
    private contractSettingService: ContractSettingService,
    private fb: FormBuilder,
    private contractTemplateService: ContractTemplateService,
    private contractTemplateSettingService: ContractTemplateSettingService,
    private dialog: MatDialog
  ) {
    super(injector);

    this.templateContractId = Number(
      this.route.snapshot.queryParamMap.get("templateContractId")
    );

    this.templateContract = Boolean(
      this.route.snapshot.queryParamMap.get("templateContract")
    );

    this.contractId = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("contractId"))
    )?.contractId;

    this.step = Number(this.route.snapshot.queryParamMap.get("step"));
    SetLocalStorageContract(this.contractId, this.step, false);

    this.contractInputUpdate
      .pipe(debounceTime(300))
      .subscribe(() => this.handleChangePositionSignature());
  }

  ngOnInit(): void {
    this.signerActionForm = this.fb.group({
      top: [null, [Validators.min(0), Validators.max(1544)]],
      left: [null, [Validators.min(0), Validators.max(1544)]],
      width: [{ value: null, disabled: true }],
      height: [{ value: null, disabled: true }],
    });

    if (this.contractId) {
      this.contractSettingService
        .GetSettingByContractId(this.contractId)
        .subscribe((value) => {
          this.signerContract = value.result.signers
            .filter((value) => {
              return value.contractRole !== 3;
            })
            .sort((a, b) => a.procesOrder - b.procesOrder);
          this.valueSignerContract = this.signerContract[0].id;
          this.fieldsColor = this.signerContract[0].color;
          this.contractName = this.signerContract[0].contractFileName;
          this.isPreOrder = this.signerContract.every(item => item.procesOrder === 1);
          this.isCheckSigner = this.signerContract.length > 1 && this.isPreOrder;
        });

      this.signerSignatureSettingService
        .getSignatureSettingForContractDesign(this.contractId)
        .subscribe(async (rs) => {
          this.contractFile = await this.generateImageStringsFromBase64PDF(
            rs.result.contractBase64.split(",")[1], 2
          );

          this.contractFile.forEach((value) => {
            return (value.fileBase64 =
              "data:image/jpeg;base64," + value.fileBase64);
          });


          this.contractFilePreview = await this.generateImageStringsFromBase64PDF(
            rs.result.contractBase64.split(",")[1], this.scalePreview
          );

          this.contractFilePreview.forEach((value) => {
            return (value.fileBase64 =
              "data:image/jpeg;base64," + value.fileBase64);
          });

          this.getContractSignatureSetting(rs.result);
          this.contractLoadding = false;
          this.isCheckType(rs.result.signatureSettings)
        });
      return
    }

    if (this.templateContractId) {
      this.contractTemplateService
        .getContractTemplate(this.templateContractId)
        .subscribe(async (rs) => {
          this.contractFile = await this.generateImageStringsFromBase64PDF(
            rs.result.contractTemplate.content.split(",")[1], 2
          );
          this.contractName = rs.result.contractTemplate.fileName;
          this.contractFile.forEach((value) => {
            return (value.fileBase64 =
              "data:image/jpeg;base64," + value.fileBase64);
          });

          this.getContractSignatureSetting(rs.result);
          this.signerContract = rs.result.signerSettings
            .filter((value) => {
              return value.contractRole !== 3;
            })
            .sort((a, b) => a.procesOrder - b.procesOrder);
          this.valueSignerContract = this.signerContract[0].id;
          this.fieldsColor = this.signerContract[0].color;
          this.contractName = rs.result.contractTemplate.fileName;
          this.contractFilePreview = await this.generateImageStringsFromBase64PDF(
            rs.result.contractTemplate.content.split(",")[1], this.scalePreview
          );
          this.contractFilePreview.forEach((value) => {
            return (value.fileBase64 =
              "data:image/jpeg;base64," + value.fileBase64);
          });

          this.getContractSignatureSetting(rs.result);
          this.contractLoadding = false;
        });
    }
  }

  getContractSignatureSetting(signature) {
    this.contractFile.forEach((contractPage) => {
      contractPage.signatureSettings = [];
      signature.signatureSettings.forEach((signature) => {
        if (signature.page === contractPage.contractPage) {
          contractPage.signatureSettings.push(signature);
        }
      });
    });
    this.contractFilePreview?.forEach((contractPage) => {
      contractPage.signatureSettings = [];
      signature.signatureSettings.forEach((signature) => {
        if (signature.page === contractPage.contractPage) {
          contractPage.signatureSettings.push({ ...signature, positionX: signature.positionX * (this.scalePreview / 2), positionY: signature.positionY * (this.scalePreview / 2), width: signature.width * (this.scalePreview / 2), height: signature.height * (this.scalePreview / 2), fontSize: signature.fontSize * (this.scalePreview / 2) });
        }
      });
    });
  }

  generateImageStringsFromBase64PDF = async (base64PDF, scale: number) => {
    const arrayBuffer = Uint8Array.from(atob(base64PDF), (c) =>
      c.charCodeAt(0)
    ).buffer;
    const base64Images = [];
    const pdf = await pdfjsLib.getDocument(arrayBuffer).promise;
    const totalPages = pdf.numPages;

    for (let pageNumber = 1; pageNumber <= totalPages; pageNumber++) {
      const page = await pdf.getPage(pageNumber);
      const viewport = page.getViewport({ scale: scale });
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

  handleScrollPage(idPage) {
    this.indexPdf = idPage;
    document.getElementById(`${idPage}`).scrollIntoView({ behavior: "smooth" });
  }

  handleBlurcontract() {
    this.focusSignatureId = null;
  }
  handleChangePositionSignature() {
    this.currentSigner.fontColor = this.fontColor;
    this.currentSigner.fontFamily = this.fontFamily;
    this.currentSigner.fontSize = this.fontSize;
    this.currentSigner.positionX = this.signerActionForm.value.left;
    this.currentSigner.positionY = this.signerActionForm.value.top;
    this.updateSignature(this.currentSigner);
  }

  handleChangesigner() {
    const signer = this.signerContract.find(
      (value) => value.id === this.valueSignerContract
    );

    this.fieldsColor = signer.color;
  }

  handleChangesignerEdit() {
    this.currentSigner.contractSettingId = this.valueSignerContractEdit;
    if (this.templateContractId) {
      this.currentSigner.contractTemplateSignerId = this.valueSignerContractEdit;
    }
    this.updateSignature(this.currentSigner);
  }

  valueSignatureFocusId($event) {
    this.checkSignatureType($event.signatureType);
    if (
      $event.signatureType === ContractSettingType.Text ||
      $event.signatureType === ContractSettingType.DatePicker
    ) {
      this.fontColor = $event.fontColor;
      this.fontFamily = $event.fontFamily;
      this.fontSize = $event.fontSize;
    }
    this.focusSignatureId = $event.id;
    this.valueSignerContractEdit = this.contractId ? $event.contractSettingId : $event.contractTemplateSignerId;
    this.oldValueSignerContractEdit = this.valueSignerContractEdit;
    this.currentSigner = $event;
    this.signerActionForm.patchValue({
      top: $event.positionY,
      left: $event.positionX,
      width: $event.width,
      height: $event.height,
    });
  }

  valueUpdateSignature($event) {
    if ($event.signatureType === ContractSettingType.Text) {
      this.sizeTextArea.width = $event.width
      this.sizeTextArea.height = $event.height
    }

    const signature = {
      ...$event,
      positionX: $event.dx || $event.positionX,
      positionY: $event.dy || $event.positionY,
    };

    delete signature.dropPoint;
    this.signerActionForm.patchValue({
      top: signature.positionY,
      left: signature.positionX,
    });
    this.currentSigner = signature;
    this.updateSignature(signature);
  }

  updateSignature(value) {
    if (this.contractId) {
      this.signerSignatureSettingService
        .updateSignerSignatureSetting(value)
        .pipe(
          catchError((): any => {
            this.valueSignerContractEdit = this.oldValueSignerContractEdit
            this.currentSigner.contractSettingId = this.valueSignerContractEdit;
          }
          ),
          switchMap(() =>
            this.signerSignatureSettingService.getSignatureSettingForContractDesign(
              this.contractId
            )
          )
        )
        .subscribe((rs) => {
          this.getContractSignatureSetting(rs.result);
          this.isCheckType(rs.result.signatureSettings)
        });
    } else {
      let payload = {
        id: value.id,
        contractTemplateSignerId: value.contractTemplateSignerId,
        signatureType: value.signatureType,
        signatureTypeId: 0,
        page: value.page,
        positionX: value.positionX,
        positionY: value.positionY,
        width: value.width,
        height: value.height,
        fontSize: value.fontSize,
        fontFamily: value.fontFamily,
        fontColor: value.fontColor,
        isSigned: false,
        valueInput: value.valueInput
      };

      this.contractTemplateSettingService
        .updateContractTemplateSetting(payload)
        .pipe(
          catchError((): any => {
            this.valueSignerContractEdit = this.oldValueSignerContractEdit
            this.currentSigner.contractTemplateSignerId = this.valueSignerContractEdit;
          }
          ),
          switchMap(() =>
            this.contractTemplateService.getContractTemplate(
              this.templateContractId
            )
          )
        )
        .subscribe((rs) => {
          this.getContractSignatureSetting(rs.result);
        });
    }
  }

  valueDeleteSignature(event) {
    this.focusSignatureId = null;
    if (this.contractId) {
      this.signerSignatureSettingService
        .deleteSignerSignatureSetting(event.id)
        .pipe(
          switchMap(() => {
            this.currentSiger = this.currentSiger?.filter(x => x.id !== event.id)
            this.isCheckLengthInput = this.isCheckLengthInput?.filter(x => x !== event.contractSettingId)
            this.isCheckLengthSigner = this.isCheckLengthSigner?.filter(x => x !== event.contractSettingId)
            if (this.currentSiger.length === 0 || this.isCheckLengthSigner.length === 0 || this.isCheckLengthInput.length === 0) {
              this.messageCheck = undefined
            }
            return this.signerSignatureSettingService.getSignatureSettingForContractDesign(
              this.contractId
            );
          })
        )
        .subscribe((rs) => {
          this.getContractSignatureSetting(rs.result);
          this.isCheckType(rs.result.signatureSettings)
        });
      return
    }
    if (this.templateContractId) {
      this.contractTemplateSettingService
        .deleteContractTemplateSetting(event.id)
        .pipe(
          switchMap(() => {
            return this.contractTemplateService.getContractTemplate(
              this.templateContractId
            );
          })
        )
        .subscribe((rs) => {
          this.getContractSignatureSetting(rs.result);
        });
      return;
    }
  }

  changeContractInput() {
    this.contractInputUpdate.next();
  }

  handlechangeFontSize(event) {
    if (this.currentSigner.signatureType === ContractSettingType.Text) {
      this.sizeTextArea.fontSize = event.value
    }
    if (this.currentSigner.signatureType === ContractSettingType.DatePicker) {
      this.sizeDatePicker.fontSize = event.value
      this.currentSigner.width = 25 * this.sizeDatePicker.fontSize / 2
      this.currentSigner.height = 2 * this.sizeDatePicker.fontSize + 4
    }
    this.handleChangePositionSignature();
  }

  handlechangeFontFamily(event) {
    if (this.currentSigner.signatureType === ContractSettingType.Text) {
      this.sizeTextArea.fontFamily = event.value
    }
    if (this.currentSigner.signatureType === ContractSettingType.DatePicker) {
      this.sizeDatePicker.fontFamily = event.value
    }
    this.handleChangePositionSignature();
  }

  handleChangeColor() {
    this.handleChangePositionSignature();
  }

  checkSignatureType(value) {
    switch (value) {
      case ContractSettingType.Text:
        this.contractInput = {
          value: ContractSettingType.Text,
          lable: "Text",
        };
        return {
          width: 25 * this.sizeTextArea.fontSize / 2 || AppConsts.DEFAULT_INPUT_WIDTH,
          height: 2 * this.sizeTextArea.fontSize + 4 || AppConsts.DEFAULT_INPUT_HEIGHT,
          fontSize: this.sizeTextArea.fontSize,
          fontFamily: this.sizeTextArea.fontFamily,
          fontColor: this.fontColor,
        };
      case ContractSettingType.Electronic:
        this.contractInput = {
          value: ContractSettingType.Electronic,
          lable: "Signature",
        };
        return {
          width: AppConsts.DEFAULT_SIGNATURE_WIDTH,
          height: AppConsts.DEFAULT_SIGNATURE_HEIGHT,
        };
      case ContractSettingType.DatePicker:
        this.contractInput = {
          value: ContractSettingType.DatePicker,
          lable: "Date",
        };

        return {
          width: 25 * this.sizeDatePicker.fontSize / 2 || AppConsts.DEFAULT_INPUT_WIDTH,
          height: 2 * this.sizeDatePicker.fontSize + 4 || AppConsts.DEFAULT_INPUT_HEIGHT,
          fontSize: this.sizeDatePicker.fontSize,
          fontFamily: this.sizeDatePicker.fontFamily,
          fontColor: this.fontColor,
        };
      case ContractSettingType.Digital:
        this.contractInput = {
          value: ContractSettingType.Digital,
          lable: "USBToken",
        };
        return {
          width: AppConsts.DEFAULT_SIGNATURE_WIDTH_DIGITAL,
          height: AppConsts.DEFAULT_SIGNATURE_HEIGHT_DIGITAL,
        };

      case ContractSettingType.Stamp:
        this.contractInput = {
          value: ContractSettingType.Stamp,
          lable: "Stamp",
        };
        return {
          width: AppConsts.DEFAULT_SIGNATURE_WIDTH_DIGITAL,
          height: AppConsts.DEFAULT_SIGNATURE_HEIGHT_DIGITAL,
        };
    }
  }

  handleDragEnd(type) {
    this.onDrag = false
  }

  drop(event: any) {
    const containerBounds =
      event.container.element.nativeElement.getBoundingClientRect();
    const itemBounds = event.dropPoint;
    const indexPage = event.container.element.nativeElement.id;
    const signatureTypeId = event.item.element.nativeElement.data;
    let positionInContainer = {
      x: itemBounds.x - containerBounds.left,
      y: itemBounds.y - containerBounds.top,
    };
    const signatureType = this.checkSignatureType(signatureTypeId);
    this.signerActionForm.patchValue({
      top: positionInContainer.y,
      left: positionInContainer.x,
      ...signatureType,
    });
    const signature = {
      contractSettingId: this.valueSignerContract,
      isSigned: false,
      positionX: positionInContainer.x,
      positionY: positionInContainer.y,
      signatureType: signatureTypeId,
      page: +indexPage,
      ...signatureType,
    };

    if (
      itemBounds.x > containerBounds.right - signatureType.width
    ) {
      signature.positionX = positionInContainer.x - signatureType.width;
      signature.positionY = positionInContainer.y;
    }

    if (
      itemBounds.y > containerBounds.bottom - signatureType.height
    ) {
      signature.positionX = positionInContainer.x - signatureType.width;
      signature.positionY = positionInContainer.y - signatureType.height;
    }

    if (
      itemBounds.x < containerBounds.left + signatureType.width
      && itemBounds.y > containerBounds.bottom - signatureType.height
    ) {
      signature.positionX = positionInContainer.x;
      signature.positionY = positionInContainer.y - signatureType.height;
    }

    if (
      itemBounds.x >= containerBounds.left &&
      itemBounds.x <= containerBounds.right &&
      itemBounds.y >= containerBounds.top &&
      itemBounds.y <= containerBounds.bottom
    ) {

      if (this.contractId) {
        this.signerSignatureSettingService
          .createSignerSignatureSetting(signature)
          .pipe(
            catchError((): any => {
              this.valueSignerContractEdit = this.oldValueSignerContractEdit
              this.currentSigner.contractSettingId = this.valueSignerContractEdit;
            }
            ),
            switchMap((value: any) => {
              this.focusSignatureId = value.result;
              this.currentSigner = {
                id: value.result,
                ...signature,
              };
              return this.signerSignatureSettingService.getSignatureSettingForContractDesign(
                this.contractId
              );
            })
          )
          .subscribe((rs) => {
            this.getContractSignatureSetting(rs.result);
            this.valueSignerContractEdit = this.valueSignerContract;
            this.oldValueSignerContractEdit = this.valueSignerContractEdit
            this.isCheckType(rs.result.signatureSettings)
          });
      }
      else {
        let payload = {
          isSigned: false,
          contractTemplateSignerId: this.valueSignerContract,
          signatureType: signature.signatureType,
          signatureTypeId: 0,
          page: signature.page,
          positionX: signature.positionX,
          positionY: signature.positionY,
          width: signature.width,
          height: signature.height,
          fontSize: signature.fontSize,
          fontFamily: signature.fontFamily,
          fontColor: signature.fontColor,
        };

        this.contractTemplateSettingService
          .createContractTemplateSetting(payload)
          .pipe(
            catchError((): any => {
              this.valueSignerContractEdit = this.oldValueSignerContractEdit
              this.currentSigner.contractTemplateSignerId = this.valueSignerContractEdit;
            }
            ),
            switchMap((rs: any) => {
              this.focusSignatureId = rs.result;
              this.currentSigner = {
                id: rs.result,
                ...payload,
              };
              return this.contractTemplateService.getContractTemplate(
                this.templateContractId
              );
            })
          )
          .subscribe((value) => {
            this.getContractSignatureSetting(value.result);
            this.valueSignerContractEdit = this.valueSignerContract;
            this.oldValueSignerContractEdit = this.valueSignerContractEdit
          });
        return;
      }
    }
  }

  isCheckType(data) {
    let idInputIncludes = [];
    let idSigner = [];
    const seenValues = [];
    this.isCheckSignerType = data.some(
      (x) =>
        x.signatureType === ContractSettingType.Electronic ||
        x.signatureType !== ContractSettingType.Digital || x.signatureType === ContractSettingType.Stamp
    );
    this.isCheckInputType = data.some(
      (x) =>
        x.signatureType !== ContractSettingType.DatePicker &&
        x.signatureType !== ContractSettingType.Text
    );

    data.forEach((itemId) => {
      if (
        itemId.signatureType === ContractSettingType.Text ||
        itemId.signatureType === ContractSettingType.DatePicker
      ) {
        idInputIncludes.push(itemId.contractSettingId);
      }

      if (
        itemId.signatureType === ContractSettingType.Electronic ||
        itemId.signatureType === ContractSettingType.Digital || itemId.signatureType === ContractSettingType.Stamp
      ) {
        idSigner.push(itemId.contractSettingId);
      }
    });

    this.isCheckLengthInput = Array.from(new Set(idInputIncludes));

    this.isCheckLengthSigner = Array.from(new Set(idSigner));

    if (this.isCheckSigner) {
      if (
        this.isCheckSignerType &&
        this.isCheckLengthSigner.length === this.signerContract.length || this.isCheckSignerType && this.isCheckLengthSigner.length >= 1
      ) {
        data.forEach((value) => {
          if (
            (value.signatureType === ContractSettingType.Text) ||
            (value.signatureType === ContractSettingType.DatePicker) &&
            !seenValues.includes(value)
          ) {
            seenValues.push(value);
            this.currentSiger = seenValues;
            this.messageCheck = `
              <div style="font-size: 17px;">
              ${this.ecTransform('OtherSignersWillReceiveAnEmailAfter')}
              </div>`;
            return
          }
        });
      }
      if (
        (this.isCheckInputType &&
          this.isCheckLengthInput.length === this.signerContract.length) || (this.isCheckInputType && this.isCheckLengthInput.length >= 2)
      ) {
        data.forEach((value) => {
          if (
            (value.signatureType === ContractSettingType.Electronic) ||
            (value.signatureType === ContractSettingType.Digital) || (value.signatureType === ContractSettingType.Stamp) &&
            !seenValues.includes(value)
          ) {
            seenValues.push(value);
            this.currentSiger = seenValues;
            this.messageCheck = `
            <div style="font-size: 17px;">
            ${this.ecTransform('OtherSignersWillReceiveAnEmailAfter')}
            </div>`;
            return;
          }
        });
      }
    }
  }

  handleNext() {
    if (this.templateContractId && this.templateContract) {
      this.contractTemplateService.updateProcessOrder(this.templateContractId).pipe().subscribe()
      this.router.navigate(["/app/templates"]);
      abp.message.success(this.ecTransform('TemplateSavedSuccessfully'));
      return
    }
    let contractSettingIdList = [];
    const encode = this.route.snapshot.queryParamMap.get("contractId");
    this.contractFile.forEach((contractFile) => {
      contractFile.signatureSettings.forEach((signature) => {
        contractSettingIdList.push(this.contractId ? signature.contractSettingId : signature.contractTemplateSignerId);
      });
    });

    let signatureUnique = Array.from(new Set(contractSettingIdList));

    if (this.signerContract.length === signatureUnique.length) {

      if (this.messageCheck) {
        abp.message.confirm(this.messageCheck, this.ecTransform('AreYouSureYouWantToContinue'), (res) => {
          if (res) {
            this.contractService._currentStep.next(contractStep.EmailSetting);
            this.contractService.updateProcessOrder(this.contractId).pipe().subscribe();
            this.router.navigate(["/app/home/process/emailSetting"], {
              queryParams: {
                contractId: encode,
                step: contractStep.EmailSetting,
              },
              queryParamsHandling: "merge",
            });
          }
          return
        }, { isHtml: true })
        return
      }

      this.contractService._currentStep.next(contractStep.EmailSetting);
      this.router.navigate(["/app/home/process/emailSetting"], {
        queryParams: {
          contractId: encode,
          step: contractStep.EmailSetting,
        },
        queryParamsHandling: "merge",
      });
      return
    } else {
      abp.message.error(this.ecTransform('MissingSignatureLocationForRecipients'));
    }
  }

  handleBack() {
    this.contractFile.forEach((contractFile) => {
      contractFile.signatureSettings.forEach((signature) => {
        this.updateSignature(signature)
      });
    });
    if (this.contractId) {
      const encode = this.route.snapshot.queryParamMap.get("contractId");
      this.contractService._currentStep.next(contractStep.SignerSetting);
      this.router.navigate(["/app/home/process/setting"], {
        queryParams: {
          contractId: encode,
          ...(this.templateContractId && { templateContractId: this.templateContractId }),
          step: contractStep.SignerSetting,
        },
        queryParamsHandling: "merge",
      });
      return
    }

    if (this.templateContractId) {
      this.contractService._currentStep.next(contractStep.SignerSetting);
      this.router.navigate(["/app/templates/templates-create/setting"], {
        queryParams: {
          templateContractId: this.templateContractId,
          templateContract: true,
          step: contractStep.SignerSetting,
        },
      });
      return;
    }
  }

  isAllowEditSignature() {
    return this.isGranted(
      PERMISSIONS_CONSTANT.ProcessStep_StepSignature_EditSignature
    );
  }

  isAllowAddSignature() {
    return this.isGranted(
      PERMISSIONS_CONSTANT.ProcessStep_StepSignature_AddSignature
    );
  }

  isAllowAddOtherSignature() {
    return this.isGranted(
      PERMISSIONS_CONSTANT.ProcessStep_StepSignature_AddCustomeSignature
    );
  }

  previewContract() {
    this.contractFile.forEach((contractFile) => {
      contractFile.signatureSettings.forEach((signature) => {
        this.updateSignature(signature)
      });
    });
    const dialogRef = this.dialog.open(PreviewContractComponent, {
      data: {
        contractFile: this.contractFile,
        contractFilePreviewDesign: this.contractFilePreview,
        isSignedPreview: true,
        contractFileName: this.contractName
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
}
