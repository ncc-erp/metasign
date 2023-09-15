import { Component, OnInit, Output, Injector } from '@angular/core';
import { ContactService } from '@app/service/api/contact.service';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { EventEmitter } from '@angular/core';

@Component({
  selector: 'app-create-edit-contact',
  templateUrl: './create-edit-contact.component.html',
  styleUrls: ['./create-edit-contact.component.css']
})
export class CreateEditContactComponent extends AppComponentBase implements OnInit {
  saving: boolean = false
  contact: any
  initContact: {
    companyName: string,
    customerName: string,
    phone: string,
    email: string
  }

  @Output() onSave: EventEmitter<any> = new EventEmitter();

  constructor(injector: Injector, private contactService: ContactService, public bsModalRef: BsModalRef) { super(injector); }


  ngOnInit(): void {
    this.initContact = this.contact
    if (!this.contact) {
      this.contact = {
        companyName: '',
        customerName: '',
        phone: '',
        email: ''
      }
    }
  }

  onSaveClick() {
    this.saving = true
    if (this.initContact) {
      this.contactService.updateContact(this.contact).subscribe((res) => {
        this.notify.success(this.l(this.ecTransform('SuccessfullyEditedCustomerInformation')));
        this.onSave.emit()
        this.bsModalRef.hide()
      }, () => {
        this.saving = false
      })
    } else {
      this.contactService.createContact({
        companyName: this.contact?.companyName,
        customerName: this.contact?.customerName,
        phone: this.contact?.phone,
        email: this.contact?.email
      }).subscribe(() => {
        this.notify.success(this.l(this.ecTransform('AddedNewCustomersSuccessfully')));
        this.onSave.emit()
        this.bsModalRef.hide()
      }, () => {
        this.saving = false
      })
    }
  }

  handlePhoneNumber(event: KeyboardEvent) {
    const key = event.key;
    if (key !== '+' && isNaN(+key)) {
      event.preventDefault();
      return
    }
    if (key === '+' && this.contact.phone.length !== 0 && this.contact.phone.includes('+')) {
      event.preventDefault();
      return
    }
  }
}
