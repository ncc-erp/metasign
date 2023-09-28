import { DialogComponentBase } from '@shared/dialog-component-base';
import { ContractTemplateType } from './../../../../../contract-templates/enum/contract-template.enum';
import { ContractTempaleteDto } from '@app/service/model/contract.dto';
import { ContractTemplateService } from './../../../../../../service/api/contract-template.service';
import { Component, Injector, OnInit } from '@angular/core';

@Component({
  selector: 'app-dialog-contract-editor',
  templateUrl: './dialog-contract-editor.component.html',
  styleUrls: ['./dialog-contract-editor.component.css']
})
export class DialogContractEditorComponent extends DialogComponentBase<any> implements OnInit {
  loadingTemplate: boolean = false;
  contractTemplate: string;
  constructor(
    injector: Injector,
    private contractTemplateService: ContractTemplateService,
  ) {
    super(injector);
  }

  ngOnInit() {
  }

  // handleSaveContractTemplate() {
  //   let contractPayload: ContractTempaleteDto = {
  //     name: 'Mẫu hợp đồng',
  //     fileName: 'Mẫu hợp đồng.pdf',
  //     content: "",
  //     htmlContent: this.contractTemplate,
  //     type: ContractTemplateType.Me,
  //     userId: this.appSession.userId,
  //     isFavorite: false,

  //   };
  //   if (this.contractTemplate) {
  //     this.loadingTemplate = true;
  //     this.contractTemplateService.createFileTemplate(contractPayload).subscribe(value => {
  //       this.loadingTemplate = false;
  //       this.dialogRef.close(value.result);
  //     });
  //   }
  //   else {
  //     abp.message.error(this.ecTransform("EmptyOrInvalidContent"))
  //   }
  // }
}
