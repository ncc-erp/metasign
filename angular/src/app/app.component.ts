import { Component, Injector, OnInit, Renderer2 } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
// import { SignalRAspNetCoreHelper } from '@shared/helpers/SignalRAspNetCoreHelper';
import { LayoutStoreService } from '@shared/layout/layout-store.service';
import { DomSanitizer } from '@angular/platform-browser';
import { MatIconRegistry } from '@angular/material/icon';
@Component({
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']

})
export class AppComponent extends AppComponentBase implements OnInit {
  sidebarExpanded: boolean;

  constructor(
    injector: Injector,
    private renderer: Renderer2,
    private _layoutStore: LayoutStoreService,
    private domSanitizer:DomSanitizer,
    private matIconRegistry:MatIconRegistry,
  ) {
    super(injector);
    this.matIconRegistry.addSvgIcon('digital',this.domSanitizer.bypassSecurityTrustResourceUrl('../assets/img/signature/Digital.svg'));
    this.matIconRegistry.addSvgIcon('electronic',this.domSanitizer.bypassSecurityTrustResourceUrl('../assets/img/signature/Electronic.svg'));
    this.matIconRegistry.addSvgIcon('datePicker',this.domSanitizer.bypassSecurityTrustResourceUrl('../assets/img/signature/DatePicker.svg'));
    this.matIconRegistry.addSvgIcon('text',this.domSanitizer.bypassSecurityTrustResourceUrl('../assets/img/signature/Text.svg'));
    this.matIconRegistry.addSvgIcon('stamp',this.domSanitizer.bypassSecurityTrustResourceUrl('../assets/img/signature/stamp.svg'));
  }

  ngOnInit(): void {
    this.renderer.addClass(document.body, 'sidebar-mini');

    // SignalRAspNetCoreHelper.initSignalR();

    abp.event.on('abp.notifications.received', (userNotification) => {
      abp.notifications.showUiNotifyForUserNotification(userNotification);

      // Desktop notification
      Push.create('AbpZeroTemplate', {
        body: userNotification.notification.data.message,
        icon: abp.appPath + 'assets/app-logo-small.png',
        timeout: 6000,
        onClick: function () {
          window.focus();
          this.close();
        }
      });
    });

    this._layoutStore.sidebarExpanded.subscribe((value) => {
      this.sidebarExpanded = value;
    });
  }

  toggleSidebar(): void {
    this._layoutStore.setSidebarExpanded(!this.sidebarExpanded);
  }
}
