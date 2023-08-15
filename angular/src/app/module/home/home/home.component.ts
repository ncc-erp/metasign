import { PERMISSIONS_CONSTANT } from './../../../permission/permission';
import { Router } from '@angular/router';
import { Component, Injector, OnInit } from '@angular/core';
import { contractStep, EContractFilterType } from '@shared/AppEnums';
import { ContractService } from '@app/service/api/contract.service';
import { ContractStatisticDto } from '@app/service/model/contractSetting.dto';
import { SignatureUserService } from '@app/service/api/signature-user.service';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.css"],
})
export class HomeComponent extends AppComponentBase implements OnInit {
  listQuickFilterId: number[] = [EContractFilterType.AssignToMe, EContractFilterType.WatingForOther, EContractFilterType.ExpirgingSoon, EContractFilterType.Completed]
  statisticContract: ContractStatisticDto
  signatureDefault
  fileContract: string | ArrayBuffer
  fileName: string
  file: File
  name: string
  isConverting: boolean = false
  constructor(injector: Injector,
    private router: Router,
    private contractService: ContractService,
    private signatureUserService: SignatureUserService) {
    super(injector)
  }

  ngOnInit(): void {
    this.contractService
      .GetContractStatistic()
      .subscribe((res) => (this.statisticContract = res.result));
    this.signatureUserService.getSignatureUserAllService().subscribe((res) => {
      this.signatureDefault = res.result.find(
        (signature) => signature.isDefault
      );
    });
  }

  navigateProcess() {
    this.router.navigate(["/app/home/process/upload-file"], {
      queryParams: {
        step: contractStep.UploadFile,
      },
    });
  }

  handleClickDefaultSignature() {
    if (this.isGranted(PERMISSIONS_CONSTANT.Signature_View)) {
      this.router.navigate(["/app/signature"])
    }
  }

  hanleClickStatus(id: number) {
    console.log(id)
    if (this.isGranted(PERMISSIONS_CONSTANT.Contract_View)) {
      this.contractService._currentQuickFilter.next(id);
      this.router.navigate(["/app/contracts"]);
    }
  }

  onFileSelected(event) {
    this.file = event.target.files[0];
    this.isConverting = true
    if (this.isCheckTypeUpload(this.file.type)) {
      this.contractService.ConvertFile(this.file).subscribe(
        rs => {
          this.isConverting = false
          this.fileName = rs.result.fileName;
          this.fileContract = rs.result.base64String
          this.name = this.fileName?.split(".")[0];
          this.handleFileReadComplete();
        },
        () => { }
      );
      return
    }

    const reader = new FileReader();
    reader.onload = () => {
      this.fileContract = reader.result;
      this.fileName = event.target.files[0].name;
      this.name = this.fileName.split(".")[0];
      this.handleFileReadComplete();
      this.isConverting = false
    };
    reader.readAsDataURL(this.file);
  }

  handleFileReadComplete() {
    const fileUpload = {
      code: '',
      name: this.name,
      fileBase64: this.fileContract,
      expriedTime: null,
      file: this.fileName,
    }
    this.isConverting = true
    this.contractService.create(fileUpload).subscribe(
      (res) => {
        const contract = {
          contractId: res.result,
        };
        const encode = encodeURIComponent(JSON.stringify(contract));
        this.router.navigate(["/app/home/process/upload-file"], {
          queryParams: {
            contractId: encode,
            step: contractStep.UploadFile,
          },
        });
        this.contractService._currentStep.next(contractStep.UploadFile);
        this.isConverting = false
      },
      () => {
        abp.notify.error(this.ecTransform('AContractUploadFailed'));
      },
    )
  }


  isCheckTypeUpload(type: string) {
    const valueType = ["application/pdf"]
    if (!valueType.includes(type)) {
      return true;
    }
  }

  public isShowStatistic() {
    return this.isGranted(PERMISSIONS_CONSTANT.Home_View_Statistic);
  }

  public isShowDefaultSignature() {
    return this.isGranted(PERMISSIONS_CONSTANT.Home_View_Default_Signature);
  }

  public isShowStartButton() {
    return this.isGranted(PERMISSIONS_CONSTANT.Home_Start_Pocess);
  }
}

