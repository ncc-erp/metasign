import { ActivatedRoute, Router } from '@angular/router';
import { Component, ElementRef, Injector } from '@angular/core';
import { AbpSessionService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { LoginService } from './login.service';
import { CredentialResponse, PromptMomentNotification } from "google-one-tap"
import { MsalService } from '@azure/msal-angular';
import { BsModalService } from 'ngx-bootstrap/modal';
import { GoogleLoginService } from './google-login.service';
import { buttomType, loginApp } from '@shared/AppEnums';
import { TenantChangeDialogComponent } from 'account/tenant/tenant-change-dialog/tenant-change-dialog.component';
import { Oauth2Mezon } from './../../shared/AppConsts';
import { AppConsts } from '@shared/AppConsts';
@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  animations: [accountModuleAnimation()],
  host: {
    "(window:resize)":"onWindowResize($event)"
  }
})
export class LoginComponent extends AppComponentBase {
  submitting = false;
  isEnableNormalLogin: boolean = AppConsts.enableNormalLogin;
  isEnableLoginByGoogle: boolean = AppConsts.enableLoginGoogle;
  isEnableLoginByMicrosoft: boolean = AppConsts.enableLoginMicrosoft;
  isEnableLoginByMezon: boolean = AppConsts.enableLoginMezon;
  private buttonLoginGoogle;
  public tenancyName: string;
  istenancyName;
  public buttomType = buttomType
  public width: number
  constructor(
    injector: Injector,
    private loginService: LoginService,
    public authService: AppAuthService,
    private _sessionService: AbpSessionService,
    private msalService: MsalService,
    private _modalService: BsModalService,
    private el:ElementRef,private route : ActivatedRoute
  ) {
    super(injector);
  }
  ngOnInit(): void {
    //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    //Add 'implements OnInit' to the class.
    if (this.appSession.tenant) {
      this.tenancyName = this.appSession.tenant.tenancyName;
      this.istenancyName = new Boolean(this.tenancyName);
    }
    this.width = this.el.nativeElement.offsetWidth
    this.InitgoogleValidate()
 

    this.route.queryParams.subscribe(params => {
      const authorizationCode = params['code'];
      if(authorizationCode != null ){
        this.loginService.authenticateMezon(authorizationCode);
      }
    })
  }
  handlelogout()
  {    
    this.msalService.logoutPopup();
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


  handleLoginGoogle()
  {
    this.buttonLoginGoogle.click()
  }

  onWindowResize(event) {
    this.width = event.target.innerWidth;
  }

  handleOpenTenant()
  { 
    const modal = this._modalService.show(TenantChangeDialogComponent);
    if (this.appSession.tenant) {
      modal.content.tenancyName = this.appSession.tenant.tenancyName;
    }
  }

  handleLoginMicrosoft()
  {    
    const loginRequest = {
      scopes: ['openid', 'profile'],
      prompt: 'select_account', // Tắt chế độ tự động đăng nhập
    };
    this.msalService.loginPopup(loginRequest).subscribe(value =>{
      localStorage.setItem("JWT", value.idToken)
      this.loginService.authenticateGoogle(value.idToken,loginApp.microsoft)
    })
  }

  InitgoogleValidate() {

    const googleLoginWrapper = document.createElement("div");
    googleLoginWrapper.style.display = "none";
    googleLoginWrapper.classList.add("custom-google-button");
    document.body.appendChild(googleLoginWrapper);  
   
    // @ts-ignore
    google.accounts.id.initialize({
      client_id: this.appSession.googleClientId,
      callback: this.handleCredentialResponse.bind(this),
      auto_select: false,
      cancel_on_tap_outside: false,
    });
    // @ts-ignore
    google.accounts.id.renderButton(
      // @ts-ignore
      googleLoginWrapper,
      { theme: "outline", size: "large", width: 240 }
    );
    this.buttonLoginGoogle = googleLoginWrapper.querySelector("div[role=button]");
    // @ts-ignore
    google.accounts.id.prompt((notification: PromptMomentNotification) => { });
  }

  handleCredentialResponse(response: CredentialResponse) {
    localStorage.setItem("JWT", response.credential)
    this.loginService.authenticateGoogle(response.credential,loginApp.google)
  }

  signInWithMezon() {
    const OAUTH2_AUTHORIZE_URL = Oauth2Mezon.OAUTH2_AUTHORIZE_URL;
    const CLIENT_ID = AppConsts.mezonClientId;
    const REDIRECT_URI = AppConsts.appBaseUrl+"/account/login";
     const RESPONSE_TYPE = 'code';
     const SCOPE = 'openid+offline';
     const STATE = 'hkjadkjashdkjsah'; 

    const authUrl = `${OAUTH2_AUTHORIZE_URL}?client_id=${CLIENT_ID}&redirect_uri=${REDIRECT_URI}&response_type=${RESPONSE_TYPE}&scope=${SCOPE}&state=${STATE}`;
		return (window.location.href = authUrl);
  }
}
