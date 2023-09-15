
import { Component, OnInit, NgZone, Injector} from "@angular/core";
import { Router } from "@angular/router";
import { AppSessionService } from "@shared/session/app-session.service";
import { CredentialResponse } from "google-one-tap";
import { MsalService } from "@azure/msal-angular";
import { buttomType } from "@shared/AppEnums";
import { AppComponentBase } from "@shared/app-component-base";

@Component({
  selector: "app-email-login-validator",
  templateUrl: "./email-login-validator.component.html",
  styleUrls: ["./email-login-validator.component.css"],
})
export class EmailLoginValidatorComponent extends AppComponentBase implements OnInit {
  constructor(
    private injector: Injector,
    private ngZone: NgZone,
    private router: Router,
    private appSessionService: AppSessionService,
    private msalService: MsalService
  ) {
    super(injector)
  }
  buttomType = buttomType;
  buttonLoginGoogle;
  ngOnInit() {
    this.ngZone.run(() => {
      this.InitgoogleValidate();
    });
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
      {
        theme: "outline",
        size: "large",
        width: document.getElementById("google_button--parent")?.offsetWidth,
      }
    );
    // @ts-ignore
    google.accounts.id.prompt((notification: PromptMomentNotification) => {});
    this.buttonLoginGoogle =
      googleLoginWrapper.querySelector("div[role=button]");
  }

  handleLoginGoogle() {
    this.buttonLoginGoogle.click();
  }

  handleLoginMicrosoft() {
    const loginRequest = {
      scopes: ["openid", "profile"],
      prompt: "select_account", // Tắt chế độ tự động đăng nhập
      
    };
    this.msalService.loginPopup(loginRequest).subscribe((value) => {

      if (value) {    
        localStorage.setItem("email", value.account.username);
            this.router.navigate(["app/email-login/contract-lookup"], {
              queryParams: {
                email: value.account.username,
              },
              queryParamsHandling:'merge'
            });
      }
    });
  }

  handleCredentialResponse(response: CredentialResponse) {
    const idToken = response.credential;
    const payload = JSON.parse(atob(idToken.split(".")[1]));
    const name = payload.name;
    const email = payload.email;
    localStorage.setItem("email", email);
    this.ngZone.run(() => {
      this.router.navigate(["app/email-login/contract-lookup"], {
        queryParams: {
          email: email,
        },
      });
    });
  }
}
