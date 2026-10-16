using SocialBingo.Models;

namespace SocialBingo.Services;

public interface IGameStateService
{
    BingoCard? CurrentCard { get; }

    bool HasWon { get; }

    event Action? StateChanged;

    Task InitializeAsync();

    Task SelectTileAsync(int row, int column);

    Task StartNewGameAsync();
}
