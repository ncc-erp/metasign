import { ActivatedRoute, Router } from "@angular/router";
import { UploadContractComponent } from "./../../../../home/home/progress-contract/upload-contract/upload-contract.component";
import { Component, OnInit } from "@angular/core";

@Component({
  selector: "app-contract-templates-upload",
  templateUrl: "./contract-templates-upload.component.html",
  styleUrls: ["./contract-templates-upload.component.css"],
})
export class ContractTemplatesUploadComponent implements OnInit {
  templateContract: boolean;
  templateContractId: number;
  constructor(private router: Router, private route: ActivatedRoute) {
    this.templateContract = Boolean(
      this.route.snapshot.queryParamMap.get("templateContract")
    );

    this.templateContractId = Number(
      this.route.snapshot.queryParamMap.get("id")
    );
  }

  ngOnInit() {

    

  }
}
