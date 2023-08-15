
import { MatDialog } from "@angular/material/dialog";
import { switchMap } from "rxjs/operators";
import { debounceTime } from "rxjs/operators";
import { Component, OnInit, Injector } from "@angular/core";
import { FormControl } from "@angular/forms";
import { DialogComponentBase } from "@shared/dialog-component-base";
import { ContractTemplateService } from "@app/service/api/contract-template.service";
import { ContractTemplateCategory } from "@app/module/contract-templates/enum/contract-template.enum";
import { ContractTemplates } from "@app/module/contract-templates/interface/contract-templates";
import { DialogTemplatePreviewComponent } from "../dialog-template-preview/dialog-template-preview.component";

@Component({
  selector: 'app-dialog-contract-template',
  templateUrl: './dialog-contract-template.component.html',
  styleUrls: ['./dialog-contract-template.component.css']
})
export class DialogContractTemplateComponent extends DialogComponentBase<any> implements OnInit {
  constructor(
    private injector: Injector,
    private contractTemplateService: ContractTemplateService,
    private dialog: MatDialog
  ) {
    super(injector);
  }
  messTable: string;
  categoryName: string;
  loaddingContract: boolean = true;
  contractTemplateCategory = ContractTemplateCategory;
  currenCategory: number;
  contractTemplateList: ContractTemplates[];
  seachContractTemplate = new FormControl();
  payloadContractTemplate: any = {
    filterType: 1,
    gridParam: {
      maxResultCount: 2147483647,
      skipCount: 0,
    },
  };

  ngOnInit() {
    this.currenCategory = this.contractTemplateCategory.ContractTemplateAll;
    this.payloadContractTemplate.filterType = this.currenCategory;
    this.checkNameCategoty(this.currenCategory);
    this.getContractTemplate(this.payloadContractTemplate);

    this.seachContractTemplate.valueChanges
      .pipe(
        debounceTime(500),
        switchMap((value) => {
          this.loaddingContract = true;
          this.payloadContractTemplate.gridParam.searchText = value;
          return this.contractTemplateService.getAllPagingContractTemplate(
            this.payloadContractTemplate
          );
        })
      )
      .subscribe((value) => {
        this.loaddingContract = false;
        this.contractTemplateList = value.result.items;

        if (value.result.isSearch && value.result.items.length === 0) {
          this.messTable = "NoMatchingSearchResults";
          return;
        }

        if (value.result.items.length === 0) {
          this.messTable = "YouCurrentlyDoNotHaveAnyContarctTemplates";
          return;
        }
      });
  }

  handleUseContractTemplate(contractTemplateId) {
    this.dialogRef.close(contractTemplateId);
  }

  handleChangeCategory(value: number) {
    this.currenCategory = value;
    this.checkNameCategoty(this.currenCategory);
    this.payloadContractTemplate.filterType = this.currenCategory;
    this.getContractTemplate(this.payloadContractTemplate);
  }

  handlePreviewContractTemplate(id) {
    this.dialog.open(DialogTemplatePreviewComponent, {
      data: {
        id,
      },
      width: "60%",
      height: "95%",
      panelClass: 'templatepr_upload'
    });
  }

  getContractTemplate(value) {
    this.loaddingContract = true;
    this.contractTemplateService
      .getAllPagingContractTemplate(value)
      .subscribe((value) => {
        this.contractTemplateList = value.result.items;
        this.loaddingContract = false;
        if (value.result.isSearch && value.result.items.length === 0) {
          this.messTable = "NoMatchingSearchResults";
          return;
        }
        if (value.result.items.length === 0) {
          this.messTable = "YouCurrentlyDoNotHaveAnyContarctTemplates";
          return;
        }
      });
  }

  checkNameCategoty(value) {
    switch (value) {
      case this.contractTemplateCategory.ContractTemplateMy:
        this.categoryName = this.ecTransform("MyTemplates");
        break;
      case this.contractTemplateCategory.ContractTemplateSystem:
        this.categoryName = this.ecTransform("TemplateGallery");
        break
      case this.contractTemplateCategory.ContractTemplateAll:
        this.categoryName = this.ecTransform("All");
        break;
    }
  }
}
