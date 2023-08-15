import { DesignContractComponent } from './home/progress-contract/design-contract/design-contract.component';
import { SettingContractComponent } from './home/progress-contract/setting-contract/setting-contract.component';
import { ProgressContractComponent } from './home/progress-contract/progress-contract.component';
import { Routes, RouterModule } from "@angular/router";
import { UploadContractComponent } from './home/progress-contract/upload-contract/upload-contract.component';
import { ContractEmailSettingComponent } from './home/progress-contract/contract-email-setting/contract-email-setting.component';


const routes: Routes = [
  {
    path: "process",
    component: ProgressContractComponent,
    children: [
      {
        path: 'upload-file',
        component: UploadContractComponent,
      },
      {
        path: 'setting',
        component: SettingContractComponent,
      },
      {
        path: 'signatureSetting',
        component: DesignContractComponent,
      },
      {
        path: 'emailSetting',
        component: ContractEmailSettingComponent,
      },

    ],

  },
];

export const HomeRoutingModule = RouterModule.forChild(routes);
