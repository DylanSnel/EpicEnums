using EpicEnums;

namespace SampleApp.AdminPermissions;

public partial record ContentPermissions : EpicEnum<AdminPermission>
{
    public static AdminPermission View { get; } = new() { ClaimName = "Content:View", Name = "View", Description = "View content and Images" };
    public static AdminPermission Edit { get; } = new() { ClaimName = "Content:Edit", Name = "Edit", Description = "Edit content and upload images" };
    public static AdminPermission Delete { get; } = new() { ClaimName = "Content:Delete", Name = "Delete", Description = "Delete Images" };
}
