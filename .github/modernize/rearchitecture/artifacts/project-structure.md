# Project Structure

## Project type and domains

- **Classification:** frontend — .NET Blazor WebAssembly single-page application; no server-side API project is present.
- **Functional areas:** social bingo gameplay; sample counter; sample weather display; unmatched-route display.

## Layers and top-level areas

| Area | Files | Role |
|---|---:|---|
| Application entry/composition | 4 | `Program.cs` bootstraps WebAssembly and registers services; `App.razor` routes; `_Imports.razor` supplies Razor imports; `SocialBingo.csproj` defines target and package references. |
| Pages | 4 | Route entry components for home bingo, counter, weather, and not-found. |
| Components | 4 | Bingo board and reusable tile markup, each with isolated CSS. |
| Layout | 4 | Shared navigation/layout markup and isolated CSS. |
| Models | 2 | Bingo tile and card state. |
| Services | 2 | Game-state contract and implementation. |
| Static web assets | 5 | HTML host, shared CSS, two image assets, and weather JSON. |

Counts exclude generated `bin/` and `obj/`, vendored Bootstrap files under `wwwroot/lib/`, and repository/agent customization files. `Properties/launchSettings.json` configures the local HTTP launch profile.

## Runtime entry and routing

- `Program.cs` registers `App` at `#app`, `HeadOutlet` at `head::after`, a scoped base-address `HttpClient`, and scoped `IGameStateService`.
- `wwwroot/index.html` provides the host document, `/` base URL, Bootstrap and app styles, and the fingerprinted Blazor WebAssembly bootstrap script.
- `App.razor` routes components in the app assembly, applies `MainLayout` by default, focuses the `h1` after navigation, and renders `Pages/NotFound.razor` for unmatched paths.
- Routable page components and paths: `Pages/Home.razor` (`/`), `Pages/Counter.razor` (`/counter`), `Pages/Weather.razor` (`/weather`), and `Pages/NotFound.razor` (`/not-found`).
- `Layout/MainLayout.razor` composes the shared `NavMenu` and routed page body; `Layout/NavMenu.razor` links to bingo, counter, and weather.

## Functional architecture

- **Bingo:** home page → `BingoBoard` → `TileComponent`; board state/rules are delegated to `IGameStateService`/`GameStateService`; state uses `BingoCard` and `BingoTile`. `GameStateService` stores the game under a versioned browser local-storage key and signals updates through `StateChanged`.
- **Counter:** standalone component-local integer state; button event increments the count.
- **Weather:** page injects the app-scoped `HttpClient` and reads `wwwroot/sample-data/weather.json`; a nested `WeatherForecast` type models the response.
- **Not found:** a shared router fallback page with a direct `/not-found` route.

## Configuration

- `SocialBingo.csproj`: .NET target framework, nullable/implicit-using settings, and package references.
- `Properties/launchSettings.json`: local HTTP URL (`http://localhost:5057`) and Development environment.
- `wwwroot/index.html`: browser base URL and static/runtime assets.
- `.mcp.json`: repository Playwright MCP command configuration; this is development tooling, not an application runtime integration.
- `.github/` and `.kilo/`: Copilot prompt/instruction/skill and agent customization files; not part of the app runtime.
