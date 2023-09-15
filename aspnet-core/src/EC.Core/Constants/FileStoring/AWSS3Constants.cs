namespace EC.Constants.FileStoring
{
    public class AWSS3Constants
    {
        public static string Profile { get; set; }
        public static string AccessKeyId { get; set; }
        public static string SecretKey { get; set; }
        public static string Region { get; set; }
        public static string BucketName { get; set; }
        public static string Prefix { get; set; }
    }

    public class AWSS3Dto
    {
        public string Profile { get; set; }
        public string AccessKeyId { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
        public string BucketName { get; set; }
        public string Prefix { get; set; }
    }
}