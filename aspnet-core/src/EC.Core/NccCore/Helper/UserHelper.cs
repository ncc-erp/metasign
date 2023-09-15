namespace ProjectManagement.NccCore.Helper
{
    public class UserHelper
    {
        public static string GetUserName(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return null;
            }
            int index = emailAddress.IndexOf("@");
            if (index < 0)
            {
                return emailAddress;
            }
            return emailAddress.Substring(0, index).ToLower();

        }
    }
}
