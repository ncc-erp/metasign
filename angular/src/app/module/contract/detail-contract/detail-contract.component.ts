import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { ContractService } from '@app/service/api/contract.service';
import { ContractInvalidate, EButtonActionPageDetail, EContractStatusId, EStatusContract, ESubPathProcess } from '@shared/AppEnums';
import { HistoryContractComponent } from '../contract-manage/history-contract/history-contract.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { AppComponentBase } from '@shared/app-component-base';
import { SessionServiceProxy } from '@shared/service-proxies/service-proxies';
import { ContractRole } from '@shared/AppEnums';
import * as moment from 'moment';
import { IResendMail } from '../interface/interface';
import { DialogDownloadComponent } from '../contract-manage/dialog-download/dialog-download.component';
import { ConnectionPositionPair } from '@angular/cdk/overlay';
import { ContractInvalidateDialogComponent } from '@app/module/unauthen-pages/un-authen-signing/contract-invalidate-dialog/contract-invalidate-dialog.component';
import { EButtonLeftPageDetail } from '../enum/contractEnums';
@Component({
  selector: 'app-detail-contract',
  templateUrl: './detail-contract.component.html',
  styleUrls: ['./detail-contract.component.css']
})
export class DetailContractComponent extends AppComponentBase implements OnInit {
  contractsLocalStorage
  contract !: {
    contractBase64: string, contractCode: string, contractFile: string, contractName: string, createdUser: string, isAssigned: boolean,
    creationTime: string, isMyContract: boolean, recipients: [{
      name: string, email: string, role: 1, isComplete: boolean, processOrder: number,
      isSendMail: boolean, updateDate: string
    }], contractId: number, settingId: number, signUrl: string, status: number, updatedTime: string, expriedTime: string
  }
  idContract!: number
  contractGuid: string = ""
  listSignerProcessOrderMailStatus = [];
  user
  listButtonActions = [{ id: 1, name: EButtonActionPageDetail.Edit }, { id: 2, name: EButtonActionPageDetail.Sign }, { id: 3, name: EButtonActionPageDetail.Cancel }, { id: 0, name: EButtonActionPageDetail.Resend }]
  listStatusContract: { id: number, name: string, color: string }[] =
    [{ id: -1, name: EStatusContract.All, color: '' },
    { id: EContractStatusId.Draft, name: EStatusContract.Draft, color: 'badge badge-secondary' },
    { id: EContractStatusId.Inprogress, name: EStatusContract.Inprogress, color: 'badge badge-primary' },
    { id: EContractStatusId.Cancelled, name: EStatusContract.Cancelled, color: 'badge badge-danger' },
    { id: EContractStatusId.Complete, name: EStatusContract.Completed, color: 'badge badge-success' }]
  subPaths = [ESubPathProcess.UpLoadFile, ESubPathProcess.Setting, ESubPathProcess.SignatureSetting, ESubPathProcess.EmailSetting]
  isOrderSign: boolean;
  public contractNote: string;
  public isOpen = false;
  public positions = [
    new ConnectionPositionPair({
      originX: 'start',
      originY: 'top'
    }, {
      overlayX: 'center',
      overlayY: 'bottom'
    },
      293,
      135)
  ];

  constructor(
    injector: Injector,
    private contractService: ContractService,
    private route: ActivatedRoute,
    private _sessionService: SessionServiceProxy,
    private router: Router, private dialog: MatDialog,
  ) {
    super(injector);
    this.contractsLocalStorage = JSON.parse(localStorage.getItem('contracts') as string)
  }

  ngOnInit(): void {
    this.load();
  }
  load() {
    this.route.params.subscribe(res => this.idContract = JSON.parse(
      decodeURIComponent(res['id'])
    )?.contractId)
    this.contractService.GetContractDetail(this.idContract).subscribe(res => {
      this.contractNote = res.result.note;
      this.contract = res.result
      this.contractGuid = res.result.contractGuid
      this.isOrderSign = !!this.contract.recipients.find(recipient => recipient.processOrder !== 1)
      if (this.isOrderSign) {
        this.contract.recipients.sort((a, b) => a.processOrder - b.processOrder);
      }
    })
    this._sessionService.getCurrentLoginInformations().subscribe(res => this.user = res.user)
  }

  handleClickBack() {
    this.router.navigate(['/app/contracts'])
  }

  viewHistoryContract() {
    const dialogRef = this.dialog.open(HistoryContractComponent, {
      data: {
        idContract: this.idContract,

      },
      width: '60%',
      maxWidth: '60%',
      height: '60vh',
      panelClass: 'history-contract-dialog',
    })
  }

  isRecipients(userContract: number) {
    return this.contract?.recipients.filter(recipient => recipient.role === userContract)?.length;
  }

  handleEditContract() {
    let contractEdit = this.contractsLocalStorage?.find(contract => contract.contractId === this.idContract)

    const contract = {
      contractId: contractEdit ? contractEdit.contractId : this.idContract,
    };
    const encode = encodeURIComponent(JSON.stringify(contract));
    contractEdit ? this.router.navigate([`/app/home/process/${this.subPaths[contractEdit.step]}`], {
      queryParams: {
        contractId: encode,
        step: contractEdit.step
      }
    }) : this.router.navigate([`/app/home/process/${this.subPaths[1]}`], {
      queryParams: {
        contractId: encode,
        step: 1
      }
    })
  }

  cancelContract() {
    const show = this.dialog.open(ContractInvalidateDialogComponent, {
      data: {
        contractId: this.idContract,
        role: ContractInvalidate.contractCreator,
        title: "CancelContract"
      },
      width: "50%",
      height: "50%",
    });
    show.afterClosed().subscribe(() => {
      this.load();
    })
  }

  handleCopyValue(id) {
    this.isOpen = false;
    abp.notify.success(`${id} ${this.ecTransform("ContractIdCopy")}`);
  }

  downloadPdf(fileName: string): void {
    this.dialog.open(DialogDownloadComponent, {
      data: {
        fileName: fileName,
        idContract: this.idContract,
      },
      minWidth: '36%',
      minHeight: '250px',
    })
  }

  handleOpenContractId() {
    this.isOpen = !this.isOpen
  }

  isShowEditBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Contract_Detail_Edit)
  }

  isShowSendMailBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Contract_Detail_SendMail)
  }

  isShowViewHistoryBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Contract_Detail_ViewHistory)
  }

  isShowCancelBtn(contractStatus: number) {
    return contractStatus == EContractStatusId.Inprogress && this.isGranted(PERMISSIONS_CONSTANT.Contract_Detail_Cancel);
  }

  isShowDownloadBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Contract_Detail_Download)
  }

  isShowPrintBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Contract_Detail_Print)
  }

  handleShowButton(idButton: number) {
    switch (idButton) {
      case 0: {
        return this.contract?.isMyContract && this.contract?.status === EContractStatusId.Inprogress && this.isShowSendMailBtn();
      }
      case 1: {
        return this.contract?.status === EContractStatusId.Draft && this.isShowEditBtn()
      }
      case 2: {
        let userSign = this.contract?.recipients.find(recipient => recipient.email === this.appSession.user.emailAddress)
        return userSign && userSign.role === ContractRole.Signer && !userSign?.isComplete && this.contract?.status === EContractStatusId.Inprogress && this.isShowSignBtn() && userSign.isSendMail;
      }
      case 3: {
        return this.isShowCancelBtn(this.contract?.status) && this.contract.isMyContract && this.contract.status == EContractStatusId.Inprogress && this.isGranted(PERMISSIONS_CONSTANT.Contract_Detail_Cancel)
      }
    }
  }

  handleSendMailOne(data: IResendMail) {
    const payload = {
      resentToMail: data.email,
      contractId: this.idContract
    }

    this.contractService.ResendMailOne(payload).subscribe(() => {
      abp.notify.success(this.ecTransform('SendEmailSuccessfully'))
    },
      (err) => {
        abp.notify.error(this.ecTransform("SendEmailFailed"))
      })
  }

  handleSendMailAll() {
    abp.message.confirm('', this.ecTransform('AreYouSureYouWantToResendTheEmail'), (res) => {
      if (res) {
        this.contractService.ResendMailAll(this.idContract).subscribe(() => {
          abp.notify.success(this.ecTransform('SendEmailSuccessfully'))
        },
          (err) => {
            abp.notify.error(this.ecTransform("SendEmailFailed"))
          })
      }
    })
  }

  handleClickListButtonLeft(idButton: number) {
    switch (idButton) {
      case EButtonLeftPageDetail.SendMail: {
        this.handleSendMailAll()
        break
      }
      case EButtonLeftPageDetail.Edit: {
        this.handleEditContract()
        break
      }
      case EButtonLeftPageDetail.Cancel: {
        this.cancelContract()
        break
      }
    }
  }

  isShowSignBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Contract_Detail_Sign)
  }

  formatDate(date) {
    if (!date) { return ""; }
    return moment(date).format('DD/MM/YYYY | HH:mm:ss');
  }

  getContentTime(isComplete: boolean, isSendMail: boolean, isCanceled: boolean) {
    if (isCanceled) {
      return "VoidedOn "
    }
    if (isSendMail && !isComplete) {
      return "SentOn "
    }
    if (isSendMail && isComplete) {
      return "SignedOn "
    }
    return ""
  }
  signNow(signUrl) {
    if (localStorage.getItem("notSignNow")) {
      localStorage.removeItem("notSignNow")
    }
    localStorage.setItem("notSignNow", "0");
    window.open(signUrl, "_blank");
  }
}
