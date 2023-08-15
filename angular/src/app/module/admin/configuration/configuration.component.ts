import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { EFormConfiguration, EFormControlNameFormEmailSetting, EFormControlNameFormGoogleClientIdSetting, EFormControlNameS3, ELabelFormEmailSetting, ELabelFormGoogleClientIdSetting, ELabelS3 } from '@shared/AppEnums';
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
  isEnableLoginByUsername: boolean = false
  isDisabledRemiderExpriedTime: boolean = true
  remiderExpriedTime: number
  tempRemiderExpriedTime: number

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
    secretKey: new FormControl({ value: '', disabled: true }, Validators.required)
  })

  constructor(injector: Injector, private fb: FormBuilder, private configurationService: ConfigurationService) {
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

    this.configurationService.getIsEnableloginByUsername().subscribe(res => {
      res.result.isEnableLoginByUsername === 'false' ? this.isEnableLoginByUsername = false : this.isEnableLoginByUsername = true
    })

    this.configurationService.getNotiExprireTime().subscribe(res => {
      this.remiderExpriedTime = parseInt(res.result?.notiExprireTime)
      this.tempRemiderExpriedTime = this.remiderExpriedTime
    })

    this.configurationService.getAWSCredential().subscribe(value=>{
        this.formAwsS3Credential.patchValue(value.result);
    })
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

  saveChangeFromS3(formS3)
  {
    this.configurationService.setAWSCredential(formS3).subscribe(value =>{
      abp.notify.success('Edit AWS S3 Successfully')
      this.isDisabledFormS3 = true;
      this.isDisabledFormS3 ? this.formAwsS3Credential.disable() : this.formAwsS3Credential.enable();
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
        this.isDisabledFormS3 = true;
        this.isDisabledFormS3 ? this.formAwsS3Credential.disable() : this.formAwsS3Credential.enable();
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
}
