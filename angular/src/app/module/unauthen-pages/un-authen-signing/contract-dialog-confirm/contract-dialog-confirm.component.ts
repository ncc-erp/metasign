import { Component, OnInit, Injector, HostListener } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { DialogComponentBase } from "@shared/dialog-component-base";
import { ContractInvalidateDialogComponent } from "../contract-invalidate-dialog/contract-invalidate-dialog.component";
import { BreakPoint } from "@shared/AppEnums";

@Component({
  selector: "app-contract-dialog-confirm",
  templateUrl: "./contract-dialog-confirm.component.html",
  styleUrls: ["./contract-dialog-confirm.component.css"],
})
export class ContractDialogConfirmComponent
  extends DialogComponentBase<any>
  implements OnInit
{
  constructor(injector: Injector, public dialog: MatDialog) {
    super(injector);
  }
  private contractSettingId: number;
  public screenWidth: number;
  ngOnInit() {
    this.contractSettingId = this.dialogData.contractSettingId;
    this.screenWidth = window.innerWidth;
  }
  @HostListener('window:resize')
  onWindowResize() {
    this.screenWidth = window.innerWidth;
  }


  handleConfirmInvalidate() {
    this.dialogRef.close();
    this.dialog.open(ContractInvalidateDialogComponent, {
      data: {
        contractSettingId: this.contractSettingId,
        role: this.dialogData.role,
      },
      width: this.screenWidth <= BreakPoint.mobile ?  "80%" : "50%",
      height: this.screenWidth <= BreakPoint.Ipad ?  "80%" : "60%",
      panelClass: 'contract-dialog',
    });
  }
}
