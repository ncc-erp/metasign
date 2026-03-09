import { buttomType } from './../../../../shared/AppEnums';
import { ActivatedRoute, Router } from "@angular/router";
import { Component, Injector, NgZone, OnInit } from "@angular/core";
import { ContractSigningService } from "@app/service/api/contract-signing.service";
import { CredentialResponse, PromptMomentNotification } from "google-one-tap";
import { AppSessionService } from "@shared/session/app-session.service";
import { EcTranslatePipe } from "@shared/pipes/ecTranslate.pipe";
import { AppTenantAvailabilityState, loginApp } from "@shared/AppEnums";
import { AccountServiceProxy, IsTenantAvailableInput, IsTenantAvailableOutput, SessionServiceProxy } from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/app-component-base";
import { concatMap } from "rxjs/operators";
import { MsalService } from "@azure/msal-angular";
import { Oauth2Mezon } from "@shared/AppConsts";
import { AppConsts } from "@shared/AppConsts";
import { GoogleLoginService } from "account/login/google-login.service";

@Component({
  selector: "app-email-valid",
  templateUrl: "./email-valid.component.html",
  styleUrls: ["./email-valid.component.css"],
})
export class EmailValidComponent extends AppComponentBase implements OnInit {
  private contractSettingId: number;
  private contracId: number;
  public messages: string = "";
  private tenantName: string;
  statusContract: boolean;
  public isValid: boolean = true;
  private buttonLoginGoogle;
  public isdisplayLogin = true;
  buttomType = buttomType
  constructor(
    injector: Injector,
    private ngZone: NgZone,
    private contractSigningService: ContractSigningService,
    private route: ActivatedRoute,
    private router: Router,
    private appSessionService: AppSessionService,
    private EcTranslatePipe: EcTranslatePipe,
    private _accountService: AccountServiceProxy,
    private _sessionService: SessionServiceProxy,
    private msalService: MsalService,
    private googleLoginService: GoogleLoginService,
  ) {
    super(injector);
    
    console.log('Constructor - current URL:', this.router.url);
    console.log('Constructor - query params:', this.route.snapshot.queryParams);
    
    // Only parse params if not coming from Mezon callback
    const hasCode = this.route.snapshot.queryParams['code'];
    if (!hasCode) {
      const statusParam = this.route.snapshot.queryParamMap.get("status");
      if (statusParam) {
        try {
          this.statusContract = JSON.parse(decodeURIComponent(statusParam));
        } catch (e) {
          console.error('Error parsing status:', e);
        }
      }
      
      const data: any = this.getParamsFromUrl(
        decodeURIComponent(this.router.url)
      );
      this.contracId = +data.contractId;
      this.contractSettingId = +data.settingId;
      this.tenantName = data.tenantName == "" ? undefined : data.tenantName;
      
      console.log('Constructor - parsed params:', { 
        contracId: this.contracId, 
        contractSettingId: this.contractSettingId, 
        tenantName: this.tenantName 
      });
    } else {
      console.log('Constructor - Mezon callback detected, skipping param parsing');
    }
  }

  ngOnInit() {
    console.log('ngOnInit called');
    
    // Check for Mezon authorization code
    this.route.queryParams.subscribe(params => {
      console.log('queryParams subscribe triggered, params:', params);
      const authorizationCode = params['code'];
      console.log('authorizationCode:', authorizationCode);
      
      if (authorizationCode != null) {
        console.log('Mezon code detected:', authorizationCode);
        
        // Try to restore params from state if available
        const state = params['state'];
        if (state) {
          try {
            const decodedState = JSON.parse(atob(state));
            console.log('Decoded state:', decodedState);
            
            // Restore params
            if (decodedState.contractId) this.contracId = decodedState.contractId;
            if (decodedState.settingId) this.contractSettingId = decodedState.settingId;
            if (decodedState.tenantName) this.tenantName = decodedState.tenantName;
            if (decodedState.status !== undefined) this.statusContract = decodedState.status;
          } catch (e) {
            console.error('Error parsing state:', e);
          }
        }
        
        // Build clean redirect URI without code/state params
        const baseUrl = window.location.origin + window.location.pathname;
        const originalParams = new URLSearchParams();
        if (this.contracId) originalParams.append('contractId', String(this.contracId));
        if (this.contractSettingId) originalParams.append('settingId', String(this.contractSettingId));
        if (this.tenantName) originalParams.append('tenantName', this.tenantName);
        if (this.statusContract !== undefined) originalParams.append('status', String(this.statusContract));
        
        const redirectUri = AppConsts.appBaseUrl+"/app/signging/email-valid";
        console.log('Calling signingMezonAuthenticate with redirectUri:', redirectUri);
        
        // Get email from Mezon and call validEmail
        this.googleLoginService.signingMezonAuthenticate(authorizationCode, redirectUri, this.contracId)
          .subscribe((result: any) => {
            console.log('signingMezonAuthenticate result:', result);
            if (result && result.result) {
              // Store authentication token if available (similar to authenticateMezon)
              if (result.result.accessToken) {
                const tokenExpireDate = new Date(new Date().getTime() + 1000 * result.result.expireInSeconds);
                abp.auth.setToken(result.result.accessToken, tokenExpireDate);
                abp.utils.setCookieValue(
                  'Abp.AuthToken',
                  result.result.encryptedAccessToken,
                  tokenExpireDate,
                  abp.appPath
                );
                console.log('Token stored successfully');
              }
              
              // Store email and login type
              const email = result.result.email;
              if (email) {
                if (localStorage.getItem("typeLoginSigning")) {
                  localStorage.removeItem('typeLoginSigning');
                }
                localStorage.setItem("typeLoginSigning", String(loginApp.mezon));
                
                // Create a simple JWT-like token with email for compatibility
                const mezonTokenData = {
                  email: email,
                  iss: 'mezon',
                  exp: Math.floor(Date.now() / 1000) + (result.result.expireInSeconds || 3600)
                };
                const mezonToken = btoa(JSON.stringify({ alg: "none" })) + '.' + 
                                   btoa(JSON.stringify(mezonTokenData)) + '.';
                localStorage.setItem("JWT", mezonToken);
                console.log('JWT and email stored, calling validEmail');
                
                this.validEmail(email);
              } else {
                console.error('No email in result');
              }
            } else {
              console.error('Invalid result structure');
            }
          });
        return;
      }
    });

    let jwt = localStorage.getItem("JWT");
    if (!jwt || jwt == "") {
    }
    else {
      let json = this.parseJwt(jwt);
      if (localStorage.getItem("typeLoginSigning")) {
        localStorage.removeItem('typeLoginSigning');
      }
      json?.email ? localStorage.setItem("typeLoginSigning", String(loginApp.google)) : localStorage.setItem("typeLoginSigning", String(loginApp.microsoft));
      const email = json?.email ? json.email : json.preferred_username;
      this.contractSigningService.getSignerEmail(this.contractSettingId).subscribe(rs => {
        if (rs.result == email) {
          let isSignNow = localStorage.getItem("notSignNow")
          if (isSignNow == "0") {
            this.validEmail(email)
          }
        }
      })
    }
    if (this.statusContract) {
      this.contractSettingId = JSON.parse(
        decodeURIComponent(this.route.snapshot.queryParamMap.get("settingId"))
      )?.settingId;

      this.contracId = JSON.parse(
        decodeURIComponent(this.route.snapshot.queryParamMap.get("contractId"))
      )?.contractId;
    }
    if (this.tenantName) {
      const input = new IsTenantAvailableInput();
      input.tenancyName = this.tenantName;
      this._accountService.isTenantAvailable(input).pipe(
        concatMap(
          (result: IsTenantAvailableOutput) => {
            switch (result.state) {
              case AppTenantAvailabilityState.Available:
                abp.multiTenancy.setTenantIdCookie(result.tenantId);
                break;
              case AppTenantAvailabilityState.InActive:
                this.message.warn(this.l('TenantIsNotActive', this.tenantName));
                break;
              case AppTenantAvailabilityState.NotFound:
                this.message.warn(
                  this.l('ThereIsNoTenantDefinedWithName{0}', this.tenantName)
                );
                break;
            }
            return this._sessionService.getCurrentLoginInformations();
          }
        ),
        concatMap((rs) => {
          return this.contractSigningService
            .ValidContract(this.contracId)
        })
      ).subscribe(
        (rs) => {
          this.ngZone.run(() => {
            this.isValid = rs.result.isValid;
            if (!rs.result.isValid) {
              this.messages = rs.result.message;
              this.isdisplayLogin = false;
            } else {
              this.InitgoogleValidate();
            }
          });
        });
      return;
    }
    else {
      abp.multiTenancy.setTenantIdCookie(undefined);
      this.validContract();
      return;
    }
  }


  getParamsFromUrl(url) {
    const params = {};
    const queryString = url.split("?")[1];

    if (queryString) {
      queryString.split("&").forEach((param) => {
        const pair = param.split("=");
        const key = decodeURIComponent(pair[0]);
        const value = decodeURIComponent(pair[1]);
        params[key] = value;
      });
    }

    return params;
  }

  InitgoogleValidate() {
    // Don't show Google popup if we're processing Mezon callback
    const hasMezonCode = this.route.snapshot.queryParams['code'];
    if (hasMezonCode) {
      console.log('Skipping InitgoogleValidate - Mezon callback detected');
      return;
    }

    const googleLoginWrapper = document.createElement("div");
    googleLoginWrapper.style.display = "none";
    googleLoginWrapper.classList.add("custom-google-button");
    document.body.appendChild(googleLoginWrapper);
    // @ts-ignore
    google.accounts.id.initialize({
      client_id: this.appSessionService.googleClientId,
      callback: this.handleCredentialResponse.bind(this),
      auto_select: true,
      cancel_on_tap_outside: false,
    });
    // @ts-ignore
    google.accounts.id.renderButton(
      // @ts-ignore
      googleLoginWrapper,
      { theme: "outline", size: "large", width: document.getElementById("google_button--parent")?.offsetWidth }
    );
    // @ts-ignore
    // Comment out auto prompt to prevent popup from showing automatically
    // google.accounts.id.prompt((notification: PromptMomentNotification) => { });
    this.buttonLoginGoogle = googleLoginWrapper.querySelector("div[role=button]");
  }

  handleCredentialResponse(response: CredentialResponse) {
    if (localStorage.getItem("typeLoginSigning")) {
      localStorage.removeItem('typeLoginSigning');
    }
    localStorage.setItem("typeLoginSigning", String(loginApp.google))
    localStorage.setItem("JWT", response.credential);
    let result = this.parseJwt(response.credential);
    if (result.email_verified) {
      this.validEmail(result.email);
    }
  }

  validContract() {
    this.contractSigningService
      .ValidContract(this.contracId)
      .subscribe((rs) => {
        this.ngZone.run(() => {
          this.isValid = rs.result.isValid;
          if (!rs.result.isValid) {
            this.messages = rs.result.message;
            this.isdisplayLogin = false;
          } else {
            this.InitgoogleValidate();
          }
        });
      });
  }

  validEmail(email: string) {
    let dto = {
      email: email,
      contractSettingId: this.contractSettingId,
    };

    const contract = {
      contractId: this.contracId,
      settingId: this.contractSettingId,
    };

    const encode = encodeURIComponent(JSON.stringify(contract));
    this.contractSigningService.ValidEmail(dto).subscribe((rs) => {
      console.log('ValidEmail result:', rs);
      if (rs.result) {
        console.log('ValidEmail success, navigating to unAuthen-signing with params:', {
          contractId: encode,
          settingId: encode,
          tenantName: this.tenantName
        });
        this.ngZone.run(() => {
          this.router.navigate(["/app/signging/unAuthen-signing"], {
            queryParams: {
              contractId: encode,
              settingId: encode,
              tenantName: this.tenantName
            },
          }).then(
            (success) => {
              console.log('Navigation successful:', success);
            },
            (error) => {
              console.error('Navigation error:', error);
            }
          );
        });
      } else {
        this.ngZone.run(() => {
          this.messages = this.EcTranslatePipe.transform("EmailDoesNotHavePermissionToViewTheDocumentPleaseCheckAgain");
          abp.message.error(this.messages);
        });
      }
    });
  }

  handleLoginGoogle() {
    this.buttonLoginGoogle.click();
  }

  handleLoginMicrosoft() {
    const loginRequest = {
      scopes: ['openid', 'profile'],
      prompt: 'select_account', // Tắt chế độ tự động đăng nhập
    };
    this.msalService.loginPopup(loginRequest).subscribe(value => {

      if (localStorage.getItem("typeLoginSigning")) {
        localStorage.removeItem('typeLoginSigning');
      }

      localStorage.setItem("typeLoginSigning", String(loginApp.microsoft))
      localStorage.setItem("JWT", value.idToken);
      let result = this.parseJwt(value.idToken);
      if (value) {
        this.validEmail(result.preferred_username);
      }

    })
  }

  parseJwt(token) {
    var base64Url = token.split(".")[1];
    var base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    var jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map(function (c) {
          return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
        })
        .join("")
    );

    return JSON.parse(jsonPayload);
  }

signInWithMezon() {
    const OAUTH2_AUTHORIZE_URL = Oauth2Mezon.OAUTH2_AUTHORIZE_URL;
    const CLIENT_ID = AppConsts.mezonClientId;
    
    // Build base redirect URI (without query params)
    const baseUrl = window.location.origin + window.location.pathname;
    const REDIRECT_URI = baseUrl;
    
    // Save current params in state parameter
    const currentParams = {
      contractId: this.contracId,
      settingId: this.contractSettingId,
      tenantName: this.tenantName,
      status: this.statusContract
    };
    
    // Encode state as base64 JSON
    const STATE = btoa(JSON.stringify(currentParams));
    
    const RESPONSE_TYPE = 'code';
    const SCOPE = 'openid+offline';

    const authUrl = `${OAUTH2_AUTHORIZE_URL}?client_id=${CLIENT_ID}&redirect_uri=${encodeURIComponent(REDIRECT_URI)}&response_type=${RESPONSE_TYPE}&scope=${SCOPE}&state=${encodeURIComponent(STATE)}`;
    console.log('Redirecting to Mezon with state:', currentParams);
    return (window.location.href = authUrl);
  }
}
