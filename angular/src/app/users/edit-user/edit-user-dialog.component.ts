import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output
} from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { forEach as _forEach, includes as _includes, map as _map } from 'lodash-es';
import { AppComponentBase } from '@shared/app-component-base';
import {
  UserServiceProxy,
  UserDto,
  RoleDto
} from '@shared/service-proxies/service-proxies';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

@Component({
  templateUrl: './edit-user-dialog.component.html'
})
export class EditUserDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;
  user = new UserDto();
  roles: RoleDto[] = [];
  checkedRolesMap: { [key: string]: boolean } = {};
  id: number;

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public _userService: UserServiceProxy,
    public bsModalRef: BsModalRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._userService.get(this.id).subscribe((result) => {
      this.user = result;

      this._userService.getRoles().subscribe((result2) => {
        this.roles = result2.items;
        this.setInitialRolesStatus();
      });
    });
  }

  setInitialRolesStatus(): void {
    _map(this.roles, (item) => {
      this.checkedRolesMap[item.normalizedName] = this.isRoleChecked(
        item.normalizedName
      );
    });
  }

  isRoleChecked(normalizedName: string): boolean {
    return _includes(this.user.roleNames, normalizedName);
  }

  onRoleChange(role: RoleDto, $event) {
    this.checkedRolesMap[role.normalizedName] = $event.target.checked;
  }

  getCheckedRoles(): string[] {
    const roles: string[] = [];
    _forEach(this.checkedRolesMap, function (value, key) {
      if (value) {
        roles.push(key);
      }
    });
    return roles;
  }

  save(): void {
    this.saving = true;

    this.user.roleNames = this.getCheckedRoles();

    this._userService.update(this.user).subscribe(
      () => {
        this.notify.info(this.l(this.ecTransform('SavedSuccessfully')));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      () => {
        this.saving = false;
      }
    );
  }
  isGrantEditUserRole(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Admin_User_EditUserRole);
  }
}
