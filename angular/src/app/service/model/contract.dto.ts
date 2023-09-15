export interface ContractDto {
    id:number
    name: string
    code: string
    userId: number
    status: string
    expriedTime: string
    updatedUser: string
    updatedTime: string
    creatorUser: string
    creationTime: string
    file: string
    fileBase64: string
}
export interface ContractImages {
    contractPage: number
    fileBase64: any
}

export interface ContractTempaleteDto {
    htmlContent?: string,
    id?: number,
    name: string,
    fileName:string,
    content: string | ArrayBuffer,
    type: number,
    userId: number,
    isFavorite: boolean,
    massType?: number,
}
