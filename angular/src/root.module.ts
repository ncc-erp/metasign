import { NgModule, APP_INITIALIZER, LOCALE_ID } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { TabsModule } from 'ngx-bootstrap/tabs';

import { AbpHttpInterceptor } from 'abp-ng2-module';

import { SharedModule } from '@shared/shared.module';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { RootRoutingModule } from './root-routing.module';
import { AppConsts } from '@shared/AppConsts';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';

import { RootComponent } from './root.component';
import { AppInitializer } from './app-initializer';


import { IPublicClientApplication, PublicClientApplication } from '@azure/msal-browser';
import {  MSAL_INSTANCE, MsalService } from '@azure/msal-angular';
import { AppSessionService } from '@shared/session/app-session.service';


const isIE =
  window.navigator.userAgent.indexOf("MSIE ") > -1 ||
  window.navigator.userAgent.indexOf("Trident/") > -1;

  export function MSALInstanceFactory(configService: AppSessionService): IPublicClientApplication {
    return new PublicClientApplication({
      auth: {
        clientId: configService.microsoftClientId, // Sử dụng clientId từ API
        authority: 'https://login.microsoftonline.com/common', // Thay thế bằng URL authority của bạn
        redirectUri: '/' // Thay thế bằng redirect URI của ứng dụng của bạn
      },
      cache: {
        cacheLocation: 'localStorage',
        storeAuthStateInCookie: isIE, 
      }
    });
  }


export function getCurrentLanguage(): string {
  if (abp.localization.currentLanguage.name) {
    return abp.localization.currentLanguage.name;
  }

  // todo: Waiting for https://github.com/angular/angular/issues/31465 to be fixed.
  return 'en';
}

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    SharedModule.forRoot(),
    ModalModule.forRoot(),
    BsDropdownModule.forRoot(),
    CollapseModule.forRoot(),
    TabsModule.forRoot(),
    ServiceProxyModule,
    RootRoutingModule,
   
  ],
  declarations: [RootComponent],
  providers: [
    AppSessionService,
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory,
      deps: [AppSessionService]
    },
    { provide: HTTP_INTERCEPTORS, useClass: AbpHttpInterceptor, multi: true },
    {
      provide: APP_INITIALIZER,
      useFactory: (appInitializer: AppInitializer) => appInitializer.init(),
      deps: [AppInitializer],
      multi: true,
    },
    { provide: API_BASE_URL, useFactory: () => AppConsts.remoteServiceBaseUrl },
    {
      provide: LOCALE_ID,
      useFactory: getCurrentLanguage,
    },
    MsalService 
  ],
  bootstrap: [RootComponent],
})
export class RootModule { }
