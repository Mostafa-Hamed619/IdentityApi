using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AdminFullStack
{
    public static class SD
    {
        public const string Facebook = "facebook";
        public const string Google = "google";

        //Roles
        public const string AdminRole = "Admin";
        public const string ManagerRole = "Manager";
        public const string PlayerRole = "Player";

        public const string AdminUserName = "MosSEffy21@gmail.com";
        public const string SuperAdminChangeNotAllowed = "Super Admin is not allowed to change";

        public const int MaximumLoginAttempts = 5;
    }
}
