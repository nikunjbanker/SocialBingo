using System.Text.Json;
using Microsoft.JSInterop;
using SocialBingo.Models;

namespace SocialBingo.Services;

public sealed class GameStateService(IJSRuntime jsRuntime) : IGameStateService
{
    private const string StorageKey = "social-bingo.game-state.v1";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private static readonly string[] Prompts =
    [
        "Find someone who enjoys hiking",
        "Meet someone who has a pet",
        "Find someone who likes board games",
        "Talk with someone who enjoys cooking",
        "Meet someone who likes to travel",
        "Find someone who enjoys reading",
        "Talk with someone who likes gardening",
        "Meet someone who enjoys live music",
        "Find someone who speaks more than one language",
        "Talk with someone who likes puzzles",
        "Meet someone who enjoys swimming",
        "Find someone who has visited a national park",
        "Talk with someone who likes trying new foods",
        "Meet someone who enjoys drawing",
        "Find someone who likes watching documentaries",
        "Talk with someone who enjoys cycling",
        "Meet someone who likes coffee or tea",
        "Find someone who enjoys dancing",
        "Talk with someone who likes taking photos",
        "Meet someone who enjoys camping",
        "Find someone who likes learning new skills",
        "Talk with someone who enjoys museums",
        "Meet someone who likes spending time outdoors",
        "Find someone who enjoys a team sport"
    ];

    private BingoCard? _currentCard;
    private bool _hasWon;
    private bool _isInitialized;

    public BingoCard? CurrentCard => _currentCard;

    public bool HasWon => _hasWon;

    public event Action? StateChanged;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        string? savedState;
        try
        {
            savedState = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        }
        catch (JSException exception)
        {
            throw new InvalidOperationException("The saved game could not be read from browser storage.", exception);
        }

        if (string.IsNullOrWhiteSpace(savedState))
        {
            await StartNewGameAsync();
            return;
        }

        SavedGame? game;
        try
        {
            game = JsonSerializer.Deserialize<SavedGame>(savedState, JsonOptions);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException(
                "The saved bingo game is invalid. Start a new game to replace it.",
                exception);
        }

        if (game?.Card is null || !IsValidSavedGame(game))
        {
            throw new InvalidDataException(
                "The saved bingo game is invalid. Start a new game to replace it.");
        }

        _currentCard = game.Card;
        _hasWon = game.HasWon;
        _isInitialized = true;
        StateChanged?.Invoke();
    }

    public async Task SelectTileAsync(int row, int column)
    {
        EnsureInitialized();
        if (row < 0 || row >= BingoCard.Size)
        {
            throw new ArgumentOutOfRangeException(nameof(row), "Tile coordinates must be within the 5x5 card.");
        }

        if (column < 0 || column >= BingoCard.Size)
        {
            throw new ArgumentOutOfRangeException(nameof(column), "Tile coordinates must be within the 5x5 card.");
        }

        var tile = _currentCard!.GetTile(row, column);
        if (_hasWon || tile.IsFree)
        {
            return;
        }

        var previousSelection = tile.IsSelected;
        var previousWinState = _hasWon;
        tile.IsSelected = !tile.IsSelected;
        _hasWon = HasWinningLine(_currentCard);

        try
        {
            await PersistAsync();
        }
        catch
        {
            tile.IsSelected = previousSelection;
            _hasWon = previousWinState;
            throw;
        }

        StateChanged?.Invoke();
    }

    public async Task StartNewGameAsync()
    {
        var previousCard = _currentCard;
        var previousWinState = _hasWon;
        var previousInitializedState = _isInitialized;

        _currentCard = CreateCard();
        _hasWon = false;

        try
        {
            await PersistAsync();
        }
        catch
        {
            _currentCard = previousCard;
            _hasWon = previousWinState;
            _isInitialized = previousInitializedState;
            throw;
        }

        _isInitialized = true;
        StateChanged?.Invoke();
    }

    private static BingoCard CreateCard()
    {
        var tiles = new List<BingoTile>(BingoCard.Size * BingoCard.Size);
        var promptIndex = 0;

        for (var row = 0; row < BingoCard.Size; row++)
        {
            for (var column = 0; column < BingoCard.Size; column++)
            {
                var isFree = row == BingoCard.Size / 2 && column == BingoCard.Size / 2;
                tiles.Add(new BingoTile
                {
                    Row = row,
                    Column = column,
                    Prompt = isFree ? "Free space" : Prompts[promptIndex++],
                    IsSelected = isFree,
                    IsFree = isFree
                });
            }
        }

        return new BingoCard { Tiles = tiles };
    }

    private static bool IsValidSavedGame(SavedGame game)
    {
        var card = game.Card!;
        if (card.Tiles is null || card.Tiles.Count != BingoCard.Size * BingoCard.Size)
        {
            return false;
        }

        for (var index = 0; index < card.Tiles.Count; index++)
        {
            var tile = card.Tiles[index];
            var row = index / BingoCard.Size;
            var column = index % BingoCard.Size;
            var shouldBeFree = row == BingoCard.Size / 2 && column == BingoCard.Size / 2;

            if (tile is null ||
                tile.Row != row ||
                tile.Column != column ||
                string.IsNullOrWhiteSpace(tile.Prompt) ||
                tile.IsFree != shouldBeFree ||
                (tile.IsFree && !tile.IsSelected))
            {
                return false;
            }
        }

        return game.HasWon == HasWinningLine(card);
    }

    private static bool HasWinningLine(BingoCard card)
    {
        for (var index = 0; index < BingoCard.Size; index++)
        {
            if (Enumerable.Range(0, BingoCard.Size).All(column => card.GetTile(index, column).IsSelected) ||
                Enumerable.Range(0, BingoCard.Size).All(row => card.GetTile(row, index).IsSelected))
            {
                return true;
            }
        }

        return Enumerable.Range(0, BingoCard.Size).All(index =>
                   card.GetTile(index, index).IsSelected) ||
               Enumerable.Range(0, BingoCard.Size).All(index =>
                   card.GetTile(index, BingoCard.Size - index - 1).IsSelected);
    }

    private async Task PersistAsync()
    {
        var state = new SavedGame { Card = _currentCard, HasWon = _hasWon };
        var serialized = JsonSerializer.Serialize(state, JsonOptions);
        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, serialized);
        }
        catch (JSException exception)
        {
            throw new InvalidOperationException("The game could not be saved to browser storage.", exception);
        }
    }

    private void EnsureInitialized()
    {
        if (!_isInitialized || _currentCard is null)
        {
            throw new InvalidOperationException("Initialize the game state before changing tiles.");
        }
    }

    private sealed class SavedGame
    {
        public SavedGame()
        {
        }

        public BingoCard? Card { get; set; }

        public bool HasWon { get; set; }
    }
}
