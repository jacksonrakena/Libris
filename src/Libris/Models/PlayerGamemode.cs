namespace Libris.Models
{
    public enum PlayerGamemode : byte // why some enums are int and this one is byte? consistency pls 7.7
    {
        Survival = 0,
        Creative = 1,
        Adventure = 2,
        Spectator = 3
    }
}
