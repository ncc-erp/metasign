import { ImageCroppedEvent } from "ngx-image-cropper";
import { NgSignaturePadOptions } from "@almothafar/angular-signature-pad/public-api";
import { SignaturePadComponent } from "@almothafar/angular-signature-pad";
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  HostListener,
  Inject,
  Input,
  OnInit,
  ViewChild,
} from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { SignatureUserService } from "@app/service/api/signature-user.service";
import { signatureTabNotUser } from "@app/module/signature/signature-management/signature-create/signature-create.component";
import { SignerSignatureSettingService } from "@app/service/api/signer-signature-setting.service";
import { ActivatedRoute } from "@angular/router";
import { signatureUserDto } from "@app/service/model/signature-user.dto";
import { EcTranslatePipe } from "@shared/pipes/ecTranslate.pipe";
import { AppConsts } from "@shared/AppConsts";
import { CdkDragEnd } from "@angular/cdk/drag-drop";
import { IInput } from "../../../signature/interfaces/signature-interface";
import { MediaMatcher } from '@angular/cdk/layout';
import { SessionServiceProxy } from "@shared/service-proxies/service-proxies";
import { ContractSettingType } from "@shared/AppEnums";

export enum signatureTabAll {
  signatureUser,
  signatureType,
  signatureUpload,
  signatureDraw,
}
@Component({
  selector: "app-signature-dialog",
  templateUrl: "./signature-dialog.component.html",
  styleUrls: ["./signature-dialog.component.css"],
})
export class SignatureDialogComponent implements OnInit {
  signatureTab
  sizeBoxSignature: { width: number, height: number } = { width: AppConsts.DEFAULT_SIGNATURE_WIDTH, height: AppConsts.DEFAULT_SIGNATURE_HEIGHT }
  isTablet: boolean = window.innerWidth > 480 && window.innerWidth <= 768;
  isMobile: boolean = window.innerWidth >= 320 && window.innerWidth <= 480
  fontFamily: string[] = AppConsts.fontFamilySignature;
  fontColor: string[] = AppConsts.colorBasic;
  signatureUser: signatureUserDto[];
  signatureUserValue: number;
  signatureUpload: string;
  signatureDraw;
  signaturePayload: any;
  indexTab: number = 0;
  cropImg: string;
  signatureSetting: any;
  contractId: number;
  contractSettingId: number;
  public setDefaultSignature: boolean = false;
  fontSizeTabType: number = AppConsts.fontSizeSignatureTabType
  fontFamilyTabType: string = this.fontFamily[0];
  textColorTabType: string = this.fontColor[0];
  valueTextTabType: string;
  initInputTop = -38
  initInputLeft = 0;
  isShowInput: boolean = true;
  fontSize: number[] = this.isTablet || this.isMobile ? AppConsts.fontSize : AppConsts.fontSizePageSignature
  currentFontFamily: string = this.fontFamily[0]
  currentFontSize: number;
  currentTextColor: string = this.fontColor[0]
  currentInputSelected: number
  currentCanvasWidth: number
  currentCanvasHeight: number
  initInputWidth: number = 600;
  initInputHeight: number = 72;
  isDrawing: boolean = false
  lastX: number;
  lastY: number;
  strokes: any[] = []
  onDrag: boolean = false
  canvas
  ctx: CanvasRenderingContext2D
  // public width = this.initInputWidth;
  // public height = this.initInputHeight;
  listInput: IInput[] = []
  maximumSizeofSignatureImage: number = AppConsts.maximumSizeofSignatureImage

  // private mobileQuery: MediaQueryList;
  constructor(
    public dialogRef: MatDialogRef<any>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public signatureUserService: SignatureUserService,
    public signerSignatureSettingService: SignerSignatureSettingService,
    public route: ActivatedRoute,
    private ecTranslate: EcTranslatePipe,
    private mediaMatcher: MediaMatcher, private changeDetectorRef: ChangeDetectorRef,
    private _sessionService: SessionServiceProxy,
  ) {
    this.contractSettingId = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("settingId"))
    )?.settingId;

    this.contractId = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("contractId"))
    )?.contractId;
  }

  @HostListener('window: resize', ['$event'])
  onResizeInput(event) {
    if (this.canvasRef) {
      const canvas = this.canvasRef.nativeElement;
      canvas.width = event.target.innerWidth * 0.4 - 75;
      canvas.height = canvas.width / (this.sizeBoxSignature.width / this.sizeBoxSignature.height);
      this.ctx.lineWidth = 4.5
      this.redraw()
    }
  }

  @ViewChild("signature")
  public signaturePad!: SignaturePadComponent;
  @ViewChild("uploadSignature")
  public uploadSignature: any;
  @ViewChild("inputFile") public inputFile: ElementRef;
  @ViewChild("inputDrag") public inputDrag: ElementRef;
  @ViewChild('canvas', { static: false }) canvasRef: ElementRef<HTMLCanvasElement>;
  @ViewChild('canvas') set content(canvasRef: ElementRef) {
    if (canvasRef) {
      this.canvas = this.canvasRef.nativeElement;
      this.ctx = this.canvas?.getContext('2d');
      this.ctx.strokeStyle = this.currentTextColor;
      this.canvas.width = this.currentCanvasWidth;
      this.canvas.height = this.currentCanvasHeight;
      this.ctx.lineWidth = 4.5;

      let isMouseDown = false;
      let startX = 200;
      let startY = 200;

      const mouseDownHandler = (e: MouseEvent) => {
        this.isDrawing = true;
        isMouseDown = true
        startX = e.offsetX;
        startY = e.offsetY;
        this.ctx?.beginPath()
        this.ctx?.moveTo(e.clientX, e.clientY)
      };

      const mouseMoveHandler = (e: MouseEvent) => {
        if (this.isDrawing && !this.onDrag) {
          const canvas = this.canvasRef.nativeElement;
          const ctx = canvas.getContext('2d');
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
        isMouseDown = false
        this.isDrawing = false;
      };

      const mouseOverHandler = () => {
        this.ctx.beginPath()
      };

      window.addEventListener('mousedown', mouseDownHandler);
      window.addEventListener('mouseup', mouseUpHandler);
      this.canvas.addEventListener('mousemove', mouseMoveHandler);
      this.canvas.addEventListener('mouseover', mouseOverHandler);

      this.drawStrokes()

      const canvas = this.canvasRef.nativeElement;
      const ctx = canvas.getContext('2d');
      ctx.lineJoin = 'round';
      ctx.lineCap = 'round';

      canvas.addEventListener('mousedown', (e) => {
        this.onDrag = false
        this.isDrawing = true
        this.lastX = e.offsetX;
        this.lastY = e.offsetY;
      })

      canvas.addEventListener('mousemove', (e) => {
        if (this.isDrawing && !this.onDrag) {
          this.strokes.push({
            x1: this.lastX,
            y1: this.lastY,
            x2: e.offsetX,
            y2: e.offsetY,
            color: ctx.strokeStyle
          });
          this.lastX = e.offsetX,
            this.lastY = e.offsetY
        }
      })

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
  }
  ngOnInit() {
    this.signatureSetting = this.data?.signatureSetting;
    this.signatureSetting?.isLoggedIn ? this.signatureTab = signatureTabAll : this.signatureTab = signatureTabNotUser
    this._sessionService.getCurrentLoginInformations().subscribe(res => {
      res?.user?.emailAddress ? this.valueTextTabType = `${res.user.name} ${res.user.surname}` : this.valueTextTabType = this.data?.signature?.signerName
      this.save()

    })

    if (window.innerWidth >= 320 && window.innerWidth <= 480) {
      this.isMobile = true
      this.currentCanvasWidth = window.innerWidth * 0.86
      this.initInputWidth = 300
      this.initInputHeight = 38
      this.currentFontSize = this.fontSize[3]
      this.fontSizeTabType = 25
    } else if (window.innerWidth > 480 && window.innerWidth <= 768) {
      this.isTablet = true
      this.currentCanvasWidth = window.innerWidth * 0.82;
      this.initInputWidth = 480
      this.initInputHeight = 56
      this.currentFontSize = this.fontSize[9]
      this.fontSizeTabType = 56
    } else {
      this.currentCanvasWidth = window.innerWidth * 0.4 - 75
      this.currentFontSize = this.fontSize[5]
    }
    this.currentCanvasHeight = (this.currentCanvasWidth / (this.sizeBoxSignature.width / this.sizeBoxSignature.height));

    // if (this.isMobile || this.isTablet)
    // if (this.currentInputSelected + 1) {
    //   this.width = this.listInput[this.currentInputSelected].width
    //   this.height = this.listInput[this.currentInputSelected].height
    // }
    this.signatureUserService
      .GetAllByEmail(this.contractSettingId)
      .subscribe((value) => {
        this.signatureUser = value.result.filter(signature => signature.signatureType !== ContractSettingType.Stamp);
        this.signatureUserValue = this.signatureUser.find(
          (signature) => signature.isDefault === true
        )?.id;
      });

  }

  drawStrokes() {
    this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
    this.strokes.forEach((stroke) => {
      if (this.isMobile) {
        this.ctx.lineWidth = 2.5;
      } else if (this.isTablet) {
        this.ctx.lineWidth = 3;
      }
      this.ctx.strokeStyle = stroke.color;
      this.ctx.beginPath();
      this.ctx.moveTo(stroke.x1, stroke.y1);
      this.ctx.lineTo(stroke.x2, stroke.y2);
      this.ctx.stroke();
    });
  }

  onTabChanged($event) {
    this.indexTab = $event.index;
    this.listInput.map(input => input.canFillInput = false)
    if (this.indexTab === this.signatureTab.signatureType || this.indexTab === this.signatureTab.signatureDraw) {
      this.save()
    }
  }

  signaturePadOptions: NgSignaturePadOptions = {
    backgroundColor: "rgba(255, 255, 255, 0)",
    canvasWidth: 500,
    canvasHeight: 500,
    minWidth: 3,
  };

  ngAfterViewInit(): void {
    // this.signaturePad.set(
    //   "canvasWidth",
    //   this.uploadSignature.nativeElement.offsetWidth
    // );
    // this.signaturePad.set(
    //   "canvasHeight",
    //   this.uploadSignature.nativeElement.offsetHeight
    // );
  }

  redraw() {
    const canvas = this.canvasRef.nativeElement;
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    for (const stroke of this.strokes) {
      ctx.beginPath();
      ctx.moveTo(stroke.x1, stroke.y1);
      ctx.lineTo(stroke.x2, stroke.y2);
      ctx.strokeStyle = this.currentTextColor;
      stroke.color = this.currentTextColor
      ctx.stroke();
    }
  }

  resizedataURL(datas, wantedWidth, wantedHeight) {
    return new Promise(async function (resolve, reject) {
      let img = document.createElement('img');
      img.onload = function () {
        let canvas = document.createElement('canvas');
        let ctx = canvas.getContext('2d');
        canvas.width = wantedWidth;
        canvas.height = wantedHeight;
        ctx.drawImage(img, 0, 0, wantedWidth, wantedHeight);
        let dataURI = canvas.toDataURL();
        resolve(dataURI);
      };
      img.src = datas;
    })
  }


  responsiveResizeToWidth: number;

  @HostListener('window:resize')
  onWindowResize(event: Event) {
    this.setResponsiveResizeToWidth();
  }

  setResponsiveResizeToWidth() {
    // Thực hiện logic để tính toán giá trị responsiveResizeToWidth dựa trên kích thước cửa sổ
    // Ví dụ:
    const windowWidth = window.innerWidth;
    this.responsiveResizeToWidth = windowWidth < 820 ? 250 : 550;
  }


  handleRemove() {
    this.signatureUpload = null;
    this.cropImg = null;
    this.inputFile.nativeElement.value = null;
  }

  imageCropped(event: ImageCroppedEvent) {
    this.signatureUpload = event.base64;
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

  handleSave(): void {
    switch (this.indexTab) {
      case this.signatureTab.signatureDraw:
        this.signaturePayload = {
          base64: this.signatureDraw,
          isNewSignature: true,
          setDefault: this.setDefaultSignature,
        };
        break;
      case this.signatureTab.signatureUpload:
        this.signaturePayload = {
          base64: this.signatureUpload,
          isNewSignature: true,
          setDefault: this.setDefaultSignature,
        };
        break;
      case this.signatureTab.signatureType:
        this.signaturePayload = {
          base64: this.signatureDraw,
          isNewSignature: true,
          setDefault: this.setDefaultSignature,
        };
        break;
      case this.signatureTab.signatureUser:
        let signatureUser = this.signatureUser.find(
          (signature) => signature.id === this.signatureUserValue
        );
        this.signaturePayload = {
          base64: signatureUser?.fileBase64,
          isNewSignature: false,
          setDefault: false,
        };
        break;
    }
    let isAllInputValue = this.listInput.every(input => input.value)
    if (!this.signaturePayload.base64 || (this.indexTab === this.signatureTab.signatureDraw && !isAllInputValue && !this.strokes.length)) {
      abp.message.error("", this.ecTranslate.transform("EmptyOrInvalidSignature"));
      return;
    }

    this.dialogRef.close(this.signaturePayload);
  }

  clickSignature(id: number) {
    this.signatureUserValue = id
  }

  handleClearSignature(): void {
    this.isDrawing = false
    this.onDrag = false
    this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
    this.strokes = []
    this.save()
  }

  handleClearsignature(): void {
    this.signaturePad.clear();
    this.signatureDraw = null;
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

  handleClickBoxCanvas() {
    this.listInput.map(input => input.canFillInput = false)
  }

  changeFontFamily() {
    this.listInput[this.currentInputSelected].fontFamily = this.currentFontFamily
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
    this.currentTextColor = color
    this.ctx.strokeStyle = this.currentTextColor;
    this.redraw()
    this.save()
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
      if (this.indexTab === this.signatureTab.signatureDraw) {
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
            if (this.isMobile) {
              totalLines.forEach(function (line, index) {
                var y = input.positionY + input.fontSize * (index + 1);
                var lineWidth = inputCtx.measureText(line).width;
                inputCtx.fillText(line, input.positionX + (300 - lineWidth) / 2, y);
              });
            } else {
              totalLines.forEach(function (line, index) {
                var y = input.positionY + input.fontSize * (index + 1);
                inputCtx.fillText(line, input.positionX, y);
              });
            }
            totalLines = [];
          }
        });
        imageData = inputCanvas.toDataURL();
      }
      else if (this.indexTab === this.signatureTab.signatureType) {
        var croppedCanvas = document.createElement("canvas");
        var croppedCtx = croppedCanvas.getContext("2d");
        croppedCanvas.width = this.canvas.width;
        croppedCanvas.height = this.canvas.width * scaleExportImage;
        croppedCtx.font = `${this.fontSizeTabType}px ${this.fontFamilyTabType}`;
        croppedCtx.fillStyle = this.textColorTabType;
        var textHeight = this.fontSizeTabType;
        var textWidth = croppedCtx.measureText(this.valueTextTabType).width + 20;
        if (this.isMobile && textWidth <= croppedCanvas.width / 2.5) {
          croppedCtx.font = `${this.fontSizeTabType * 2.5}px ${this.fontFamilyTabType}`;
        } else if (textWidth <= croppedCanvas.width / 3) {
          croppedCtx.font = `${this.fontSizeTabType * 2.8}px ${this.fontFamilyTabType}`;
        } else if (textWidth <= croppedCanvas.width / 2.5) {
          croppedCtx.font = `${this.fontSizeTabType * 2.5}px ${this.fontFamilyTabType}`;
        } else {
          croppedCtx.font = `${this.fontSizeTabType * croppedCanvas.width / textWidth}px ${this.fontFamilyTabType}`;
        }
        var textWidth = croppedCtx.measureText(this.valueTextTabType).width;
        var x = (croppedCanvas.width - textWidth) / 2;
        var y = (croppedCanvas.height - textHeight) / 2 + textHeight / 2 + 16;
        if (this.valueTextTabType) {
          croppedCtx.fillText(this.valueTextTabType, x, y);
          imageData = croppedCanvas.toDataURL();
        } else { imageData = null }
      }
      if (this.indexTab === this.signatureTab.signatureDraw) {
        let isAllInputValue = this.listInput.every((input) => input.value);
        if (this.listInput.length && this.strokes.length) {
          if (isAllInputValue) this.signatureDraw = imageData;
          else this.signatureDraw = "";
        } else if (this.listInput.length || this.strokes.length) {
          if ((this.listInput.length && isAllInputValue) || this.strokes.length) {
            this.signatureDraw = imageData;
          } else this.signatureDraw = "";
        } else this.signatureDraw = "";
      }
      else if (this.indexTab === this.signatureTab.signatureType) {
        this.signatureDraw = imageData
      }
    }
    return this.signatureDraw;
  }

  // drawComplete(event: MouseEvent | Touch) {
  //   this.resizedataURL(
  //     this.signaturePad.toDataURL("image/png"),
  //     this.data.width,
  //     this.data.height
  //   ).then((rs) => {
  //     this.signatureDraw = rs;
  //   });
  // }

  drop(event: any) {
    if (this.isMobile) {
      this.currentFontSize = this.fontSize[3];
    } else if (this.isTablet) {
      this.currentFontSize = this.fontSize[9];
    } else
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
    this.save()
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

  onTouchStart(event: TouchEvent) {
    this.listInput.map((input) => (input.canFillInput = false));
    this.canvas = this.canvasRef.nativeElement;
    const touch = event.touches[0];
    const rect = this.canvasRef.nativeElement.getBoundingClientRect();
    const offsetX = touch.clientX - rect.left;
    const offsetY = touch.clientY - rect.top;
    this.isDrawing = true;
    this.ctx.beginPath();
    this.ctx.moveTo(offsetX, offsetY);
    this.ctx.strokeStyle = this.currentTextColor;
    this.ctx.lineWidth = 2;
    this.lastX = offsetX,
      this.lastY = offsetY
    event.preventDefault()
  }

  onTouchMove(event: TouchEvent) {
    if (this.isDrawing) {
      const touch = event.touches[0];
      const rect = this.canvasRef.nativeElement.getBoundingClientRect();
      const offsetX = touch.clientX - rect.left;
      const offsetY = touch.clientY - rect.top;
      this.ctx.lineTo(offsetX, offsetY);
      this.ctx.stroke();
      this.strokes.push({
        x1: this.lastX,
        y1: this.lastY,
        x2: offsetX,
        y2: offsetY,
        color: this.ctx.strokeStyle
      });
      this.lastX = offsetX,
        this.lastY = offsetY
    }
    event.preventDefault()
  }

  onTouchEnd(event: TouchEvent) {
    this.isDrawing = false;
    this.save();
    event.preventDefault()
  }

  setColorTextTabType(color) {
    this.textColorTabType = color;
    this.save()
  }
}
