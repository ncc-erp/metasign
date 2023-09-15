import { Component, Injector, OnInit } from '@angular/core';
import { ETypeWorker } from '@shared/AppEnums';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-create-worker',
  templateUrl: './create-worker.component.html',
  styleUrls: ['./create-worker.component.css']
})
export class CreateWorkerComponent extends DialogComponentBase<any> implements OnInit {
  listTypeWorker: string[] = [ETypeWorker.Pdfsigner, ETypeWorker.Crypto]
  typeWorkerSelected: string
  constructor(injector: Injector) { super(injector) }

  ngOnInit(): void {
  }

  addNewWorker() {
    this.dialogRef.close(this.typeWorkerSelected)
  }

}
