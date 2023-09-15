namespace EC.Constants
{
    public class Enum
    {
        public enum ContractStatus
        {
            Draft = 1,
            Inprogress = 2,
            Cancelled = 3,
            Complete = 4
        }

        public enum HistoryAction
        {
            CreateContract = 1,
            SendMail = 2,
            CancelContract = 3,
            Sign = 4,
            Complete = 5,
            VoidToSign = 6,
        }

        public enum ContractRole
        {
            Signer = 1,
            Reviewer = 2,
            Viewer = 3
        }

        public enum MailFuncEnum
        {
            Signing = 1,
        }

        public enum MailTemplateType
        {
            Mail = 1,
            Print = 2
        }

        public enum ContractFilterType
        {
            AssignToMe = 1,
            WatingForOther = 2,
            ExpirgingSoon = 3,
            Completed = 4
        }

        public enum SignatureTypeSetting
        {
            Electronic = 1,
            Digital = 2,
            Acronym = 3,
            Text = 4,
            DatePicker = 5,
            Dropdown = 6,
            Stamp = 7
        }

        public enum ContractTemplateType
        {
            Html = 1,
            Pdf = 2
        }

        public enum ContractTemplateFilterType
        {
            Me = 1,
            System = 2
        }

        public enum DownloadContractType
        {
            All = 1,
            Contract = 2,
            Certificate = 3,
        }

        public enum ContractSettingStatus
        {
            NotConfirmed = 1,
            Confirmed = 2,
            Rejected = 3,
        }

        public enum SignMethod
        {
            Image = 1,
            UsbToken = 2,
            Input = 3
        }

        public enum FileCategory
        {
            UnsignedContract,
            SignedContract,
            Attachment,
            Signature,
            Download
        }

        public enum LoginType
        {
            Google = 1,
            Microsoft = 2
        }

        public enum MassType
        {
            Singel = 1,
            Multiple = 2
        }
    }
}