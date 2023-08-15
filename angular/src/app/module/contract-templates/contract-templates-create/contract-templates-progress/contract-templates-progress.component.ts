import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { ContractService } from "@app/service/api/contract.service";
import { LayoutStoreService } from "@shared/layout/layout-store.service";

@Component({
  selector: "app-contract-templates-progress",
  templateUrl: "./contract-templates-progress.component.html",
  styleUrls: ["./contract-templates-progress.component.css"],
})
export class ContractTemplatesProgressComponent implements OnInit {
  idContract: number;
  step!: number;
  isLinear = true;
  currentStep!: number;
  templateContract: boolean;

  constructor(
    private route: ActivatedRoute,
    private layoutService: LayoutStoreService,
    private contractService: ContractService,
    private router: Router
  ) {
    this.idContract = Number(route.snapshot.queryParamMap.get("contractId"));
    this.layoutService.setSidebarExpanded(true);
    this.contractService._currentStep.next(
      Number(route.snapshot.queryParamMap.get("step"))
    );

    this.templateContract = Boolean(
      this.route.snapshot.queryParamMap.get("templateContract")
    );
  }

  ngOnInit() {
    this.contractService._currentStep$.subscribe((res) => {
      this.step = res;
    });
  }

  handleCompletedStep(step: number) {
    return step < this.step;
  }

  handleValueContract(value: number) {
    this.idContract = value;
  }

  isNotFullWidth() {
    if (this.step !== 2) {
      return "not-full-width";
    } else return "full-width";
  }
}
