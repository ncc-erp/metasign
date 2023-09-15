import { Component, Injector } from '@angular/core';
import { ContactService } from '@app/service/api/contact.service';
import { PagedListingComponentBase, PagedRequestDto, } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { CreateEditContactComponent } from './create-edit-contact/create-edit-contact.component';

class PagedContractsRequestDto extends PagedRequestDto {
  filterItems: [{
    propertyName: string,
    value: string,
    comparision: 0
  }]
  searchText: string = ''
}

export enum ActionType {
  edit,
  add,
}

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})

export class ContactComponent extends PagedListingComponentBase<any> {
  listContact = []
  listContactFilter = []
  totalCount: number
  searchText: string = ''
  statusAction: ActionType;

  constructor(injector: Injector, private contactService: ContactService, private _modalService: BsModalService) {
    super(injector)
  }

  ngOnInit(): void {
    this.refresh()
  }

  protected list(request: PagedContractsRequestDto, pageNumber: number, finishedCallback: Function): void {
    request.searchText = this.searchText
    this.contactService.getAllPagging(request).pipe(
      finalize(() => {
        finishedCallback();
      })
    )
      .subscribe((res: any) => {
        this.totalCount = res.result.totalCount
        this.listContact = res.result.items.map((contact, index) => {
          return { ...contact, position: 10 * (pageNumber - 1) + index + 1 }
        })
        this.listContactFilter = this.listContact
        this.showPaging(res.result, pageNumber);
      });
  }

  protected delete(contact: any): void {
    abp.message.confirm(
      this.l(this.ecTransform('UserDeleteWarningMessage'), contact.id),
      undefined,
      (result: boolean) => {
        if (result) {
          this.contactService.delete(contact.id).subscribe(() => {
            abp.notify.success(this.l(this.ecTransform('SuccessfullyDeleted')));
            this.refresh();
          });
        }
      }
    );
  }

  getSearchText(event: any) {
    this.searchText = event.target.value
  }

  createContact() {
    this.showCreateOrEditContact();
  }

  updateContact(contact) {
    this.showCreateOrEditContact(contact);
  }

  deleteContract(id: number) {
    abp.message.confirm("",
      `${this.ecTransform("AreYouSureYouWantToDelete")} `, (rs) => {
        if (rs) {
          this.contactService.deleteContact(id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.ecTransform('DeleteCustomerSuccessfully'));
                this.refresh()
              })
            )
            .subscribe(() => { });
        }
      })
  }

  showCreateOrEditContact(contact?: any): void {
    let createOrEditContactDialog: BsModalRef;
    if (!contact) {
      createOrEditContactDialog = this._modalService.show(CreateEditContactComponent, {
        class: 'modal-dialog-centered modal-lg'
      })
    } else {
      let initContact = { ...contact }
      createOrEditContactDialog = this._modalService.show(CreateEditContactComponent, {
        class: 'modal-dialog-centered modal-lg',
        initialState: {
          contact: initContact,
        },
      })
    }

    createOrEditContactDialog.content.onSave?.subscribe(() => {
      this.refresh()
    });
  }

  isShowCreateBtn() {
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Contact_Create)
  }

  isShowEditBtn() {
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Contact_Edit)
  }

  isShowDeleteBtn() {
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Contact_Delete)
  }
}
