import { ApiKeyService } from './../../../service/api/api-key.service';
import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { EFormConfiguration, EFormControlNameFormEmailSetting, EFormControlNameFormGoogleClientIdSetting, EFormControlNameFormMicrosoftClientIdSetting, EFormControlNameFormSignServerClientIdSetting, EFormControlNameS3, ELabelFormEmailSetting, ELabelFormGoogleClientIdSetting, ELabelFormMicrosoftClientIdSetting, ELabelFormSignServerClientIdSetting, ELabelS3 } from '@shared/AppEnums';
import { ConfigurationService } from '../../../service/api/configuration.service'
import { AppComponentBase } from 'shared/app-component-base';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

@Component({
  selector: 'app-configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.css']
})
export class ConfigurationComponent extends AppComponentBase implements OnInit {
  emailSetting = []
  isDisabledFormEmail: boolean = true
  isDisabledFormGoogleId: boolean = true
  isDisabledFormS3: boolean = true
  isCheckedEnableSsl: boolean = false
  isCheckedUseDefault: boolean = false
  googleClientId: string = ''
  SignServerClientId
  isEnableLoginByUsername: boolean = false
  isDisabledRemiderExpriedTime: boolean = true
  isDisabledFormSignServer: boolean = true
  remiderExpriedTime: number
  tempRemiderExpriedTime: number
  isDisabledFormMicrosoftId: boolean = true
  microsoftClientId: string = ''
  apiKey: string = ""
  formAwsS3

  dataForm = [{
    label: ELabelFormEmailSetting.Host,
    formControlName: EFormControlNameFormEmailSetting.Host
  },
  {
    label: ELabelFormEmailSetting.Port,
    formControlName: EFormControlNameFormEmailSetting.Port
  },
  {
    label: ELabelFormEmailSetting.DisplayName,
    formControlName: EFormControlNameFormEmailSetting.DisplayName
  },
  {
    label: ELabelFormEmailSetting.UserName,
    formControlName: EFormControlNameFormEmailSetting.UserName
  },
  {
    label: ELabelFormEmailSetting.Password,
    formControlName: EFormControlNameFormEmailSetting.Password
  },
  {
    label: ELabelFormEmailSetting.DefaultAddress,
    formControlName: EFormControlNameFormEmailSetting.DefaultAddress
  },
  ]

  dataFormGoogleClientId = [{
    label: ELabelFormGoogleClientIdSetting.GoogleClientId,
    formControlName: EFormControlNameFormGoogleClientIdSetting.GoogleClientId
  },
  ]

  dataFormMicrosoftClientId = [{
    label: ELabelFormMicrosoftClientIdSetting.microsoftClientId,
    formControlName: EFormControlNameFormMicrosoftClientIdSetting.MicrosoftClientId
  },
  ]

  dataFormSignServerClientId = [{
    label: ELabelFormSignServerClientIdSetting.AdminAPI,
    formControlName: EFormControlNameFormSignServerClientIdSetting.AdminAPI
  },
  {
    label: ELabelFormSignServerClientIdSetting.BaseAddress,
    formControlName: EFormControlNameFormSignServerClientIdSetting.BaseAddress
  },
  ]

  formRemiderExpriedTime = [{
    label: ELabelFormGoogleClientIdSetting.GoogleClientId,
    formControlName: EFormControlNameFormGoogleClientIdSetting.GoogleClientId
  },
  ]

  dataFormAwsS3Credential = [
    {
      label: ELabelS3.accessKeyId,
      formControlName: EFormControlNameS3.accessKeyId
    },
    {
      label: ELabelS3.secretKey,
      formControlName: EFormControlNameS3.secretKey
    },
    {
      label: ELabelS3.region,
      formControlName: EFormControlNameS3.region
    },
    {
      label: ELabelS3.bucketName,
      formControlName: EFormControlNameS3.bucketName
    },
    {
      label: ELabelS3.prefix,
      formControlName: EFormControlNameS3.prefix
    },
  ]

  formEmailSetting = this.fb.group({
    enableSsl: new FormControl({ value: false, disabled: true }, Validators.required),
    host: new FormControl({ value: '', disabled: true }, Validators.required),
    port: new FormControl({ value: '', disabled: true }, Validators.required),
    displayName: new FormControl({ value: '', disabled: true }, Validators.required),
    userName: new FormControl({ value: '', disabled: true }, Validators.required),
    password: new FormControl({ value: '', disabled: true }, Validators.required),
    defaultAddress: new FormControl({ value: '', disabled: true }, Validators.required),
    useDefaultCredentials: new FormControl({ value: false, disabled: true }, Validators.required)
  })

  formGoogleClientIdSetting = this.fb.group({
    googleClientId: new FormControl({ value: '', disabled: true }, Validators.required),
    enableLoginByUsername: new FormControl({ value: false, disabled: true }, Validators.required)
  })

  formAwsS3Credential = this.fb.group({
    accessKeyId: new FormControl({ value: '', disabled: true }, Validators.required),
    secretKey: new FormControl({ value: '', disabled: true }, Validators.required),
    region: new FormControl({ value: '', disabled: true }, Validators.required),
    bucketName: new FormControl({ value: '', disabled: true }, Validators.required),
    prefix: new FormControl({ value: '', disabled: true }, Validators.required),
  })

  formSignServerClientIdSetting = this.fb.group({
    AdminAPI: new FormControl({ value: '', disabled: true }, Validators.required),
    BaseAddress: new FormControl({ value: '', disabled: true }, Validators.required),
  })

  formMicrosoftClientIdSetting = this.fb.group({
    microsoftClientId: new FormControl({ value: '', disabled: true }, Validators.required),
  })

  constructor(injector: Injector, private fb: FormBuilder, private configurationService: ConfigurationService,
    private _apikeyService: ApiKeyService) {
    super(injector);
  }

  ngOnInit(): void {
    this.configurationService.getEmailSetting().subscribe(res => {
      this.emailSetting = res.result
      this.formEmailSetting.patchValue({ ...this.emailSetting, enableSsl: this.isCheckedEnableSsl, useDefaultCredentials: this.isCheckedUseDefault })
      this.handleCheckbox(this.emailSetting)
    })
    this.configurationService.getGoogleClientId().subscribe(res => {
      this.googleClientId = res.result.googleClientId
      this.formGoogleClientIdSetting.patchValue({ googleClientId: this.googleClientId })
    })

    this.configurationService.getSignServerUrlDto().subscribe(res => {
      this.SignServerClientId = res.result
      this.formSignServerClientIdSetting.patchValue({ AdminAPI: res.result.adminAPI, BaseAddress: res.result.baseAddress })
    })


    this.configurationService.getIsEnableloginByUsername().subscribe(res => {
      res.result.isEnableLoginByUsername === 'false' ? this.isEnableLoginByUsername = false : this.isEnableLoginByUsername = true
    })

    this.configurationService.getNotiExprireTime().subscribe(res => {
      this.remiderExpriedTime = parseInt(res.result?.notiExprireTime)
      this.tempRemiderExpriedTime = this.remiderExpriedTime
    })

    this.configurationService.getAWSCredential().subscribe(value => {
      this.formAwsS3 = value.result

      this.formAwsS3Credential.patchValue(value.result);
    })

    this.configurationService.getMicrosoftClientId().subscribe(value => {
      this.microsoftClientId = value.result.microsoftClientId
      this.formMicrosoftClientIdSetting.patchValue({ microsoftClientId: this.microsoftClientId })
    })

    this.getApiKey();
  }

  handleClickEdit(config) {
    switch (config) {
      case EFormConfiguration.EmailSetting:
        this.isDisabledFormEmail = !this.isDisabledFormEmail
        this.isDisabledFormEmail ? this.formEmailSetting.disable() : this.formEmailSetting.enable()
        break;

      case EFormConfiguration.GoogleClientId:
        this.isDisabledFormGoogleId = !this.isDisabledFormGoogleId
        this.isDisabledFormGoogleId ? this.formGoogleClientIdSetting.disable() : this.formGoogleClientIdSetting.enable()
        break;

      case EFormConfiguration.RemiderContractTerm:
        this.isDisabledRemiderExpriedTime = !this.isDisabledRemiderExpriedTime
        break;

      case EFormConfiguration.fromS3:
        this.isDisabledFormS3 = false;
        this.isDisabledFormS3 ? this.formAwsS3Credential.disable() : this.formAwsS3Credential.enable()
        break

      case EFormConfiguration.SignServerClientId:
        this.isDisabledFormSignServer = false;
        this.isDisabledFormSignServer ? this.formSignServerClientIdSetting.disable() : this.formSignServerClientIdSetting.enable();
        break
      case EFormConfiguration.MicrosoftClientId:
        this.isDisabledFormMicrosoftId = !this.isDisabledFormMicrosoftId
        this.isDisabledFormMicrosoftId ? this.formMicrosoftClientIdSetting.disable() : this.formMicrosoftClientIdSetting.enable()
        break

    }
  }

  handleCheckbox(emailSetting: any) {
    if (emailSetting[EFormControlNameFormEmailSetting.EnableSsl] === 'true') {
      this.isCheckedEnableSsl = true
    } else if (emailSetting[EFormControlNameFormEmailSetting.EnableSsl] === 'false') {
      this.isCheckedEnableSsl = false
    }

    if (this.emailSetting[EFormControlNameFormEmailSetting.UseDefaultCredentials] === 'true') {
      this.isCheckedUseDefault = true
    } else if (this.emailSetting[EFormControlNameFormEmailSetting.UseDefaultCredentials] === 'false') {
      this.isCheckedUseDefault = false
    }
  }

  checkEnableSsl(event: MatCheckboxChange) {
    this.isCheckedEnableSsl = event.checked
  }

  checkUseDefaultCredentials(event: MatCheckboxChange) {
    this.isCheckedUseDefault = event.checked
  }

  checkEnableLoginByUsername(event: MatCheckboxChange) {
    this.isEnableLoginByUsername = event.checked
  }

  saveChangeEmail(data: any) {
    this.configurationService.setEmailSetting({ ...data, enableSsl: this.isCheckedEnableSsl.toString(), useDefaultCredentials: this.isCheckedUseDefault.toString() }).subscribe(res => {
      this.emailSetting = { ...data, enableSsl: this.isCheckedEnableSsl.toString(), useDefaultCredentials: this.isCheckedUseDefault.toString() }
      abp.notify.success("Chỉnh sửa email thành công")
    })
    this.isDisabledFormEmail = true
    this.isDisabledFormEmail ? this.formEmailSetting.disable() : this.formEmailSetting.enable()
  }

  saveChangeGoogleId(data: any) {
    this.configurationService.setGoogleClientId({ googleClientId: data.googleClientId }).subscribe(res => {
      this.googleClientId = data.googleClientId
      abp.notify.success(this.ecTransform("EditGoogleClientIDSuccessfully"))
    })
    let enableLoginByUsername
    this.isEnableLoginByUsername ? enableLoginByUsername = 'true' : enableLoginByUsername = 'false'
    this.configurationService.setIsEnableloginByUsername({ isEnableLoginByUsername: enableLoginByUsername }).subscribe()
    this.isDisabledFormGoogleId = true
    this.isDisabledFormGoogleId ? this.formGoogleClientIdSetting.disable() : this.formGoogleClientIdSetting.enable()
  }

  saveChangeRemiderTime() {
    this.isDisabledRemiderExpriedTime = true;
  }

  saveChangeFromS3(formS3) {
    this.configurationService.setAWSCredential(formS3).subscribe(value => {
      abp.notify.success('Edit AWS S3 Successfully')
      this.isDisabledFormS3 = true;
      this.isDisabledFormS3 ? this.formAwsS3Credential.disable() : this.formAwsS3Credential.enable();
    })
  }

  saveChangeSignServerId(data) {

    this.configurationService.setSignServerUrlDto({ baseAddress: data.BaseAddress, adminAPI: data.AdminAPI }).subscribe(res => {
      this.SignServerClientId = data.SignServerClientId
      abp.notify.success(this.ecTransform("EditSignServerClientIdSuccessfully"))
      this.isDisabledFormSignServer = true;
      this.isDisabledFormSignServer ? this.formSignServerClientIdSetting.disable() : this.formSignServerClientIdSetting.enable();
    })
  }

  saveChangeMicrosoftId(data) {
    this.configurationService.setMicrosoftClientId({ microsoftClientId: data.microsoftClientId }).subscribe(res => {
      this.microsoftClientId = data.microsoftClientId
      this.isDisabledFormMicrosoftId = true
      this.isDisabledFormMicrosoftId ? this.formMicrosoftClientIdSetting.disable() : this.formMicrosoftClientIdSetting.enable()
      abp.notify.success(this.ecTransform("EditmicrosoftClientIDSuccessfully"))
    })
  }


  cancelChange(config) {
    switch (config) {
      case EFormConfiguration.EmailSetting:
        this.handleCheckbox(this.emailSetting)
        this.formEmailSetting.patchValue({ ...this.emailSetting, enableSsl: this.isCheckedEnableSsl, useDefaultCredentials: this.isCheckedUseDefault });
        this.isDisabledFormEmail = true
        this.isDisabledFormEmail ? this.formEmailSetting.disable() : this.formEmailSetting.enable()
        break;

      case EFormConfiguration.GoogleClientId:
        this.handleCheckbox(this.emailSetting)
        this.formGoogleClientIdSetting.patchValue({ googleClientId: this.googleClientId })
        this.isDisabledFormGoogleId = true
        this.isDisabledFormGoogleId ? this.formGoogleClientIdSetting.disable() : this.formGoogleClientIdSetting.enable()
        break;

      case EFormConfiguration.RemiderContractTerm:
        this.isDisabledRemiderExpriedTime = true;
        this.remiderExpriedTime = this.tempRemiderExpriedTime;
        break

      case EFormConfiguration.fromS3:
        this.formAwsS3Credential.patchValue({ accessKeyId: this.formAwsS3.accessKeyId, secretKey: this.formAwsS3.secretKey })
        this.isDisabledFormS3 = true;
        this.isDisabledFormS3 ? this.formAwsS3Credential.disable() : this.formAwsS3Credential.enable();
        break

      case EFormConfiguration.SignServerClientId:
        this.formSignServerClientIdSetting.patchValue({ AdminAPI: this.SignServerClientId.adminAPI, BaseAddress: this.SignServerClientId.baseAddress })
        this.isDisabledFormSignServer = true;
        this.isDisabledFormSignServer ? this.formSignServerClientIdSetting.disable() : this.formSignServerClientIdSetting.enable();
        break

      case EFormConfiguration.MicrosoftClientId:
        this.formMicrosoftClientIdSetting.patchValue({ microsoftClientId: this.microsoftClientId })
        this.isDisabledFormMicrosoftId = true
        this.isDisabledFormMicrosoftId ? this.formMicrosoftClientIdSetting.disable() : this.formMicrosoftClientIdSetting.enable()
        break
    }
  }

  isGrandEdit() {
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_Edit)
  }

  setNotiExprireTime() {
    this.isDisabledRemiderExpriedTime = true
    this.configurationService.setNotiExprireTime({ notiExprireTime: this.remiderExpriedTime.toString() }).subscribe(res => {
      abp.notify.success(this.ecTransform("EditNotiExprireTimeSuccessfully"))
    })
  }

  handleExpriedTime(event: KeyboardEvent) {
    const key = event.key;
    if (key === '.') {
      event.preventDefault();
      return
    }
  }

  getApiKey() {
    this._apikeyService.GetApiKey().subscribe(rs => {
      this.apiKey = rs.result
    })
  }

  generateApiKey() {
    this._apikeyService.GenerateApiKey().subscribe(rs => {
      abp.message.success(`Generate thành công, api key của bạn là:<h5>${rs.result}</h5>`, "Success", { isHTML: true })
      this.getApiKey()
    })
  }
}
