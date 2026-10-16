# Tech Stack

## Language and runtime

- C# and Razor components.
- .NET 11 target (`net11.0`), configured in `SocialBingo.csproj`.
- Project references prerelease ASP.NET Core Components WebAssembly/Gateway packages, release `11.0.0-rc.1.26425.128`.
- No `global.json` or explicit SDK pin is present.

## Frameworks and dependencies

| Dependency | Version/source | Use |
|---|---|---|
| `Microsoft.AspNetCore.Components.WebAssembly` | `11.0.0-rc.1.26425.128` | Blazor WebAssembly hosting and component runtime. |
| `Microsoft.AspNetCore.Components.Gateway` | `11.0.0-rc.1.26425.128`, `PrivateAssets=all` | Build/package integration declared by the project. |
| Bootstrap CSS/JS | Vendored static assets under `wwwroot/lib/bootstrap/dist/`; bundled CSS identifies version `5.3.3` | Global styling and any Bootstrap behavior used by the app. |

The app uses built-in `System.Net.Http.Json`, `System.Text.Json`, and `Microsoft.JSInterop` APIs. No separately declared test framework, backend API client, database/ORM, or server project is present.

## Build and runtime configuration

- Build/restore through the .NET SDK and `SocialBingo.csproj`.
- `Properties/launchSettings.json` provides the `http` profile at `http://localhost:5057` with `ASPNETCORE_ENVIRONMENT=Development`.
- `Program.cs` registers a base-address `HttpClient` and the scoped game-state service.
- Browser execution requires the Blazor WebAssembly runtime and browser APIs used by the app, including `localStorage`.
- Bootstrap is checked in as static files; it is not a NuGet or npm dependency in the project manifest.
