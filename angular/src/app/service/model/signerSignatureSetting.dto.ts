import { SignatureSettings } from "./design-contract.dto";

export interface ContractSignerSignatureSettingDto {
  contractId: number;
  contractBase64: string;
  contractName: string;
  isComplete: boolean;
  isLoggedIn: boolean;
  role: number;
  status: number;
  signatureDefault: string;
  signatureSettings: SignatureSettings[];
}

export interface ContractSignatureSettingDto {
  isComplete: boolean;
  contractPage: number;
  fileBase64: any;
  width: number;
  height: number;
  contractName: string;
  signatureSettings: SignatureSettings[];
}
