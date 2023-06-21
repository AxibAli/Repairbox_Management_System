﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.Common.Commons
{
    public class CustomMessage
    {
        public const string ADDED_SUCCESSFULLY = "{0} created successfully.";
        public const string DELETED_SUCCESSFULLY = "{0} deleted successfully.";
        public const string UPDATED_SUCCESSFULLY = "{0} updated successfully.";
        public const string NOT_FOUND = "{0} not found.";

        // Auth
        public const string USER_NOT_EXIST = "User not exist.";
        public const string INCORRECT_CREDENTIALS = "Email or Password is incorrect.";
        public const string EMAIL_NOT_EXIST = "Email not exist.";
        public const string NEW_PASSWORD_SENT = "New password has sent to the registered email address.";
        public const string EMAIL_ALREADY_EXIST = "Email already exists.";
        public const string USERNAME_ALREADY_EXIST = "Username already exists.";
        public const string LOGIN_SUCCESSFUL = "User successfully logged in.";
        public const string LOGOUT_SUCCESSFUL = "User successfully logged out.";
    }

    public class DeveloperConstants
    {
        public const string ENDPOINT_PREFIX = "api/v1/[controller]";
        public const int PAGE_SIZE = 10;
    }

    public static class ResponseMessage
    {
        public const bool SUCCESS = true;
        public const bool FAILURE = false;
    }
}
