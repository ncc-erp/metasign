import { Injector } from "@angular/core";
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
@Component({
  selector: "app-contract-dialog-mail",
  templateUrl: "./contract-dialog-mail.component.html",
  styleUrls: ["./contract-dialog-mail.component.css"],
})
export class ContractDialogMailComponent
  extends DialogComponentBase<any>
  implements OnInit
{
  contractFile;
  public templateType = ContractTemplateType;
  public contractTemplate;
  public loadingTemplate: boolean = true;
  public contractName: string;
  public contractSigner;
  public listBatchSigner;
  public isSignerEmpty: boolean = false;
  public isSignatureEmpty;
  dialogRef;
  public contractRole = ContractRole
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
    let batchSignerTemp = [];

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

      value.signer.result.signers.forEach((value, index) => {
        if (value.massContractTemplateSigner.length === 0) {
          this.isSignerEmpty = true;
          return;
        }
        batchSignerTemp.push({
          color: AppConsts.signerColor[index],
          contractRole: value.contractRole,
          role: value.role,
          rowData: value.massContractTemplateSigner,
        });
      });
      if (value.signer.result.signers.length !== 0) {
        this.listBatchSigner = batchSignerTemp;
      }
     
    });
  }

  handleSendMail() {
    if (this.isSignerEmpty) {
      abp.message.error("Hãy điền đầy đủ người ký theo lô mẫu hợp đồng");
      return;
    }
    let signer = this.listBatchSigner.filter(data=> data.contractRole === ContractRole.Signer);
    let contractSettingIdList = [];
    this.isSignatureEmpty.forEach((value) => {
      contractSettingIdList.push(value.contractTemplateSignerId);
    });

    let signatureUnique = Array.from(new Set(contractSettingIdList));

    if (signer.length === signatureUnique.length) {
      this.loadingTemplate = true
      this.contractService
        .createMassContract(this.dialogData.contractTemplateId)
        .subscribe(() => {
          this.loadingTemplate = false
          this.dialogRef.close();
          abp.message.success(this.ecTransform("SendEmailSuccessfully"));
        });
      return;
    } else {
      abp.message.error("Tồn tại người ký chưa được thiết lập vị trí ký");
    }
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
}
