namespace Common.Constant
{
    public static class ConstantMessages
    {
        //Actions
        public const string ACTION_SETUP = "setup";
        public const string ACTION_CHANGE = "change";

        //Roles
        public const string ROLE_SUPERADMIN = "superadmin";
        public const string ROLE_ADMIN = "admin";
        public const string ROLE_MENTOR = "mentor";
        public const string ROLE_MENTEE = "mentee";

        //Emails
        public const string EMAIL_DISPLAY_NAME = "Shikshadaan";
        public const string EMAIL_SUBJECT_SETUP = "Setup your shikshadaan account";
        public const string EMAIL_SUBJECT_CHANGE = "Password change request";
        public const double TOKEN_EXPIRY_DAYS = 1;
        public const bool NOT_EXPIRED = false;
        public const bool EXPIRED = true;

        public const string USER_CANNOT_BE_DELETED = "User cannot be deleted";
        public const string USER_DELETED_SUCCESS = "User deleted successfully";
        public const string FILTER_CANNOT_BE_NULL = "Filter cannot be null";
        public const string RECORD_FOUND = "Record found";
        public const string RECORD_NOT_FOUND = "Record not found";

        // FAILURE MESSAGES

        public const string NOT_NULL_EMAIL_AND_NAME = "EmailId or Name cannot be empty !";
        public const string INVALID_ACTION = "Invalid action !";
        public const string INVALID_ROLE = "Invalid role !";
        public const string NULL_PARAMETER_PASSED = "Parameter cannot be null !";
        public static string SOMETHING_WENT_WRONG = "An error occurred while processing your request. Please try again later.";


        //Emails
        public const string INVALID_TOKEN = "Invalid token !";
        public const string TOKEN_EXPIRED = "Token is expired !";
        public const string TIME_LIMIT_EXCEEDED = "Time to register is exceeded, contact your support team !";
        public static string INVALID_EMAIL = "Invalid email format: ";

        // Users
        public const string ACCOUNT_NOT_NULL = "account cannot be empty !";
        public const string USER_ROLES_AND_PERMISSIONS_NULL = "Roles and permisssions are null !";
        public const string MULTIPLE_SUPERADMIN_FAILURE = "Cannot add multiple superadmins, one superadmin already exists !";
        public const string ACCOUNT_CANNOT_BE_ADDED = "account cannot be added !";
        public const string NOT_FOUND = "Not found !";
        public const string INVALID = "Invalid ";
        public const string ALREADY_EXISTS = "already exists";
        public const string INVALID_ID_THAT_CANNOT_BE_UPDATED = "Invalid ids that cannot be updated !";
        public const string ACCOUNT_ADDED_EXCEPT = "Account added except ";
        public const string UPDATED_SUCCESSFULLY = "Upated successfully";
        public static string ACCOUNT_ERROR_OCCURRED = "An error occurred while adding the accounts!";
        public static string INVALID_DOB = "Invalid date of birth format: ";
        public static string NOT_ASSIGN_PERMISSION = "Not Assign the permission for this user ";

        // SUCCESS MESSAGES

        //Emails
        public const string MAIL_SENT_SUCCESS = "Mail sent successfully to email : ";
        public const string DATA_RENDERED_SUCCESS = "Data rendered successfully";
        public const string MAIL_SENT_TO = "Mail sent to";
        public static string EMAIL_EXIST = "Email is Already Exist: ";

        //SuperAdmins register
        public const string ACCOUNT_ADDED_SUCCESS = "Account added successfully !";

        //Login
        public const string LOGIN_SUCCESS = " logged in successfully !";
        public const string WRONG_PASSWORD = "Password is wrong !";
        public const string WRONG_EMAIL = "Email is wrong !";

        // Reset Passworda
        public const string PASSWORD_RESET_SUCCESS = "Password has been reset successfully";

        // User Permission 
        public const string USERROLE_CANNOT_BE_ADDED = "user role and permission cannot be added !";
        public const string USERROLE_ADDED_SUCCESS = "User role permission added successfully !";
        public const string GET_PERMISSION_SUCCESS = "Permission fetched successfully !";
        public const string GET_PERMISSION_FAILURE = "Permission cannot be fetched !";
        public const string ACCOUNT_UPDATED_SUCCESS = "Account updated successfully";



    }
}
