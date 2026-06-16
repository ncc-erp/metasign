namespace EC.Models.TokenAuth
{
    public class AuthenticateResultModel
    {
        public string AccessToken { get; set; }

        public string EncryptedAccessToken { get; set; }

        public int ExpireInSeconds { get; set; }

        public long UserId { get; set; }
    }

    public class MezonSigningAuthenticateResult : AuthenticateResultModel
    {
        public string Email { get; set; }
    }
}
