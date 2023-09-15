import { ContractSettingType } from "@shared/AppEnums";

export interface SignerContract {
  id: number;
  contractId: number;
  signerName: string;
  signerEmail: string;
  contractRole: number;
  procesOrder: number;
  password: string;
  contractFileName: string;
  color?: string;
}

export interface ContractFileImage {
  [x: string]: any;
  contractPage: number;
  fileBase64: string;
  width: number;
  height: number;
}

export interface ContractFile {
  page: number;
  contractBase64: any;
  width: number;
  height: number;
  contractId: number;
  contractName: string;
  isComplete: boolean;
  isLoggedIn: boolean;
  role: number;
  signatureDefault: null;
  status: number;
  signatureSettings: SignatureSettings[];
}

export interface colorSignature {
  backgroundColor: string;
  borderColor: string;
}

export interface contractInput {
  value: ContractSettingType;
  lable: string;
}

export interface SignatureSettings {
  page?: number;
  id: number;
  contractSettingId: number;
  signatureType: ContractSettingType;
  isSigned: boolean;
  positionX: number;
  positionY: number;
  width: number;
  height: number;
  isAllowSigning?: boolean;
  valueInput?: string;
  color?: colorSignature;
  signerName: string;
  fontColor?: string;
  fontFamily?: string;
  fontSize?: number;
  isTemporarySigned?: boolean;
  signatureTypeName?: string;
  heightPage: number;
  loading?: boolean;
}

export interface FillInputDto {
  signerSignatureSettingId: number
  fontSize: number
  fontFamily: string
  content: string
  pageHeight: number
  color: string
  signatureType: number
  base64Pdf: string
}

export interface SignInputDto {
  base64Pdf: string
  listInput: FillInputDto[]
}
