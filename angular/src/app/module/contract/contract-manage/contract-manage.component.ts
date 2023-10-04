import { ContractDto } from './../../../service/model/contract.dto';
import { Component, Injector } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ContractService } from '@app/service/api/contract.service';
import { ContractInvalidate, ContractStatus, contractStep, EContractFilterType, EContractStatusId, EDisplayedColumnContract, EQuickFilter, EStatusContract, ESubPathProcess } from '@shared/AppEnums';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { finalize, concatMap, startWith, map } from 'rxjs/operators';
import { HistoryContractComponent } from './history-contract/history-contract.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { SessionServiceProxy } from '@shared/service-proxies/service-proxies';
import { Observable } from 'rxjs';
import { EChangeSortContract, EFilterSigner, ESortContract } from '../enum/contractEnums';
import { IStatusFindName } from '../interface/interface';
import { DialogDownloadComponent } from './dialog-download/dialog-download.component';
import * as moment from 'moment';
import { ConfigurationService } from '@app/service/api/configuration.service';
import { ContractInvalidateDialogComponent } from '@app/module/unauthen-pages/un-authen-signing/contract-invalidate-dialog/contract-invalidate-dialog.component';

class PagedContractsRequestDto extends PagedRequestDto {
  filterType: number
  filterItems: [{
    propertyName: string,
    value: string,
    comparision: 0

  }]
  searchText: string = '';
  sort?: string;
  sortDirection?: number
}

@Component({
  selector: 'app-contract-manage',
  templateUrl: './contract-manage.component.html',
  styleUrls: ['./contract-manage.component.css']
})
export class ContractManageComponent extends PagedListingComponentBase<any> {
  statusFindName!: IStatusFindName
  $filterSigner: Observable<string[]>;
  contractFilterType = EContractFilterType
  listContract = [];
  listSigners = [];
  filterSigner = 'All';
  searchSinger = new FormControl('');
  statusAllContract: boolean = false;
  showSignerFilter = true;
  totalCount: number
  contractsLocalStorage!: [{ contractId: number, step: number }]
  searchText: string = ''
  currentFilter: number = -1
  currentStatus: number = -1
  checkedNumberSort = EChangeSortContract
  messTable: string;
  user
  currentDate = moment().format()
  listStatusContract: { id: number, name: string, color: string, icon: string }[] = [{ id: -1, name: EStatusContract.All, color: '', icon: 'fas fa-list' },
  { id: EContractStatusId.Draft, name: EStatusContract.Draft, color: 'badge badge-secondary', icon: 'fa-regular fa-file' },
  { id: EContractStatusId.Inprogress, name: EStatusContract.Inprogress, color: 'badge badge-primary', icon: 'fa-regular fa-pen-to-square' },
  { id: EContractStatusId.Cancelled, name: EStatusContract.Cancelled, color: 'badge badge-danger', icon: 'fas fa-xmark' },
  { id: EContractStatusId.Complete, name: EStatusContract.Completed, color: 'badge badge-success', icon: 'fa-regular fa-circle-check' }]
  listQuickFilter: { id: number, name: string, icon: string }[] =
    [{ id: EContractFilterType.AssignToMe, name: EQuickFilter.AssignToMe, icon: 'fas fa-circle-exclamation' },
    { id: EContractFilterType.WatingForOther, name: EQuickFilter.WaitingOthers, icon: 'fa-regular fa-clock' },
    { id: EContractFilterType.ExpirgingSoon, name: EQuickFilter.ExpirgingSoon, icon: 'fas fa-triangle-exclamation' },
    { id: EContractFilterType.Completed, name: EQuickFilter.Completed, icon: 'fa-regular fa-circle-check' }]
  listQuickFilterId: number[] = [EContractFilterType.AssignToMe, EContractFilterType.WatingForOther, EContractFilterType.ExpirgingSoon, EContractFilterType.Completed]
  subPaths = [ESubPathProcess.UpLoadFile, ESubPathProcess.Setting, ESubPathProcess.SignatureSetting, ESubPathProcess.EmailSetting]
  displayedColumns: { name: string, width: number, isCheckedSort: boolean, statusSort?: number }[] =
    [{ name: EDisplayedColumnContract.NumericalOrder, width: 5, isCheckedSort: false, },
    { name: EDisplayedColumnContract.Name, width: 22.5, isCheckedSort: true, statusSort: EChangeSortContract.Default },
    { name: EDisplayedColumnContract.Code, width: 12.5, isCheckedSort: false }, { name: EDisplayedColumnContract.Status, width: 15, isCheckedSort: false },
    { name: EDisplayedColumnContract.CreationTime, width: 15, isCheckedSort: true, statusSort: EChangeSortContract.Default },
    { name: EDisplayedColumnContract.UpdatedTime, width: 15, isCheckedSort: true, statusSort: EChangeSortContract.Default },
    { name: EDisplayedColumnContract.ExpriedTime, width: 10, isCheckedSort: false }]
  expriedTime: number
  public search: string = ""

  constructor(injector: Injector,
    private contractService: ContractService,
    private router: Router,
    private dialog: MatDialog,
    private _sessionService: SessionServiceProxy,
    private configurationService: ConfigurationService
  ) {
    super(injector)
    this.contractsLocalStorage = JSON.parse(localStorage.getItem('contracts') as string)
    this._sessionService.getCurrentLoginInformations().subscribe(res => this.user = res.user)
  }

  ngOnInit() {
    this.contractService._currentStatus$.subscribe(res => {
      if (this.currentStatus !== res) {
        this.currentStatus = res
      }
    })
    this.contractService._currentQuickFilter$.subscribe(res => {
      if (this.currentFilter !== res && res) {
        this.currentFilter = res;
        this.contractService._currentStatus.next(-2)
      }
    }
    )
    this.refresh();
    this.GetAllSigners();

    this.configurationService.getNotiExprireTime().subscribe(res => {
      this.expriedTime = parseInt(res.result?.notiExprireTime)
    })
  }

  handleChangeSortContract(name: string, isCheckedSort: boolean) {
    if (isCheckedSort) {
      this.displayedColumns.forEach(item => {
        if (item.name !== name) {
          item.statusSort = EChangeSortContract.Default
          return
        }
        if (item.name === name) {
          if (item.statusSort >= EChangeSortContract.Down) {
            item.statusSort = EChangeSortContract.Default
            this.handleCheckTypeSortContract(name, item.statusSort)
            this.getDataPage(1);
            return
          }
          item.statusSort++
          this.handleCheckTypeSortContract(name, item.statusSort)
          this.getDataPage(1);
        }
      })
    }
  }

  handleCheckTypeSortContract(name: string, status: number) {
    switch (name) {
      case EDisplayedColumnContract.Name:
        this.statusFindName = {
          sort: ESortContract.Name,
          status
        }
        return
      case EDisplayedColumnContract.CreationTime:
        this.statusFindName = {
          sort: ESortContract.CreationTime,
          status
        }
        return
      case EDisplayedColumnContract.UpdatedTime:
        this.statusFindName =
        {
          sort: ESortContract.UpdatedTime,
          status
        }
        return
    }
  }

  handleResetOption() {
    this.statusAllContract = true
    this.filterSigner = EFilterSigner.All;
    this.getDataPage(1);
  }

  GetAllSigners() {
    this.contractService.GetAllSigners().pipe(concatMap(res => {
      if (res.success) {
        this.listSigners = res.result;
      }
      return this.$filterSigner = this.searchSinger.valueChanges.pipe(
        startWith(''),
        map(value => this._filter(value || '')),
      )
    })).subscribe()
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase().trim();
    return this.listSigners.filter(option =>
      {
       return option.email?.toLowerCase().includes(filterValue)
      }
      );
  }

  protected list(request: PagedContractsRequestDto, pageNumber: number, finishedCallback: Function): void {
    let contractsigner;
    let newRequest
    request.sort = this.statusFindName?.sort

    request.searchText = this.searchText
    if (this.statusFindName?.status !== EChangeSortContract.Default) {
      request.sortDirection = this.statusFindName?.status

    } else {
      newRequest = request;
      delete newRequest.sortDirection;
      delete newRequest.sort;
      request = newRequest
    }

    if (this.currentStatus !== -1 && this.currentStatus !== -2) {
      request.filterItems = [{
        propertyName: "status",
        value: this.currentStatus.toString(),
        comparision: 0
      }]
    }

    switch (this.filterSigner) {
      case EFilterSigner.All:
        contractsigner = ''
        break;
      case EFilterSigner.Me:
        contractsigner = this.appSession.user.emailAddress
        break;
      default:
        contractsigner = this.filterSigner
        break;
    }

    let requestContractFilter = {
      filterType: this.currentFilter,
      gridParam: request,
      signerEmail: contractsigner,
      search: this.search
    }

    this.contractService.GetContractByFilterPaging(requestContractFilter).pipe(
      finalize(() => {
        finishedCallback();
      })
    )
      .subscribe((res: any) => {
        this.totalCount = res.result.totalCount
        this.listContract = res.result.items.map((contract, index) => {
          return { ...contract, position: 10 * (pageNumber - 1) + index + 1 }
        })
        this.showPaging(res.result, pageNumber);
        if (res.result.isSearch && res.result.items.length === 0) {
          this.messTable = "NoMatchingSearchResults";
          return;
        }

        if (res.result.items.length === 0) {
          this.messTable = "YouCurrentlyDoNotHaveAnyContracts";
          return;
        }
      });
  }

  protected delete(user: any): void {
    abp.message.confirm(
      this.l(this.ecTransform('UserDeleteWarningMessage'), user.fullName),
      undefined,
      (result: boolean) => {
        if (result) {
          this.contractService.delete(user.id).subscribe(() => {
            abp.notify.success(this.l(this.ecTransform('SuccessfullyDeleted')));
            this.refresh();
          });
        }
      }
    );
  }

  clearFilters(): void {
    this.searchText = '';
    this.getDataPage(1);
  }

  handleClickStatus(id: number) {
    this.showSignerFilter = true;
    this.contractService._currentQuickFilter.next(-1)
    this.contractService._currentStatus.next(id)
    this.getDataPage(1);
  }

  handleClickQuickFilter(id: number) {
    this.showSignerFilter = false;
    this.filterSigner = EFilterSigner.All;
    this.contractService._currentStatus.next(-2)
    this.contractService._currentQuickFilter.next(id)
    this.getDataPage(1);
  }


  handleSearchContract() {
    this.listContract = []
    this.refresh()
  }

  addNewContract() {
    this.router.navigate(["/app/home/process/upload-file"], {
      queryParams: {
        step: contractStep.UploadFile
      }
    })
  }

  handleEditContract(IdContract: number) {
    let contractEdit = this.contractsLocalStorage?.find(contract => contract.contractId === IdContract)
    const contract = {
      contractId: contractEdit ? contractEdit.contractId : IdContract
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

  viewHistoryContract(id: number) {
    const dialogRef = this.dialog.open(HistoryContractComponent, {
      data: {
        idContract: id,
      },
      width: '60%',
      height: '60%',
    })
  }

  cancelContract(contract: ContractDto) {
    const show = this.dialog.open(ContractInvalidateDialogComponent, {
      data: {
        contractId: contract.id,
        role: ContractInvalidate.contractCreator,
        title: "CancelContract"
      },
      width: "50%",
      height: "50%",
    });
    show.afterClosed().subscribe(() => { this.refresh() });
  }

  deleteContract(contract: ContractDto) {
    abp.message.confirm(`
    <div style="font-size: 26px;">
      ${this.ecTransform("AreYouSureYouToDeleteContract")}
      <strong style="font-size: 26px;">${contract.name}</strong>
      ${this.ecTransform("ContractOrNo")}
    </div>`, ` `, (rs) => {
      if (rs) {
        this.contractService.delete(contract.id).subscribe(rs => {
          abp.notify.success(this.ecTransform("ContractDeletedSuccessfully"))
          this.refresh()
        })
      }
    }, { ishtml: true })
  }

  isShowEditBtn(contractStatus: number) {
    if (this.isGranted(PERMISSIONS_CONSTANT.Contract_Edit)) {
      if (contractStatus == EContractStatusId.Draft /*|| contractStatus == EContractStatusId.Cancelled*/) {
        return true
      }
    }
    return false
  }

  isShowCancelBtn(contractStatus: number) {
    return contractStatus == EContractStatusId.Inprogress && this.isGranted(PERMISSIONS_CONSTANT.Contract_Cancel);
  }

  isShowDeleteBtn(contractStatus: number) {
    return contractStatus == EContractStatusId.Draft && this.isGranted(PERMISSIONS_CONSTANT.Contract_Delete);
  }

  iShowCreateBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Contract_Create);
  }

  isshowViewHistoryBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Contract_ViewHistory);
  }

  isShowSignBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Contract_Sign)
  }

  isShowDownloadBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Contract_Download)
  }

  viewDetailContract(contractId: number) {
    const contract = {
      contractId: contractId,
    };
    const encode = encodeURIComponent(JSON.stringify(contract));
    if (this.isGranted(PERMISSIONS_CONSTANT.Contract_Detail_View)) {
      this.router.navigate(['/app/contracts/details/' + encode])
    }
  }

  isShowResendMail(contractStatus: number, contract: boolean) {
    return contractStatus == EContractStatusId.Inprogress && contract;
  }

  handleShowTooltip(quickViewsItemId: number) {
    switch (quickViewsItemId) {
      case EContractFilterType.AssignToMe: {
        return "ContractsNeedMySignature"
      }
      case EContractFilterType.WatingForOther: {
        return "ContractsNeedOthersSignatures"
      }
      case EContractFilterType.ExpirgingSoon: {
        return "ContractsWillExpireWithin"
      }
      case EContractFilterType.Completed: {
        return null
      }
    }
  }

  handleSendMailAll(idContract: number) {
    abp.message.confirm('', this.ecTransform('AreYouSureYouWantToResendTheEmail'), (res) => {
      if (res) {
        this.contractService.ResendMailAll(idContract).subscribe(() => {
          abp.notify.success(this.ecTransform('SendEmailSuccessfully'))
        },
          (err) => {
            abp.notify.error(this.ecTransform("SendEmailFailed"))
          })
      }
    })
  }

  downloadPdf(id: number, fileName: string): void {
    this.dialog.open(DialogDownloadComponent, {
      data: {
        fileName: fileName,
        idContract: id,
      },
      minWidth: '36%',
      minHeight: '250px',
      panelClass: 'download-dialog'
    })
  }

  calculateRemainingTime(startDate, endDate) {
    const duration = moment.duration(moment(endDate).diff(moment(startDate)));
    return Math.ceil(duration.asDays());
  }

  handleShowDaysLeft(contract) {
    return contract?.expriedTime && contract.isExprireSoon && this.currentFilter !== EContractFilterType.Completed && (this.currentStatus === -1 || contract?.status === ContractStatus.Inprogress)
  }
  signNow(signUrl) {
    if (localStorage.getItem("notSignNow")) {
      localStorage.removeItem("notSignNow")
    }
    localStorage.setItem("notSignNow", "0");
    window.open(signUrl, "_blank");
  }
}


