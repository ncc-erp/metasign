namespace EC.Constants.FileStoring
{
    public class FileStoringConstants
    {
        public static string Provider { get; set; }
        public static readonly string AWSS3 = "AWSS3";
        public static string ContractFolder { get; set; }
        public static string AttachmentFolder { get; set; }
        public static string UnsignedFolder { get; set; }
        public static string SignedFolder { get; set; }
        public static string SignatureFolder { get; set; }
        public static string DownloadFolder { get; set; }
    }
}
