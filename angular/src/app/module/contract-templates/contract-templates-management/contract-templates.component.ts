import { ContractTemplates } from "./../interface/contract-templates";
import { Component, OnInit, Injector } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { contractStep } from "@shared/AppEnums";
import { ContractTemplateCategory } from "../enum/contract-template.enum";
@Component({
  selector: "app-contract-templates",
  templateUrl: "./contract-templates.component.html",
  styleUrls: ["./contract-templates.component.css"],
})
export class ContractTemplatesComponent
  extends AppComponentBase
  implements OnInit {
  public statusCategorycontract: number | null;
  public contractTemplateList: ContractTemplates[];
  public contractCategory = ContractTemplateCategory;
  


  constructor(
    injector: Injector,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    super(injector);
  }

  ngOnInit() {
    this.route.queryParamMap.subscribe((value: ParamMap) => {
      this.statusCategorycontract = Number(value.get("status"));
    });
  }

  handleCreateBatchContract()
  {
    this.router.navigate(["/app/templates/templates-create/upload"], {
      queryParams: {
        step: contractStep.UploadFile,
        templateContract: true,
        batchContract: true
      },
      queryParamsHandling: "merge",
    });
  } 
  handleCreateContract() {
    this.router.navigate(["/app/templates/templates-create/upload"], {
      queryParams: {
        step: contractStep.UploadFile,
        templateContract: true,
      },
      queryParamsHandling: "merge",
    });
  }
}
