using EC.Debugging;

namespace EC
{
    public class ECConsts
    {
        public const string LocalizationSourceName = "EC";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;

        public const int DELAY_SEND_MAIL_SECOND = 1;

        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "4849fef1b6f94365ba2ced5b336da504";
    }
}
