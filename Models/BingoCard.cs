namespace SocialBingo.Models;

public sealed class BingoCard
{
    public const int Size = 5;

    public List<BingoTile> Tiles { get; set; } = [];

    public BingoTile GetTile(int row, int column)
    {
        if (row is < 0 or >= Size)
        {
            throw new ArgumentOutOfRangeException(nameof(row));
        }

        if (column is < 0 or >= Size)
        {
            throw new ArgumentOutOfRangeException(nameof(column));
        }

        return Tiles[row * Size + column];
    }
}
