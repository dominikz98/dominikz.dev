namespace dominikz.Domain.Enums;

[Flags]
public enum MovieGenresFlags
{
    All = 0,
    Horror = 2,
    Drama = 4,
    Mystery = 8,
    Thriller = 16,
    Action = 32,
    Adventure = 64,
    Fantasy = 128,
    Comedy = 256,
    Western = 512,
    SciFi = 1024,
    Animation = 2048,
    Crime = 4096,
    Musical = 8192,
    War = 16384,
    Romance = 32768,
    Biography = 65536,
    History = 131072,
    Family = 262144
}
