import { Component } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { loginApp } from '@shared/AppEnums';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { SessionServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'header-user-menu',
  templateUrl: './header-user-menu.component.html',
})
export class HeaderUserMenuComponent {
  constructor(private _authService: AppAuthService, private _sessionService: SessionServiceProxy,private msalService: MsalService) { }
  user

  ngOnInit() {
    this._sessionService.getCurrentLoginInformations().subscribe(res => {
      this.user = res.user;
      localStorage.setItem('user', JSON.stringify(this.user))
    })
  }

  logout(): void {

    this._authService.logout();
    localStorage.clear();
   
  }


}
