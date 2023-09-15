import {
  Component,
  ElementRef,
  OnInit,
  ViewChild,
  Injector,
} from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { ContractTemplateSignerService } from "@app/service/api/contract-template-signer.service";
import { ContractService } from "@app/service/api/contract.service";
import { AppConsts } from "@shared/AppConsts";
import { ContractRole, contractStep } from "@shared/AppEnums";
import { AppComponentBase } from "@shared/app-component-base";
import { DialogMessTableComponent } from "@shared/components/dialog-mess-table/dialog-mess-table.component";
import { DialogComponentBase } from "@shared/dialog-component-base";
import * as FileSaver from "file-saver";
@Component({
  selector: "app-contract-templates-setting",
  templateUrl: "./contract-templates-setting.component.html",
  styleUrls: ["./contract-templates-setting.component.css"],
})
export class ContractTemplatesSettingComponent
  extends AppComponentBase
  implements OnInit
{
  public listBatchSigner;
  private templateContractId: number;
  public batchContract: boolean;
  private isbatchContract;
  public contractRole = ContractRole;
  @ViewChild("fileInput") public fileInput: ElementRef;
  constructor(
    private injector: Injector,
    private contractService: ContractService,
    private route: ActivatedRoute,
    private router: Router,
    private contractTemplateSignerService: ContractTemplateSignerService,
    private matDialog: MatDialog
  ) {
    super(injector);
    this.templateContractId = Number(
      this.route.snapshot.queryParamMap.get("templateContractId")
    );

    this.batchContract = Boolean(
      this.route.snapshot.queryParamMap.get("batchContract")
    );
  }

  ngOnInit() {
    let data = [];
    this.contractTemplateSignerService
      .getAllByContractTemplateId(this.templateContractId)
      .subscribe((value) => {
        value.result.signers.forEach((value, index) => {
          data.push({
            id: value.id,
            color: AppConsts.signerColor[index],
            contractRole: value.contractRole,
            role: value.role,
            rowData: value.massContractTemplateSigner,
          });
        });

        if (value.result.signers.length !== 0) {
          this.isbatchContract = true;
          this.listBatchSigner = data;
        }
      });
  }

  handleDownLoadMassTemplate() {
    this.contractService
      .downLoadMassTemplate(this.templateContractId)
      .subscribe((value) => {
        let byteCharacters = atob(value.result.base64);
        let byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        let byteArray = new Uint8Array(byteNumbers);

        let blob = new Blob([byteArray], { type: value.result.fileType });
        FileSaver.saveAs(blob, value.result.fileName);
      });
  }
  checkFileUpload(type: string) {
    const validImageTypes = [
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    ];
    if (!validImageTypes.includes(type)) {
      return true;
    }
    return false;
  }

  handleUploadFile(event) {
    if (this.checkFileUpload(event.target.files[0].type)) {
      abp.message.error(this.ecTransform('JustSupportXlsx'));
      this.fileInput.nativeElement.value = null;
      return;
    }

    const formData = new FormData();
    formData.append("File", event.target.files[0]);
    formData.append("TemplateId", this.templateContractId.toString());

    this.contractService.validImportMassTemplate(formData).subscribe(
      (value) => {
        if (value.result.failList.length > 0) {
          this.matDialog.open(DialogMessTableComponent, {
            data: { failList: value.result.failList },
            width: "40%",
            height: "95%",
            panelClass: "signature_dialog",
          });

          this.fileInput.nativeElement.value = null;
          return;
        }

        this.listBatchSigner = value.result.successList.map((value, index) => {
          return {
            id: null,
            color: AppConsts.signerColor[index],
            contractRole: value.contractRole,
            role: value.role,
            rowData: value.rowData,
          };
        });
        this.fileInput.nativeElement.value = null;
        abp.message.success(this.ecTransform('UploadSuccessFully'));
      },
      () => {
        this.fileInput.nativeElement.value = null;
        abp.message.error(this.ecTransform('PleaseFillCorectForm'));
      }
    );
  }

  handleBack() {
    this.router.navigate(["/app/templates/templates-create/upload"], {
      queryParams: {
        templateContractId: this.templateContractId,
        step: contractStep.UploadFile,
        templateContract: true,
      },
      queryParamsHandling: "merge",
    });
    this.contractService._currentStep.next(contractStep.UploadFile);
  }

  handleNext() {
    let listBatchSigner = this.listBatchSigner.map((value, index) => {
      return {
        id: value.id ? value.id : null,
        color: AppConsts.signerColor[index],
        contractRole: value.contractRole,
        email: value.role,
        name: value.role,
        role: value.role,
        procesOrder: 1,
      };
    });
    let playLoad = {
      contractTemplateId: this.templateContractId,
      contractTemplateSigners: listBatchSigner,
      massContractTemplateSigners: this.listBatchSigner,
    };

    this.contractTemplateSignerService
      .createContractTemplateSigner(playLoad)
      .subscribe(
        () => {
          this.router.navigate(["/app/templates/templates-create/design"], {
            queryParams: {
              step: contractStep.SignatureSetting,
              templateContractId: this.templateContractId,
            },
            queryParamsHandling: "merge",
          });
        },
        () => {}
      );
  }
}
