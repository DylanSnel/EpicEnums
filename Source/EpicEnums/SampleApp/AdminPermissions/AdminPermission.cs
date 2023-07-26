namespace SampleApp.AdminPermissions;

public partial record AdminPermission
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string ClaimName { get; init; }
}
