namespace MedManagerApi.Models;

public static class AppRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string User = "User";

    public static IEnumerable<string> GetAllRoles()
    {
        return new[] { SuperAdmin, Admin, User };
    }
}
