import { ContractLookupService } from "./../../../service/api/contract-lookup.service";
import { Component, OnInit, Injector} from "@angular/core";
import { ContractRole, ContractSettingType, ContractStatus, EContractStatusId, ENameStatusContract, EStatusContract } from "@shared/AppEnums";
import { ActivatedRoute, Router } from "@angular/router";
import { FormBuilder, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { DialogDownloadComponent } from "@app/module/contract/contract-manage/dialog-download/dialog-download.component";
import { AppComponentBase } from "@shared/app-component-base";
@Component({
    selector: "app-contract-lookup-page",
    templateUrl: "./contract-lookup-page.component.html",
    styleUrls: ["./contract-lookup-page.component.css"],
})
export class ContractLookupPageComponent extends AppComponentBase implements OnInit {
   
    public signatureType = ContractSettingType
    public valueContract: string;
    public contracts = [];
    public contractStatus = ContractStatus;
    private emailContract: string;
    public listCopyRecipient
    public listSigner
    public isProcessOrder: boolean = false
    public isSearched: boolean = false;
    isTablet: boolean = window.innerWidth > 480 && window.innerWidth <= 768;
    isMobile: boolean = window.innerWidth >= 320 && window.innerWidth <= 480
    listStatusContract: { id: number, name: string, color: string }[] = [{ id: -1, name: EStatusContract.All, color: '' },
    { id: EContractStatusId.Draft, name: EStatusContract.Draft, color: ' text-secondary' },
    { id: EContractStatusId.Inprogress, name: EStatusContract.Inprogress, color: ' text-primary' },
    { id: EContractStatusId.Cancelled, name: EStatusContract.Cancelled, color: ' text-danger' },
    { id: EContractStatusId.Complete, name: EStatusContract.Completed, color: ' text-success' }]
    isNull: boolean = false;
    formSearch = this.fb.group({
        id: ['', [Validators.required]]
    });

    constructor(
        private contractLookupService: ContractLookupService,
        private route: ActivatedRoute,
        private fb: FormBuilder,
        private dialog: MatDialog,
        private router: Router,
        private injector: Injector,
    ) {
        super(injector)
    }

    ngOnInit() {
        this.emailContract = this.route.snapshot.queryParamMap.get("email");
        if(localStorage.getItem('email') !==  this.emailContract)
        {
            this.router.navigate(["app/email-login"]);
        }
    }

    handleSearchGuid() {
        if (!this.formSearch.valid) {
            this.formSearch.get('id').markAsTouched();
        }
        let contract = {
            email: this.emailContract,
            guid: this.valueContract,
        };

        this.contractLookupService
            .getContractDetailByGuid(contract)
            .subscribe((value) => {
                this.isNull = value.result ?  false : true
                this.contracts = [
                    {
                        name: value.result.contractName,
                        position: 1,
                        ...value.result,
                    },
                ];
                this.listSigner = this.contracts[0]?.recipients.filter(recipient => recipient?.role === ContractRole.Signer).sort((a, b) => a?.processOrder - b?.processOrder)
                this.listCopyRecipient = this.contracts[0]?.recipients.filter(recipient => recipient?.role === ContractRole.Viewer)
                this.isProcessOrder = this.listSigner.some(signer => signer?.processOrder !== 1)
            });
        this.isSearched = true
    }

    displayStatusContract(status) {
        return this.listStatusContract.find(statusItem => statusItem.id === status).name
    }

    handleColorStatus(status) {
        return this.listStatusContract.find(statusItem => statusItem.id === status).color
    }

    downloadPdf(fileName: string): void {
        this.dialog.open(DialogDownloadComponent, {
            data: {
                fileName: fileName,
                idContract: this.contracts[0]?.contractId,
            },
            minWidth: '36%',
            minHeight: '250px',
        })
    }

    ngOnDestroy(): void {
        localStorage.removeItem('email');
    }

}
