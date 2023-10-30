import { PagedListingComponentBase, PagedRequestDto } from './../../../../shared/paged-listing-component-base';
import { Injector } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ContractTemplateType } from '@app/module/contract-templates/enum/contract-template.enum';
import { ContractTemplates } from '@app/module/contract-templates/interface/contract-templates';
import { ContractTemplateService } from '@app/service/api/contract-template.service';
import { UploadTemplateComponent } from './upload-template/upload-template.component';
import { PreviewContractComponent } from '@app/module/home/home/progress-contract/upload-contract/preview-contract/preview-contract.component';
import * as pdfjsLib from "pdfjs-dist/webpack";

@Component({
  selector: 'app-contract-template',
  templateUrl: './contract-template.component.html',
  styleUrls: ['./contract-template.component.css']
})
export class ContractTemplateComponent extends PagedListingComponentBase<any> implements OnInit {
  tempaltes: ContractTemplates[] = []
  totalItem: number = 0
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {

    let payload = {
      filterType: ContractTemplateType.System,
      gridParam: request
    } as GetContractTemplatePaging

    this.contractTemplateService.getAllPagingContractTemplate(payload).subscribe(rs => {
      this.tempaltes = rs.result.items;
      this.totalItem = rs.result.totalCount
      this.showPaging(rs.result.items, pageNumber);
    })
  }
  protected delete(entity: any): void {
    throw new Error('Method not implemented.');
  }

  constructor(injector: Injector,
    private dialog: MatDialog,
    private contractTemplateService: ContractTemplateService) {
    super(injector);
  }

  ngOnInit(): void {
    this.refresh()
  }
  createTempalte() {
    let ref = this.dialog.open(UploadTemplateComponent, {
      width: "700px"
    })

    ref.afterClosed().subscribe(rs => {
      this.refresh()
    })
  }

  onPreview(template: ContractTemplates) {
    let contractfiles = []
    this.convertPDFToImageStrings(template.content?.toString().split(",")[1])
      .then((base64Images) => {
        contractfiles = base64Images;
        contractfiles.forEach((value) => {
          return (value.fileBase64 =
            "data:image/jpeg;base64," + value.fileBase64);
        });

        const dialogRef = this.dialog.open(PreviewContractComponent, {
          data: {
            contractFile: contractfiles,
            contractFileName: template.name
          },
          width: '1000px',
          maxWidth: '1000px',
          panelClass: 'email-dialog',
        })
        dialogRef.afterClosed().subscribe(rs => {
          if (rs) {
          }
        })

      })
      .catch((error) => {
        console.error(error);
      });


  }

  // handleCreateFileContract(fileContract: string) {
  //   this.convertPDFToImageStrings(fileContract?.toString().split(",")[1])
  //     .then((base64Images) => {
  //       this.contractFile = base64Images;
  //       this.contractFile.forEach((value) => {
  //         return (value.fileBase64 =
  //           "data:image/jpeg;base64," + value.fileBase64);
  //       });
  //     })
  //     .catch((error) => {
  //       console.error(error);
  //     });
  // }


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


  onDelete(template: ContractTemplates) {
    abp.message.confirm(`Bạn có chắc muốn xóa template <strong>${template.name}</strong>?`, "", rs => {
      if (rs) {
        this.contractTemplateService.delete(template.id).subscribe(rs => {
          abp.notify.success("Delete successful")
          this.refresh()
        })
      }
    }, { isHTML: true })
  }

  onEdit(template: ContractTemplates) {
    let ref = this.dialog.open(UploadTemplateComponent, {
      width: "700px",
      data: template
    })

    ref.afterClosed().subscribe(rs => {
      this.refresh()
    })
  }
}
export interface GetContractTemplatePaging {
  filterType: ContractTemplateType
  gridParam: PagedRequestDto
}

export interface CreateTemplateDto {
  content: string
  fileName: string
  htmlContent: string
  isFavorite: boolean
  massField: string
  massType: number
  massWordContent: string
  name: string
  type: ContractTemplateType
  userId: number
}
