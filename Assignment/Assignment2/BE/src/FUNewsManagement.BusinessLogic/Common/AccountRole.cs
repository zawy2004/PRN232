namespace FUNewsManagement.BusinessLogic.Common;

public enum AccountRole
{
    Staff = 1,
    Lecturer = 2,
    Admin = 3,
}

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Staff = "Staff";
    public const string StaffOrAdmin = "Staff,Admin";

    public static string FromAccountRole(int? accountRole) =>
        accountRole == (int)AccountRole.Admin ? Admin : Staff;
}
