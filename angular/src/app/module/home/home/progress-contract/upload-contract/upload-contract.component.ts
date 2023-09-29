import { AppComponentBase } from "shared/app-component-base";
import { MassType, contractStep } from "./../../../../../../shared/AppEnums";
import { ContractService } from "./../../../../../service/api/contract.service";
import { ContractDto, ContractTempaleteDto } from "./../../../../../service/model/contract.dto";
import {
  Component,
  OnInit,
  Injector,
  ViewChild,
  ElementRef,
} from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { FormBuilder } from "@angular/forms";
import * as pdfjsLib from "pdfjs-dist/webpack";
import { ContractTemplateType } from "@app/module/contract-templates/enum/contract-template.enum";
import { ContractTemplateService } from "@app/service/api/contract-template.service";
import { MatDialog } from "@angular/material/dialog";
import { PreviewContractComponent } from "./preview-contract/preview-contract.component";
import { DialogContractTemplateComponent } from "./dialog-contract-template/dialog-contract-template.component";
import { DialogContractEditorComponent } from "./dialog-contract-editor/dialog-contract-editor.component";
import * as moment from "moment";
@Component({
  selector: "app-upload-contract",
  templateUrl: "./upload-contract.component.html",
  styleUrls: ["./upload-contract.component.css"],
})
export class UploadContractComponent
  extends AppComponentBase
  implements OnInit {
  fileContract: string | ArrayBuffer;
  file: File;
  contractFile
  activeFrom: boolean = false;
  isClicked: boolean = false;
  public fileName: string = "";
  public isSaved: boolean = false;
  public contract = {} as ContractDto;
  private contractId: number;
  private step: number;
  public progress: number = 0;
  public isConverting: boolean = false;
  public isUploadComplete: boolean = false;
  public templateContract: boolean;
  public wordBase64 = "";
  public matchMassField = "";
  templateContractId: number;
  templateContractEdit: boolean;
  useTemplateContract: boolean;
  createSigner: boolean;
  isCreateContractTemplate: boolean;
  htmlContent: string;
  status: number;
  currentDate = moment().format()
  batchContract
  @ViewChild("fileInput") public fileInput: ElementRef;

  contractForm = this.fb.group({
    name: [""],
    code: [""],
    expriedTime: [""],
  });

  constructor(
    injector: Injector,
    private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private contractService: ContractService,
    private contractTemplateService: ContractTemplateService,
    private dialog: MatDialog,
  ) {
    super(injector);
    this.contractId = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("contractId"))
    )?.contractId;

    this.step = Number(this.route.snapshot.queryParamMap.get("step"));
    this.templateContract = Boolean(
      this.route.snapshot.queryParamMap.get("templateContract")
    );
    this.useTemplateContract = Boolean(
      this.route.snapshot.queryParamMap.get("useTemplateContract")
    );

    this.templateContractId = Number(
      this.route.snapshot.queryParamMap.get("templateContractId")
    );

    this.createSigner = Boolean(
      this.route.snapshot.queryParamMap.get("createSigner")
    )
    this.isCreateContractTemplate = Boolean(
      this.route.snapshot.queryParamMap.get("isCreateContractTemplate")
    )

    this.batchContract = Boolean(
      this.route.snapshot.queryParamMap.get("batchContract")
    );
  }

  ngOnInit() {
    if (this.contractId) {
      this.isUploadComplete = true;
      this.getContractById();
      return
    }

    if (this.templateContractId) {
      this.getContractTemplate()
      return
    }
  }

  getContractTemplate() {
    this.contractTemplateService
      .getContractTemplate(this.templateContractId)
      .subscribe((rs) => {
        this.htmlContent = rs.result.contractTemplate.htmlContent;
        this.contract = rs.result;
        this.contractForm.controls["name"].setValue(
          rs.result.contractTemplate.name
        );
        this.contractForm.controls["code"].setValue(
          rs.result.contractTemplate.code
        );
        this.contractForm.controls["expriedTime"].setValue(
          rs.result.contractTemplate.expriedTime
        );
        this.fileContract = rs.result.contractTemplate.content;
        this.fileName = rs.result.contractTemplate.fileName;
        this.matchMassField = rs.result.contractTemplate.massField;
        this.wordBase64 = rs.result.contractTemplate.massWordContent;
        this.isSaved = true;
        this.activeFrom = true;
        this.isUploadComplete = true;
        this.handleCreateFileContract(this.fileContract as string)
      });
  }

  handleSelectContractTemplate() {
    let dialogRef = this.dialog.open(DialogContractTemplateComponent, {
      width: '60%',
      height: '95%',
      panelClass: 'my-dialog-container',
    })

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.templateContractId = result;
        this.useTemplateContract = true;
        this.getContractTemplate()
        this.router.navigate(["/app/home/process/upload-file"], {
          queryParams: {
            templateContractId: result,
            useTemplateContract: true,
            step: contractStep.UploadFile,
          },
        });
      }
    });
  }

  handleCreateFileContract(fileContract: string) {
    this.convertPDFToImageStrings(fileContract?.toString().split(",")[1])
      .then((base64Images) => {
        this.contractFile = base64Images;
        this.contractFile.forEach((value) => {
          return (value.fileBase64 =
            "data:image/jpeg;base64," + value.fileBase64);
        });
      })
      .catch((error) => {
        console.error(error);
      });
  }

  handleCreateContractTemplate() {
    const dialogRef = this.dialog.open(DialogContractEditorComponent, {
      width: '60%',
      height: '95%',
      panelClass: 'email-dialog',
    })
    dialogRef.afterClosed().subscribe(value => {
      if (value) {
        this.templateContractId = value;
        this.isCreateContractTemplate = true;
        this.getContractTemplate();
        this.router.navigate(["/app/templates/templates-create/upload"], {
          queryParams: {
            templateContractId: value,
            isCreateContractTemplate: true,
            templateContract: true,
            step: contractStep.UploadFile,
          }
        });
      }
    })
  }

  onSubmitContractForm() {
    const value = this.contractForm.value;
    const fileUpload = {
      ...value,
      name: value.name.trim(),
      fileBase64: this.fileContract,
      expriedTime: value.expriedTime && moment(value.expriedTime).format(),
      file: this.fileName,
      ...(this.status && { status: this.status }),
    };
    if (this.templateContractId && this.useTemplateContract) {
      this.isClicked = true;
      let contract = {
        name: value.name.trim(),
        code: value.code ? value.code : '',
        contractTemplateId: this.templateContractId,
        expriedTime: value.expriedTime && moment(value.expriedTime).format(),
      }
      this.contractService.createContractFromTemplate(contract).subscribe(value => {
        this.isClicked = false;
        this.contractId = value.result
        fileUpload.id = this.contractId;
        fileUpload.userId = this.contract.userId;
        const contract = {
          contractId: value.result,
        };
        const encode = encodeURIComponent(JSON.stringify(contract));
        this.contractService._currentStep.next(contractStep.SignerSetting);
        abp.notify.success(this.ecTransform('DocumentUpdatedSuccessfully'));

        this.router.navigate(["app/home/process/setting"], {
          queryParams: {
            templateContractId: this.templateContractId,
            contractId: encode,
            createSigner: true,
            step: contractStep.SignerSetting,
          },
          queryParamsHandling: "merge",
        });
      },
        () => {
          this.isClicked = false;
        })
      return
    }

    if (this.contractId && this.templateContractId) {
      this.isClicked = true;
      fileUpload.id = this.contractId;
      fileUpload.contractTemplateId = this.templateContractId;
      fileUpload.userId = this.contract.userId;
      this.updateContract(fileUpload);
      return
    }

    if (this.templateContract) {
      let contractPayload: ContractTempaleteDto = {
        name: this.contractForm.value.name,
        fileName: this.isCreateContractTemplate ? this.contractForm.value.name : fileUpload.file,
        content: this.fileContract,
        type: ContractTemplateType.Me,
        userId: this.appSession.userId,
        isFavorite: false,
        htmlContent: this.htmlContent ? this.htmlContent : null,
        massType: this.batchContract ? MassType.Multiple : MassType.Singel,
        massField: this.matchMassField,
        massWordContent:this.wordBase64
      };
      this.isClicked = true
      if (this.templateContractId) {
        contractPayload.id = this.templateContractId;
        contractPayload.content = this.fileContract;
        contractPayload.massWordContent = this.wordBase64
        contractPayload.massField =this.matchMassField
        this.contractTemplateService
          .updateFileTemplate(contractPayload)
          .subscribe(() => {
            this.isClicked = false;
            abp.notify.success(this.ecTransform('DocumentUpdatedSuccessfully'));
            this.contractService._currentStep.next(contractStep.SignerSetting);
            this.router.navigate(["/app/templates/templates-create/setting"], {
              queryParams: {
                templateContractId: this.templateContractId,
                step: contractStep.SignerSetting,
              },
              queryParamsHandling: "merge",
            });
          },
            () => {
              this.isClicked = false;
            });
      } else {
        this.contractTemplateService
          .createFileTemplate(contractPayload)
          .subscribe((value) => {
            this.isClicked = false;
            abp.notify.success(this.ecTransform('DocumentUploadedSuccessfully'));
            this.contractService._currentStep.next(contractStep.SignerSetting);
            this.router.navigate(["/app/templates/templates-create/setting"], {
              queryParams: {
                templateContractId: value.result,
                step: contractStep.SignerSetting,
              },
              queryParamsHandling: "merge",
            });
          },
            () => {
              this.isClicked = false;
            });
      }
      return;
    }
    if (this.contractId) {
      fileUpload.id = this.contractId;
      fileUpload.userId = this.contract.userId;
      this.isClicked = true
      this.updateContract(fileUpload);
      return
    } else {
      this.isClicked = true
      this.createContract(fileUpload);
    }
  }

  createContract(formvalue) {
    this.isConverting = true;
    this.contractService.create(formvalue).subscribe({
      next: (rs) => {
        const contract = {
          contractId: rs.result,
        };
        this.isClicked = false;
        const encode = encodeURIComponent(JSON.stringify(contract));
        abp.notify.success(this.ecTransform('DocumentUploadedSuccessfully'));
        this.router.navigate(["app/home/process/setting"], {
          queryParams: {
            contractId: encode,
            step: contractStep.SignerSetting,
          },
        });
        this.contractService._currentStep.next(contractStep.SignerSetting);
        this.isConverting = false;
      },
      error: () => {
        this.isClicked = false;
        this.isConverting = false;
        abp.notify.error(this.ecTransform('AContractUploadFailed'));
      },
    });
  }

  updateContract(formvalue) {
    this.isConverting = true;
    this.contractService.update(formvalue).subscribe({
      next: (rs) => {
        const contract = {
          contractId: rs.result.id,
        };
        this.isClicked = false;
        abp.notify.success(this.ecTransform('DocumentUpdatedSuccessfully'));
        const encode = encodeURIComponent(JSON.stringify(contract));
        this.contractService._currentStep.next(contractStep.SignerSetting);
        this.router.navigate(["app/home/process/setting"], {
          queryParams: {
            ...(this.createSigner && { createSigner: this.createSigner }),
            ...(this.templateContractId && { templateContractId: this.templateContractId }),
            contractId: encode,
            step: contractStep.SignerSetting,
          },
          queryParamsHandling: "merge",
        });
        this.isConverting = false;
      },
      error: () => {
        this.isClicked = false;
        abp.notify.error(this.ecTransform('AContractUploadFailed'));
        this.isConverting = false;
      },
    });
    this.contractService._currentStep.next(contractStep.SignerSetting);
  }

  getContractById() {
    this.contractService.get(this.contractId).subscribe((rs) => {
      this.contract = rs.result;
      this.contractForm.controls["name"].setValue(rs.result.name);
      this.contractForm.controls["code"].setValue(rs.result.code);
      this.contractForm.controls["expriedTime"].setValue(rs.result.expriedTime);
      this.fileContract = this.contract.fileBase64;
      this.fileName = rs.result.file;
      this.isSaved = true;
      this.activeFrom = true;
      this.isUploadComplete = true;
      this.status = rs.result.status;
      this.handleCreateFileContract(this.fileContract as string)
    });
  }

  checkFileUpload(type: string) {
    const validImageTypes = ["application/pdf"];
    if (!validImageTypes.includes(type)) {
      return true;
    }
    return false;
  }

  onFileSelected(event): void {
    this.file = event.target.files[0];
    if (this.checkFileUpload(this.file?.type)) {
      this.isConverting = true;
      this.isUploadComplete = false;
      this.contractService.ConvertFile(this.file).subscribe(
        rs => {
          if (this.contractId && this.templateContractId) {
            this.handleResetParams();
          }
          if (this.templateContractId && this.useTemplateContract) {
            this.handleResetParams();
          }

          this.isUploadComplete = true;
          this.isConverting = false;
          this.fileName = rs.result.fileName;
          this.fileContract = rs.result.base64String;
          this.matchMassField = rs.result.matchMassField;
          this.wordBase64 = rs.result.wordBase64;
          let contractName = this.fileName?.split(".")[0];
          this.contractForm.controls["name"].setValue(contractName);

          this.handleCreateFileContract(this.fileContract as string)
        },
        () => {
          this.isConverting = false;
          if (this.fileName) {
            this.isUploadComplete = true;
          }
        }
      );
      this.activeFrom = true;
      return;
    }

    const reader = new FileReader();
    reader.readAsDataURL(this.file);
    reader.onload = () => {
      this.activeFrom = true;
      this.fileContract = reader.result;
      this.fileName = event.target.files[0].name;
      let contractName = this.fileName.split(".")[0];
      this.contractForm.controls["name"].setValue(contractName);
      this.isUploadComplete = true;
      if (this.contractId && this.templateContractId) {
        this.handleResetParams();
      }

      if (this.templateContractId && this.useTemplateContract) {
        this.handleResetParams();
      }
      this.handleCreateFileContract(this.fileContract as string)
    };
  };

  handleResetParams() {
    const contract = {
      contractId: this.contractId,
    };
    const encode = encodeURIComponent(JSON.stringify(contract));
    this.templateContractId = null;
    this.createSigner = null;
    this.useTemplateContract = null;
    this.router.navigate(["/app/home/process/upload-file"], {
      queryParams: {
        step: contractStep.UploadFile,
        ...(this.contractId && { contractId: encode }),
      },
    });
  }

  onDateChange(event: any): void {
    const selectedDate = event.value;
    if (selectedDate) {
      selectedDate.setHours(23);
      selectedDate.setMinutes(59);
      selectedDate.setSeconds(59);
    }
  }

  convertPDFToImageStrings(base64PDF: string): Promise<any[]> {
    return new Promise(async (resolve, reject) => {
      const arrayBuffer = Uint8Array.from(atob(base64PDF), (c) =>
        c.charCodeAt(0)
      ).buffer;
      const base64Images = [];
      const pdf = await pdfjsLib.getDocument(arrayBuffer).promise;
      const totalPages = pdf.numPages;

      for (let pageNumber = 1; pageNumber <= totalPages; pageNumber++) {
        const page = await pdf.getPage(pageNumber);
        const viewport = page.getViewport({ scale: 1.45 });
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

      resolve(base64Images);
    });
  }

  previewContract() {
    const dialogRef = this.dialog.open(PreviewContractComponent, {
      data: {
        contractFile: this.contractFile,
        contractFileName: this.fileName
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

  handleRemoveContract(event) {
    event.stopPropagation()
    abp.message.confirm("", this.ecTransform('AreYouSureYouWantToDeleteThisDocument'), (rs) => {
      if (rs) {

        if (this.templateContract) {
          this.isUploadComplete = false;
          this.fileInput.nativeElement.value = null;
          this.contractFile = []
          this.router.navigate(["/app/templates/templates-create/upload"], {
            queryParams: {
              step: contractStep.UploadFile,
              templateContract: true,
              ...(this.batchContract && { batchContract: true }),
            },
          });

          if (this.templateContractId) {
            this.contractTemplateService.deleteFileTemplate(this.templateContractId).subscribe(() => {
              this.templateContractId = null;
            });
          }
          return
        }
        this.fileInput.nativeElement.value = null;
        this.isUploadComplete = false
        this.contractFile = []
        this.router.navigate(['/app/home/process/upload-file'])
        abp.notify.success(this.ecTransform('DocumentDeletedSuccessfully'));
        if (this.contractId + 1) {
          const payload = {
            contractId: this.contractId,
            reason: ""
          }
          this.contractService.CancelContract(payload).subscribe(res => {
            abp.notify.success(this.ecTransform('DocumentDeletedSuccessfully'));
          })
        }
      }
    });
  }
}
