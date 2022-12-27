namespace dominikz.shared.Contracts;

[Flags]
public enum PermissionFlags
{
    None = 0,
    CreateOrUpdate = 1,
    Blog = 2,
    Media = 4,
    Account = 8
}