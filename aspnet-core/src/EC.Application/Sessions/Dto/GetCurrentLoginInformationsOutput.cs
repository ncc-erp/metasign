namespace EC.Sessions.Dto

{
    public class GetCurrentLoginInformationsOutput
    {
        public ApplicationInfoDto Application { get; set; }
        public UserLoginInfoDto User { get; set; }
        public TenantLoginInfoDto Tenant { get; set; }
        public string GoogleClientId { get; set; }
        public string MicrosoftClientId { get; set; }
        public string IsEnableLoginByUsername { get; set; }
    }
}