import { Component, OnInit, Input, Inject, Injector } from '@angular/core';
import { ContractSettingType } from '@shared/AppEnums';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-contract-preview',
  templateUrl: './contract-preview.component.html',
  styleUrls: ['./contract-preview.component.css']
})
export class ContractPreviewComponent extends DialogComponentBase<any> implements OnInit   {
  
  @Input() contractFile;
  @Input() contractSigner;
  @Input() contractName: string;
  ContractSettingType = ContractSettingType;
  
  constructor(
    injector: Injector,
  ) {
    super(injector);   
  }

  ngOnInit() {


  }


}
