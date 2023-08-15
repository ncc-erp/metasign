import { DialogComponentBase } from '@shared/dialog-component-base';
import { Component, OnInit, Injector } from '@angular/core';
import { ContractTeamlateAction, ContractTemplateType } from '@app/module/contract-templates/enum/contract-template.enum';
import { DomSanitizer } from '@angular/platform-browser';
import { ContractTemplateService } from '@app/service/api/contract-template.service';
import * as pdfjsLib from "pdfjs-dist/webpack";
@Component({
  selector: 'app-dialog-template-preview',
  templateUrl: './dialog-template-preview.component.html',
  styleUrls: ['./dialog-template-preview.component.css']
})
export class DialogTemplatePreviewComponent extends DialogComponentBase<any> implements OnInit {
  contractFile;
  public templateType = ContractTemplateType;
  public contractTemplate;
  public loadingTemplate: boolean = true;
  public contractName: string;
  public contractSigner;
  public actionContractTemplate = ContractTeamlateAction;
  public contractTemplateT: string;
  dialogRef;
  constructor(
    injector: Injector,
    public sanitizer: DomSanitizer,
    private contractTemplateService: ContractTemplateService
  ) {
    super(injector);
  }

  ngOnInit() {
    this.contractTemplateService
      .getContractTemplate(this.dialogData.id)
      .subscribe(async (value) => {
        this.contractName = value.result.contractTemplate.name;
        this.contractTemplate = value.result.contractTemplate;
        this.contractSigner = value.result.signerSettings;
        this.contractFile = await this.convertPDFToImageStrings(
          value.result.contractTemplate.content.split(",")[1]
        );
        this.contractFile.forEach((value) => {
          return (value.fileBase64 =
            "data:image/jpeg;base64," + value.fileBase64);
        });

        value.result.signatureSettings.forEach((signature) => {
          this.contractSigner.forEach((Signer) => {
            if (signature.contractTemplateSignerId === Signer.id) {
              signature.role = Signer.role;
            }
          });
        });
        this.getContractSignatureSetting(value.result);
        this.loadingTemplate = false;
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
}
