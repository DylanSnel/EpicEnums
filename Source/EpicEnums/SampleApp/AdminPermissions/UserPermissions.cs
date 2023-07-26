using EpicEnums;

namespace SampleApp.AdminPermissions;

public partial record UserPermissions : EpicEnum<AdminPermission>
{
    public static AdminPermission View { get; } = new() { ClaimName = "Users:View", Name = "View", Description = "View users and their properties" };
    public static AdminPermission Edit { get; } = new() { ClaimName = "Users:Edit", Name = "Edit", Description = "Edit users properties" };
    public static AdminPermission Delete { get; } = new() { ClaimName = "Users:Delete", Name = "Delete", Description = "Delete users" };
}
