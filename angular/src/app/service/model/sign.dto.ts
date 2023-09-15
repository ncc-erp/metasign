export interface SignDto {
    signerSignatureSettingid: number,
    signartureBase64: string,
    signatureUserId: string,
    isNewSignature:boolean,
    setDefault: boolean,
    signatureType:number,
    certSerial: string,
    organizationCA:string,
    ownCA:string,
    beginDateCA:string,
    endDateCA:string,
    uid:string,
    isDownloadApp?:boolean
    pageHeight:any,
    height?: number,
    page?:number,
    width?:number,
    x?:number,
    y?:number

}


export interface SignMultipleDto {
    contractId: number,
    signSignatures: SignDto[],
    contractBase64?:string
}
