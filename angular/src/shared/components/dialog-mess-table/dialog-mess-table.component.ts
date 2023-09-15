import { Component, Injector, OnInit } from "@angular/core";
import { MatTableModule } from "@angular/material/table";
import { DialogComponentBase } from "@shared/dialog-component-base";

@Component({
  selector: "app-dialog-mess-table",
  templateUrl: "./dialog-mess-table.component.html",
  styleUrls: ["./dialog-mess-table.component.css"],
})
export class DialogMessTableComponent
  extends DialogComponentBase<any>
  implements OnInit
{
  displayedColumns: string[] = ["row", "reasonFail"];
  dataSource = this.dialogData.failList;
  constructor(private injector: Injector) {
    super(injector);
  }

  ngOnInit() {
 
  }

  handleCloseDialog() {
    this.dialogRef.close();
  }
}
