import { ActionType } from "@app/module/signature/signature-management/signature-management.component";
import { ContractSettingType } from "@shared/AppEnums";
export interface signatureUserDto {
  id: number;
  signatureTypeId: number;
  signatureTypeName: string;
  userId: number;
  file: string;
  fileBase64: string;
  isDefault: boolean;
  signatureType:number
}

export interface signatureType {
  lable: string;
  value: ContractSettingType;
}

export interface signatureType {
  lable: string;
  value: ContractSettingType;
}

export interface actionEditSignature {
  type: ActionType,
  id: number,
  isDefault: number
}
