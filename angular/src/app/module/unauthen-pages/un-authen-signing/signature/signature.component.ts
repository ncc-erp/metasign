import { AppComponentBase } from "@shared/app-component-base";
import { map } from "rxjs/operators";
import {
  Component,
  OnInit,
  Input,
  Injector,
  Output,
  EventEmitter,
  ElementRef,
  ViewChild,
} from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { SignatureDialogComponent } from "../signature-dialog/signature-dialog.component";
import { DomSanitizer } from "@angular/platform-browser";
import { ContractSettingType } from "@shared/AppEnums";
import { SignatureSettings } from "@app/service/model/design-contract.dto";
import * as moment from "moment";
import { AppConsts } from "@shared/AppConsts";
import { SignatureDialogStampComponent } from "../signature-dialog-stamp/signature-dialog-stamp.component";
@Component({
  selector: "app-signature",
  templateUrl: "./signature.component.html",
  styleUrls: ["./signature.component.css"],
})
export class SignatureComponent extends AppComponentBase implements OnInit {
  @ViewChild('myTextarea') myTextarea: ElementRef;

  @Input() signature: SignatureSettings;
  @Input() signatureSetting;
  @Input() imageSignatureDigital: string;
  @Input() statusEmitSignatureDigital: boolean
  @Output() signatureValue = new EventEmitter<any>();
  @Output() contractValue = new EventEmitter<any>();
  totalLineTextarea: string[];
  ContractSettingType = ContractSettingType;
  valueContractText: string;
  height: number
  @Input() signatureDefaultElectronic: any;
  @Input() signatureDefaultStamp: any;

  constructor(
    private injector: Injector,
    public dialog: MatDialog,
    public sanitizer: DomSanitizer,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.height = this.signature.height
    this.valueContractText = this.signature.valueInput
  }

  ngAfterViewInit() {
    if (this.signature.isAllowSigning) {
      this.handleValue()
    }
    if (this.signature.signatureType === ContractSettingType.DatePicker) {
      this.onDate()
    }
  }

  handleValue() {
    this.signature.valueInput = this.valueContractText
    this.totalLineTextarea = this.handleLineTextarea(this.signature)
    this.signature.valueInput = this.totalLineTextarea.join('\n')
    this.contractValue.emit(this.signature);
    this.height = this.signature.height
    let heightOneLine = 25 * (this.signature.fontSize / (AppConsts.fontSize[AppConsts.defaultFontSize]))
    if (this.signature.height / heightOneLine < this.totalLineTextarea.length) {
      this.height = this.totalLineTextarea.length * heightOneLine
    }
  }

  handleLineTextarea(signature) {
    let totalLines = []
    if (
      signature?.signatureType === ContractSettingType.Text &&
      signature.valueInput !== undefined
    ) {
      const inputCanvas = document.createElement('canvas');
      const inputCtx = inputCanvas.getContext('2d');
      inputCtx.font = `${signature.fontSize}px ${signature?.fontFamily}`;
      let totalWords = signature.valueInput?.split("\n");
      let widthTextarea = signature.width;
      totalWords?.forEach(function (word) {
        var currentLine = "";
        let lines = [];
        let wordItem = word.split(" ");
        wordItem.forEach((item) => {
          var testLine = currentLine + item + " ";
          var metrics = inputCtx.measureText(testLine);
          var lineWidth = metrics.width;
          if (lineWidth >= widthTextarea && currentLine) {
            lines.push(currentLine);
            currentLine = item + " ";
          } else {
            currentLine = testLine;
          }
        });
        lines.push(currentLine);
        totalLines = [...totalLines, ...lines];
      });
    }
    return totalLines;
  }

  onDate() {
    this.signature.valueInput = moment(this.valueContractText).format("DD/MM/YYYY").toString()
    this.contractValue.emit(this.signature);
  }

  openDialog(): void {
    let dialogRef
    if (this.signature.signatureType === ContractSettingType.Stamp) {
      dialogRef = this.dialog.open(SignatureDialogStampComponent, {
        data: {
          signature: this.signature,
          signatureDefault: this.signatureDefaultStamp?.contractBase64,
          signatureSetting: this.signatureSetting,
          width: this.signature.width,
          height: this.signature.height,
          signatureType: this.signature.signatureType
        },
        height: "85%",
        width: "45%",
        panelClass: 'signature-dialog',
      });
    }
    else {
      dialogRef = this.dialog.open(SignatureDialogComponent, {
        data: {
          signature: this.signature,
          signatureDefault: this.signatureDefaultElectronic?.contractBase64,
          signatureSetting: this.signatureSetting,
          width: this.signature.width,
          height: this.signature.height,
          signatureType: this.signature.signatureType
        },
        height: "85%",
        width: "45%",
        panelClass: 'signature-dialog',
      });
    }


    dialogRef
      .afterClosed()
      .pipe(
        map((value: any) => {
          if (value) {
            return {
              signerSignatureSettingid: this.signature.id,
              signartureBase64: value.base64,
              isNewSignature: value.isNewSignature,
              setDefault: value.setDefault,
              signatureUserId: null,
              signatureType: this.signature.signatureType,
              pageHeight: this.signature.heightPage
            };
          } else {
            return null;
          }
        })
      )
      .subscribe((value) => {
        if (value) {
          if (value.signatureType === ContractSettingType.Stamp) {
            this.signatureDefaultStamp =
            {
              contractBase64: value.signartureBase64,
              signatureType: value.signatureType
            }

          }
          else {
            if (this.signatureDefaultElectronic) {
              this.signatureDefaultElectronic =
              {
                contractBase64: value.signartureBase64,
                signatureType: value.signatureType
              }
            }
          }
          this.signatureValue.emit(value);
        }
      });
  }

  handleClickSignatureDigital() {
    let signature = {
      signerSignatureSettingid: this.signature.id,
      signatureType: this.signature.signatureType,
      page: this.signature.page,
      x: Math.round(this.signature.positionX),
      y: Math.round(this.signature.positionY),
      width: this.signature.width,
      height: this.signature.height,
      pageHeight: this.signature.heightPage
    }
    if (this.statusEmitSignatureDigital) {
      this.signatureValue.emit(signature);
    }
  }

  handleClickSignatureElectronic() {
    if (this.signatureDefaultElectronic?.contractBase64) {
      let signature = {
        signerSignatureSettingid: this.signature.id,
        signartureBase64: this.signatureDefaultElectronic?.contractBase64,
        isNewSignature: false,
        signatureType: this.signature.signatureType,
        pageHeight: this.signature.heightPage
      }
      this.signatureValue.emit(signature);
    } else {
      this.openDialog();
    }
  }

  handleClickSignatureStamp() {

    if (this.signatureDefaultStamp?.contractBase64) {
      let signature = {
        signerSignatureSettingid: this.signature.id,
        signartureBase64: this.signatureDefaultStamp.contractBase64,
        isNewSignature: false,
        signatureType: this.signature.signatureType,
        pageHeight: this.signature.heightPage
      }

      this.signatureValue.emit(signature);
    } else {
      this.openDialog();
    }
  }
}
