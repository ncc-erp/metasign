import { ContractSigningService } from "@app/service/api/contract-signing.service";
import { Component, OnInit, NgZone, ChangeDetectorRef } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import {
  AccountServiceProxy,
  SessionServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { AppSessionService } from "@shared/session/app-session.service";
import { CredentialResponse } from "google-one-tap";

@Component({
  selector: "app-email-login-validator",
  templateUrl: "./email-login-validator.component.html",
  styleUrls: ["./email-login-validator.component.css"],
})
export class EmailLoginValidatorComponent implements OnInit {
  constructor(
    private contractSigningService: ContractSigningService,
    private ngZone: NgZone,
    private route: ActivatedRoute,
    private _accountService: AccountServiceProxy,
    private _sessionService: SessionServiceProxy,
    private router: Router,
    private appSessionService: AppSessionService,
    private ref: ChangeDetectorRef
  ) {}

  ngOnInit() {


    this.ngZone.run(() => {
      this.InitgoogleValidate();
    });
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
      {
        theme: "outline",
        size: "large",
        width: document.getElementById("google_button--parent")?.offsetWidth,
      }
    );
    // @ts-ignore
    google.accounts.id.prompt((notification: PromptMomentNotification) => {});
  }

  handleCredentialResponse(response: CredentialResponse) {
    const idToken = response.credential;
    const payload = JSON.parse(atob(idToken.split(".")[1]));
    const name = payload.name;
    const email = payload.email;
    localStorage.setItem('email',email)
    this.ngZone.run(() => {
      this.router.navigate(["app/email-login/contract-lookup"], {
        queryParams: {
          email: email,
        },
      });
    });
  }
}
