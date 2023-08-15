import { ContractFileStoringService } from './../../../../service/api/contract-file-storing.service';
import { Component, Injector, OnInit } from "@angular/core";
import { MatDialogRef } from "@angular/material/dialog";
import { DialogComponentBase } from "@shared/dialog-component-base";
import { EDownloadType } from "../../enum/contractEnums";

@Component({
  selector: "app-dialog-download",
  templateUrl: "./dialog-download.component.html",
  styleUrls: ["./dialog-download.component.css"],
})
export class DialogDownloadComponent
  extends DialogComponentBase<any>
  implements OnInit {
  idContract: number;
  fileName: string;
  currentType: number;
  subtasks = [
    { name: "All", completed: true, type: EDownloadType.All },
    { name: "Document", completed: true, type: EDownloadType.Document1PDF },
    { name: "CertificateOfCompletion", completed: true, type: EDownloadType.CertificatePDF },
  ];

  constructor(injector: Injector,
    public dialogRef: MatDialogRef<DialogDownloadComponent>,
    private contractFileStoringService: ContractFileStoringService) {
    super(injector);
  }

  ngOnInit(): void {
    Object.assign(this, this.dialogData);
  }

  updateAll2FilesState(subtask: any) {
    const all2Files = this.subtasks[0];

    if (subtask === all2Files) {
      this.currentType = subtask.type;
      this.subtasks
        .slice(1)
        .map((sub) => (sub.completed = all2Files.completed));
    } else {
      this.currentType = subtask.type;
      const otherSubtasks = this.subtasks.slice(1);
      const allCompleted = otherSubtasks.every((sub) => sub.completed);
      all2Files.completed = allCompleted;
    }
  }

  sendDataToServer() {
    const selectedSubtaskTypes = this.subtasks
      .filter((subtask) => subtask.completed)
      .map((subtask) => subtask.type);
    const normalizedSubtaskTypes = selectedSubtaskTypes.includes(EDownloadType.All)
      ? EDownloadType.All
      : Number(selectedSubtaskTypes.join(""));

    const dataToSend = normalizedSubtaskTypes;
    this.dialogRef.close()

    switch (dataToSend) {
      case EDownloadType.All:
        this.contractFileStoringService.getPresignedDownloadUrl(this.idContract, EDownloadType.All).subscribe(value => {
          this.downloadfilecontract(value.result)
        })
        break;
      case EDownloadType.CertificatePDF:
        this.contractFileStoringService.getPresignedDownloadUrl(this.idContract, EDownloadType.CertificatePDF).subscribe(value => {
          this.downloadfilecontract(value.result)
        })
        break;

      case EDownloadType.Document1PDF:
        this.contractFileStoringService.getPresignedDownloadUrl(this.idContract, EDownloadType.Document1PDF).subscribe(value => {
          this.downloadfilecontract(value.result)
        })
        break;
    }
  }

  downloadfilecontract(url) {
    const link = url;
    const a = document.createElement("a");
    a.setAttribute('href', link);
    a.click();
  }
}
