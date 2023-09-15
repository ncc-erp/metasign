import { loginApp } from '@shared/AppEnums';
import { AppAuthService } from './../../shared/auth/app-auth.service';
import { GoogleLoginService } from './google-login.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private _googleLoginService: GoogleLoginService, private authService:AppAuthService) { }
  authenticateGoogle(googleToken: string,typeLogin: loginApp, finallyCallback?: () => void): void {
    finallyCallback = finallyCallback || (() => { });

  
   let payload  = {
      googleToken:googleToken,
      type:typeLogin,
      secretCode: '',
    }
    this._googleLoginService.googleAuthenticate(payload)
        .subscribe((result) => {
          this.authService.processAuthenticateResult(result.result)
        },

        error =>{ abp.message.error(error.error.error.message); console.log(error); });
}


}
