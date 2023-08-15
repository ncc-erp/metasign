import { signatureType } from "./../../../../service/model/signature-user.dto";
import { AppComponentBase } from "@shared/app-component-base";
import { SignatureUserService } from "./../../../../service/api/signature-user.service";
import { ElementRef, HostListener, OnInit, ViewChild } from "@angular/core";
import { Component, Inject, Injector } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { ActionType } from "../signature-management.component";
import { ImageCroppedEvent } from "ngx-image-cropper";
import { ContractSettingType } from "@shared/AppEnums";
import { AppConsts } from "@shared/AppConsts";
import { CdkDragEnd } from "@angular/cdk/drag-drop";
import { IInput } from "../../interfaces/signature-interface";
import { SessionServiceProxy } from "@shared/service-proxies/service-proxies";

export enum signatureTabNotUser {
  signatureType,
  signatureUpload,
  signatureDraw,
}
@Component({
  selector: "app-signature-create",
  templateUrl: "./signature-create.component.html",
  styleUrls: ["./signature-create.component.css"],
})
export class SignatureCreateComponent
  extends AppComponentBase
  implements OnInit {
  sizeBoxSignature: { width: number, height: number } = { width: AppConsts.DEFAULT_SIGNATURE_WIDTH, height: AppConsts.DEFAULT_SIGNATURE_HEIGHT }
  fontFamily: string[] = AppConsts.fontFamilySignature;
  fontColor: string[] = AppConsts.colorBasic;
  lableDialog: boolean = false;
  isNeedUpload: boolean = false;
  signatureBase64Edit: string;
  signatureUpload: string;
  signatureDraw: string;
  signaturelist: signatureType[] = [
    {
      lable: "Signature",
      value: ContractSettingType.Electronic,
    },
  ];
  signaturePayload: string;
  selectedValue: number;
  indexTab: number = 0;
  cropImg: string;
  isDefault: boolean = false;
  containWithinAspectRatio: string;
  fontSizeTabType: number = AppConsts.fontSizeSignatureTabType
  fontFamilyTabType: string = this.fontFamily[0];
  textColorTabType: string = this.fontColor[0];
  valueTextTabType: string;

  @ViewChild("canvas", { static: true }) canvasRef: ElementRef<HTMLCanvasElement>;
  initInputTop = -38;
  initInputLeft = 0;
  fontSize: number[] = AppConsts.fontSizePageSignature;
  currentFontFamily: string = this.fontFamily[0];
  currentFontSize: number = this.fontSize[5];
  currentTextColor: string = this.fontColor[0];
  currentInputSelected: number;
  currentCanvasWidth: number = window.innerWidth * 0.4 - 80;
  currentCanvasHeight: number = (this.currentCanvasWidth / (this.sizeBoxSignature.width / this.sizeBoxSignature.height));
  initInputWidth: number = 600;
  initInputHeight: number = 72;
  isDrawing: boolean = false;
  lastX: number;
  lastY: number;
  strokes: any[] = [];
  canvas;
  ctx: CanvasRenderingContext2D;
  onDrag: boolean = false;
  listInput: IInput[] = [];
  public width = this.initInputWidth;
  public height = this.initInputHeight;
  maximumSizeofSignatureImage: number = AppConsts.maximumSizeofSignatureImage

  constructor(
    private injector: Injector,
    private signatureUserService: SignatureUserService,
    public dialogRef: MatDialogRef<SignatureCreateComponent>,
    private _sessionService: SessionServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
  }

  @HostListener("window: resize", ["$event"])
  onResizeInput(event) {
    const canvas = this.canvasRef.nativeElement;
    canvas.width = event.target.innerWidth * 0.4 - 80;
    canvas.height = canvas.width / (this.sizeBoxSignature.width / this.sizeBoxSignature.height);
    this.ctx.lineWidth = 4.5;
    this.redraw();
  }

  onTabChanged($event) {
    this.indexTab = $event.index;
    this.listInput.map((input) => (input.canFillInput = false));
    this.save()
  }

  @ViewChild("signature")
  public signaturePad: any;
  @ViewChild("uploadSignature")
  public uploadSignature: ElementRef;
  @ViewChild("inputFile") public inputFile: ElementRef;
  @ViewChild("inputDrag") public inputDrag: ElementRef;
  @ViewChild("valueInput") public valueInput: ElementRef;
  @ViewChild("dialog") dialog: ElementRef;

  ngAfterViewInit(): void {
    const canvas = this.canvasRef.nativeElement;
    const ctx = canvas.getContext("2d");
    ctx.lineJoin = "round";
    ctx.lineCap = "round";

    canvas.addEventListener("mousedown", (e) => {
      this.onDrag = false;
      this.isDrawing = true;
      this.lastX = e.offsetX;
      this.lastY = e.offsetY;
    });

    canvas.addEventListener("mousemove", (e) => {
      if (this.isDrawing && !this.onDrag) {
        this.strokes.push({
          x1: this.lastX,
          y1: this.lastY,
          x2: e.offsetX,
          y2: e.offsetY,
          color: ctx.strokeStyle,
        });
        (this.lastX = e.offsetX), (this.lastY = e.offsetY);
      }
    });

    canvas.addEventListener("mouseup", () => {
      this.listInput.map((input) => (input.canFillInput = false));
      this.currentInputSelected = -1;
      if (!(this.currentInputSelected + 1) && !this.onDrag) {
        this.save();
      }
      this.isDrawing = false;
    });
    canvas.addEventListener("mouseleave", () => { this.save() });
  }

  redraw() {
    const canvas = this.canvasRef.nativeElement;
    const ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    for (const stroke of this.strokes) {
      ctx.beginPath();
      ctx.moveTo(stroke.x1, stroke.y1);
      ctx.lineTo(stroke.x2, stroke.y2);
      ctx.strokeStyle = this.currentTextColor;
      stroke.color = this.currentTextColor;
      ctx.stroke();
    }
  }

  handleRemove() {
    this.signatureUpload = null;
    this.cropImg = null;
    this.isNeedUpload = false;
    this.inputFile.nativeElement.value = null;
  }

  ngOnInit() {
    this._sessionService.getCurrentLoginInformations().subscribe(res => {
      this.valueTextTabType = `${res.user.name} ${res.user.surname}`
      this.save()
    })

    if (this.currentInputSelected + 1) {
      this.width = this.listInput[this.currentInputSelected].width;
      this.height = this.listInput[this.currentInputSelected].height;
    }
    this.canvas = this.canvasRef.nativeElement;
    this.ctx = this.canvas.getContext("2d");
    this.selectedValue = this.signaturelist[0].value;
    if (this.data.type === ActionType.edit) {
      this.lableDialog = true;
      this.isNeedUpload = true;
      this.isDefault = this.data.isDefault;
      this.signatureUserService
        .getSignatureUserService(this.data.id)
        .subscribe((value) => {
          this.selectedValue = value.result.signatureType;
          this.signaturePayload = value.result.fileBase64;
          this.signatureBase64Edit = value.result.fileBase64;
        });
    } else {
      this.lableDialog = false;
      this.isNeedUpload = false;
    }

    this.ctx.strokeStyle = this.currentTextColor;
    this.canvas.width = this.currentCanvasWidth;
    this.canvas.height = this.currentCanvasHeight;
    this.ctx.lineWidth = 4.5;

    let isMouseDown = false;
    let startX = 200;
    let startY = 200;

    const mouseDownHandler = (e: MouseEvent) => {
      this.isDrawing = true;
      isMouseDown = true;
      startX = e.offsetX;
      startY = e.offsetY;
      this.ctx.beginPath();
      this.ctx.moveTo(e.clientX, e.clientY);
    };

    const mouseMoveHandler = (e: MouseEvent) => {
      if (this.isDrawing && !this.onDrag) {
        const canvas = this.canvasRef.nativeElement;
        const ctx = canvas.getContext("2d");
        ctx.strokeStyle = this.currentTextColor;
        const rect = canvas.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        ctx.beginPath();
        ctx.moveTo(startX, startY);
        ctx.lineTo(x, y);
        ctx.stroke();
        startX = x;
        startY = y;
      }
    };

    const mouseUpHandler = () => {
      isMouseDown = false;
      this.isDrawing = false;
    };

    const mouseOverHandler = () => {
      this.ctx.beginPath();
    };

    window.addEventListener("mousedown", mouseDownHandler);
    window.addEventListener("mouseup", mouseUpHandler);
    this.canvas.addEventListener("mousemove", mouseMoveHandler);
    this.canvas.addEventListener("mouseover", mouseOverHandler);
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

  async handleSave() {
    switch (this.indexTab) {
      case signatureTabNotUser.signatureDraw:
        this.signaturePayload = this.save();
        break;
      case signatureTabNotUser.signatureUpload:
        this.signaturePayload = this.signatureUpload;
        break;
      case signatureTabNotUser.signatureType:
        this.signaturePayload = this.save();
        break;
    }

    const obj = {
      id: this.data.id,
      signatureType: this.selectedValue,
      userId: this.appSession.userId,
      file: "demo",
      fileBase64: this.signaturePayload,
      type: this.data,
      isDefault: this.isDefault,
    };
    let isAllInputValue = this.listInput.every((input) => input.value);
    if (
      !this.signaturePayload ||
      (this.indexTab === signatureTabNotUser.signatureDraw &&
        !isAllInputValue &&
        !this.strokes.length)
    ) {
      abp.message.error("", this.ecTransform("EmptyOrInvalidSignature"));
      this.signaturePayload = this.signatureBase64Edit;
      return;
    }
    this.dialogRef.close(obj);
  }

  convertBackgroundTransparent(base64) {
    let img = new Image();
    img.src = base64;

    return new Promise((resolve, reject) => {
      img.onload = function () {
        let canvas = document.createElement("canvas");
        canvas.width = img.width;
        canvas.height = img.height;
        let ctx = canvas.getContext("2d");

        ctx.drawImage(img, 0, 0, canvas.width - 10, canvas.height - 10);

        let imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
        let data = imageData.data;

        for (let i = 0; i < data.length; i += 4) {
          let red = data[i];
          let green = data[i + 1];
          let blue = data[i + 2];

          if ([red, green, blue].every((item) => item > 230)) {
            data[i + 3] = 0;
          }
        }
        ctx.putImageData(imageData, 0, 0);

        img.src = canvas.toDataURL();
        resolve(img.src);
      };
    });
  }

  imageCropped(event: ImageCroppedEvent) {
    this.signatureUpload = event.base64;
  }

  handleClearSignature(): void {
    this.isDrawing = false;
    this.onDrag = false;
    this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
    this.strokes = [];
    this.save();
  }

  removeInput(index: number) {
    this.listInput.splice(index, 1);
    this.save();
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  handleCanFillInput(index: number) {
    this.currentInputSelected = index;
    this.listInput[index].canFillInput = true;
    this.currentFontSize = this.listInput[this.currentInputSelected].fontSize;
    this.listInput.map((input, indexInput) => {
      index === indexInput
        ? (input.canFillInput = true)
        : (input.canFillInput = false);
    });
    setTimeout(() => {
      this.inputDrag.nativeElement.focus();
    }, 0);
  }

  changeFontFamily() {
    this.listInput[this.currentInputSelected].fontFamily =
      this.currentFontFamily;
  }

  changeFontSize() {
    let input = this.listInput[this.currentInputSelected]
    const textarea = this.inputDrag.nativeElement;
    if (this.currentInputSelected + 1) {

      if (input.fontSize > this.currentFontSize) {
        input.height = textarea.scrollHeight - 3 * input.fontSize / 2;
      }
      else {
        input.height = textarea.scrollHeight + 3 * input.fontSize / 2;
      }

    }
    input.fontSize = this.currentFontSize;
    input.isResized = true
    this.save()
  }

  getTextarea(event) {
    this.save();
  }

  setCurrentColor(color: string) {
    this.currentTextColor = color;
    this.ctx.strokeStyle = this.currentTextColor;
    this.redraw();
    this.save();
  }

  save() {
    var imageData;
    var scaleExportImage = this.sizeBoxSignature.height / this.sizeBoxSignature.width;
    const signatureCanvas = document.createElement("canvas");
    signatureCanvas.width = this.canvas.width;
    signatureCanvas.height = this.canvas.height;
    signatureCanvas.getContext("2d").drawImage(this.canvas, 0, 0);
    const signatureData = signatureCanvas.toDataURL();
    const signatureImage = new Image();
    signatureImage.src = signatureData;
    signatureImage.onload = () => {
      const inputCanvas = document.createElement("canvas");
      inputCanvas.width = this.canvas.width;
      inputCanvas.height = this.canvas.width * scaleExportImage;
      const inputCtx = inputCanvas.getContext("2d");
      inputCtx.drawImage(signatureImage, 0, 0);
      if (this.indexTab === signatureTabNotUser.signatureDraw) {
        var totalLines = [];
        this.listInput.map(input => {
          if (input.value) {
            inputCtx.fillStyle = this.currentTextColor;
            inputCtx.font = `${input.fontSize}px ${input?.fontFamily}`;
            let totalWords = input?.value.split("\n");
            let widthTextarea = input?.width + 4;
            totalWords.forEach(function (word) {
              var currentLine = "";
              let lines = [];
              let wordItem = word.split(" ");
              wordItem.forEach((item) => {
                var testLine = currentLine + item + " ";
                var metrics = inputCtx.measureText(testLine);
                var lineWidth = metrics.width;
                if (lineWidth > widthTextarea && currentLine) {
                  lines.push(currentLine);
                  currentLine = item + " ";
                } else {
                  currentLine = testLine;
                }
              });
              lines.push(currentLine);
              totalLines = [...totalLines, ...lines];
            });
            totalLines.forEach(function (line, index) {
              var y = input.positionY + input.fontSize * (index + 1);
              inputCtx.fillText(line, input.positionX, y);
            });
            totalLines = [];
          }
        });
        imageData = inputCanvas.toDataURL();

      }
      else if (this.indexTab === signatureTabNotUser.signatureType) {
        var croppedCanvas = document.createElement("canvas");
        var croppedCtx = croppedCanvas.getContext("2d");
        croppedCanvas.width = this.canvas.width;
        croppedCanvas.height = this.canvas.width * scaleExportImage;
        croppedCtx.font = `${this.fontSizeTabType}px ${this.fontFamilyTabType}`;
        croppedCtx.fillStyle = this.textColorTabType;
        var textHeight = this.fontSizeTabType;
        var textWidth = croppedCtx.measureText(this.valueTextTabType).width + 20;
        if (textWidth <= croppedCanvas.width / 3) {
          croppedCtx.font = `${this.fontSizeTabType * 2.8}px ${this.fontFamilyTabType}`;
        } else if (textWidth <= croppedCanvas.width / 2.5) {
          croppedCtx.font = `${this.fontSizeTabType * 2.25}px ${this.fontFamilyTabType}`;
        } else {
          croppedCtx.font = `${this.fontSizeTabType * croppedCanvas.width / textWidth}px ${this.fontFamilyTabType}`;
        }
        textWidth = croppedCtx.measureText(this.valueTextTabType).width;
        var x = (croppedCanvas.width - textWidth) / 2;
        var y = (croppedCanvas.height - textHeight) / 2 + textHeight / 2 + 16;
        croppedCtx.fillText(this.valueTextTabType, x, y);
        imageData = croppedCanvas.toDataURL();
        if (this.valueTextTabType) {
          croppedCtx.fillText(this.valueTextTabType, x, y);
          imageData = croppedCanvas.toDataURL();
        } else { imageData = null }
      }

      if (this.indexTab === signatureTabNotUser.signatureDraw) {
        let isAllInputValue = this.listInput.every((input) => input.value);
        if (this.listInput.length && this.strokes.length) {
          if (isAllInputValue) this.signatureDraw = imageData;
          else this.signatureDraw = "";
        } else if (this.listInput.length || this.strokes.length) {
          if ((this.listInput.length && isAllInputValue) || this.strokes.length) {
            this.signatureDraw = imageData;
          } else this.signatureDraw = "";
        } else this.signatureDraw = "";
      } else if (this.indexTab === signatureTabNotUser.signatureType) {
        this.signatureDraw = imageData
      }
    };
    return this.signatureDraw;
  }

  drop(event: any) {
    this.currentFontSize = this.fontSize[5];
    let input = {
      value: "",
      fontSize: this.currentFontSize,
      color: this.fontColor[0],
      fontFamily: this.fontFamily[0],
      canFillInput: false,
      width: this.initInputWidth,
      height: this.initInputHeight,
    };
    let positionY;
    if (
      event.distance.y <= this.currentCanvasHeight + (-this.initInputTop) - 36 &&
      event.distance.y > this.currentCanvasHeight + (-this.initInputTop) - this.initInputHeight) {
      positionY = this.currentCanvasHeight - this.initInputHeight
    } else {
      positionY = event.distance.y + this.initInputTop
    }

    if (event.distance.y >= -this.initInputTop &&
      event.distance.y <= this.currentCanvasHeight + (-this.initInputTop) - 36) {
      if (event.distance.x >= 0 &&
        event.distance.x <= this.currentCanvasWidth - this.initInputWidth
      ) {
        this.listInput.push({
          ...input,
          positionX: event.distance.x,
          positionY: positionY,
          onDrag: false,
        });
      } else if (
        event.distance.x > this.currentCanvasWidth - this.initInputWidth &&
        event.distance.x <= this.currentCanvasWidth - 100
      ) {
        this.listInput.push({
          ...input,
          positionX: this.currentCanvasWidth - this.initInputWidth - 16,
          positionY: positionY,
          onDrag: false,
        });
      }
    }
  }

  mouseDownTextarea(i) {
    this.onDrag = true;
    this.listInput[i].canFillInput = true
  }

  handleDragEnd(event: CdkDragEnd, index: number) {
    this.isDrawing = false;
    this.onDrag = false;
    this.listInput[index].onDrag = false;
    this.currentInputSelected = -1
    const newPositionX = this.listInput[index].positionX + event.distance.x;
    const newPositionY = this.listInput[index].positionY + event.distance.y;
    const buttonWidth = this.listInput[index].width + 16;
    const buttonHeight = this.listInput[index].height + 8

    this.listInput[index].positionX = Math.max(
      0,
      Math.min(newPositionX, this.currentCanvasWidth - buttonWidth)
    );
    this.listInput[index].positionY = Math.max(
      0,
      Math.min(newPositionY, this.currentCanvasHeight - buttonHeight)
    );
  }

  getDisplayContent(content) {
    return content.replace(/\n/g, "<br>");
  }

  setColorTextTabType(color) {
    this.textColorTabType = color;
    this.save()
  }
}
