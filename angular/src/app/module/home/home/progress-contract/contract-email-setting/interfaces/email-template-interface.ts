export interface IEmailTemplate {
  id: number;
  name: string;
  description?: string | null;
  bodyMessage: string;
  type: number;
  cCs?: string[];
  arrCCs?: string[];
  sendToEmail?: string | null;
  language: string;
  templateType: number;
}
