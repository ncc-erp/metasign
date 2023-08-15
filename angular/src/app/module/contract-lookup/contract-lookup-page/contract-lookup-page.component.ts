import { ContractLookupService } from "./../../../service/api/contract-lookup.service";
import { Component, OnInit } from "@angular/core";
import { ContractRole, ContractSettingType, ContractStatus, EContractStatusId, ENameStatusContract } from "@shared/AppEnums";
import { ActivatedRoute, Router } from "@angular/router";
import { FormBuilder, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { DialogDownloadComponent } from "@app/module/contract/contract-manage/dialog-download/dialog-download.component";
@Component({
    selector: "app-contract-lookup-page",
    templateUrl: "./contract-lookup-page.component.html",
    styleUrls: ["./contract-lookup-page.component.css"],
})
export class ContractLookupPageComponent implements OnInit {
   
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
    listStatusContract: { id: number, name: string, color: string }[] = [{ id: -1, name: ENameStatusContract.All, color: '' },
    { id: EContractStatusId.Draft, name: ENameStatusContract.Draft, color: ' text-secondary' },
    { id: EContractStatusId.Inprogress, name: ENameStatusContract.Inprogress, color: ' text-primary' },
    { id: EContractStatusId.Cancelled, name: ENameStatusContract.Cancelled, color: ' text-danger' },
    { id: EContractStatusId.Complete, name: ENameStatusContract.Completed, color: ' text-success' }]

    formSearch = this.fb.group({
        id: ['', [Validators.required]]
    });

    constructor(
        private contractLookupService: ContractLookupService,
        private route: ActivatedRoute,
        private fb: FormBuilder,
        private dialog: MatDialog,
        private router: Router,
    ) {
     
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
