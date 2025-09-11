import { IEmailTemplate } from "@app/module/home/home/progress-contract/contract-email-setting/interfaces/email-template-interface";

export interface MailPreviewInfoDto {
    templateId: number,
    name: string
    description: string
    mailFuncType: number,
    subject: string,
    bodyMessage: string,
    sendToEmail: string,
    propertiesSupport: string[],
    cCs: string[],
    currentUserLoginId: number,
    tenantId: number,
    templateType: number
}
export interface EmailDto {
    id: number,
    name: string,
    cCs: string,
    ArrCCs: string[],
    description: string,
    templateType: string,
    language: string
}
export interface MailDialogData {
    mailInfo?: MailPreviewInfo,
    showEditButton?: boolean,
    templateId?: number,
    isAllowSendMail?: boolean,
    showDialogHeader?: boolean,
    title?: string,
}

export class MailPreviewInfo {
    templateId: number;
    name: string;
    description: string;
    type?: number;
    bodyMessage: string;
    subject: string;
    cCs: string[];
    arrCCs?: string[];
    mailFuncType?: number;
    propertiesSupport: string[];
    sendToEmail: string;
    templateType: number;
    tenantId?: number;
}
export interface EditEmailDialogData {
    templateId?: number;
    mailInfo?: MailPreviewInfo;
    title?: string;
    showDialogHeader?: boolean;
    temporarySave?: boolean;
    isEditTemplate?: boolean;
    emailTemplateList?: IEmailTemplate[];
    contractId?: number;
}
export interface UpdateEmailTemplate {
    id: number;
    name: string;
    bodyMessage: string;
    subject: string;
    description: string;
    type: number;
    listCC: string[];
    sendToEmail: string;
}

