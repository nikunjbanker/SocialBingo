# Data Model

## Application entities

### `BingoCard` — `Models/BingoCard.cs`

- `Size`: `const int`, fixed board dimension of 5.
- `Tiles`: mutable `List<BingoTile>`, initialized empty; service initializes it with 25 row-major elements.
- `GetTile(int row, int column)`: checks both coordinates against `[0, 4]`, then indexes `Tiles[row * Size + column]`.

### `BingoTile` — `Models/BingoTile.cs`

- `Row`: `int`, zero-based row.
- `Column`: `int`, zero-based column.
- `Prompt`: `string`, initialized to empty string.
- `IsSelected`: `bool`.
- `IsFree`: `bool`.

### Persisted `SavedGame` — private nested type in `Services/GameStateService.cs`

- `Card`: nullable `BingoCard`.
- `HasWon`: `bool`.
- Serialized using `JsonSerializerDefaults.Web`, which produces web-default camelCase property names.
- Stored in browser `localStorage` under `social-bingo.game-state.v1`.
- Restore validation requires 25 row-major tiles, nonempty prompts, only the center tile marked free, the free tile selected, and a saved win flag consistent with the selected rows/columns/diagonals.

### `WeatherForecast` — nested in `Pages/Weather.razor`

- `Date`: `DateOnly`.
- `TemperatureC`: `int`.
- `Summary`: nullable `string`.
- `TemperatureF`: computed `int` using `32 + (int)(TemperatureC / 0.5556)`.
- Deserialized from objects in `wwwroot/sample-data/weather.json`; no persistence or database mapping.

## Relationships and persistence

- One `BingoCard` owns an ordered collection of 25 `BingoTile` values; the center position (row 2, column 2, zero-based) is the free tile.
- A persisted `SavedGame` contains one card and its win flag.
- There is no database, ORM, relational schema, or transaction boundary. Persistence is per-browser local storage.
- Weather forecast records are independent rows in a static JSON array; no relationships are declared.

## Key entities

- **BingoCard:** fixed 5×5 collection and row/column lookup.
- **BingoTile:** prompt and selected/free state for one position.
- **SavedGame:** serialized card and win status used to resume a game.
- **WeatherForecast:** display model for the app-local sample weather data.
