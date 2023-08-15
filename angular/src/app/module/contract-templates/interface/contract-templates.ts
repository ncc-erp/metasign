import { ContractTemplateType } from "./../enum/contract-template.enum";
export interface ContractTemplates {
  id: number;
  name: string;
  content: string;
  type: ContractTemplateType;
  userId: number;
  isFavorite: true;
}

export interface sortTable{
  sort: string;
  status: number
}
