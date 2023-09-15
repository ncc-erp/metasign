import { TenantAvailabilityState } from "@shared/service-proxies/service-proxies";

export class AppTenantAvailabilityState {
  static Available: number = TenantAvailabilityState._1;
  static InActive: number = TenantAvailabilityState._2;
  static NotFound: number = TenantAvailabilityState._3;
}

export enum ContractInvalidate {
  contractSigner = 1,
  contractCreator = 2
}


export enum loginApp {
  google = 1,
  microsoft = 2
}

export enum contractStep {
  UploadFile = 0,
  SignerSetting = 1,
  SignatureSetting = 2,
  EmailSetting = 3,
}

export enum ContractRole {
  Signer = 1,
  reviewer = 2,
  Viewer = 3,
}

export enum SignType {
  contractSigning = 1,
  massContractSigning = 2
}

export enum TemplateType {
  Mail = 1,
  Print = 2,
}
export enum EmailFunc {
  Signing = 1,
}

export enum ETabDetailWorker {
  Properties = 0,
  Certificates = 1,
}

export enum ETypeWorker {
  Pdfsigner = 'pdfsigner',
  Crypto = 'keystore-crypto'
}

export enum EStatusContract {
  All = "All",
  Draft = "Drafts",
  Inprogress = "WaitingToSign",
  Completed = "Completed",
  Cancelled = "Cancelled",
}

export enum ENameStatusContract {
  All = "Tất cả",
  Draft = "Bản nháp",
  Inprogress = "Chờ ký",
  Completed = "Hoàn thành",
  Cancelled = "Hủy bỏ",
}

export enum buttomType {
  login = 1,
  tenant = 2

}

export enum MassType {
  Singel = 1,
  Multiple = 2
}



export enum EContractStatusId {
  Draft = 1,
  Inprogress = 2,
  Cancelled = 3,
  Complete = 4,
}

export enum EContractFilterType {
  AssignToMe = 1,
  WatingForOther = 2,
  ExpirgingSoon = 3,
  Completed = 4,
}

export enum EQuickFilter {
  AssignToMe = "NeedToSign",
  WaitingOthers = "WaitingToOthers",
  ExpirgingSoon = "ExpirationSoon",
  Completed = "Completed",
}

export enum EDisplayedColumnContract {
  NumericalOrder = "No",
  Name = "ContractName",
  Code = "ContractCode",
  ExpriedTime = "ExpriedTime",
  Status = "Status",
  CreationTime = "CreatedTime",
  UpdatedTime = "UpdatedTime",
}

export enum EButtonActionPageDetail {
  Resend = "ResendToAll",
  Edit = "Edit",
  Sign = "SignNow",
  Cancel = "Cancel"
}

export enum ELabelFormEmailSetting {
  Host = "Host",
  Port = "Port",
  DisplayName = "Display name",
  UserName = "Username",
  Password = "Password",
  DefaultAddress = "Default from address",
}


export enum ELabelS3 {
  accessKeyId = "AccessKeyId",
  secretKey = "SecretKey",
  region = "Region",
  bucketName = "Bucket Name",
  prefix = "Prefix"
}

export enum EFormControlNameS3 {
  accessKeyId = "accessKeyId",
  secretKey = "secretKey",
  region = "region",
  bucketName = "bucketName",
  prefix = "prefix"
}

export enum ELabelFormGoogleClientIdSetting {
  GoogleClientId = "ID",
  IsEnableLoginByUsername = "Login by Username"
}

export enum ELabelFormMicrosoftClientIdSetting {
  microsoftClientId = "ID",
}

export enum ELabelFormSignServerClientIdSetting {
  BaseAddress = "baseAddress",
  AdminAPI = "adminAPI"
}

export enum EFormConfiguration {
  EmailSetting = 'EmailSetting',
  GoogleClientId = "GoogleClientId",
  RemiderContractTerm = 'RemiderContractTerm',
  fromS3 = 'fromS3',
  SignServerClientId = 'SignServerClientId',
  MicrosoftClientId = "MicrosoftClientId"
}

export enum EFormControlNameFormEmailSetting {
  EnableSsl = "enableSsl",
  Host = "host",
  Port = "port",
  DisplayName = "displayName",
  UserName = "userName",
  Password = "password",
  DefaultAddress = "defaultAddress",
  UseDefaultCredentials = "useDefaultCredentials",
}

export enum EFormControlNameFormGoogleClientIdSetting {
  GoogleClientId = "googleClientId",
  IsEnableLoginByUsername = "Login by Username"
}

export enum EFormControlNameFormMicrosoftClientIdSetting {
  MicrosoftClientId = "microsoftClientId",
}

export enum EFormControlNameFormSignServerClientIdSetting {
  BaseAddress = "BaseAddress",
  AdminAPI = "AdminAPI",
}

export enum ESubPathProcess {
  UpLoadFile = "upload-file",
  Setting = "setting",
  SignatureSetting = "signatureSetting",
  EmailSetting = "emailSetting",
}

export enum ContractStatus {
  Draft = 1,
  Inprogress = 2,
  Cancelled = 3,
  Complete = 4,
}


export enum ContractSettingType {
  Electronic = 1,
  Digital = 2,
  Acronym = 3,
  Text = 4,
  DatePicker = 5,
  Dropdown = 6,
  Stamp = 7
}

export enum EPdfSignerStatusId {
  Active = 1,
  Offline = 2,
  Disable = 3,
}

export enum ENameStatusPdfSigner {
  Active = "Active",
  Offline = "Offline",
  Disable = "Disabled",
}

export enum EResStatusPdfSigner {
  Active = "ACTIVE",
  Offline = "OFFLINE",
  Disable = "DISABLED",
}

export enum EColorStatusPdfSigner {
  Active = "badge badge-success",
  Offline = "badge badge-danger",
  Disable = "badge badge-secondary",
}

export enum EDisplayedColumnSignature {
  NumericalOrder = "No",
  Signature = "Signature",
  CreationTime = "CreatedTime",
  UpdatedTime = "UpdatedTime",
  DefaultSignature = "DefaultSignature",
  Actions = "Actions",
}

export enum EKeyColumnSignature {
  CreationTime = "creationTime",
  UpdatedTime = "lastModificationTime",
  IsDefault = "isDefault"
}

export enum ETypeSort {
  default = -1,
  up = 1,
  down = 2
}

export enum BreakPoint {
  Ipad = 1280,
  IpadMini = 1000,
  mobile = 600,
}
