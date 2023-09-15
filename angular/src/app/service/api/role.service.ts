import { HttpClient } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class RoleService extends BaseApiService{

  changeUrl() {
    return 'Role';
  }

  constructor(injector: Injector) {
    super(injector);
  }
  public getRole(): Observable<any> {
    return this.processGet('GetRoles');
  }

  public getAllUsersInRole(roleId: number):Observable<any>{
    return this.processGet("GetAllUsersInRole?roleId=" + roleId);
  }
  public GetUsersInRole(input):Observable<any>{
    return this.processPost("GetUsersInRole", input);
  }
  public RemoveUserFromRole(id: number):Observable<any>{
    return this.processDelete("RemoveUserFromRole?Id=" + id);
  }
  public GetAllUsersNotInRole(roleId: number):Observable<any>{
    return this.processGet("GetAllUsersNotInRole?roleId=" + roleId);
  }
  public AddUserIntoRole(input):Observable<any>{
    return this.processPost("AddUserIntoRole" , input);
  }
}
