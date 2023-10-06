import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ContractService } from "@app/service/api/contract.service";

@Component({
  selector: "app-send-mail-result",
  templateUrl: "./send-mail-result.component.html",
  styleUrls: ["./send-mail-result.component.css"],
})
export class SendMailResultComponent implements OnInit {
  public signUrl: string = "";
  private settingId: string;
  private contractId: string;
  constructor(
    private route: ActivatedRoute,
    private contractService: ContractService
  ) {
    this.settingId = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("settingId"))
    )?.settingId;

    this.contractId = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("contractId"))
    )?.contractId;
  }

  ngOnInit(): void {
    this.getSignUrl();
  }

  getSignUrl() {
    this.contractService
      .GetSignUrl(+this.settingId, +this.contractId)
      .subscribe((rs) => {
        this.signUrl = rs.result;
      });
  }

  navigateContract() {
    this.contractService._currentQuickFilter.next(-1)
    this.contractService._currentStatus.next(-1)
  }
}
