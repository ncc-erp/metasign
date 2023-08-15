import { ContractEmailDto } from "./../../../../../service/model/contract-email.dto";
import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { DomSanitizer } from "@angular/platform-browser";
import { ContractService } from "@app/service/api/contract.service";
import { contractStep } from "@shared/AppEnums";
import { EditEmailDialogData, MailPreviewInfo, MailPreviewInfoDto } from "@app/service/model/admin/emailTemplate.dto";
import { SetLocalStorageContract } from "@shared/helpers/FunctionsHelper";
import { AppComponentBase } from "@shared/app-component-base";
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";
import { MatDialog } from "@angular/material/dialog";
import { EditMailDialogComponent } from "@app/module/admin/email-template/edit-mail-dialog/edit-mail-dialog.component";
import { concatMap } from "rxjs/operators";

enum ContractRole {
  Signer = 1,
  reviewer = 2,
  Viewer = 3,
}
@Component({
  selector: "app-contract-email-setting",
  templateUrl: "./contract-email-setting.component.html",
  styleUrls: ["./contract-email-setting.component.css"],
})
export class ContractEmailSettingComponent
  extends AppComponentBase
  implements OnInit {
  private contractId: number = 0;
  private step: number = 0;
  public fileBase64: any;
  public contractInfo;
  nameFile: string;
  signers: ContractEmailDto[];
  viewers: ContractEmailDto[];
  contractsLocalStorage!: [{ contractId: number; step: number }];
  isOrderSign: boolean
  public mailInfo: MailPreviewInfo = {} as MailPreviewInfo;
  public contractMailContent = {} as MailPreviewInfoDto;
  public displayMailContent;
  public dbclick: boolean = false;
  useTemplateContract: boolean;
  templateContractId: number;
  arrangeSign;
  contractIdNew: number;

  constructor(
    injector: Injector,
    private contractService: ContractService,
    private sanitizer: DomSanitizer,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog,
  ) {
    super(injector);
    this.contractId = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("contractId"))
    )?.contractId;
    this.step = Number(this.route.snapshot.queryParamMap.get("step"));
    SetLocalStorageContract(this.contractId, this.step, false);
    this.contractsLocalStorage = JSON.parse(
      localStorage.getItem("contracts") as string
    );
  }

  ngOnInit(): void {
    this.getContractbyId();
    this.getMailContent();
    this.arrangeSign = JSON.parse(
      localStorage.getItem("statusArrangeSignerContract")
    );
  }

  getContractbyId() {
    this.contractService.get(this.contractId).subscribe((rs) => {
      this.fileBase64 = this.sanitizer.bypassSecurityTrustResourceUrl(
        rs.result.fileBase64
      );
      this.getContractMailInfo();
    });
  }

  convertBase64ToImage(base64Data: string): string {
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: "application/pdf" });
    const url = URL.createObjectURL(blob);
    return url;
  }

  getContractMailInfo() {
    this.contractService.GetSendMailInfo(this.contractId).subscribe((rs) => {
      this.nameFile = rs.result.file;
      this.signers = rs.result.signers.filter((value) => {
        return (
          value.contractRole === ContractRole.Signer ||
          value.contractRole === ContractRole.reviewer
        );
      });
      this.isOrderSign = !!this.signers.find(signer => signer.procesOrder !== 1)
      if (this.isOrderSign) {
        this.signers.sort((a, b) => a.procesOrder - b.procesOrder);
      }
      this.viewers = rs.result.signers.filter((value) => {
        return value.contractRole === ContractRole.Viewer;
      });
    });
  }

  handleBack() {
    const encode = this.route.snapshot.queryParamMap.get("contractId");
    this.router.navigate(["/app/home/process/signatureSetting"], {
      queryParams: {
        step: contractStep.SignatureSetting,
        contractId: encode,
      },
      queryParamsHandling: "merge",
    });

    this.contractService._currentStep.next(contractStep.SignatureSetting);
  }

  clearStore() {
    if (this.arrangeSign) {
      const statusArrange = this.arrangeSign.find(
        (value) => value.contractId === this.contractId
      );

      const indexArrange = this.arrangeSign.findIndex(
        (value) => value.contractId === this.contractId
      );

      if (statusArrange) {
        this.arrangeSign.splice(indexArrange, 1);
        localStorage.setItem(
          "statusArrangeSignerContract",
          JSON.stringify(this.arrangeSign)
        );
      }
    }
  }

  onSendMail() {
    this.contractService._currentQuickFilter.next(-1)
    this.contractService._currentStatus.next(-1)
    this.dbclick = true;
    if (this.dbclick) {
      let dto = {
        contractId: this.contractId,
        mailContent: this.contractMailContent,
      };
      this.contractService.setNotiExpiredContract(this.contractId).pipe(
        concatMap(() => {
          return this.contractService.SendMailToViewer(dto)
        }),
        concatMap(() => {
          return this.contractService.SendMail(dto)
        })
      ).subscribe((rs) => {
        this.dbclick = false
        SetLocalStorageContract(this.contractId, this.step, true);
        if ((rs.result.isAssigned && rs.result.isOrder && rs.result.isfirstSigner) || (rs.result.isAssigned && !rs.result.isOrder)) {
          this.clearStore();
          const contract = {
            settingId: rs.result.settingId,
            contractId: rs.result.contractId,
          };
          const encode = encodeURIComponent(JSON.stringify(contract));
          this.router.navigate(["app/send-mail-result"], {
            queryParams: {
              settingId: encode,
              contractId: encode,
            },
          });
        } else {
          this.clearStore();
          abp.message.success(this.ecTransform('SendEmailSuccessfully'));
          this.router.navigate(["app/contracts"]);
        }
      }, () => {
        this.dbclick = false;
      });
    }
  }

  getMailContent() {
    this.contractService
      .GetContractMailContent(this.contractId)
      .subscribe((rs) => {

        this.contractMailContent = rs.result;
        this.displayMailContent = this.sanitizer.bypassSecurityTrustHtml(
          this.contractMailContent.bodyMessage
        );
      });
  }
  public isShowSendMailBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.ProcessStep_StepSendMail_Send);
  }

  editMailTemplate() {
    const dialogData: EditEmailDialogData = {
      templateId: this.contractMailContent.templateId,
      mailInfo: { ...this.contractMailContent },
      showDialogHeader: false,
      temporarySave: true,
      isEditTemplate: false
    }

    const editDialog = this.dialog.open(EditMailDialogComponent, {
      data: dialogData,
      width: '1600px',
      height: '90%',
      panelClass: 'email-dialog'
    })

    editDialog.afterClosed().subscribe(rs => {
      if (rs) {
        this.displayMailContent = this.sanitizer.bypassSecurityTrustHtml(rs.bodyMessage);
        this.contractMailContent = rs
        console.log(this.contractMailContent)
      }
    })
  }
}
