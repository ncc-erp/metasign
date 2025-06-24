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
  ) {
    super(injector);
    this.statusContract = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("status"))
    );
    const data: any = this.getParamsFromUrl(
      decodeURIComponent(this.router.url)
    );
    this.contracId = +data.contractId;
    this.contractSettingId = +data.settingId;
    this.tenantName = data.tenantName == "" ? undefined : data.tenantName;
  }

  ngOnInit() {
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
    google.accounts.id.prompt((notification: PromptMomentNotification) => { });
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
      if (rs.result) {
        this.ngZone.run(() => {

          this.router.navigate(["app/signging/unAuthen-signing"], {
            queryParams: {
              contractId: encode,
              settingId: encode,
              tenantName: this.tenantName
            },
          });
        });
      } else {
        this.ngZone.run(() => {
          this.messages = this.EcTranslatePipe.transform("EmailDoesNotHavePermissionToViewTheDocumentPleaseCheckAgain");
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
      const REDIRECT_URI = "http://localhost:4200/account/login";
       const RESPONSE_TYPE = 'code';
       const SCOPE = 'openid+offline';
       const STATE = 'hkjadkjashdkjsah'; 
  
      const authUrl = `${OAUTH2_AUTHORIZE_URL}?client_id=${CLIENT_ID}&redirect_uri=${REDIRECT_URI}&response_type=${RESPONSE_TYPE}&scope=${SCOPE}&state=${STATE}`;
      return (window.location.href = authUrl);
    }
}
