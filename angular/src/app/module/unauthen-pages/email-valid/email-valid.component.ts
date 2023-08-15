import { ActivatedRoute, Router } from "@angular/router";
import { Component, Injector, NgZone, OnInit } from "@angular/core";
import { ContractSigningService } from "@app/service/api/contract-signing.service";
import { CredentialResponse, PromptMomentNotification } from "google-one-tap";
import { AppSessionService } from "@shared/session/app-session.service";
import { EcTranslatePipe } from "@shared/pipes/ecTranslate.pipe";
import { AppTenantAvailabilityState } from "@shared/AppEnums";
import { AccountServiceProxy, IsTenantAvailableInput, IsTenantAvailableOutput, SessionServiceProxy } from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "@shared/app-component-base";
import { concatMap } from "rxjs/operators";

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
  constructor(
    injector: Injector,
    private ngZone: NgZone,
    private contractSigningService: ContractSigningService,
    private route: ActivatedRoute,
    private router: Router,
    private appSessionService: AppSessionService,
    private EcTranslatePipe: EcTranslatePipe,
    private _accountService: AccountServiceProxy,
    private _sessionService: SessionServiceProxy
  ) {
    super(injector);
    this.statusContract = JSON.parse(
      decodeURIComponent(this.route.snapshot.queryParamMap.get("status"))
    );
  }

  ngOnInit() {
    const data: any = this.getParamsFromUrl(
      decodeURIComponent(this.router.url)
    );

    this.contracId = +data.contractId;
    this.contractSettingId = +data.settingId;
    this.tenantName = data.tenantName == "" ? undefined : data.tenantName;
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
      document.getElementById("google-button"),
      { theme: "outline", size: "large",  width: document.getElementById("google_button--parent")?.offsetWidth }
    );
    // @ts-ignore
    google.accounts.id.prompt((notification: PromptMomentNotification) => { });
  }

  handleCredentialResponse(response: CredentialResponse) {
    localStorage.setItem("JWT", response.credential);
    let result = this.parseJwt(response.credential);
    if (result.email_verified) {
      this.validEamail(result.email);
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
          } else {
            this.InitgoogleValidate();
          }
        });
      });
  }

  validEamail(email: string) {
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
          let url = `contractId=${this.contracId}&settingId=${this.contractSettingId}&tenantName=${this.tenantName}`;

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
}
