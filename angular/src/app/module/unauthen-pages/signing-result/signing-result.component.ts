import { Component, OnInit, Injector } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import { DialogDownloadComponent } from "@app/module/contract/contract-manage/dialog-download/dialog-download.component";
import { ContractService } from "@app/service/api/contract.service";
import { AppComponentBase } from "@shared/app-component-base";
import { saveAs } from "file-saver";

@Component({
  selector: "app-signing-result",
  templateUrl: "./signing-result.component.html",
  styleUrls: ["./signing-result.component.css"],
})
export class SigningResultComponent extends AppComponentBase implements OnInit {
  contractId: number;
  contracInfo: string;
  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private contractService: ContractService,
  ) {
    super(injector);
    this.contractId = Number(this.route.snapshot.queryParamMap.get("id"));
    this.contracInfo = this.route.snapshot.queryParamMap.get("contracInfo");
  }

  ngOnInit(): void {
  }

  handleDownLoad() {
      this.dialog.open(DialogDownloadComponent, {
        data: {
          fileName: this.contracInfo,
          idContract: this.contractId,
        },
        minWidth: '36%',
        minHeight: '250px',
      });
  }
}
