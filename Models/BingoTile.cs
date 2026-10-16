namespace SocialBingo.Models;

public sealed class BingoTile
{
    public int Row { get; set; }

    public int Column { get; set; }

    public string Prompt { get; set; } = string.Empty;

    public bool IsSelected { get; set; }

    public bool IsFree { get; set; }
}
