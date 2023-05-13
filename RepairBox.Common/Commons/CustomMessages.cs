using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.Common.Commons
{
    public class CustomMessage
    {
        //Admin
        public const string ADMIN_EXIST = "Admin already exist.";
        public const string ADMIN_ADDED = "Admin register successfully.";
        public const string ADMIN_UPDATED = "Admin updated successfully.";

        // Auth
        public const string USER_NOT_EXIST = "User not exist.";
        public const string INCORRECT_CREDENTIALS = "Email or Password is incorrect.";

        public const string EMAIL_NOT_EXIST = "Email not exist.";
        public const string NEW_PASSWORD_SENT = "New password has sent to the registered email address.";

    }

    public class DeveloperConstants
    {
        public const string ENDPOINT_PREFIX = "api/v1/[controller]";
    }

    public static class ResponseMessage
    {
        public const bool SUCCESS = true;
        public const bool FAILURE = false;
    }
}
