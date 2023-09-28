import { ContractTemplateSignerService } from './../../../../../service/api/contract-template-signer.service';
import { AppConsts } from "@shared/AppConsts";
import { ContractSettingService } from "@app/service/api/contract-seting.service";
import { ContractTemplateService } from '@app/service/api/contract-template.service';
import {
  contractRoles,
  ContractSettingDto,
} from "./../../../../../service/model/contractSetting.dto";
import { ActivatedRoute, Router } from "@angular/router";
import { Component, Injector, OnInit } from "@angular/core";
import {
  FormArray,
  FormBuilder,
  FormControl,
  Validators,
} from "@angular/forms";
import { contractStep } from "@shared/AppEnums";
import { SetLocalStorageContract } from "@shared/helpers/FunctionsHelper";
import { ContractService } from "@app/service/api/contract.service";
import { AppComponentBase } from "@shared/app-component-base";
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";
import { CdkDragDrop, moveItemInArray } from "@angular/cdk/drag-drop";

enum ContractRole {
  Signer = 1,
  reviewer = 2,
  Viewer = 3,
}
@Component({
  selector: "app-setting-contract",
  templateUrl: "./setting-contract.component.html",
  styleUrls: ["./setting-contract.component.css"],
})
export class SettingContractComponent
  extends AppComponentBase
  implements OnInit {
  formSigner = new FormArray([]);
  formReviewer = new FormArray([]);
  currentStep!: number;
  public contractId: number;
  public contractSetting: ContractSettingDto[] = [];
  signerColor: number = 0;
  signerOrder: number = 1;
  toggleOrderSign: boolean = false;
  oldToggleOrderSign: boolean = false;
  dbclick = false;
  arrangeSign;
  templateContractId: number;
  useTemplateContract: boolean;
  createSigner: boolean;
  public batchContract: boolean;
  templateContract: boolean;
  contractRoles: contractRoles[] = [
    {
      lable: "Người Ký",
      value: ContractRole.Signer,
    },
  ];
  user: string = localStorage.getItem('user')
  userData = JSON.parse(this.user);
  constructor(
    injector: Injector,
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private contractSettingService: ContractSettingService,
    private contractService: ContractService,
    private contractTemplateService: ContractTemplateService,
    private contractTemplateSignerService: ContractTemplateSignerService
  ) {
    super(injector);
    this.contractId = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("contractId"))
    )?.contractId;

    this.templateContract = Boolean(
      this.route.snapshot.queryParamMap.get("templateContract")
    );

    this.currentStep = Number(this.route.snapshot.queryParamMap.get("step"));

    this.templateContractId = Number(
      this.route.snapshot.queryParamMap.get("templateContractId")
    );

    this.createSigner = Boolean(
      this.route.snapshot.queryParamMap.get("createSigner")
    )

    this.useTemplateContract = Boolean(
      this.route.snapshot.queryParamMap.get("useTemplateContract")
    );

    SetLocalStorageContract(this.contractId, this.currentStep, false);
  }

  ngOnInit() {
    this.getContractSetting();
    this.batchContract = Boolean(
      this.route.snapshot.queryParamMap.get("batchContract")
    );
  }

  handleAddMe() {
    const formSignerGroup = this.fb.group({
      id: [null],
      signerName: `${this.userData.surname} ${this.userData.name}`,
      signerEmail: this.userData.emailAddress,
      contractRole: [ContractRole.Signer],
      role: [""],
      procesOrder: [this.signerOrder++],
      contractTemplateSignerId: [null],
      color: [AppConsts.signerColor[this.signerColor++]],
    });

    if (this.formSigner.value.length === 1 && !this.formSigner.value[0].signerEmail && !this.formSigner.value[0].signerName) {
      let signerColorTwo = 1;
      formSignerGroup.value.color = AppConsts.signerColor[0];
      this.signerColor = signerColorTwo;
      this.formSigner.at(0).patchValue(formSignerGroup.value)
      return
    }
    this.formSigner.insert(this.formSigner.length, formSignerGroup);
    if (this.formSigner.value.length > 1) {
      this.signerColor = 0
      const signer = this.formSigner.value.map(item => {
        return {
          ...item,
          color: AppConsts.signerColor[this.signerColor++]
        }
      })
      this.formSigner.patchValue(signer)
    }
  }

  handleAddMeReviewer() {
    const formReviewerGroup = this.fb.group({
      id: [null],
      signerName: `${this.userData.surname} ${this.userData.name}`,
      signerEmail: this.userData.emailAddress,
      procesOrder: [1],
      contractRole: [ContractRole.Viewer],
    });
    this.formReviewer.insert(this.formReviewer.length, formReviewerGroup);
  }

  handleNext() {
    if (!this.formSigner.valid || !this.formReviewer.valid) {
      this.formSigner.controls.forEach((form) => form.markAllAsTouched());
      this.formReviewer.controls.forEach((form) => form.markAllAsTouched());
      return;
    }

    if (this.formSigner.value.length === 0) {
      abp.message.error(this.ecTransform('PleaseAddASigner'));
      return;
    }

    const signerContract = this.formSigner.value.concat(
      this.formReviewer.value
    );

    if (signerContract.length === 0) {
      abp.message.error(this.ecTransform('YouHaveNotSetUpAContractParticipant'));
      return;
    }

    if (!this.toggleOrderSign) {
      signerContract.map(signer => signer.procesOrder = 1)
    } else {
      signerContract.map((signer, index) => signer.procesOrder = index + 1)
    }
    this.dbclick = true

    if (this.templateContractId && this.contractId) {
      const payloadContract = {
        contractId: this.contractId,
        contractSettings: signerContract,
      };

      this.contractSettingService.create(payloadContract).subscribe((rs) => {
        this.dbclick = false;
        this.createSigner = false;
        this.contractService._currentStep.next(contractStep.SignatureSetting);
        const encode = this.route.snapshot.queryParamMap.get("contractId");
        this.router.navigate(["/app/home/process/signatureSetting"], {
          queryParams: {
            contractId: encode,
            step: contractStep.SignatureSetting,
            templateContractId: this.templateContractId,
          },
        });
      }, () => {
        this.dbclick = false
      });
      return
    }

    if (this.templateContractId) {
      let payload = {
        contractTemplateId: this.templateContractId,
        contractTemplateSigners: signerContract
      }
      this.contractTemplateSignerService.createContractTemplateSigner(payload).subscribe(() => {
        this.dbclick = false
        this.router.navigate(["/app/templates/templates-create/design"], {
          queryParams: {
            step: contractStep.SignatureSetting,
            templateContractId: this.templateContractId
          },
          queryParamsHandling: "merge",
        });
        this.contractService._currentStep.next(contractStep.SignatureSetting);
      }, () => {
        this.dbclick = false
      })
      return
    }
    this.contractService._currentStep.next(contractStep.SignatureSetting);
    const payloadContract = {
      contractId: this.contractId,
      contractSettings: signerContract,
    };
    this.createContractSetting(payloadContract);
  }

  handleDeleteSigner(index: number, idFormControl: number) {
    this.signerColor = 0;
    this.signerOrder = 1;

    this.formSigner.removeAt(index);
    const color = this.formSigner.value.map((element) => {
      return {
        ...element,
        procesOrder: this.signerOrder++,
        color: AppConsts.signerColor[this.signerColor++],
      };
    });

    this.formSigner.patchValue(color);

    if (this.formSigner.value.length <= 1) {
      this.toggleOrderSign = false;
    }

    if (idFormControl !== null) {

      if (this.contractId) {
        this.contractSettingService.delete(idFormControl).subscribe({
          next: () => abp.notify.success(this.ecTransform('TheSignerHasBeenRemovedSuccessfully')),
        });
        return
      }
      if (this.templateContractId) {
        this.contractTemplateSignerService.deteleContractTemplateSigner(idFormControl).subscribe(value => {
          abp.notify.success(this.ecTransform('TheSignerHasBeenRemovedSuccessfully'))
        })
        return;
      }
    }
  }

  handleAddSigner() {
    const formSignerGroup = this.fb.group({
      id: [null],
      signerName: [""],
      signerEmail: [""],
      contractRole: [ContractRole.Signer],
      role: [""],
      procesOrder: [this.signerOrder++],
      contractTemplateSignerId: [null],
      color: [AppConsts.signerColor[this.signerColor++]],
    });
    this.formSigner.push(formSignerGroup);
  }

  handleDeleteReviewer(index: number, idFormControl: FormControl) {
    this.formReviewer.removeAt(index);
    if (idFormControl.value.id !== null) {

      if (this.contractId) {
        this.contractSettingService.delete(idFormControl.value.id).subscribe({
          next: () => {
            abp.notify.success(this.ecTransform('TheCopyRecipientHasBeenRemovedSuccessfully'))
          },
        });
        return
      }
      if (this.templateContractId) {
        this.contractTemplateSignerService.deteleContractTemplateSigner(idFormControl.value.id).subscribe(value => {
          abp.notify.success(this.ecTransform('TheCopyRecipientHasBeenRemovedSuccessfully'))
        })
        return;
      }
    }

  }

  handleAddReviewer() {
    const formReviewerGroup = this.fb.group({
      id: [null],
      signerName: [""],
      signerEmail: [""],
      role: [""],
      procesOrder: [1],
      contractRole: [ContractRole.Viewer],
    });
    this.formReviewer.push(formReviewerGroup);
  }

  handleBack() {

    if (this.createSigner && this.templateContractId) {
      const encode = this.route.snapshot.queryParamMap.get("contractId");
      this.contractService._currentStep.next(contractStep.UploadFile);
      this.router.navigate(["/app/home/process/upload-file"], {
        queryParams: {
          createSigner: true,
          templateContractId: this.templateContractId,
          contractId: encode,
          step: contractStep.UploadFile,
        },
      });
      return
    }

    if (this.contractId) {
      const encode = this.route.snapshot.queryParamMap.get("contractId");
      this.contractService._currentStep.next(contractStep.UploadFile);
      this.router.navigate(["/app/home/process/upload-file"], {
        queryParams: {
          ...(this.templateContractId && { templateContractId: this.templateContractId }),
          contractId: encode,
          step: contractStep.UploadFile,
        },
      });
      return
    }

    if (this.templateContractId) {
      this.router.navigate(["/app/templates/templates-create/upload"], {
        queryParams: {
          templateContractId: this.templateContractId,
          step: contractStep.UploadFile,
          templateContract: true
        },
        queryParamsHandling: "merge",
      });
      this.contractService._currentStep.next(contractStep.UploadFile);
      return
    }
  }

  drop(event: CdkDragDrop<any>) {
    let previousIndex = event.previousIndex;
    let currentIndex = event.currentIndex;
    this.oldToggleOrderSign = this.toggleOrderSign
    const signArr = [...this.formSigner.value];
    if (signArr.length > 1) {
      this.signerColor = 0;
      this.signerOrder = 1;
      this.toggleOrderSign = true;
      if (this.contractId) {
        this.contractService.checkHasInput(this.contractId).pipe().subscribe(rs => {
          if (rs.result.hasInput && rs.result.hasSign) {
            abp.message.confirm('Việc thay đổi thứ tự sẽ làm mất các vị trí ký đã được thiết lập trước đó', 'Bạn có chắc chắn muốn thay đổi thứ tự ký không?', (rs) => {
              if (rs) {
                this.contractService.removeAllSignature(this.contractId).pipe().subscribe((rs) => {
                  if (rs.success) {
                    abp.notify.success('Thay đổi thứ tự ký thành công!')
                  }
                })
              }
              else {
                this.signerColor = 0;
                moveItemInArray(signArr, currentIndex, previousIndex);
                const signArrClone = signArr.map((element) => {
                  return {
                    ...element,
                    procesOrder: this.signerOrder++,
                    color: AppConsts.signerColor[this.signerColor++],
                  };
                });
                this.formSigner.patchValue(signArrClone);
                return
              }
            })
          }
        }
        );
      }
      if (this.templateContractId) {
        this.contractTemplateService.checkHasInput(this.templateContractId).pipe().subscribe(rs => {
          if (rs.result.hasInput && rs.result.hasSign) {
            abp.message.confirm('Việc thay đổi thứ tự sẽ làm mất các vị trí ký đã được thiết lập trước đó', 'Bạn có chắc chắn muốn thay đổi thứ tự ký không?', (rs) => {
              if (rs) {
                this.contractTemplateService.removeAllSignature(this.templateContractId).pipe().subscribe((rs) => {
                  if (rs.success) {
                    abp.notify.success('Thay đổi thứ tự ký thành công!')
                  }
                })
              }
              else {
                this.signerColor = 0;
                moveItemInArray(signArr, currentIndex, previousIndex);
                const signArrClone = signArr.map((element) => {
                  return {
                    ...element,
                    procesOrder: this.signerOrder++,
                    color: AppConsts.signerColor[this.signerColor++],
                  };
                });
                this.formSigner.patchValue(signArrClone);
                return
              }
            })
          }
        }
        );
      }
      moveItemInArray(signArr, event.previousIndex, event.currentIndex);
      const signArrClone = signArr.map((element) => {
        return {
          ...element,
          procesOrder: this.signerOrder++,
          color: AppConsts.signerColor[this.signerColor++],
        };
      });
      this.formSigner.patchValue(signArrClone);
    }
  }

  createContractSetting(payloadContract) {
    this.contractSettingService.create(payloadContract).subscribe(() => {
      const encode = this.route.snapshot.queryParamMap.get("contractId");
      this.dbclick = false;
      this.router.navigate(["/app/home/process/signatureSetting"], {
        queryParams: {
          contractId: encode,
          step: contractStep.SignatureSetting,
        },
        queryParamsHandling: "merge",
      });
    }, () => {
      this.dbclick = false;
    });
  }

  handleGetSignerContract() {
    this.formSigner = new FormArray([]);
    this.formReviewer = new FormArray([]);
    this.contractSetting.forEach((value) => {
      if (
        value.contractRole == ContractRole.Signer ||
        value.contractRole == ContractRole.reviewer
      ) {
        this.signerOrder++;
        this.formSigner.push(
          this.fb.group({
            id: [this.createSigner ? null : value.id],
            signerName: [value.signerName],
            signerEmail: [value.signerEmail],
            contractRole: [value.contractRole],
            procesOrder: [value.procesOrder],
            role: [value.role],
            contractTemplateSignerId: [this.createSigner ? value.id : value.contractTemplateSignerId],
            color: [AppConsts.signerColor[this.signerColor++]],
          })
        );
      }
      if (value.contractRole == ContractRole.Viewer) {
        this.formReviewer.push(
          this.fb.group({
            id: [this.createSigner ? null : value.id],
            signerName: [value.signerName],
            signerEmail: [value.signerEmail],
            role: [value.role],
            contractRole: [value.contractRole],
            procesOrder: [value.procesOrder],
          })
        );
      }
    });
    if (this.formSigner.controls.length == 0) {
      this.handleAddSigner();
    }
  }

  getContractSetting() {
    this.formSigner = new FormArray([]);
    this.formReviewer = new FormArray([]);
    if (this.templateContractId && this.createSigner) {
      this.getContractTemplateSigner();
      return
    }

    if (this.contractId) {
      this.contractSettingService
        .GetSettingByContractId(this.contractId)
        .subscribe((rs) => {
          this.toggleOrderSign = rs.result.isOrder;
          this.contractSetting = rs.result.signers.sort(
            (a, b) => a.procesOrder - b.procesOrder
          );
          this.handleGetSignerContract()
          if (this.formSigner.controls.length == 0) {
            this.handleAddSigner();
          }
        });
      return
    }

    if (this.templateContractId) {
      this.getContractTemplateSigner();
      return
    }
  }

  getContractTemplateSigner() {
    this.contractTemplateSignerService.getAllByContractTemplateId(this.templateContractId).subscribe((rs => {
      this.toggleOrderSign = rs.result.isOrder;
      this.contractSetting = rs.result.signers?.sort(
        (a, b) => a.procesOrder - b.procesOrder
      );
      this.handleGetSignerContract();
    }));
  }

  public isShowAddSignerBtn() {
    return this.isGranted(
      PERMISSIONS_CONSTANT.ProcessStep_StepSetting_AddSigner
    );
  }

  public isShowAddReviewerBtn() {
    return this.isGranted(
      PERMISSIONS_CONSTANT.ProcessStep_StepSetting_AddReviewer
    );
  }

  public isShowDeleteBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.ProcessStep_StepSetting_Delete);
  }

  setOrderChange(event) {
    if (event) {
      if (this.contractId) {
        this.contractService.checkHasInput(this.contractId).pipe().subscribe(rs => {
          if (rs.result.hasInput && rs.result.hasSign) {
            abp.message.confirm('Việc thay đổi thứ tự sẽ làm mất các vị trí ký đã được thiết lập trước đó', 'Bạn có chắc chắn muốn thay đổi thứ tự ký không?', (rs) => {
              if (rs) {
                this.contractService.removeAllSignature(this.contractId).pipe().subscribe((rs) => {
                  if (rs.success) {
                    abp.notify.success('Thay đổi thứ tự ký thành công!')
                  }
                })
              }
              else {
                this.toggleOrderSign = false;
              }
            })
          }
        }
        );
      }
      if (this.templateContractId) {
        this.contractTemplateService.checkHasInput(this.templateContractId).pipe().subscribe(rs => {
          if (rs.result.hasInput && rs.result.hasSign) {
            abp.message.confirm('Việc thay đổi thứ tự sẽ làm mất các vị trí ký đã được thiết lập trước đó', 'Bạn có chắc chắn muốn thay đổi thứ tự ký không?', (rs) => {
              if (rs) {
                this.contractTemplateService.removeAllSignature(this.templateContractId).pipe().subscribe((rs) => {
                  if (rs.success) {
                    abp.notify.success('Thay đổi thứ tự ký thành công!')
                  }
                })
              }
              else {
                this.toggleOrderSign = false;
              }
            })
          }
        }
        );
      }
    }
  }
}
