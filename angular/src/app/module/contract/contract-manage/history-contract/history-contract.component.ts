import { Component, Injector, OnInit } from '@angular/core';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { ContractHistoryService } from '../../../../service/api/contract-history.service'
import { EHistoryAction, EHistoryActionName } from '../../enum/contractEnums'
import { EContractStatusId, EStatusContract } from '@shared/AppEnums';
import { MatDialogRef } from '@angular/material/dialog';


@Component({
  selector: 'app-history-contract',
  templateUrl: './history-contract.component.html',
  styleUrls: ['./history-contract.component.css']
})
export class HistoryContractComponent extends DialogComponentBase<any> implements OnInit {
  public idContract: number
  listHistory
  isShowDialog: boolean
  listActions: { id: number, name: string }[] = [{ id: EHistoryAction.CreateContract, name: EHistoryActionName.CreateContract },
  { id: EHistoryAction.SendMail, name: EHistoryActionName.SendMail },
  { id: EHistoryAction.CancelContract, name: EHistoryActionName.CancelContract },
  { id: EHistoryAction.Sign, name: EHistoryActionName.Sign },
  { id: EHistoryAction.Complete, name: EHistoryActionName.Complete },
  { id: EHistoryAction.VoidTosign, name: EHistoryActionName.VoidToSign }]
  listStatusContract: { id: number, name: string, color: string, icon: string }[] = [{ id: -1, name: EStatusContract.All, color: '', icon: 'fas fa-list' },
  { id: EContractStatusId.Draft, name: EStatusContract.Draft, color: 'badge badge-secondary', icon: 'fa-regular fa-file' },
  { id: EContractStatusId.Inprogress, name: EStatusContract.Inprogress, color: 'badge badge-primary', icon: 'fa-regular fa-pen-to-square' },
  { id: EContractStatusId.Cancelled, name: EStatusContract.Cancelled, color: 'badge badge-danger', icon: 'fas fa-xmark' },
  { id: EContractStatusId.Complete, name: EStatusContract.Completed, color: 'badge badge-success', icon: 'fa-regular fa-circle-check' }]

  constructor(injector: Injector, private contractHistoryService: ContractHistoryService, public dialogRef: MatDialogRef<HistoryContractComponent>) { super(injector) }

  ngOnInit(): void {
    Object.assign(this, this.dialogData)
    this.contractHistoryService.GetHistoryContractById(this.idContract).subscribe(res => this.listHistory = res.result)
  }
}
