import {
  Component,
  OnInit,
  Injector
} from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/app-component-base';
import {
  ChangeUserLanguageDto
} from '@shared/service-proxies/service-proxies';
import { filter as _filter } from 'lodash-es';

@Component({
  selector: 'header-language-menu',
  templateUrl: './header-language-menu.component.html',
})
export class HeaderLanguageMenuComponent extends AppComponentBase
  implements OnInit {
  languages: abp.localization.ILanguageInfo[];
  currentLanguageName: string;
  currentLanguageDisplayName: string;
  currentLanguageIcon: string;
  isMobile: boolean = window.innerWidth >= AppConsts.screenWidthMobile.min && window.innerWidth <= AppConsts.screenWidthMobile.max

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    this.languages = _filter(
      this.localization.languages,
      (l) => !l.isDisabled
    );
    this.currentLanguageName = abp.utils.getCookieValue('Abp.Localization.CultureName');
    const foundLanguage = this.localization.languages.find(item => item.name === this.currentLanguageName);
    this.currentLanguageIcon = foundLanguage.icon;
    this.currentLanguageDisplayName = foundLanguage.displayName;

  }

  changeLanguage(languageName: string): void {
    const input = new ChangeUserLanguageDto();
    input.languageName = languageName;

    abp.utils.setCookieValue(
      'Abp.Localization.CultureName',
      languageName,
      new Date(new Date().getTime() + 5 * 365 * 86400000),
      abp.appPath
    );

    window.location.reload();
  }
}
