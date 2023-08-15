import { Component, Injector } from '@angular/core';
import { AbpSessionService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { LoginService } from './login.service';
import { CredentialResponse, PromptMomentNotification } from "google-one-tap"

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  animations: [accountModuleAnimation()]
})
export class LoginComponent extends AppComponentBase {
  submitting = false;
  isEnableLoginByUsername: boolean = false;

  constructor(
    injector: Injector,
    private loginService: LoginService,
    public authService: AppAuthService,
    private _sessionService: AbpSessionService
  ) {
    super(injector);
  }
  ngOnInit(): void {
    //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    //Add 'implements OnInit' to the class.
    this.InitgoogleValidate()
    this.appSession.isEnableLoginByUsername === 'false' ? this.isEnableLoginByUsername = false : this.isEnableLoginByUsername = true
    console.log("test release")
  }


  get multiTenancySideIsTeanant(): boolean {
    return this._sessionService.tenantId > 0;
  }

  get isSelfRegistrationAllowed(): boolean {
    if (!this._sessionService.tenantId) {
      return false;
    }

    return true;
  }

  login(): void {
    this.submitting = true;
    this.authService.authenticate(() => (this.submitting = false));
  }

  signInWithGoogle() {
    // this.googleAuthService.signIn(GoogleLoginProvider.PROVIDER_ID).then((rs: any) =>{
    //   this.loginService.authenticateGoogle(rs.idToken)
    // })
  }

  InitgoogleValidate() {
    // @ts-ignore
    google.accounts.id.initialize({
      client_id: this.appSession.googleClientId,
      callback: this.handleCredentialResponse.bind(this),
      auto_select: true,
      cancel_on_tap_outside: false,
    });
    // @ts-ignore
    google.accounts.id.renderButton(
      // @ts-ignore
      document.getElementById("google-login-button"),
      { theme: "outline", size: "large", width: 240 }
    );
    // @ts-ignore
    google.accounts.id.prompt((notification: PromptMomentNotification) => { });
  }

  handleCredentialResponse(response: CredentialResponse) {
    localStorage.setItem("JWT", response.credential)
    this.loginService.authenticateGoogle(response.credential)
  }
}
