import { Component, Injector, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { SignServerDataService } from '../../services/sign-server.service';
import { SubscriptionLike } from 'rxjs';

@Component({
  selector: 'app-create-properties',
  templateUrl: './create-properties.component.html',
  styleUrls: ['./create-properties.component.css']
})
export class CreatePropertiesComponent extends DialogComponentBase<any> implements OnInit {
  propertiesWorker
  propertiesCanBeAdded: { editable: boolean, key: string }[];

  formCreateProperties = this.fb.group({
    name: new FormControl('', Validators.required),
    value: new FormControl('', Validators.required),
  })
  subscription: SubscriptionLike

  constructor(injector: Injector, private fb: FormBuilder, @Inject(MAT_DIALOG_DATA) public data: any, private signServerDataService: SignServerDataService) {
    super(injector)
  }

  ngOnInit(): void {
    this.propertiesWorker = this.data.propertiesWorker;
    this.subscription = this.signServerDataService._propertiesCanBeAdded$.subscribe(res => {
      this.propertiesCanBeAdded = res
    })
  }

  ngOnDestroy() {
    if (this.subscription) { this.subscription.unsubscribe() }
  }

  submitForm(data) {
    this.dialogRef.close(data);
  }
}
