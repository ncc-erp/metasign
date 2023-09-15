import {
  ContractTemplateCategory,
  ContractTemplateTypeView,
  EChangeSortContractTemplate,
  EDisplayedColumnContract,
} from "../../enum/contract-template.enum";
import { finalize } from "rxjs/operators";
import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { ContractTemplateService } from "@app/service/api/contract-template.service";
import {
  ContractTemplates,
  sortTable,
} from "../../interface/contract-templates";
import { ContractTemplateType } from "../../enum/contract-template.enum";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { MatDialog } from "@angular/material/dialog";
import { ContractTemplateDialogComponent } from "../contract-template-dialog/contract-template-dialog.component";
import { MassType, contractStep } from "@shared/AppEnums";
import { ContractDialogMailComponent } from "../contract-dialog-mail/contract-dialog-mail.component";

@Component({
  selector: "app-contract-template-list",
  templateUrl: "./contract-template-list.component.html",
  styleUrls: ["./contract-template-list.component.css"],
})
export class ContractTemplateListComponent
  extends PagedListingComponentBase<any>
  implements OnInit
{
  public contractTemplateList: ContractTemplates[];
  statusCategorycontract: number | null;
  contractCategory = ContractTemplateType;
  searchText: string;
  totalPage: number;
  titleContractTemplate: string;
  checkTableSort = EChangeSortContractTemplate;
  messTable: string;
  dialogRef;
  contractTemplateTableColumn = [
    {
      id: 1,
      name: EDisplayedColumnContract.NumericalOrder,
      status: EChangeSortContractTemplate.Default,
      isCheckedSort: false,
    },
   
    {
      id: 2,
      name: EDisplayedColumnContract.Name,
      status: EChangeSortContractTemplate.Default,
      isCheckedSort: true,
    },
    {
      id: 5,
      width: 200,
      name: EDisplayedColumnContract.TemplateType,
      status: EChangeSortContractTemplate.Default,
      isCheckedSort: true,
    },
    {
      id: 3,
      width: 200,
      name: EDisplayedColumnContract.CreationTime,
      status: EChangeSortContractTemplate.Default,
      isCheckedSort: true,
    },
    {
      id: 4,
      width: 200,
      name: EDisplayedColumnContract.UpdatedTime,
      status: EChangeSortContractTemplate.Default,
      isCheckedSort: true,
    },
  ];
  statusSortTable: sortTable;

  constructor(
    private injector: Injector,
    private route: ActivatedRoute,
    private contractTemplateService: ContractTemplateService,
    private router: Router,
    private matDialog: MatDialog
  ) {
    super(injector);
  }

  handleSortTable(id) {
    this.contractTemplateTableColumn.forEach((value) => {
      if (value.id !== id) {
        value.status = EChangeSortContractTemplate.Default;
      }
      if (id === value.id) {
        if (value.status >= EChangeSortContractTemplate.Down) {
          value.status = EChangeSortContractTemplate.Default;
          this.handleCheckTypeSortContract(value.name, value.status);
          this.getDataPage(1);
          return;
        }
        value.status++;
        this.handleCheckTypeSortContract(value.name, value.status);
        this.getDataPage(1);
      }
    });
  }

  handleCheckTypeSortContract(name: string, status: number) {
    switch (name) {
      case EDisplayedColumnContract.Name:
        this.statusSortTable = {
          sort: "name",
          status,
        };
        return;
      case EDisplayedColumnContract.CreationTime:
        this.statusSortTable = {
          sort: "creationTime",
          status,
        };
        return;
      case EDisplayedColumnContract.UpdatedTime:
        this.statusSortTable = {
          sort: "lastModifycationTime",
          status,
        };
        return;
    }
  }

  ngOnInit() {
    this.route.queryParamMap.subscribe((rs: ParamMap) => {
      this.statusCategorycontract = Number(rs.get("status"));
      this.checkTitleContractTemplate(this.statusCategorycontract);
      this.getDataPage(1);
    });
  }

  checkTitleContractTemplate(value) {
    switch (value) {
      case ContractTemplateCategory.ContractTemplateAll:
        this.titleContractTemplate = "AllTemplates";
        break;
      case ContractTemplateCategory.ContractTemplateMy:
        this.titleContractTemplate = "MyTemplates";
        break;
      case ContractTemplateCategory.ContractTemplateSystem:
        this.titleContractTemplate = "TemplatesGallery";
        break;
    }
  }

  handleRefreshPage() {
    this.getDataPage(1);
  }

  handleDeleteContractTemplate(contractTemplate) {
    abp.message.confirm(
      `<p style='font-size:21px;'>
      ${this.ecTransform("AreYouSureYouWantToDeleteTemplate")}
      <strong style='font-size:21px;'>${contractTemplate.name}</strong>
      ${this.ecTransform("TemplateOrNo")}</p>`,
      " ",
      (rs) => {
        if (rs) {
          this.contractTemplateService
            .delete(contractTemplate.id)
            .subscribe((rs) => {
              abp.notify.success(
                `${this.ecTransform("ContractTemplateDeletedSuccessfully")}`
              );
              this.refresh();
            });
        }
      },
      { ishtml: true }
    );
  }

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.searchText = this.searchText;

    let requestContract: any = {
      gridParam: request,
    };
    if (this.statusCategorycontract !== -1) {
      requestContract.filterType = this.statusCategorycontract;
    }

    if (this.statusSortTable?.status !== EChangeSortContractTemplate.Default) {
      request.sortDirection = this.statusSortTable?.status;
      request.sort = this.statusSortTable?.sort;
    } else {
      delete request.sortDirection;
      delete request.sort;
    }

    this.contractTemplateService
      .getAllPagingContractTemplate(requestContract)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((res) => {
        this.totalPage = res.result.totalCount;
        this.contractTemplateList = res.result.items.map((value, index) => {
          return { ...value, position: 10 * (pageNumber - 1) + index + 1 };
        });

        this.showPaging(res.result, pageNumber);

        if (res.result.isSearch && res.result.items.length === 0) {
          this.messTable = "NoMatchingSearchResults";
          return;
        }

        if (res.result.items.length === 0) {
          this.messTable = "YouCurrentlyDoNotHaveAnyTemplates";
          return;
        }
      });
  }

  protected delete(entity: any): void {
    throw new Error("Method not implemented.");
  }

  handlePreviewContractTemplate(id) {
    this.openDiaLog(ContractTemplateTypeView.PreviewMyTemplate, id);
  }

  handleUseContractTemplate(contractTemplateId, messTableid) {
    if (MassType.Multiple === messTableid) {
      this.dialogRef = this.matDialog.open(ContractDialogMailComponent, {
        data: { contractTemplateId },
        autoFocus: false ,
        width: "90%",
        height: "95%",
        panelClass: "mail_dialog",
      });
    } else {
      this.router.navigate(["/app/home/process/upload-file"], {
        queryParams: {
          templateContractId: contractTemplateId,
          useTemplateContract: true,
          step: contractStep.UploadFile,
        },
        queryParamsHandling: "merge",
      });
    }
  }

  handleEditContractTemplate(contractTemplateId, messTableid)
  {
    if (MassType.Multiple === messTableid) {
      this.router.navigate(["/app/templates/templates-create/upload"], {
        queryParams: {
          templateContract: true,
          templateContractId: contractTemplateId,
          batchContract: true
        },
        queryParamsHandling: "merge",
      });
    } else {
      this.router.navigate(["/app/templates/templates-create/upload"], {
        queryParams: {
          templateContract: true,
          templateContractId: contractTemplateId
        },
        queryParamsHandling: "merge",
      });
    }
  }

  openDiaLog(type, id) {
    this.dialogRef = this.matDialog.open(ContractTemplateDialogComponent, {
      data: { type, id },
      width: "60%",
      height: "95%",
      panelClass: "signature_dialog",
    });

    this.dialogRef.afterClosed().subscribe((rs) => {
      if (rs) {
        this.contractTemplateService.updateFileTemplate(rs).subscribe(() => {
          abp.message.success(this.ecTransform("EditSuccessfully"));
        });
      }
    });
  }
}
