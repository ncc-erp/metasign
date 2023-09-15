import { Component, OnInit, Injector } from "@angular/core";
import { Router } from "@angular/router";
import { ContractSettingService } from "@app/service/api/contract-seting.service";
import { ContractService } from "@app/service/api/contract.service";
import { ContractInvalidate } from "@shared/AppEnums";
import { DialogComponentBase } from "@shared/dialog-component-base";

@Component({
  selector: "app-contract-invalidate-dialog",
  templateUrl: "./contract-invalidate-dialog.component.html",
  styleUrls: ["./contract-invalidate-dialog.component.css"],
})
export class ContractInvalidateDialogComponent
  extends DialogComponentBase<any>
  implements OnInit {
  constructor(
    injector: Injector,
    private router: Router,
    private contractSettingService: ContractSettingService,
    private contractService: ContractService
  ) {
    super(injector);
  }
  public remainingCharacters: number = 200;
  public valueContractInvalidate: string;
  public ContractInvalidate = ContractInvalidate;
  private contractSettingId: number;
  private contractId: number;
  public titleDialog:string;


  ngOnInit() {
    this.contractSettingId = this.dialogData.contractSettingId;
    this.contractId = this.dialogData.contractId;
    this.titleDialog = this.dialogData.title;
  }

  handleContractInvalidate() {
    if (this.valueContractInvalidate?.trim()) {
      if (this.contractSettingId) {
        const payload = {
          contractSettingId: this.contractSettingId,
          reason: this.valueContractInvalidate,
        };
        this.contractSettingService
          .voidOrDeclineToSignDto(payload)
          .subscribe(() => {

            this.dialogRef.close();
            if (this.dialogData.role === ContractInvalidate.contractCreator) {
              this.router.navigate([`/app/contracts`]);
            } else {
              this.router.navigate([`/app/signging/signing-reject`]);
            }
          });
      }
      if (this.contractId) {
        const payload = {
          contractId: this.contractId,
          reason: this.valueContractInvalidate,
        };
        this.contractService.CancelContract(payload).subscribe(() => {
          abp.notify.success(this.ecTransform("ContractCancelledSuccessfully"))
          this.dialogRef.close();
        })
      }
    } else {
      abp.message.error("điền ký tự vào hợp đồng");
    }
  }
}
