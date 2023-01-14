namespace dominikz.Domain.Enums.Media;

[Flags]
public enum GameGenresFlags
{
    All = 0,
    VirtualReality = 1,
    Indie = 2,
    Survival = 4,
    Horror = 8,
    Action = 16,
    Puzzle = 32,
    OpenWorld = 64,
    Adventure = 128,
    Shooter = 256,
    RealTime = 512,
    Strategy = 1024,
    Tactic = 2048,
    Rpg = 4096,
    Sandbox = 8192,
    Simulation = 16384,
    Stealth = 32768,
    Racing = 65536,
    Construction = 131072,
    JumpNRun = 262144,
    BattleRoyal = 524288,
    HackAndSlay = 1048576,
    ClickAndPoint = 2097152,
    Party = 4194304
}