export enum EHistoryAction {
        CreateContract = 1,
        SendMail = 2,
        CancelContract = 3,
        Sign = 4,
        Complete = 5,
        VoidTosign = 6,
}

export enum EHistoryActionName {
        CreateContract = "CreatedDocument",
        SendMail = "SentMail",
        CancelContract = "CancelledDocument",
        Sign = "Signed",
        Complete = "Completed",
        VoidToSign = "VoidToSign",
}

export enum EUserContract {
        Signer = 1,
        Approver = 2,
        RecipientOfACopy = 3
}

export enum EChangeSortContract {
        Default = -1,
        Up = 0,
        Down = 1
}

export enum EDownloadType {
        All = 1,
        Document1PDF = 2,
        CertificatePDF = 3
}

export enum EFilterSigner {
        All = "All",
        Me = "Me"
}

export enum ESortContract {
        Name = "name",
        CreationTime = "creationTime",
        UpdatedTime = "updatedTime"
}

export enum EButtonLeftPageDetail {
        SendMail,
        Edit,
        Sign,
        Cancel
}




