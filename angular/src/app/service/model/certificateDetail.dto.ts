export interface CertificateDetailDto {
    certSerial?: string
    organizationCA?: string
    ownCA?: string
    uid?: string
    beginDateCA?: string
    endDateCA?: string
    isDownloadApp?: boolean
}

// export default interface SignDigitalDto {
//     base64Pdf: string
//     page: number
//     certSerial: string
//     position: DigitalSignaturePositionDto
// }
// export interface DigitalSignaturePositionDto {
//     x: string
//     y: string
//     llx: string
//     lly: string
// }
