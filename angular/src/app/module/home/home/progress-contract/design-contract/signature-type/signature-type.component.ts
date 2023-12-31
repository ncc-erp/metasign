import { ContractSettingType } from "./../../../../../../../shared/AppEnums";
import {
  Output,
  Input,
  ViewChild,
  ElementRef,
  EventEmitter,
  Injector,
  HostListener,
} from "@angular/core";
import { CdkDragEnd } from "@angular/cdk/drag-drop";
import { Component } from "@angular/core";
import { SignatureSettings } from "@app/service/model/design-contract.dto";
import { AppComponentBase } from "@shared/app-component-base";
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";
import { ActivatedRoute } from "@angular/router";
import { AppConsts } from "@shared/AppConsts";

@Component({
  selector: "app-signature-type",
  templateUrl: "./signature-type.component.html",
  styleUrls: ["./signature-type.component.css"],
})
export class SignatureTypeComponent extends AppComponentBase {
  private isResizing = false;
  private px = 0;
  private py = 0;
  private initialInputWidth;
  private initialInputHeight;
  public width;
  public height;
  public isTextareaFocused: boolean = false;
  public onDrag: boolean;
  public valueInputTextArea: string
  public batchContract: boolean = false
  @Input() signatureValue: SignatureSettings;
  @Input() initSizeTextArea;
  @Input() initSizeDatePicker;
  @Input() focusSignatureId: number;
  @Output() updatePositionSignature = new EventEmitter();
  @Output() deletePositionSignature = new EventEmitter();
  @Output() signatureFocusId = new EventEmitter();
  @ViewChild("signatureType", { read: ElementRef }) signatureType: ElementRef;
  @ViewChild("textarea", { read: ElementRef }) textarea: ElementRef;
  @ViewChild("previewinput", { read: ElementRef }) previewinput: ElementRef;
  ContractSettingType = ContractSettingType;
  constructor(injector: Injector, private route: ActivatedRoute,) {
    super(injector)
    this.batchContract = Boolean(
      this.route.snapshot.queryParamMap.get("batchContract")
    );

  }

  ngOnInit() {
    this.initialInputHeight = this.initSizeTextArea.height
    this.initialInputWidth = this.initSizeTextArea.width
    this.width = this.signatureValue.width
    this.height = this.signatureValue.height
  }

  ngAfterViewInit(): void {
    this.signatureType!.nativeElement.style.left = `${this.signatureValue.positionX
      }px`;
    this.signatureType.nativeElement.style.top = `${this.signatureValue.positionY
      }px`;
    if (this.signatureValue.signatureType !== (this.ContractSettingType.DatePicker || ContractSettingType.Text)) {
      this.signatureType.nativeElement.style.width = `${this.signatureValue.width}px`;
      this.signatureType.nativeElement.style.height = `${this.signatureValue.height}px`;
    }
    if ((!this.signatureValue.valueInput && !this.isTextareaFocused) || this.isTextareaFocused) {
      this.signatureType.nativeElement.style.backgroundColor = `${this.signatureValue.color}`;
    }
  }

  onTextareaFocus() {
    this.isTextareaFocused = true
    this.valueInputTextArea = this.signatureValue.valueInput
    this.signatureType.nativeElement.style.backgroundColor = 'transparent';
  }

  onTextareaFocusOut() {
    if (this.isTextareaFocused && this.valueInputTextArea !== this.signatureValue.valueInput) {
      this.updatePositionSignature.emit({ ...this.signatureValue });
    }
    this.focusSignatureId = -1
    this.isTextareaFocused = false;
    if ((!this.signatureValue.valueInput && !this.isTextareaFocused) || this.isTextareaFocused) {
      this.signatureType.nativeElement.style.backgroundColor = `${this.signatureValue.color}`;
    }
  }

  onDateChange() {
    this.updatePositionSignature.emit({ ...this.signatureValue });
  }

  handleValueTextarea($event) {
    $event.target.value ?
      this.signatureType.nativeElement.style.backgroundColor = 'transparent' : this.signatureType.nativeElement.style.backgroundColor = `${this.signatureValue.color}`;
  }

  handleDeleteSignature() {
    this.deletePositionSignature.emit(this.signatureValue);
  }

  handleSignatureFocus() {
    this.signatureFocusId.emit(this.signatureValue);
  }

  public dragEnded(event: CdkDragEnd) {
    this.isTextareaFocused = false
    this.onDrag = false
    const initialPosition = {
      x: this.signatureValue.positionX,
      y: this.signatureValue.positionY,
    };

    const offset = { ...(<any>event.source._dragRef)._passiveTransform };
    let signaturePosition = {
      ...this.signatureValue,
      dropPoint: event.dropPoint,
      dx: initialPosition.x + offset.x,
      dy: initialPosition.y + offset.y,
    };
    this.updatePositionSignature.emit(signaturePosition);
  }

  isShowDeleteSignature() {
    return this.isGranted(PERMISSIONS_CONSTANT.ProcessStep_StepSignature_RemoveSignature)
  }

  @HostListener('document: mousemove', ['$event'])
  onCornerMove(event: MouseEvent) {
    if (!this.isResizing) {
      return
    }
    let offsetX = event.clientX - this.px;
    let offsetY = event.clientY - this.py
    this.width += offsetX;
    this.height += offsetY;
    this.signatureType.nativeElement.style.transform = `translate(0,0)`;
    switch (this.signatureValue.signatureType) {
      case ContractSettingType.Text:
        this.width <= AppConsts.DEFAULT_INPUT_WIDTH ? this.signatureValue.width = AppConsts.DEFAULT_INPUT_WIDTH : this.signatureValue.width = this.width
        this.height <= AppConsts.DEFAULT_INPUT_HEIGHT ? this.signatureValue.height = AppConsts.DEFAULT_INPUT_HEIGHT : this.signatureValue.height = this.height
        break;
      case ContractSettingType.Electronic:
        this.width <= AppConsts.DEFAULT_SIGNATURE_WIDTH ? this.signatureValue.width = AppConsts.DEFAULT_SIGNATURE_WIDTH : this.signatureValue.width = this.width
        this.height <= AppConsts.DEFAULT_SIGNATURE_HEIGHT ? this.signatureValue.height = AppConsts.DEFAULT_SIGNATURE_HEIGHT : this.signatureValue.height = this.height
        break;
      case ContractSettingType.Digital:
        this.width <= AppConsts.DEFAULT_SIGNATURE_WIDTH_DIGITAL ? this.signatureValue.width = AppConsts.DEFAULT_SIGNATURE_WIDTH_DIGITAL : this.signatureValue.width = this.width
        this.height <= AppConsts.DEFAULT_SIGNATURE_HEIGHT_DIGITAL ? this.signatureValue.height = AppConsts.DEFAULT_SIGNATURE_HEIGHT_DIGITAL : this.signatureValue.height = this.height
        break;
      case ContractSettingType.Stamp:
        this.width <= AppConsts.DEFAULT_SIGNATURE_WIDTH ? this.signatureValue.width = AppConsts.DEFAULT_SIGNATURE_WIDTH : this.signatureValue.width = this.width
        this.height <= AppConsts.DEFAULT_SIGNATURE_HEIGHT ? this.signatureValue.height = AppConsts.DEFAULT_SIGNATURE_HEIGHT : this.signatureValue.height = this.height
        break;
    }
    this.px = event.clientX
    this.py = event.clientY
  }

  @HostListener('document: mouseup', ['$event'])
  onCornerRelease(event: MouseEvent) {
    if (this.isResizing) {
      let signaturePosition = {
        ...this.signatureValue,
        dx: this.signatureValue.positionX,
        dy: this.signatureValue.positionY,
      };
      this.updatePositionSignature.emit(signaturePosition);
    }
    this.isResizing = false
  }

  onResize(event: MouseEvent) {
    this.isResizing = true;
    this.px = event.clientX;
    this.py = event.clientY;
    event.preventDefault();
    event.stopPropagation();
  }
}
