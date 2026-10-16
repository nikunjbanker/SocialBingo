# SocialBingo Project Guidelines

## Architecture

- This is a .NET 11 Blazor WebAssembly app. `Program.cs` registers `App` and `HeadOutlet` as root components and provides a scoped `HttpClient` based at the app host URL.
- `App.razor` owns routing, uses `MainLayout` for routed pages, and directs unmatched routes to `Pages/NotFound.razor`. Routable pages live in `Pages/` and declare routes with `@page`.
- `Layout/MainLayout.razor` hosts the shared navigation and page content. Update `Layout/NavMenu.razor` when adding a page that should be reachable from the navigation.
- The home route renders `Components/BingoBoard.razor`; reusable board UI belongs in `Components/`, while bingo data types and game state live in `Models/` and `Services/`.
- Static assets live in `wwwroot/`. `wwwroot/index.html` loads Bootstrap and the shared `wwwroot/css/app.css`; `*.razor.css` files hold component-scoped styles. Use `::deep` in isolated styles when styling markup rendered by child components such as `NavLink`.
- `Pages/Weather.razor` demonstrates loading app-local JSON with the injected `HttpClient` from `wwwroot/sample-data/weather.json`. Keep these URLs relative to the app base URL (`<base href="/">` in `wwwroot/index.html`).

## Build and run

- Restore dependencies: `dotnet restore SocialBingo.csproj`
- Build: `dotnet build SocialBingo.csproj`
- Run with the configured HTTP launch profile: `dotnet run --project SocialBingo.csproj` (http://localhost:5057)

The project targets `net11.0` and references ASP.NET Core Components WebAssembly `11.0.0-rc.1.26425.128`. There is currently no test project or configured test/lint command, so no single-test command is available.

## Conventions

- Keep page routing in Razor components via `@page`, and use the shared `MainLayout` unless a page explicitly needs another layout.
- Put a page's component logic in its Razor `@code` block and resolve shared services such as `HttpClient` through Blazor dependency injection.
- Add page-specific CSS in the matching `.razor.css` file; reserve `wwwroot/css/app.css` for styles shared across the app.

## State management and API

- Keep transient UI state in the owning Razor component unless it needs to be shared; `Pages/Counter.razor` is the example of component-local state. Shared bingo state belongs in `IGameStateService`/`GameStateService`, registered as a scoped service in `Program.cs`.
- `GameStateService` is the authority for the 5x5 card, tile selection, free center, row/column/diagonal win detection, and locking selections after a win. Persist state through browser `localStorage`; initialize it before rendering the board and use the service's `StateChanged` event to refresh subscribed components.
- `Program.cs` registers one scoped `HttpClient` with `builder.HostEnvironment.BaseAddress`. Inject this client for HTTP requests rather than constructing clients in components.
- The existing `Pages/Weather.razor` request reads local JSON from `wwwroot/sample-data/weather.json` using a base-relative URL and `GetFromJsonAsync` during `OnInitializedAsync`. This is a static-asset example, not a configured backend API; no API service, remote base URL, or backend contract is currently defined.
