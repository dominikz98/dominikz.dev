namespace dominikz.shared.Contracts;

[Flags]
public enum RightFlags
{
    None = 0,
    BlogCreate = 1,
    BlogEdit = 2,
    BlogDelete = 4,
    MediaAdd = 8,
    MediaEdit = 16,
    MediaDelete = 32,
    CreateAccount = 64
}