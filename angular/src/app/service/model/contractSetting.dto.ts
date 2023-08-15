import { ContractRole } from "@shared/AppEnums";




export interface ContractSigners
{
  isOrder: boolean
  signers: ContractSettingDto[]
}

export interface ContractSettingDto {
  id: number;
  contractId: number;
  signerName: string;
  signerEmail: string;
  contractRole: number;
  password: string;
  procesOrder: number;
  color?: string;
  contractFileName: string;
  role?: string ,
  contractTemplateSignerId?: number
}

export interface contractRoles {
  lable: string;
  value: ContractRole;
}

export interface ContractStatisticDto {
  waitForMe: number,
  waitForOther: number,
  exprireSoon: number,
  complete: number
}