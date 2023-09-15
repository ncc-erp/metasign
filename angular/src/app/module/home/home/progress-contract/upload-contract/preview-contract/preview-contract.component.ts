import { Component, OnInit, Injector, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SignerSignatureSettingService } from '@app/service/api/signer-signature-setting.service';
import { ContractSettingType } from '@shared/AppEnums';
import { DialogComponentBase } from '@shared/dialog-component-base';
@Component({
  selector: 'app-preview-contract',
  templateUrl: './preview-contract.component.html',
  styleUrls: ['./preview-contract.component.css']
})
export class PreviewContractComponent extends DialogComponentBase<any> implements OnInit {
  base64Pdf: string
  contractFile
  isSignedPreview: boolean
  contractFileName: string
  contractLoading: boolean = true
  listSignatureTypeId = {
    electronic: ContractSettingType.Electronic,
    digital: ContractSettingType.Digital,
    acronym: ContractSettingType.Acronym,
    text: ContractSettingType.Text,
    datePicker: ContractSettingType.DatePicker,
    dropdown: ContractSettingType.Dropdown,
    stamp: ContractSettingType.Stamp
  }

  constructor(injector: Injector, @Inject(MAT_DIALOG_DATA) public data: any, private signerSignatureSettingService: SignerSignatureSettingService) { super(injector) }

  ngOnInit(): void {
    this.data.isSignedPreview ? this.contractFile = this.data.contractFilePreviewDesign : this.contractFile = this.data.contractFile
    this.contractFileName = this.data.contractFileName
    this.contractLoading = false
  }
}
