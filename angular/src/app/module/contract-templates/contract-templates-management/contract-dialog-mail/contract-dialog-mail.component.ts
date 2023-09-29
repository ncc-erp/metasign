import { ElementRef, Injector, ViewChild } from "@angular/core";
import { Component, OnInit } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { ContractTemplateService } from "@app/service/api/contract-template.service";
import { ContractTemplateType } from "../../enum/contract-template.enum";
import * as pdfjsLib from "pdfjs-dist/webpack";
import { DialogComponentBase } from "@shared/dialog-component-base";
import { ContractService } from "@app/service/api/contract.service";
import { ContractTemplateSignerService } from "@app/service/api/contract-template-signer.service";
import { AppConsts } from "@shared/AppConsts";
import { forkJoin } from "rxjs";
import { ContractRole } from "@shared/AppEnums";
import { ContractEmailDto } from "@app/service/model/contract-email.dto";
import * as FileSaver from "file-saver";
@Component({
  selector: "app-contract-dialog-mail",
  templateUrl: "./contract-dialog-mail.component.html",
  styleUrls: ["./contract-dialog-mail.component.css"],
})
export class ContractDialogMailComponent
  extends DialogComponentBase<any>
  implements OnInit {
  contractFile;
  public templateType = ContractTemplateType;
  public contractTemplate;
  public loadingTemplate: boolean = true;
  public contractName: string;
  public contractSigner;
  signers: ContractEmailDto[];
  viewers: ContractEmailDto[];
  massField: string[];
  @ViewChild("fileInput") public fileInput: ElementRef;
  public successList=[];
  public failList = [];
  public isOrderSign = false;
  public listBatchSigner;
  public isSignerEmpty: boolean = false;
  public isSignatureEmpty;
  dialogRef;
  public contractRole = ContractRole
  file: any;
  constructor(
    injector: Injector,
    public sanitizer: DomSanitizer,
    private contractTemplateService: ContractTemplateService,
    private contractTemplateSignerService: ContractTemplateSignerService,
    private contractService: ContractService
  ) {
    super(injector);
  }

  ngOnInit() {
    forkJoin(
      [
        this.contractTemplateSignerService.getAllByContractTemplateId(
          this.dialogData.contractTemplateId
        ),
        this.contractTemplateService.getContractTemplate(
          this.dialogData.contractTemplateId
        ),
      ],
      (signer, contract) => {
        return {
          signer,
          contract,
        };
      }
    ).subscribe(async (value) => {
      this.contractName = value.contract.result.contractTemplate.name;
      this.contractTemplate = value.contract.result.contractTemplate;
      this.contractSigner = value.contract.result.signerSettings;
      this.isSignatureEmpty = value.contract.result.signatureSettings;
      this.massField = value.contract.result.contractTemplate.listField;
      this.signers = value.signer.result.signers.filter((value) => {
        return (
          value.contractRole === ContractRole.Signer ||
          value.contractRole === ContractRole.reviewer
        );
      });
      this.viewers = value.signer.result.signers.filter((value) => {
        return value.contractRole === ContractRole.Viewer;
      });
      this.isOrderSign = !!this.signers.find(signer => signer.procesOrder !== 1)
      if (this.isOrderSign) {
        this.signers.sort((a, b) => a.procesOrder - b.procesOrder);
      }
      this.contractFile = await this.convertPDFToImageStrings(
        value.contract.result.contractTemplate.content.split(",")[1]
      );

      this.contractFile.forEach((value) => {
        return (value.fileBase64 =
          "data:image/jpeg;base64," + value.fileBase64);
      });

      value.contract.result.signatureSettings.forEach((signature) => {
        this.contractSigner.forEach((Signer) => {
          if (signature.contractTemplateSignerId === Signer.id) {
            signature.role = Signer.role;
          }
        });
      });

      this.getContractSignatureSetting(value.contract.result);
      this.loadingTemplate = false;
      if (value.signer.result.signers.length === 0) {
        this.isSignerEmpty = true;
        return;
      }
    });
  }

  handleSendMail() {
    if (this.failList.length > 0) {
      abp.message.error(this.ecTransform("FileError"));
      return;
    }
    if (this.successList.length < 1) {
      abp.message.error(this.ecTransform("NoData"));
      return;
    }

    let contractSettingIdList = [];
    this.isSignatureEmpty.forEach((value) => {
      contractSettingIdList.push(value.contractTemplateSignerId);
    });

    let signatureUnique = Array.from(new Set(contractSettingIdList));
    if (this.signers.length === signatureUnique.length) {
      this.loadingTemplate = true
    } else {
      abp.message.error(this.ecTransform("SignerNotBeenSet"));
      return;
    }

    const payload =  {
      id: this.dialogData.contractTemplateId.toString(),
      rowData: this.successList
    }
    this.contractService
      .createMassContract(payload)
      .subscribe(() => {
        this.loadingTemplate = false
        this.dialogRef.close();
        abp.message.success(this.ecTransform("SendEmailSuccessfully"));
      });
  }

  getContractSignatureSetting(signature) {
    this.contractFile.forEach((contractPage) => {
      contractPage.signatureSettings = [];
      signature.signatureSettings.forEach((signature) => {
        if (signature.page === contractPage.contractPage) {
          contractPage.signatureSettings.push({
            ...signature,
            positionX: signature.positionX,
            positionY: signature.positionY,
            width: signature.width,
            height: signature.height,
            fontSize: signature.fontSize,
          });
        }
      });
    });
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
        const viewport = page.getViewport({ scale: 2 });
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

  handleCloseDialog() {
    this.dialogRef.close();
  }

  handleDownLoadMassTemplate() {
    this.contractTemplateService
      .downloadMassTemplate(this.dialogData.contractTemplateId)
      .subscribe((value) => {
        let byteCharacters = atob(value.result.base64);
        let byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        let byteArray = new Uint8Array(byteNumbers);

        let blob = new Blob([byteArray], { type: value.result.fileType });
        FileSaver.saveAs(blob, value.result.fileName);
      });
  }
  checkFileUpload(type: string) {
    const validImageTypes = [
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      "application/vnd.ms-excel.sheet.macroEnabled.12"
    ];
    if (validImageTypes.includes(type)) {
      return true;
    }
    return false;
  }
  onFileSelected(event): void {
    this.file = event.target.files[0];
    if (this.checkFileUpload(this.file?.type)) {
      const formData = new FormData();
      formData.append("File", event.target.files[0]);
      formData.append("TemplateId", this.dialogData.contractTemplateId.toString());
      this.contractTemplateService.validImportMassTemplate(formData).subscribe((rs) => {
        this.failList = rs.result.failList;
        this.successList = rs.result.successList;
        if (this.failList.length > 0) {
          let errormessage = "";
          this.failList.forEach(element => {
            errormessage += `<div class="d-flex">
            <div class="d-flex mr-2"><b>${this.ecTransform("Cell")}: &nbsp;</b> <span> ${element.address}</span></div >
            <div class="d-flex mr-2"><b>${this.ecTransform("Error")}: &nbsp;</b> <span> ${this.ecTransform(element.reasonFail)}</span></div >
            </div>`
          });
          abp.message.error(`<div style="max-height: 500px; overflow-y: auto;">${errormessage}</div>`, this.ecTransform("UploadMassTemplateFailTitle"), true);
          this.fileInput.nativeElement.value = null;
        }
      });
    }
    else {
      abp.message.error(this.ecTransform('JustSupportXlsx'));
      this.file = null;
      this.fileInput.nativeElement.value = null;
      return;
    }
  };
}


